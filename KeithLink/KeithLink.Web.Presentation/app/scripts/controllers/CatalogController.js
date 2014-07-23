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

    $scope.categories = [
      { 
        id: 'BP000',
        name: 'Fresh Meat',
        description: 'Fresh Meat',
        subcategories: null,
        active: false
    }, { 
        id: 'BP000',
        name: 'Fresh Meat',
        description: 'Fresh Meat',
        subcategories: null,
        active: false
    } ];

  }]);
