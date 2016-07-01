'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', '$state', '$stateParams', '$modal', '$analytics', '$filter', '$timeout', 'ProductService', 'CategoryService', 'Constants', 'PricingService', 'CartService', 'blockUI',
    function(
      $scope, $state, $stateParams, // angular dependencies
      $modal, // ui bootstrap library
      $analytics, //google analytics
      $filter,
      $timeout,
      ProductService, CategoryService, Constants, PricingService, CartService, // bek custom services
      blockUI,
      LocalStorage,
      PagingModel
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

    $scope.selectedSortParameter = 'Item #';
    $scope.sortParametervalue = 'itemnumber';
    $scope.sortReverse = false;

    $scope.itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemIndex = 0;

    $scope.numberFacetsToShow = 4;  // determines when to show the 'Show More' link for each facet
    $scope.maxSortCount = 200;      // max number of items that can be sorted by price



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
      }
    };

    $scope.selectSortParameter = function(parametername, parametervalue){
      $scope.selectedSortParameter = parametername;
      $scope.sortParametervalue = parametervalue;
    }

    $scope.sortParameters = [{
      name: 'Item #',
      value: 'itemnumber'
    }, {
      name: 'Name',
      value: 'name'
    }, {
      name: 'Brand',
      value: 'brand_extended_description'
    }];

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

    $scope.blockUIAndChangePage = function(page){
      $(document).ready(function(){
        $("html, body").animate({ scrollTop: 0 }, 500);
      })
      
      $scope.startingPoint = 0;
      $scope.endPoint = 0;       
      var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});
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

    $scope.pagingPageSize = 25;

     $scope.setStartAndEndPoints = function(page){
      var foundStartPoint = false;
        page.forEach(function(item, index){
          if(page && item.itemnumber === page[0].itemnumber){
            $scope.startingPoint = index;
            $scope.endPoint = angular.copy($scope.startingPoint + $scope.pagingPageSize);
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
    
     $scope.pageChanged = function(page) {      
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.loadingPage = true;    
      $scope.currentPage = page.currentPage;
      $scope.startingPoint = ((page.currentPage - 1)*$scope.pagingPageSize) + 1;
      $scope.endPoint = angular.copy($scope.startingPoint + $scope.pagingPageSize);
      $scope.firstPageItem = ($scope.currentPage * $scope.pagingPageSize) - ($scope.pagingPageSize - 1);
      $scope.setRange();
      var visited = $filter('filter')($scope.visitedPages, {page: $scope.currentPage});
      if(!visited.length){
        var sortDirection = $scope.sortReverse ? 'desc' : 'asc';
        var params = ProductService.getSearchParams(null, $scope.startingPoint, $scope.sortField, sortDirection, $stateParams.dept);
        ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType,params, $stateParams.deptName).then(function(data){
          $scope.products = data.products;
        })
        blockUI.stop();
        blockUI.stop();
      }else {
        $scope.setStartAndEndPoints($scope.products);
        $scope.visitedPages.forEach(function(page){
          if(page.page == $scope.currentPage){
            $scope.products = page.items;
          }
        })
      }
     };

    $scope.setRange = function(){
      $scope.endPoint = $scope.endPoint;
      $scope.rangeStart = $scope.startingPoint;
      $scope.rangeEnd = ($scope.endPoint > $scope.totalProducts) ? $scope.totalProducts : $scope.endPoint - 1;
      if($scope.rangeStart === 0){
        $scope.rangeStart++;
        if($scope.rangeEnd === $scope.pagingPageSize - 1){
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
        $scope.firstPageItem = ($scope.currentPage * $scope.pagingPageSize) - ($scope.pagingPageSize);
        $scope.products = $scope.products.slice($scope.firstPageItem, ($scope.currentPage * $scope.pagingPageSize));
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

      if ($scope.paramType === 'category') {
        CategoryService.getCategories($state.params.catalogType).then(function(categories) {
          angular.forEach(categories, function(item, index) {
            if (item.search_name === $scope.paramId) { // for the bread crumb, we map from the search name back to the display name
              displayText = item.name;
            }
          });

          $scope.featuredBreadcrumb = {
            click: $scope.clearFacets,
            clickData: null,
            displayText: displayText
          };
          breadcrumbs.unshift($scope.featuredBreadcrumb);
        });
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

      // categories
      if ($scope.facets.categories.selected.length > 0) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.categories.selected = data;
            $scope.facets.brands.selected = [];
            $scope.facets.dietary.selected = [];
            $scope.facets.itemspecs.selected = [];
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.categories.selected,
          displayText: $scope.facets.categories.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.categories.selected.length;
      }

      // brands
      if ($scope.facets.brands.selected.length > 0) {
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
      }

      // manufacturers
      if ($scope.facets.mfrname.selected.length > 0) {
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
      }

      // dietary
      if ($scope.facets.dietary.selected.length > 0) {
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
      }

      // item specifications
      if ($scope.facets.itemspecs.selected.length > 0) {
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
      }

      // temp zones
      if ($scope.facets.temp_zone.selected.length > 0) {
        breadcrumbs.push({
          click: function(data) {
            $scope.facets.temp_zone.selected = data;
            loadProducts().then(refreshFacets);
          },
          clickData: $scope.facets.temp_zone.selected,
          displayText: 'Temp Zone: ' + $scope.facets.temp_zone.selected.join(breadcrumbSeparator)
        });
        filterCount += $scope.facets.temp_zone.selected.length;
      }

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

      $scope.breadcrumbs = breadcrumbs;
      $scope.filterCount = filterCount;
    }
      
    $scope.itemNumberDesc = false;
    $scope.UNFISortByItemNumber= function(ascendingDate) {
      $scope.sortField = 'itemnumber'
      
      if($state.params.catalogType != 'BEK'){
        $scope.products = $scope.products.sort(function(obj1, obj2){
          var sorterval1 = parseInt(obj1.itemnumber);
          var sorterval2 = parseInt(obj2.itemnumber);

          $scope.itemNumberDesc = !ascendingDate;  

          if(ascendingDate){      
            return sorterval1 - sorterval2;
          }
          else{
            return sorterval2 - sorterval1;
          }   
        });
      }
      else{
        $scope.sortTable('itemnumber');
      }
    };

    /*************
    LOAD PRODUCT DATA
    *************/
    function getData() {
      var facets;
      $scope.aggregateCount;
      
      $scope.aggregateCount = ($scope.facets.brands.selected.length + $scope.facets.itemspecs.selected.length + $scope.facets.categories.selected.length + $scope.facets.dietary.selected.length + $scope.facets.mfrname.selected.length + $scope.facets.temp_zone.selected.length)

      if($scope.aggregateCount !== 0){
        facets = ProductService.getFacets(
          $scope.facets.categories.selected,
          $scope.facets.brands.selected,
          $scope.facets.mfrname.selected,
          $scope.facets.dietary.selected,
          $scope.facets.itemspecs.selected,
          $scope.facets.temp_zone.selected
        )
      }
      var sortDirection = $scope.sortReverse ? 'desc' : 'asc';
      // console.log("catalog type in search controller: " + $scope.$state.params.catalogType);
      if($scope.sortField === 'itemnumber' && $state.params.catalogType != 'BEK'){
        $scope.sortField  = '';
      }
      var params = ProductService.getSearchParams(null, $scope.itemIndex, $scope.sortField, sortDirection, facets, $stateParams.dept);
      return ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType,params, $stateParams.deptName);
    }

    //Load list of products and block UI with message
    function loadProducts(appendResults) {
      startLoading();
      return blockUI.start("Loading Products...").then(function(){
        return getData().then(function(data) {
        var page = 1;
        $scope.products = data.products;
        $scope.totalProducts = data.totalcount;
        resetPage(data.products, true);
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
        if($scope.aggregateCount !==0){
          updateFacetCount($scope.facets.brands, data.facets.brands);
          updateFacetCount($scope.facets.itemspecs, data.facets.itemspecs);
          updateFacetCount($scope.facets.categories, data.facets.categories);
          updateFacetCount($scope.facets.dietary, data.facets.dietary);
          updateFacetCount($scope.facets.mfrname, data.facets.mfrname);
          updateFacetCount($scope.facets.temp_zone, data.facets.temp_zone);
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
      facets.available.forEach(function(facet){
        var facetName = $filter('filter') (data, {name: facet.name})
        facet.count = 0;
        if(facetName.length > 0 && facet.name){
          facet.count = facetName[0].count;
        }
      })
    }

    $scope.clearFacets = function() {
      $scope.facets.categories.selected = [];
      $scope.facets.brands.selected = [];
      $scope.facets.mfrname.selected = [];
      $scope.facets.dietary.selected = [];
      $scope.facets.itemspecs.selected = [];
      $scope.facets.temp_zone.selected = [];
      loadProducts().then(refreshFacets);
      $scope.noFiltersSelected = true;
    }

    function refreshFacets(facets) {
      // set the $scope.facets object using the response data
      if(facets){
        $scope.facets.categories.available = facets.categories;
        $scope.facets.brands.available = facets.brands;
        $scope.facets.mfrname.available = facets.mfrname;
        $scope.facets.dietary.available = facets.dietary;
        $scope.facets.itemspecs.available = addIcons(facets.itemspecs);
        $scope.facets.temp_zone.available = facets.temp_zone;
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
    CLICK EVENTS
    *************/
    $scope.sortTable = function(field) {
      $scope.itemsPerPage = 50;
      $scope.itemIndex = 0;

      if (field !== 'caseprice' || $scope.totalItems <= $scope.maxSortCount) {
        if ($scope.sortField !== field) { // different field
          $scope.sortField = field;
          $scope.sortReverse = false;
        } else { // same field
          $scope.sortReverse = !$scope.sortReverse;
        }
        loadProducts();
      }
    };

    $scope.sortResults = function(sortdirection, sortfield){
      startLoading();
      return blockUI.start("Loading Products...").then(function(){
        var params = ProductService.getSearchParams(null, $scope.startingPoint, sortfield, sortdirection, $stateParams.dept);
        ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType, params, $stateParams.deptName).then(function(data){
          $scope.products = data.products;
        })
      stopLoading();
      blockUI.stop();
      })
    }

    $scope.toggleSelection = function(facetList, selectedFacet) {
      $scope.noFiltersSelected = false;
      $scope.itemsPerPage = 50;
      $scope.itemIndex = 0;

      var idx = facetList.indexOf(selectedFacet);
      if (idx > -1) {
        facetList.splice(idx, 1);
      } else {
        facetList.push(selectedFacet);
      }

      loadProducts().then($scope.refreshFacets);
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
              $scope.facets.categories.selected,
              $scope.facets.brands.selected,
              $scope.facets.mfrname.selected,
              $scope.facets.dietary.selected,
              $scope.facets.itemspecs.selected,
              $scope.facets.temp_zone.selected
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
