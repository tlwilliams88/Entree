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
    
    CategoryService.getCategories().then(function(data) {
      $scope.categories = data.data.categories;
    });

    ProductService.getProducts().then(function(data) {
      $scope.featuredProducts = data.data.products;
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
