'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$stateParams', 'ProductService', function ($scope, $stateParams, ProductService) {
    
    ProductService.getProduct($stateParams.itemId).then(function(response) {
      $scope.item = response.data;
    });

  }]);
