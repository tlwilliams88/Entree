'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$stateParams', 'ProductService', 'ListService', 'OrderService', function ($scope, $stateParams, ProductService, ListService, OrderService) {
    
    var itemNumber = $stateParams.itemNumber;
    $scope.loadingDetails = true;
    ProductService.getProductDetails(itemNumber).then(function(response) {
      $scope.item = response.data;
      $scope.loadingDetails = false;
    });

    // TODO: move into context menu controller
    $scope.lists = ListService.lists;
    ListService.getAllLists({
      'header': true
    });

    $scope.orders = OrderService.orders;
    OrderService.getAllOrders({
      'header': true
    });
  }]);