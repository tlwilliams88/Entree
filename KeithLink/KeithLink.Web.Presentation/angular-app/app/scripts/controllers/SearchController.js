'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', 'ProductService', 'CategoryService', '$stateParams',
    function($scope, ProductService, CategoryService, $stateParams) {
      //debugger;
      // clear keyword search term at top of the page
      if ($scope.userBar) {
        $scope.userBar.universalSearchTerm = '';
      }

      $scope.loadingResults = true;

      $scope.itemsPerPage = 30;
      $scope.itemIndex = 0;

      $scope.oneAtATime = true;
      $scope.items = ['Item 1', 'Item 2', 'Item 3'];
      $scope.selectedCategory = '';
      $scope.selectedSubcategory = '';
      $scope.selectedBrands = [];
      $scope.selectedAllergens = [];
      $scope.isBrandShowing = false;
      $scope.brandHiddenNumber = 3;
      $scope.isAllergenShowing = false;
      $scope.allergenHiddenNumber = 3;
      $scope.isSpecShowing = false;
      $scope.specHiddenNumber = 3;
      $scope.hidden = true;

      function getData() {
        var type = $stateParams.type;
        var branchId = $scope.currentUser.currentLocation.branchId;

        if (type === 'category') {

          var categoryId = $stateParams.id;
          return ProductService.getProductsByCategory(branchId, categoryId, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory);

        } else if (type === 'search') {

          var searchTerm = $stateParams.id;
          $scope.searchTerm = "\"" + searchTerm + "\"";
          return ProductService.getProducts(branchId, searchTerm, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory);
        } else if (type === 'brand') {

          var brandName = $stateParams.id;
          return ProductService.getProducts(branchId, brandName, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory);
        }
      }

      function loadProducts(appendResults) {
        $scope.loadingResults = true;

        return getData().then(function(data) {
          $scope.filterCount = 0;

          // append results to existing data
          if (appendResults) {
            $scope.products.push.apply($scope.products, data.products);
            // replace existing data
          } else {
            $scope.products = data.products;
          }
          $scope.totalItems = data.totalcount;

          $scope.breadcrumbs = [];
          //check initial page view
          if ($stateParams.type === 'category') {
            $scope.breadcrumbs.push({
              type: "category",
              id: $stateParams.id,
              name: $stateParams.id
            });
            $scope.filterCount++;
          }
          if ($stateParams.type === 'brand') {
            $scope.selectedBrands.push($stateParams.id);
            $scope.filterCount++;
          }

          //check for selected facets
          if ($scope.selectedCategory) {
            $scope.breadcrumbs.push({
              type: "category",
              id: $scope.selectedCategory,
              name: $scope.selectedCategory
            });
            $scope.filterCount++;
          }
          var brandsBreadcrumb = "Brand: ";
          angular.forEach($scope.selectedBrands, function(item, index) {
            brandsBreadcrumb += item + " or ";
            $scope.filterCount++;
          });
          if (brandsBreadcrumb != "Brand: ") {
            $scope.breadcrumbs.push({
              type: "brand",
              id: $scope.selectedBrands,
              name: brandsBreadcrumb.substr(0, brandsBreadcrumb.length - 4)
            });
          }
          if ($stateParams.type === 'search') {
            $scope.breadcrumbs.push({
              type: "search",
              id: "search",
              name: "\"" + $stateParams.id + "\""
            });
          }
          $scope.loadingResults = false;

          return data.facets;
        });
      }

      loadProducts().then(function(facets) {
        $scope.categories = facets.categories;
        $scope.brands = facets.brands;
        $scope.allergens = facets.allergens;
      });

      $scope.infiniteScrollLoadMore = function() {

        if ($scope.products.length >= $scope.totalItems || $scope.loadingResults) {
          return;
        }

        $scope.itemIndex += $scope.itemsPerPage;

        console.log('more: ' + $scope.itemIndex);
        loadProducts(true);
      };

      $scope.breadcrumbClickEvent = function(type, id) {
        if (type === "category") {
          $scope.selectedBrands = [];
          loadProducts().then(function(facets) {
            $scope.categories = facets.categories;
          });
        }
        if (type === "brand") {
          $scope.selectedBrands = id;
          loadProducts();
        }
      }

      $scope.showContextMenu = function(e, idx) {
        $scope.contextMenuLocation = {
          'top': e.y,
          'left': e.x
        };
        $scope.isContextMenuDisplayed = true;
      };

      $scope.showBrand = function() {
        $scope.isBrandShowing = true;
        $scope.brandHiddenNumber = 100;
      };
      $scope.hideBrand = function() {
        $scope.isBrandShowing = false;
        $scope.brandHiddenNumber = 3;
      };

      $scope.showAllergen = function() {
        $scope.isAllergenShowing = true;
        $scope.allergenHiddenNumber = 100;
      };
      $scope.hideAllergen = function() {
        $scope.isAllergenShowing = false;
        $scope.allergenHiddenNumber = 3;
      };

      $scope.showSpec = function() {
        $scope.isSpecShowing = true;
        $scope.specHiddenNumber = 100;
      };
      $scope.hideSpec = function() {
        $scope.isSpecShowing = false;
        $scope.specHiddenNumber = 3;
      };

      $scope.toggleSelection = function toggleSelection(selectedFacet, filter) {
        //selectedFacet.show = !selectedFacet.show;
        var idx;
        if (filter === 'brand') {
          idx = $scope.selectedBrands.indexOf(selectedFacet);

          // is currently selected
          if (idx > -1) {
            $scope.selectedBrands.splice(idx, 1);
          }
          // is newly selected
          else {
            $scope.selectedBrands.push(selectedFacet);
          }

          loadProducts().then(function(facets) {
            $scope.categories = facets.categories;
          });
        } else if (filter === 'allergen') {
          idx = $scope.selectedAllergens.indexOf(selectedFacet);

          // is currently selected
          if (idx > -1) {
            $scope.selectedAllergens.splice(idx, 1);
          }
          // is newly selected
          else {
            $scope.selectedAllergens.push(selectedFacet);
          }
        } else if (filter === 'subcategory') {
          $scope.selectedSubcategory = selectedFacet.id;
        } else {
          $scope.selectedCategory = selectedFacet.name;
          $scope.selectedSubcategory = '';

          loadProducts().then(function(facets) {
            $scope.brands = facets.brands;
            $scope.allergens = facets.allergens;
          });
        }
      };

      $scope.itemSpecifications = [{
        id: 1,
        name: 'Item Being Replaced',
        iconClass: 'text-red icon-cycle'
      }, {
        id: 2,
        name: 'Replacement Item',
        iconClass: 'text-green icon-cycle'
      }, {
        id: 3,
        name: 'Deviated Cost',
        iconClass: 'text-regular icon-dollar'
      }, {
        id: 4,
        name: 'Item Details Shet',
        iconClass: 'text-regular icon-cell-sheet'
      }, {
        id: 5,
        name: 'Child Nutrition Sheet',
        iconClass: 'text-regular icon-apple'
      }, {
        id: 6,
        name: 'Non-Stock Item',
        iconClass: 'text-regular icon-user'
      }, {
        id: 7,
        name: 'Material Safety Data Sheet',
        iconClass: 'text-regular icon-safety'
      }];

    }
  ]);