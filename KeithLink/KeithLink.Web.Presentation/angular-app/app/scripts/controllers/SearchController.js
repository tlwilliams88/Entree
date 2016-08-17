'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', '$state', '$stateParams', '$modal', '$analytics', '$filter', '$timeout', 'ProductService', 'CategoryService', 'Constants', 'PricingService', 'CartService', 'ApplicationSettingsService', 'blockUI', 'LocalStorage', 'SessionService',
    function(
      $scope, $state, $stateParams, // angular dependencies
      $modal, // ui bootstrap library
      $analytics, //google analytics
      $filter,
      $timeout,
      ProductService, CategoryService, Constants, PricingService, CartService, ApplicationSettingsService, // bek custom services
      blockUI,
      LocalStorage,
      SessionService
    ) {

    // clear keyword search term at top of the page
    if ($scope.userBar) {
      $scope.userBar.universalSearchTerm = '';
    }

    CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });

    // TODO: do not call these functions directly from view
    $scope.canOrderItem = PricingService.canOrderItem;
    $scope.hasCasePrice = PricingService.hasCasePrice;
    $scope.hasPrice = PricingService.hasPrice;
    $scope.hasPackagePrice = PricingService.hasPackagePrice;

    $scope.paramType = $stateParams.type; // Category, Search, Brand
    $scope.paramId = $stateParams.id; // search term, brand id, category id

    $scope.selectedSortParameter = 'Relevance';
    $scope.sortParametervalue = '';
    $scope.sortReverse = false;

    if(LocalStorage.getDefaultView()){
      $scope.defaultSearchView = LocalStorage.getDefaultView();
    } else {
      $scope.defaultSearchView = 'ViewWithImages';
    }

    $scope.itemsPerPage = LocalStorage.getPageSize();
    $scope.itemIndex = 0;

    $scope.numberFacetsToShow = 4;  // determines when to show the 'Show More' link for each facet
    $scope.maxSortCount = 200;      // max number of items that can be sorted by price

    $scope.sortDirection = 'asc';
    $scope.categoryIsOpen = false;

    $scope.hideMobileFilters = true;
    if ($state.params.catalogType == "BEK") {
        $scope.pageTitle = "Product Catalog";
    } else {
        if ($state.params.catalogType == "UNFI"){
            $scope.pageTitle = "Natural and Organic";
        } else {
            $scope.pageTitle = "Specialty Catalog";
        }
    }

	$scope.noFiltersSelected = true;

    $scope.products = [];
    $scope.facets = {
      categories: {
        available: [],
        selected: []
      },
      brands: {
        available: [],
        selected: [],
        showMore: true // display Show More button for this facet
      },
      mfrname:{
        available: [],
        selected: [],
        showMore: true
      },
      itemspecs: {
        available: [],
        selected: [],
        showMore: true
      },
      dietary: {
        available: [],
        selected: [],
        showMore: true
      },
      temp_zone: {
        available: [],
        selected: [],
        showMore: true
      },
      parentcategories: {
        available: [],
        selected: [],
        showMore: true
      },
      subcategories: {
        selected: [],
        showMore: true
      }
    };

    $scope.initPagingValues = function(){
      $scope.visitedPages = [];
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.itemCountOffset = 0;
    }

    $scope.initPagingValues();

    /*************
    PAGINATION
    *************/

    //Toggles variable for determining Search Result View, and amount of results requested
    $scope.toggleListView = function(selectedview){
      var selectedSearchView = {key: 'defaultSearchView', value: selectedview};
      $scope.defaultSearchView = selectedview;
      LocalStorage.setDefaultView(selectedview);

      ApplicationSettingsService.saveApplicationSettings(selectedSearchView);
    }

    $scope.blockUIAndChangePage = function(page, toggleView){
        $scope.startingPoint = 0;
        $scope.endPoint = 0;
        $scope.toggleView = toggleView;  

        var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});

        if ($scope.toggleView){
          visited = [];
          $scope.currentPage = 1;
        }

        $(document).ready(function(){
          $(document.activeElement).blur();
          $("html, body").animate({ scrollTop: 0 }, 500);
        })

        return blockUI.start("Loading Products...").then(function(){

          if(visited.length > 0){
            $timeout(function() {
              $scope.isChangingPage = true;
              $scope.pageChanged(page, visited);
            }, 100);
          }
          else{
              $scope.pageChanged(page, visited);
          }

        })

    }

     $scope.setStartAndEndPoints = function(page){
      var foundStartPoint = false;
        page.forEach(function(item, index){
          if(page && item.itemnumber === page[0].itemnumber){
            $scope.startingPoint = index;
            $scope.endPoint = angular.copy($scope.startingPoint + $scope.itemsPerPage);
            foundStartPoint = true;
          }
        })

        if(!foundStartPoint){
          appendProducts(page);
        }
        //We need two calls for stop here because we have two paging directives on the view. If the page change is triggered 
        //automatically (deleting all items on page/saving) the event will fire twice and two loading overlays will be generated.
        blockUI.stop();
        blockUI.stop();
     }
    
     $scope.pageChanged = function(pages, visited) {
      var facets;
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.loadingPage = true;    
      $scope.currentPage = pages.currentPage;
      $scope.startingPoint = ((pages.currentPage - 1)*$scope.itemsPerPage) + 1;
      $scope.startingResult = $scope.startingPoint;
      $scope.endPoint = angular.copy($scope.startingPoint + $scope.itemsPerPage);
      $scope.firstPageItem = ($scope.currentPage * $scope.itemsPerPage) - ($scope.itemsPerPage - 1);
      $scope.setRange();
      $scope.aggregateCount;
      
      $scope.aggregateCount = ($scope.facets.brands.selected.length + $scope.facets.itemspecs.selected.length + $scope.facets.dietary.selected.length + $scope.facets.mfrname.selected.length + $scope.facets.temp_zone.selected.length + $scope.facets.parentcategories.selected.length + $scope.facets.subcategories.selected.length)

      if($scope.aggregateCount !== 0){
        facets = ProductService.getFacets(
          $scope.facets.brands.selected,
          $scope.facets.mfrname.selected,
          $scope.facets.dietary.selected,
          $scope.facets.itemspecs.selected,
          $scope.facets.temp_zone.selected,
          $scope.facets.parentcategories.selected,
          $scope.facets.subcategories.selected
        )
      }

      var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.startingPoint, $scope.sortField, $scope.sortDirection, facets, $stateParams.dept);
      ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType,params, $stateParams.deptName).then(function(data){
        $scope.products = data.products;
        if($scope.toggleView){
          resetPage($scope.products, true);
        }
      blockUI.stop();
      blockUI.stop();
      })
     };

    $scope.setRange = function(){
      $scope.endPoint = $scope.endPoint;
      $scope.rangeStart = $scope.startingPoint;
      $scope.rangeEnd = ($scope.endPoint > $scope.totalProducts) ? $scope.totalProducts : $scope.endPoint - 1;
      if($scope.rangeStart === 0){
        $scope.rangeStart++;
        if($scope.rangeEnd === $scope.itemsPerPage - 1){
          $scope.rangeEnd ++;
        }
      }
    }

    function resetPage(results, initialPageLoad) {
      $scope.initPagingValues();
      $scope.activeElement = true;
      $scope.totalItems = $scope.totalProducts;
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;

      if(initialPageLoad){   
        $scope.currentPage = 1;
        $scope.firstPageItem = ($scope.currentPage * $scope.itemsPerPage) - ($scope.itemsPerPage);
        $scope.products = $scope.products.slice($scope.firstPageItem, ($scope.currentPage * $scope.itemsPerPage));
        $scope.setStartAndEndPoints($scope.products);
        $scope.visitedPages.push({page: 1, items: $scope.products, deletedCount: 0});
      }
      $scope.setRange();
    };

    function appendProducts(results) {
      $scope.visitedPages.push({page: $scope.currentPage, items: results, deletedCount: 0});
      //Since pages can be visited out of order, sort visited pages into numeric order.
      $scope.visitedPages = $scope.visitedPages.sort(function(obj1, obj2){   
        var sorterval1 = obj1.page;      
        var sorterval2 = obj2.page;       
        return sorterval1 - sorterval2;
      });

      if($scope.totalProducts === 0){
        $scope.startingPoint = 0;
        $scope.endPoint = 0;
      }
      else{
       $scope.setStartAndEndPoints(results);
      }
    };

    function startLoading() {
      $scope.loadingResults = true;
    }

    function stopLoading() {
      $scope.loadingResults = false;
      blockUI.stop();
    }

    if($stateParams.sortingParams && $stateParams.sortingParams.sort.length){
      $scope.sort = $stateParams.sortingParams.sort;
    }

    /*************
    BREADCRUMBS
    *************/
    function setBreadcrumbs(data) {
      var breadcrumbs = [],
        filterCount = 0,
        breadcrumbSeparator = ', ';

      // top level breadcrumb based on the type of search
      var displayText;

      // search term
      if ($scope.paramType === 'search') {
       $scope.featuredBreadcrumb = {
          click: $scope.clearFacets,
          clickData: null,
          displayText: $stateParams.deptName
        };
        breadcrumbs.unshift($scope.featuredBreadcrumb);
        
        $scope.featuredBreadcrumb = {
          click: $scope.clearFacets,
          clickData: '',
          displayText: '"' + $scope.paramId + '"'
        };
        breadcrumbs.push($scope.featuredBreadcrumb);
      }

      // categories
      if ($scope.facets.parentcategories.selected.length > 0 && $scope.facets.parentcategories.selected.length < 3) { // parent categories less than 3
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.parentcategories.selected = data;
            $scope.facets.subcategories.selected = [];
            $scope.facets.brands.selected = [];
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.parentcategories.selected,
          displayText: 'Categories: ' + $scope.facets.parentcategories.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.parentcategories.selected.length;
      } else if ($scope.facets.parentcategories.selected.length >= 3) { // parent categories more than 2
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.parentcategories.selected = data;
            $scope.facets.subcategories.selected = [];
            $scope.facets.brands.selected = [];
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.parentcategories.selected,
          displayText: 'Categories: ' + $scope.facets.parentcategories.selected.length + ' selected'
        });
        filterCount += $scope.facets.parentcategories.selected.length;
      }  

      // sub-categories
      if ($scope.facets.subcategories.selected.length > 0 && $scope.facets.subcategories.selected.length < 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.subcategories.selected = data;
            $scope.facets.brands.selected = [];
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.subcategories.selected,
          displayText: 'Sub-Categories: ' + $scope.facets.subcategories.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.subcategories.selected.length;
      } else if ($scope.facets.subcategories.selected.length >= 3) { // sub-categories more than 2
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.subcategories.selected = data;
            $scope.facets.brands.selected = [];
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.subcategories.selected,
          displayText: 'Sub-Categories: ' + $scope.facets.subcategories.selected.length + ' selected'
        });
        filterCount += $scope.facets.subcategories.selected.length;
      }

      if ($scope.paramType === 'brand') {
        data.facets.brands.forEach(function(brand) {
          if (brand.brand_control_label && brand.brand_control_label.toUpperCase() === $scope.paramId.toUpperCase()) {
            displayText = brand.name;
          }
        });

        $scope.featuredBreadcrumb = {
          click: $scope.clearFacets,
          clickData: null,
          displayText: displayText
        };
        breadcrumbs.push($scope.featuredBreadcrumb);
      }

      // brands
      if ($scope.facets.brands.selected.length > 0 && $scope.facets.brands.selected.length < 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.brands.selected = data;
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.brands.selected,
          displayText: 'Brands: ' + $scope.facets.brands.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.brands.selected.length;
      } else if ($scope.facets.brands.selected.length >= 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.brands.selected = data;
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.brands.selected,
          displayText: 'Brands: ' + $scope.facets.brands.selected.length + ' selected'
        });
        filterCount += $scope.facets.brands.selected.length;
      }

      // manufacturers
      if ($scope.facets.mfrname.selected.length > 0 && $scope.facets.mfrname.selected.length < 3) {
          breadcrumbs.push({
            click: function (data) {
              $scope.facets.mfrname.selected = data;
              $scope.facets.dietary.selected = [];
              $scope.facets.itemspecs.selected = [];
              loadProducts().then(refreshFacets);
            },
            clickData: $scope.facets.mfrname.selected,
            displayText: 'Manufacturers: ' + $scope.facets.mfrname.selected.join(breadcrumbSeparator)
          });
          filterCount += $scope.facets.mfrname.selected.length;
      } else if ($scope.facets.mfrname.selected.length >= 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.mfrname.selected = data;
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.mfrname.selected,
          displayText: 'Manufacturers: ' + $scope.facets.mfrname.selected.length + ' selected'
        });
        filterCount += $scope.facets.mfrname.selected.length;
      }

      // dietary
      if ($scope.facets.dietary.selected.length > 0 && $scope.facets.dietary.selected.length < 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.dietary.selected = data;
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.dietary.selected,
          displayText: 'Dietary: ' + $scope.facets.dietary.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.dietary.selected.length;
      } else if ($scope.facets.dietary.selected.length >= 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.dietary.selected = data;
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.dietary.selected,
          displayText: 'Dietary: ' + $scope.facets.dietary.selected.length + ' selected'
        });
        filterCount += $scope.facets.dietary.selected.length;
      }

      // item specifications
      if ($scope.facets.itemspecs.selected.length > 0 && $scope.facets.itemspecs.selected.length < 3) {
        var specDisplayNames = [];
        $scope.facets.itemspecs.selected.forEach(function(spec) {
          specDisplayNames.push(getIconDisplayInfo(spec).displayname);
        });
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.itemspecs.selected = data;
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.itemspecs.selected,
          displayText: 'Item Specifications: ' + specDisplayNames.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.itemspecs.selected.length;
      } else if ($scope.facets.itemspecs.selected.length >= 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.itemspecs.selected = data;
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.itemspecs.selected,
          displayText: 'Item Specifications: ' + $scope.facets.itemspecs.selected.length + ' selected'
        });
        filterCount += $scope.facets.itemspecs.selected.length;
      }

      // temp zones
      if ($scope.facets.temp_zone.selected.length > 0 && $scope.facets.temp_zone.selected.length < 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.temp_zone.selected = data;
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.temp_zone.selected,
          displayText: 'Temp Zone: ' + $scope.facets.temp_zone.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.temp_zone.selected.length;
      } else if ($scope.facets.temp_zone.selected.length >= 3) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.temp_zone.selected = data;
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.temp_zone.selected,
          displayText: 'Temp Zone: ' + $scope.facets.temp_zone.selected.length + ' selected'
        });
        filterCount += $scope.facets.temp_zone.selected.length;
      }

      $scope.breadcrumbs = breadcrumbs;
      $scope.filterCount = filterCount;
    }

    /*************
    LOAD PRODUCT DATA
    *************/
    function getData() {
      var facets;
      $scope.userProfile = SessionService.userProfile;
      $scope.currentCustomer = LocalStorage.getCurrentCustomer();
      $scope.aggregateCount;
      
      $scope.aggregateCount = ($scope.facets.brands.selected.length + $scope.facets.itemspecs.selected.length + $scope.facets.dietary.selected.length + $scope.facets.mfrname.selected.length + $scope.facets.temp_zone.selected.length + $scope.facets.parentcategories.selected.length + $scope.facets.subcategories.selected.length)

      if($scope.aggregateCount !== 0){
        facets = ProductService.getFacets(
          $scope.facets.brands.selected,
          $scope.facets.mfrname.selected,
          $scope.facets.dietary.selected,
          $scope.facets.itemspecs.selected,
          $scope.facets.temp_zone.selected,
          $scope.facets.parentcategories.selected,
          $scope.facets.subcategories.selected
        )
      }

      // console.log("catalog type in search controller: " + $scope.$state.params.catalogType);
      if($scope.sortField === 'itemnumber' && $state.params.catalogType != 'BEK'){
        $scope.sortField  = '';
      }

      // if(!$scope.startingResult){
      //   $scope.startingResult = 0;
      // }

      var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.startingPoint, $scope.sortField, $scope.sortDirection, facets, $stateParams.dept);
      return ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType,params, $stateParams.deptName);
    }

    //Load list of products and block UI with message
    function loadProducts(appendResults, fromFunction) {
      startLoading();
      return blockUI.start("Loading Products...").then(function(){
        return getData().then(function(data) {
        var page = 1;
        $scope.products = data.products;
        $scope.totalProducts = data.totalcount;
        if(fromFunction !== 'sorting'){
          resetPage(data.products, true);
        }
        $scope.totalItems = data.totalcount;
        if (data.catalogCounts != null) {
            $scope.bekItemCount = data.catalogCounts.bek;
            $scope.unfiItemCount = data.catalogCounts.unfi;
        } else {
            $scope.bekItemCount = 0;
            $scope.unfiItemCount = 0;
        }
        // append results to existing data (for infinite scroll)
        if (appendResults) {
          $scope.products.push.apply($scope.products, data.products);
        // replace existing data (for sort, filter)
        } else {
          $scope.setStartAndEndPoints($scope.products);
        }
        if($scope.aggregateCount !==0 || $scope.noFiltersSelected){
          updateFacetCount($scope.facets.brands, data.facets.brands);
          updateFacetCount($scope.facets.itemspecs, data.facets.itemspecs);
          updateFacetCount($scope.facets.dietary, data.facets.dietary);
          updateFacetCount($scope.facets.mfrname, data.facets.mfrname);
          updateFacetCount($scope.facets.temp_zone, data.facets.temp_zone);
          updateParentCategoryFacetCount($scope.facets.parentcategories, data.facets.parentcategories);
        }

        setBreadcrumbs(data);
        stopLoading();

        blockUI.stop();

        delete $scope.searchMessage;
        
        return data.facets;
        })
      }, function(error) {
        $scope.searchMessage = 'Error loading products.';
      }).finally(function() {
      });
    }



    /*************
    FACETS
    *************/

    function updateFacetCount(facets, data){

      if(facets && facets.available){

        facets.available.forEach(function(facet){

          var facetName = $filter('filter') (data, {name: facet.name})
          facet.count = 0;

          if(facetName.length > 0 && facet.name){

            facet.count = facetName[0].count;

          }

        })

      }

      $scope.noFiltersSelected = false;
    }

    function updateParentCategoryFacetCount(categories, data){

      if (categories && categories.available){
        categories.available.forEach(function(category){
          var facetName = $filter('filter')(data, {name: category.name})
          category.count = 0;

          if (facetName && facetName.length > 0 && category.name){
            category.count = facetName[0].count;

            if (facetName[0].categories.length){
              category.categories.forEach(function(subcategory){
                  var foundSubcategory = $filter('filter')(facetName[0].categories, {name: subcategory.name})

                  if (foundSubcategory.length){
                    subcategory.count = foundSubcategory[0].count;
                  } else {
                    subcategory.count = 0;
                  }
              })
            }
          }
        })
      }
      $scope.noFiltersSelected = false;
    }

    $scope.clearFacets = function() {
      $scope.facets.brands.selected = [];
      $scope.facets.mfrname.selected = [];
      $scope.facets.dietary.selected = [];
      $scope.facets.itemspecs.selected = [];
      $scope.facets.temp_zone.selected = [];
      $scope.facets.parentcategories.selected = [];
      $scope.facets.subcategories.selected = [];
      loadProducts().then(refreshFacets);
      $scope.noFiltersSelected = true;
    }

    function refreshFacets(facets) {
      var selectedCategory;
      // set the $scope.facets object using the response data
      if(facets){

        $scope.facets.brands.available = facets.brands;
        $scope.facets.mfrname.available = facets.mfrname;
        $scope.facets.dietary.available = facets.dietary;
        $scope.facets.itemspecs.available = addIcons(facets.itemspecs);
        $scope.facets.temp_zone.available = facets.temp_zone;
        $scope.facets.parentcategories.available = facets.parentcategories;

        if($scope.facets.parentcategories.available.length && $scope.facets.subcategories.selected.length){

          $scope.facets.parentcategories.available.forEach(function(parentcategory){

            parentcategory.categories.forEach(function(category){

              selectedCategory = $scope.facets.subcategories.selected.indexOf(category.name);

              if(selectedCategory > -1){

                category.isSelected = true;

              }

            })

          })

        }

      }

    }

    /*************
    ITEM SPEC ICONS
    *************/
    function getIconDisplayInfo(name) {
      var itemSpec = {};
      switch (name) {
        case 'itembeingreplaced':
          itemSpec.displayname = 'Replacement Item';
          itemSpec.iconclass = 'text-green icon-cycle';
          break;
        case 'replacementitem':
          itemSpec.displayname = 'Item Being Replaced';
          itemSpec.iconclass = 'text-red icon-cycle';
          break;
        case 'childnutrition':
          itemSpec.displayname = 'Child Nutrition Sheet';
          itemSpec.iconclass = 'text-regular icon-apple';
          break;
        case 'sellsheet':
          itemSpec.displayname = 'Product Information Sheet';
          itemSpec.iconclass = 'text-regular icon-sellsheet';
          break;
        case 'nonstock':
          itemSpec.displayname = 'Non-Stock Item';
          itemSpec.iconclass = 'text-regular icon-user';
          break;
        // cannot filter by deviated cost
        // case 'deviatedcost':
        //   itemSpec.displayname = 'DeviatedCost';
        //   itemSpec.iconclass = 'text-regular icon-dollar';
        //   break;

        // case 'materialsafety':
        //   itemSpec.displayname = 'Material Safety Data Sheet';
        //   itemSpec.iconclass = 'text-regular icon-safety';
        //   break;
      }
      return itemSpec;
    }

    function addIcons(itemspecs) {
      var itemspecsArray = [];
      angular.forEach(itemspecs, function(item, index) {
        var itemSpec = getIconDisplayInfo(item.name);
        itemSpec.name = item.name;
        itemSpec.count = item.count;
        itemspecsArray.push(itemSpec);
      });
      return itemspecsArray;
    }

    /*************
    SORTING
    *************/
    $scope.selectSortParameter = function(parametername, parametervalue){
      if($scope.selectedSortParameter == parametername){
        return;
      } else {
        $scope.selectedSortParameter = parametername;
        $scope.sortField = parametervalue;

        if(parametername == 'Price Low to High') {
          $scope.sortDirection = 'asc';
        } else if (parametername == 'Price High to Low') {
          $scope.sortDirection = 'desc';
        } else {
          $scope.sortDirection = 'asc'
        }

        loadProducts('', 'sorting');
      }
    }

    $scope.sortParameters = [{
      name: 'Relevance',
    }, {
      name: 'Item #',
      value: 'itemnumber'
    }, {
      name: 'Name',
      value: 'name_not_analyzed'
    }, {
      name: 'Brand',
      value: 'brand_not_analyzed'
    }, {
      name: 'Price Low to High',
      value: 'caseprice'
    }, {
      name: 'Price High to Low',
      value: 'caseprice'
    }];

    /*************
    CLICK EVENTS
    *************/

    $scope.toggleSubCategories = function(category, index){
      var subCategories = $('.subcategories_' + index);
      category.IsOpen = !category.IsOpen;

      if (subCategories[0].style.display === 'block' && category.IsOpen == false){
        subCategories.css('display','none');
      } else {
        subCategories.css('display','block');
      }

    }

    $scope.toggleSelection = function(category, facetList, selectedFacet, parentcategory, index) {
      $scope.startingPoint = 0;
      $scope.noFiltersSelected = !$scope.noFiltersSelected;
      var parentCategorySelected,
          parentCategoryAvailableIdx,
          parentCategorySelectedIdx,
          subcategorySelected,
          totalSelectedSubCategories,
          isSelected,
          unSelectedCategory,
          idx;

      idx = facetList.indexOf(selectedFacet);

      if (idx > -1) {
        parentCategorySelected = $filter('filter')($scope.facets.parentcategories.available, {name:selectedFacet});

        if (parentCategorySelected.length){ // Parent Category is un-selected here

          if(parentCategorySelected[0].IsOpen){ // Toggles sub-categories display to none if category is open
            $scope.toggleSubCategories(parentCategorySelected[0], index); // Closes Category when parent category is unselected
          }

          isSelected = document.activeElement.checked;

          parentCategorySelected[0].categories.forEach(function(category){ //unselects and removes sub-category when parent Category is unselected
            category.isSelected = isSelected;
            unSelectedCategory = $scope.facets.subcategories.selected.indexOf(category.name);

            if ($scope.facets.parentcategories.selected.length > 0 && unSelectedCategory > -1){
              $scope.facets.subcategories.selected.splice(unSelectedCategory, 1) // Removes only specific sub-categories associated with category
            } else if ($scope.facets.parentcategories.selected.length == 0) {
              $scope.facets.subcategories.selected = []; // Removes all remaining sub-categories for last remaining category

            }

          })

          facetList.splice(idx, 1);
        } else { // All other aggregates are un-selected here

          if(parentcategory){
            // Identify parent category selected
            parentCategorySelected = $filter('filter')($scope.facets.parentcategories.available, {name:parentcategory});

            // Find index of parent category in available parent categories
            parentCategoryAvailableIdx = $scope.facets.parentcategories.available.indexOf(parentCategorySelected[0]);

            // Find index of parent category in selected parent categories
            parentCategorySelectedIdx = $scope.facets.parentcategories.selected.indexOf(parentCategorySelected[0].name);

            // Identify sub-category selected within selected parent category
            subcategorySelected = $filter('filter')(parentCategorySelected[0].categories, {name:selectedFacet});

            // Find total count of selected sub-categories within selected parent category
            totalSelectedSubCategories = $filter('filter')(parentCategorySelected[0].categories, {isSelected:true});// Finds all selected sub-categories

            if(totalSelectedSubCategories.length == 1){
              // If only one selected sub-category remains un-select parent category and collapses category
              $scope.facets.parentcategories.selected.splice(parentCategorySelectedIdx, 1);
              $scope.toggleSubCategories(parentCategorySelected[0], parentCategoryAvailableIdx);
            }
            subcategorySelected[0].isSelected = false;// Sets selected sub-category isSelected to false
          }

          facetList.splice(idx, 1);
        }
      } else {

        if (parentcategory){ // Runs if the aggregate selected is a sub-category

          if ($scope.facets.parentcategories.selected.length == 0){ // Runs if parent category selected array is empty
            $scope.facets.parentcategories.selected.push(parentcategory);
          } else { // Runs if parent category selected array is not empty

            if ($scope.facets.parentcategories.selected.indexOf(parentcategory) == -1){ // Only push to parentcategories.selected array if parentcategory does not exist
              $scope.facets.parentcategories.selected.push(parentcategory);
            }
          }

          facetList.push(selectedFacet); // In this case this pushes the sub-category to the facetList of selected facets
        } else { // All other aggregate selections other than sub-category run here
          facetList.push(selectedFacet);

          if(category == true){
            parentCategorySelected = $filter('filter')($scope.facets.parentcategories.available, {name:selectedFacet});
          }

          if (parentCategorySelected && parentCategorySelected.length){
            isSelected = document.activeElement.checked;

            parentCategorySelected[0].categories.forEach(function(category){
              category.isSelected = isSelected;
              $scope.facets.subcategories.selected.push(category.name);
            })
          }
        }
      }

      loadProducts();
    };

    $scope.goToItemDetails = function(item) {
      ProductService.selectedProduct = item;
      $state.go('menu.catalog.products.details', {
        itemNumber: item.itemnumber
      });
    };

    $scope.openExportModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/exportmodal.html',
        controller: 'ExportModalController',
        resolve: {
          location: function() {
            return {category:'Search', action:'Export Search Results'}
          },
          headerText: function () {
            return 'Product Catalog (limited to 500 items)';
          },
          exportMethod: function() {
            return ProductService.exportProducts;
          },
          exportConfig: function() {
            return ProductService.getExportConfig();
          },
          exportParams: function() {
            // return search url with params
            var sortDirection = $scope.sortReverse ? 'desc' : 'asc';
            var facets = ProductService.getFacets(
              $scope.facets.brands.selected,
              $scope.facets.mfrname.selected,
              $scope.facets.dietary.selected,
              $scope.facets.itemspecs.selected,
              $scope.facets.temp_zone.selected,
              $scope.facets.parentcategories.selected,
              $scope.facets.subcategories.selected
            );

            var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.itemIndex, $scope.sortField, sortDirection, facets, $stateParams.dept);
            return ProductService.getSearchUrl($scope.paramType, $scope.paramId, $scope.$state.params.catalogType) + '?' + jQuery.param(params); // search query string param
          }
        }
      });
    };

    $scope.bekCatalogSwitch = function () {
        //change state to unfi
        $state.go($state.current,{catalogType: "BEK"}, {reload: true});
    }

    $scope.unfiCatalogSwitch = function () {
        //change state to unfi
        $state.go($state.current,{catalogType: "UNFI"}, {reload: true});
    }
    // INIT
    loadProducts().then(refreshFacets);

  }]);
