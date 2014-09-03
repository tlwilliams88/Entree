'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', 'item', 'ProductService', 'ListService', 'CartService', function ($scope, item, ProductService, ListService, CartService) {
    
    $scope.item = item;
    $scope.item.quantity = 1;
    
    ProductService.getProductDetails(item.itemnumber).then(function(item) {
      $scope.item.productimages = item.productimages;
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

    $scope.canOrderProduct = function(item) {
      return ProductService.canOrderProduct(item);
    };
  }]);