'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', '$state', 'CategoryService', 'ProductService', 'CartService', 'BrandService', 'ListService',
    function ($scope, $state, CategoryService, ProductService, CartService, BrandService, ListService) {
    
    $scope.myInterval = -1;

    CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });

    $scope.loadingRecentlyViewedItems = true;
    $scope.loadingCategories = true;
    $scope.loadingBrands = true;
    $scope.loadingRecommendedItems = true;
    if ($state.params.catalogType == "BEK") {
        $scope.pageTitle = "Product Catalog";
    } else {
        $scope.pageTitle = "Specialty Catalog";
    }

    ProductService.getRecentlyViewedItems().then(function(response) {
      var recentItems = response.successResponse;
      $scope.loadingRecentlyViewedItems = false;
      $scope.recentlyViewedItems = recentItems;
    });

    ListService.getRecommendedItems().then(function(response) {
      var recomItems = response.successResponse;
      $scope.loadingRecommendedItems = false;
      $scope.recommendedItems = recomItems;
    });

    CategoryService.getCategories($state.params.catalogType).then(function(categories) {
      $scope.loadingCategories = false;
      $scope.categories = categories;
    });

    BrandService.getHouseBrands().then(function(brands){
      $scope.loadingBrands = false;
      $scope.brands = brands;
    });

  }]);
