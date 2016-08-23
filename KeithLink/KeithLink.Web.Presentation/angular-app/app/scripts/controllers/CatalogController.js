'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', '$state', 'OrderService', 'CategoryService', 'ProductService', 'CartService', 'BrandService', 'ListService',
    function ($scope, $state, OrderService, CategoryService, ProductService, CartService, BrandService, ListService) {
    
    $scope.myInterval = -1;

    CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });

    $scope.loadingRecentlyViewedItems = true;
    $scope.loadingrecentlyOrderedUnfiItems = true;
    $scope.loadingCategories = true;
    $scope.loadingBrands = true;
    $scope.loadingRecommendedItems = true;

    if ($state.params.catalogType === 'BEK') {
      $scope.pageTitle = 'Product Catalog';
      ProductService.getRecentlyViewedItems().then(function(items) {
        $scope.recentlyViewedItems = items;
        $scope.loadingRecentlyViewedItems = false;

        ListService.getRecommendedItems().then(function(items) {
          $scope.recommendedItems = items;
          $scope.loadingRecommendedItems = false;

          CategoryService.getCategories($state.params.catalogType).then(function(categories) {
            $scope.categories = categories;
            $scope.loadingCategories = false;

            BrandService.getHouseBrands().then(function(data){
              $scope.brands = data;
              $scope.loadingBrands = false;
            });
          });
        });
      });
    } else {
      $scope.pageTitle = 'Specialty Catalog';
      CategoryService.getCategories($state.params.catalogType).then(function(categories) {
        $scope.categories = categories;
        $scope.loadingCategories = false;

        OrderService.getRecentlyOrderedUNFIItems().then(function(recentlyOrdered){
          if(recentlyOrdered){
            $scope.recentlyOrderedUnfiItems = recentlyOrdered.items;
          }else{
            $scope.recentlyOrderedUnfiItems = [];
          }          
          $scope.loadingrecentlyOrderedUnfiItems = false;
        });
      });
    }

    $scope.clearRecentlyViewedItems = function(){
      ProductService.clearRecentlyViewedItems().then(function() {
        $scope.recentlyViewedItems = '';
        $scope.displayMessage('success', 'Successfully cleared recently viewed items.');
      });
    };

    $scope.clearRecentlyOrderedItems = function(){
      OrderService.clearRecentlyOrderedUNFIItems().then(function(){
        $scope.recentlyOrderedUnfiItems = '';
        $scope.displayMessage('success', 'Successfully cleared recently ordered items.');
      });
    };

  }]);
