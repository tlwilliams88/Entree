'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', 'ProductService', function ($scope, ProductService) {
    
    ProductService.getProducts().then(function(data){
      $scope.products = data.data.products;
      $scope.predicate = 'id';
      
    });



  }]);