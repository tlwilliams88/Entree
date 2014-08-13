'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', '$state', 'CategoryService', 'ProductService', 'BrandService', function ($scope, $state, CategoryService, ProductService, BrandService) {
    
    $scope.loadingCategories = true;
    $scope.loadingBrands = true;

    CategoryService.getCategories().then(function(data) {
      $scope.loadingCategories = false;
      $scope.categories = data.categories;
    });

    BrandService.getHouseBrands().then(function(data){
      $scope.loadingBrands = false;
      $scope.brands = data;
    });

  }]);
