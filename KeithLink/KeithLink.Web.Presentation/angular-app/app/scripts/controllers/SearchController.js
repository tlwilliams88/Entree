'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', 'ProductService', 'CategoryService', 'ListService', '$stateParams',
    function($scope, ProductService, CategoryService, ListService, $stateParams) {
      // clear keyword search term at top of the page
      if ($scope.userBar) {
        $scope.userBar.universalSearchTerm = '';
      }

      $scope.loadingResults = true;

      $scope.itemsPerPage = 30;
      $scope.itemIndex = 0;

      $scope.oneAtATime = true;
      $scope.selectedCategory = '';
      $scope.selectedSubcategory = '';
      $scope.selectedBrands = [];
      $scope.selectedAllergens = [];
      $scope.selectedDietary = [];
      $scope.selectedSpecs = [];
      $scope.isBrandShowing = false;
      $scope.isAllergenShowing = false;
      $scope.isDietaryShowing = false;
      $scope.isSpecShowing = false;
      $scope.brandHiddenNumber = 3;
      $scope.allergenHiddenNumber = 3;
      $scope.dietaryHiddenNumber = 3;
      $scope.specHiddenNumber = 3;
      $scope.constantHiddenNumber = 4;
      $scope.brandCount = 0;
      $scope.allergenCount = 0;
      $scope.dietaryCount = 0;
      $scope.specCount = 0;
      $scope.hidden = true;
      $scope.sortField = "itemnumber";
      $scope.sortDirection = "asc";
      $scope.asc = true;
      $scope.paramType = $stateParams.type;
      $scope.categoryName = '';

      function getCategoryById(categoryId) {
        return CategoryService.getCategories().then(function(data) {
          angular.forEach(data.categories, function(item, index) {
            if (item.id === categoryId)
              $scope.categoryName = item.name;
          });
          return ProductService.getProductsByCategory(categoryId, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedAllergens, $scope.selectedDietary, $scope.selectedSpecs, $scope.sortField, $scope.sortDirection);
        });
      }

      function getData() {
        var type = $stateParams.type;
        var branchId = $scope.currentUser.currentLocation.branchId;

        if (type === 'category') {
          var categoryId = $stateParams.id;
          return getCategoryById(categoryId);

        } else if (type === 'search') {

          var searchTerm = $stateParams.id;
          $scope.searchTerm = "\"" + searchTerm + "\"";
          return ProductService.getProducts(searchTerm, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedAllergens, $scope.selectedDietary, $scope.selectedSpecs, $scope.sortField, $scope.sortDirection);
        } else if (type === 'brand') {
          var brandName = $stateParams.id;
          $scope.selectedBrands.push(brandName);
          return ProductService.getProducts('', $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedAllergens, $scope.selectedDietary, $scope.selectedSpecs, $scope.sortField, $scope.sortDirection);
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
              type: "topcategory",
              id: $stateParams.id,
              name: $scope.categoryName
            });
            $scope.filterCount++;
          }
          if ($stateParams.type === 'search') {
            $scope.breadcrumbs.push({
              type: "allcategories",
              id: $stateParams.id,
              name: "All Categories"
            });
          }

          //check for selected facets
          if ($scope.selectedCategory) {
            $scope.breadcrumbs.push({
              type: "category",
              id: $scope.selectedCategory.name,
              name: $scope.selectedCategory.categoryname
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
          var allergensBreadcrumb = "Allergens: ";
          angular.forEach($scope.selectedAllergens, function(item, index) {
            allergensBreadcrumb += item + " or ";
            $scope.filterCount++;
          });
          if (allergensBreadcrumb != "Allergens: ") {
            $scope.breadcrumbs.push({
              type: "allergen",
              id: $scope.selectedAllergens,
              name: allergensBreadcrumb.substr(0, allergensBreadcrumb.length - 4)
            });
          }
          var dietaryBreadcrumb = "Dietary: ";
          angular.forEach($scope.selectedDietary, function(item, index) {
            dietaryBreadcrumb += item + " or ";
            $scope.filterCount++;
          });
          if (dietaryBreadcrumb != "Dietary: ") {
            $scope.breadcrumbs.push({
              type: "dietary",
              id: $scope.selectedDietary,
              name: dietaryBreadcrumb.substr(0, dietaryBreadcrumb.length - 4)
            });
          }
          var specBreadcrumb = "Item Specifications: ";
          angular.forEach($scope.selectedSpecs, function(item, index) {
            specBreadcrumb += changeSpecDisplayName(item) + " or ";
            $scope.filterCount++;
          });
          if (specBreadcrumb != "Item Specifications: ") {
            $scope.breadcrumbs.push({
              type: "spec",
              id: $scope.selectedSpecs,
              name: specBreadcrumb.substr(0, specBreadcrumb.length - 4)
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
          $scope.brandCount = data.facets.brands.length;
          $scope.allergenCount = data.facets.allergens.length;
          $scope.dietaryCount = data.facets.dietary.length;
          $scope.specCount = data.facets.itemspecs.length;

          return data.facets;
        });
      }

      loadProducts().then(function(facets) {
        refreshScopeFacets(facets);
      });

      $scope.infiniteScrollLoadMore = function() {

        if (($scope.products && $scope.products.length >= $scope.totalItems) || $scope.loadingResults) {
          return;
        }

        $scope.itemIndex += $scope.itemsPerPage;

        console.log('more: ' + $scope.itemIndex);
        loadProducts(true);
      };

      $scope.breadcrumbClickEvent = function(type, id) {
        $scope.loadingResults = true;
        if (type === "topcategory" || type === "allcategories") {
          $scope.selectedBrands = [];
          $scope.selectedAllergens = [];
          $scope.selectedSpecs = [];
          $scope.selectedDietary = [];
          $scope.selectedCategory = '';
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        }
        if (type === "category") {
          $scope.selectedBrands = [];
          $scope.selectedAllergens = [];
          $scope.selectedSpecs = [];
          $scope.selectedDietary = [];
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        }
        if (type === "brand") {
          $scope.selectedAllergens = [];
          $scope.selectedSpecs = [];
          $scope.selectedDietary = [];
          $scope.selectedBrands = id;
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        }
        if (type === "allergen") {
          $scope.selectedBrands = [];
          $scope.selectedSpecs = [];
          $scope.selectedDietary = [];
          $scope.selectedAllergens = id;
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        }
        if (type === "dietary") {
          $scope.selectedBrands = [];
          $scope.selectedSpecs = [];
          $scope.selectedAllergens = [];
          $scope.selectedDietary = id;
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        }
        if (type === "spec") {
          $scope.selectedBrands = [];
          $scope.selectedAllergens = [];
          $scope.selectedDietary = [];
          $scope.selectedSpecs = id;
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
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

      $scope.showDietary = function() {
        $scope.isDietaryShowing = true;
        $scope.dietaryHiddenNumber = 100;
      };
      $scope.hideDietary = function() {
        $scope.isDietaryShowing = false;
        $scope.dietaryHiddenNumber = 3;
      };

      $scope.showSpec = function() {
        $scope.isSpecShowing = true;
        $scope.specHiddenNumber = 100;
      };
      $scope.hideSpec = function() {
        $scope.isSpecShowing = false;
        $scope.specHiddenNumber = 3;
      };

      $scope.sortTable = function sortTable(field) {
        if ($scope.sortField != field) {
          $scope.sortField = field;
          $scope.sortDirection = "asc";
          $scope.asc = true;
        } else {
          if ($scope.sortDirection == "asc") {
            $scope.sortDirection = "desc";
            $scope.asc = false;
          } else {
            $scope.sortDirection = "asc";
            $scope.asc = true;
          }
        }
        loadProducts();
      }

      $scope.toggleSelection = function toggleSelection(selectedFacet, filter) {
        $scope.loadingResults = true;
        $scope.itemsPerPage = 30;
        $scope.itemIndex = 0;
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
            refreshScopeFacets(facets);
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
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        } else if (filter === 'dietary') {
          idx = $scope.selectedDietary.indexOf(selectedFacet);

          // is currently selected
          if (idx > -1) {
            $scope.selectedDietary.splice(idx, 1);
          }
          // is newly selected
          else {
            $scope.selectedDietary.push(selectedFacet);
          }
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        } else if (filter === 'spec') {
          idx = $scope.selectedSpecs.indexOf(selectedFacet);

          // is currently selected
          if (idx > -1) {
            $scope.selectedSpecs.splice(idx, 1);
          }
          // is newly selected
          else {
            $scope.selectedSpecs.push(selectedFacet);
          }
          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        } else if (filter === 'subcategory') {
          $scope.selectedSubcategory = selectedFacet.id;
        } else {
          $scope.selectedCategory = selectedFacet;
          $scope.selectedSubcategory = '';

          loadProducts().then(function(facets) {
            refreshScopeFacets(facets);
          });
        }
      };

      function refreshScopeFacets(facets) {
        $scope.categories = facets.categories;
        $scope.brands = facets.brands;
        $scope.allergens = facets.allergens;
        $scope.dietary = facets.dietary;
        if (facets.itemspecs && facets.itemspecs.length > 0) {
          $scope.itemspecs = addIcons(facets.itemspecs);
        } else {
          $scope.itemspecs = [];
        }
        $scope.nonstock = facets.nonstock;
      }

      function addIcons(itemspecs) {
        var itemspecsArray = [];
        angular.forEach(itemspecs, function(item, index) {
          var itemname = '';
          var itemcount = 0;
          //if coming from bookmark, set item name
          if(!item.name)
          {
            if(item){
              itemname = item;
            }
          }
          else{
            itemname = item.name;
            itemcount = item.count;
          }

          if (itemname === "itembeingreplaced") {
            itemspecsArray.push({
              name: itemname,
              displayname: "Item Being Replaced",
              iconclass: "text-red icon-cycle",
              count: itemcount
            });
          }
          if (itemname === "replacementitem") {
            itemspecsArray.push({
              name: itemname,
              displayname: "Replacement Item",
              iconclass: "text-green icon-cycle",
              count: itemcount
            });
          }
          if (itemname === "cndoc") {
            itemspecsArray.push({
              name: itemname,
              displayname: "Child Nutrition Sheet",
              iconclass: "text-regular icon-apple",
              count: itemcount
            });
          }
          //THESE ITEM.NAMES ARE CURRENTLY JUST GUESSES --- I HAVE NOT SEEN WHAT THESE 4 ARE CALLED YET
          if (itemname === "DeviatedCost") {
            itemspecsArray.push({
              name: itemname,
              displayname: "DeviatedCost",
              iconclass: "text-regular icon-dollar",
              count: itemcount
            });
          }
          if (itemname === "ItemDetailsSheet") {
            itemspecsArray.push({
              name: itemname,
              displayname: "Item Details Sheet",
              iconclass: "text-regular icon-cell-sheet",
              count: itemcount
            });
          }
          if (item.name === "NonStock") {
            itemspecsArray.push({
              name: itemname,
              displayname: "Non-Stock Item",
              iconclass: "text-regular icon-user",
              count: itemcount
            });
          }
          if (item.name === "MaterialSafety") {
            itemspecsArray.push({
              name: itemname,
              displayname: "Material Safety Data Sheet",
              iconclass: "text-regular icon-safety",
              count: itemcount
            });
          }
        });
        return itemspecsArray;
      }

      function changeSpecDisplayName(name){
        if(name === "itembeingreplaced")
          return "Item Being Replaced"
        if(name === "replacementitem")
          return "Replacement Item"
        if(name === "cndoc")
          return "Child Nutrition Sheet"
      }

      ListService.getAllLists({
        'header': true
      }).then(function(data) {
        $scope.lists = data;
      });
    }
  ]);