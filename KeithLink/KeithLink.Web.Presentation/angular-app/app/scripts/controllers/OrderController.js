'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:OrderController
 * @description
 * # OrderController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('OrderController', ['$scope', 'OrderService', function($scope,  OrderService) {
    
    $scope.loadingResults = true;

    $scope.orders = OrderService.orders;
    
    OrderService.getAllOrders().then(function(data) {
      $scope.setCurrentOrder(OrderService.orders[0]);
      $scope.loadingResults = false;
    });

    $scope.setCurrentOrder = function(order) {
      angular.forEach(order.items, function(item, itemIndex) {
        item.editNotes = item.notes;
        item.editQuantity = item.quantity;
        item.editEach = item.each;
        // item.editPosition = item.position;
      });
      $scope.currentOrder = order;
    };

    $scope.startEditOrderName = function(orderName) {
      $scope.editOrder = {};
      $scope.editOrder.name = angular.copy(orderName);
      $scope.currentOrder.isRenaming = true;
    };

    $scope.saveOrder = function(order) {

      var updatedOrder = angular.copy(order);
      angular.forEach(order.items, function(item, itemIndex) {
        item.notes = item.editNotes;
        item.quantity = item.editQuantity;
        item.each = item.editEach;
        // item.editPosition = item.position;
      });

      OrderService.updateOrder(order).then(function() {
        $scope.currentOrder.isRenaming = false;
        $scope.sortBy = 'position';
        $scope.sortOrder = false;
        $scope.currentOrder = updatedOrder;
        console.log('Successfully saved order ' + order.name);
      }, function() {
        console.log('Error saving order ' + order.name);
      });
    };

    $scope.renameOrder = function (orderId, orderName) {
      var order = angular.copy($scope.currentOrder);
      order.name = orderName;

      OrderService.updateOrder(order).then(function(data) {
        $scope.currentOrder.isRenaming = false;
        $scope.currentOrder.name = orderName;
        console.log('Successfully renamed order to ' + orderName + '.');
      }, function() {
        console.log('Error renaming order.');
      });
    };

    $scope.deleteOrder = function(orderId) {
      OrderService.deleteOrder(orderId).then(function() {
        console.log('Successfully deleted order.');
      }, function() {
        console.log('Error deleting order.');
      });
    };

  }]);