'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', '$state', '$stateParams', '$modal', 'ProductService', 'CategoryService', 'Constants', 'PricingService',
    function(
      $scope, $state, $stateParams, // angular dependencies
      $modal, // ui bootstrap library
      ProductService, CategoryService, Constants, PricingService // bek custom services
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
    $scope.isMobile = ENV.mobileApp;
    $scope.glossaryUrl = "/Assets/help/Glossary.pdf";

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
        CategoryService.getCategories().then(function(data) {
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
          if (brand.brand_control_label.toUpperCase() === $scope.paramId.toUpperCase()) {
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
          clickData: '',
          displayText: '"' + $scope.paramId + '"'
        };
        breadcrumbs.push($scope.featuredBreadcrumb);  
      }

      $scope.breadcrumbs = breadcrumbs;
      $scope.filterCount = filterCount;
    }

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
      var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.itemIndex, $scope.sortField, sortDirection, facets);
      return ProductService.searchCatalog($scope.paramType, $scope.paramId, params);
    }

    function loadProducts(appendResults) {
      $scope.loadingResults = true;

      return getData().then(function(data) {
        $scope.totalItems = data.totalcount;

        // append results to existing data (for infinite scroll)
        if (appendResults) {
          $scope.products.push.apply($scope.products, data.products);
        // replace existing data (for sort, filter)
        } else {
          $scope.products = data.products;
        }

        setBreadcrumbs(data);

        delete $scope.searchMessage;
        
        return data.facets;
      }, function(error) {
        $scope.searchMessage = 'Error loading products.';
      }).finally(function() {
        $scope.loadingResults = false;
      });
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
          itemSpec.displayname = 'Item Being Replaced';
          itemSpec.iconclass = 'text-red icon-cycle';
          break;
        case 'replacementitem':
          itemSpec.displayname = 'Replacement Item';
          itemSpec.iconclass = 'text-green icon-cycle';
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

      $scope.itemIndex += $scope.itemsPerPage;
      loadProducts(true);
    };

    $scope.toggleSelection = function(facetList, selectedFacet) {
      $scope.itemsPerPage = 50;
      $scope.itemIndex = 0;

      var idx = facetList.indexOf(selectedFacet);
      if (idx > -1) {
        facetList.splice(idx, 1);
      } else {
        facetList.push(selectedFacet);
      }

      loadProducts().then(refreshFacets);
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
            var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.itemIndex, $scope.sortField, sortDirection, facets);
            return ProductService.getSearchUrl($scope.paramType, $scope.paramId) + '?' + jQuery.param(params); // search query string param
          }
        }
      });
    };

    // INIT
    loadProducts().then(refreshFacets);

  }]);