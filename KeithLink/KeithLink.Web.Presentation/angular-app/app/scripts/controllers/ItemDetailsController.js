'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$stateParams', 'ProductService', 'ListService', 'CartService', function ($scope, $stateParams, ProductService, ListService, CartService) {
    
    var itemNumber = $stateParams.itemNumber;
    $scope.loadingDetails = true;
    ProductService.getProductDetails(itemNumber).then(function(response) {
      $scope.item = response.data;
      $scope.item.quantity = 1;
      $scope.loadingDetails = false;
    });

    // TODO: move into context menu controller
    $scope.lists = ListService.lists;
    ListService.getAllLists({
      'header': true
    });

    $scope.carts = CartService.carts;
    CartService.getAllCarts({
      'header': true
    });
  }]);