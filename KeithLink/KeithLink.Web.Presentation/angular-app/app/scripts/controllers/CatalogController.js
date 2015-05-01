'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', '$state', 'CategoryService', 'ProductService', 'BrandService', 'ListService',
    function ($scope, $state, CategoryService, ProductService, BrandService, ListService) {
    
    $scope.myInterval = -1;

    $scope.loadingRecentlyViewedItems = true;
    $scope.loadingCategories = true;
    $scope.loadingBrands = true;
    $scope.loadingRecommendedItems = true;
    $scope.isMobile = ENV.mobileApp;
    $scope.glossaryUrl = "/Assets/help/Glossary.pdf";

    ProductService.getRecentlyViewedItems().then(function(items) {
      $scope.loadingRecentlyViewedItems = false;
      $scope.recentlyViewedItems = items;
    });

    ListService.getRecommendedItems().then(function(items) {
      $scope.loadingRecommendedItems = false;
      $scope.recommendedItems = items;
    });

    CategoryService.getCategories().then(function(data) {
      $scope.loadingCategories = false;
      $scope.categories = data.categories;
    });

    BrandService.getHouseBrands().then(function(data){
      $scope.loadingBrands = false;
      $scope.brands = data;
    });

  }]);
