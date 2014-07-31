'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', '$state', 'categories', 'ProductService', function ($scope, $state, categories, ProductService) {
    
    $scope.categories = categories;

    // $scope.loadingCategories = true;

    // CategoryService.getCategories().then(function(data) {
    //   $scope.loadingCategories = false;
    //   $scope.categories = data.categories;
    // });

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
