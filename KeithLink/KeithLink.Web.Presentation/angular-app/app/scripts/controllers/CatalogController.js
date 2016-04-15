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

    ProductService.getRecentlyViewedItems().then(function(items) {
      $scope.recentlyViewedItems = items;
      $scope.loadingRecentlyViewedItems = false;
      
      ListService.getRecommendedItems().then(function(items) {
        $scope.recommendedItems = items;
        $scope.loadingRecommendedItems = false;
        
        CategoryService.getCategories($state.params.catalogType).then(function(data) {
          $scope.categories = data.categories;
          $scope.loadingCategories = false;
          
          BrandService.getHouseBrands().then(function(data){
            $scope.brands = data;
            $scope.loadingBrands = false;
            
          });
        });
      });
    });

    $scope.clearItems = function(items){
      if(items){
        ProductService.clearRecentlyViewedItems(items).then(function() {
          $scope.loadingRecentlyViewedItems = false;
          $scope.recentlyViewedItems = '';
          $scope.displayMessage('success', 'Successfully cleared recently viewed items.')
        })
      }

    };

  }]);
