'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', '$state', 'CategoryService', 'ProductService', function ($scope, $state, CategoryService, ProductService) {
    
    $scope.loadingCategories = true;

    CategoryService.getCategories().then(function(response) {
      $scope.loadingCategories = false;
      $scope.categories = response.data.categories;
    });

    $scope.brands = [
      {
        id: 1,
        imageUrl: 'images/admiral.jpg',
        name: 'Admiral Of The Fleet'
      },{
        id: 2,
        imageUrl: 'images/admiral.jpg',
        name: 'Cortona'
      },{
        id: 3,
        imageUrl: 'images/admiral.jpg',
        name: 'Ellington Farms'
      },{
        id: 4,
        imageUrl: 'images/admiral.jpg',
        name: 'Golden Harvest'
      }
    ];

  }]);
