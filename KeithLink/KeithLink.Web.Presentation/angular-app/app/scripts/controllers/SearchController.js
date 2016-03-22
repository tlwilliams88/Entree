'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', '$state', '$stateParams', '$modal', '$analytics', '$filter', 'ProductService', 'CategoryService', 'Constants', 'PricingService', 'blockUI',
    function(
      $scope, $state, $stateParams, // angular dependencies
      $modal, // ui bootstrap library
      $analytics, //google analytics
      $filter,
      ProductService, CategoryService, Constants, PricingService, // bek custom services
      blockUI
    ) {

    // clear keyword search term at top of the page
    if ($scope.userBar) {
      $scope.userBar.universalSearchTerm = '';
    }

    // TODO: do not call these functions directly from view
    $scope.canOrderItem = PricingService.canOrderItem;
    $scope.hasCasePrice = PricingService.hasCasePrice;
    $scope.hasPrice = PricingService.hasPrice;
    $scope.hasPackagePrice = PricingService.hasPackagePrice;

    $scope.paramType = $stateParams.type; // Category, Search, Brand
    $scope.paramId = $stateParams.id; // search term, brand id, category id

    $scope.loadingResults = false;
    $scope.sortField = null;
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
      }
    };

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
        CategoryService.getCategories($state.params.catalogType).then(function(data) {
          angular.forEach(data.categories, function(item, index) {
            if (item.search_name === $scope.paramId) { // for the bread crumb, we map from the search name back to the display name
              displayText = item.name;
            }
          });

          $scope.featuredBreadcrumb = {
            click: clearFacets,
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
          click: clearFacets,
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

      // search term
      if ($scope.paramType === 'search') {
       $scope.featuredBreadcrumb = {
          click: clearFacets,
          clickData: null,
          displayText: $stateParams.deptName
        };
        breadcrumbs.unshift($scope.featuredBreadcrumb);
        $analytics.eventTrack('Search Department', {  category: 'Department', label: $stateParams.deptName });

        $scope.featuredBreadcrumb = {
          click: clearFacets,
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
      var facets = ProductService.getFacets(
        $scope.facets.categories.selected,
        $scope.facets.brands.selected,
        $scope.facets.mfrname.selected,
        $scope.facets.dietary.selected,
        $scope.facets.itemspecs.selected
      );
      var sortDirection = $scope.sortReverse ? 'desc' : 'asc';
      // console.log("catalog type in search controller: " + $scope.$state.params.catalogType);
      if($scope.sortField === 'itemnumber' && $state.params.catalogType != 'BEK'){
        $scope.sortField  = '';
      }
      var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.itemIndex, $scope.sortField, sortDirection, facets, $stateParams.dept);
      return ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType,params);
    }

    //Load list of products and block UI with message
    function loadProducts(appendResults) {
      $scope.loadingResults = true;


      return blockUI.start("Loading Products...").then(function(){
        return getData().then(function(data) {
        $scope.totalItems = data.totalcount;
        if (data.catalogCounts != null) {
            $scope.bekItemCount = data.catalogCounts.bek;
            $scope.unfiItemCount = data.catalogCounts.unfi;
        } else {
            $scope.bekItemCount = 0;
            $scope.unfiItemCount = 0;
        }


        var selectedBrands = [],
            selectedCategories = [],
            selectedDietary = [],
            selectedSpecs = [],
            selectedMfr = []

        // append results to existing data (for infinite scroll)
        if (appendResults) {
          $scope.products.push.apply($scope.products, data.products);
        // replace existing data (for sort, filter)
        } else {
          $scope.products = data.products;
          $scope.facets.brands.available.forEach(function(brand){
           var brandName = $filter('filter') (data.facets.brands, {name: brand.name})
           brand.count = 0;
           if(brandName.length > 0 && brand.name){
            selectedBrands.push(brandName[0]);
           }
          })
          $scope.facets.categories.available.forEach(function(category){
          var categoryName = $filter('filter') (data.facets.categories, {name: category.name})
           category.count = 0;
           if(categoryName.length > 0 && category.name){
            selectedCategories.push(categoryName[0]);
           }
          })
          $scope.facets.dietary.available.forEach(function(dietary){
          var dietaryName = $filter('filter') (data.facets.dietary, {name: dietary.name})
           dietary.count = 0;
           if(dietaryName.length > 0 && dietary.name){
            selectedDietary.push(dietaryName[0]);
           }
          })
          $scope.facets.itemspecs.available.forEach(function(itemSpec){
          var specName = $filter('filter') (data.facets.itemspecs, {name: itemSpec.name})
           itemSpec.count = 0;
           if(specName && specName.length > 0 && itemSpec.name){
            selectedSpecs.push(specName[0]);
           }
          })
          $scope.facets.mfrname.available.forEach(function(mfr){
          var mfrName = $filter('filter') (data.facets.mfrname, {name: mfr.name})
           mfr.count = 0;
           if(mfrName.length > 0 && mfr.name){
            selectedMfr.push(mfrName[0]);
           }
          })
          $scope.searchResults(selectedBrands, selectedCategories, selectedDietary, selectedSpecs, selectedMfr, $scope.facets);
          }

        setBreadcrumbs(data);

        blockUI.stop();

        delete $scope.searchMessage;
        
        return data.facets;
        })
      }, function(error) {
        $scope.searchMessage = 'Error loading products.';
      }).finally(function() {
        $scope.loadingResults = false;
      });
    }

  //Apply product count to each facet using temporary array's created in function loadProducts
   $scope.searchResults = function(selectedBrands, selectedCategories, selectedDietary, selectedSpecs, selectedMfr, availableFacets){
        
        availableFacets.brands.available.forEach(function(brand){
          var brandName = $filter('filter') (selectedBrands, {name: brand.name})
          if(brandName.length > 0){
            brand.count = brandName[0].count;
          }
        })
        availableFacets.categories.available.forEach(function(category){
          var categoryName = $filter('filter') (selectedCategories, {name: category.name})
          if(categoryName.length > 0){
          category.count = categoryName[0].count || 0;
          }
        })
        availableFacets.dietary.available.forEach(function(dietary){
          var dietaryName = $filter('filter') (selectedDietary, {name: dietary.name})
          if(dietaryName.length > 0) {
            dietary.count = dietaryName[0].count || 0;
          }
        })
        availableFacets.itemspecs.available.forEach(function(itemSpec){
          var specName = $filter('filter') (selectedSpecs, {name: itemSpec.name})
          if(specName.length > 0){
            itemSpec.count = specName[0].count || 0;
          }
        })
        availableFacets.mfrname.available.forEach(function(mfr){
          var mfrName = $filter('filter') (selectedMfr, {name: mfr.name})
          if(mfrName.length > 0){
            mfr.count = mfrName[0].count || 0;
          }
        })
    }

    /*************
    FACETS
    *************/
    function clearFacets() {
      $scope.facets.categories.selected = [];
      $scope.facets.brands.selected = [];
      $scope.facets.mfrname.selected = [];
      $scope.facets.dietary.selected = [];
      $scope.facets.itemspecs.selected = [];
      loadProducts().then(refreshFacets);
    }

    $scope.clearFacetsButton = function() {
      $scope.facets.categories.selected = [];
      $scope.facets.brands.selected = [];
      $scope.facets.mfrname.selected = [];
      $scope.facets.dietary.selected = [];
      $scope.facets.itemspecs.selected = [];
      loadProducts().then(refreshFacets);
      $scope.noFiltersSelected = true;
    }

    function refreshFacets(facets) {
      // set the $scope.facets object using the response data
      $scope.facets.categories.available = facets.categories;
      $scope.facets.brands.available = facets.brands;
      $scope.facets.mfrname.available = facets.mfrname;
      $scope.facets.dietary.available = facets.dietary;
      $scope.facets.itemspecs.available = addIcons(facets.itemspecs);
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

    $scope.infiniteScrollLoadMore = function() {
      if (($scope.products && $scope.products.length >= $scope.totalItems) || $scope.loadingResults) {
        return;
      }
      var sortfieldholder = $scope.sortField;
      $scope.itemIndex += $scope.itemsPerPage;
      loadProducts(true).then(function(){
        if(sortfieldholder === 'itemnumber' && $state.params.catalogType != 'BEK'){
          $scope.UNFISortByItemNumber(!$scope.itemNumberDesc);
        }
      });      
    };

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
              $scope.facets.itemspecs.selected
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
