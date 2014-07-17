'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', 'CategoryService', 'ProductService', function ($scope, CategoryService, ProductService) {
    
    CategoryService.getCategories().then(function() {
      $scope.categories = CategoryService.categories;
    });
    
    ProductService.getProducts().then(function() {
      $scope.featuredProducts = ProductService.products;
    });

    $scope.brands = [
      {
        id: 1,
        name: 'Admiral Of The Fleet'
      },{
        id: 2,
        name: 'Cortona'
      },{
        id: 3,
        name: 'Ellington Farms'
      },{
        id: 4,
        name: 'Golden Harvest'
      }
    ];

  }]);
