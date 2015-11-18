'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$modal', '$filter', 'order', 'OrderService', 'ListService',
  function ($scope, $modal, $filter, order, OrderService, ListService) {


  $scope.filterDeletedItems = function(order){ 
    order.items =  OrderService.filterDeletedOrderItems(order);
    return order;
  };

  $scope.order = $scope.filterDeletedItems(order);

  $scope.getOrder = function(orderNumber) {
    OrderService.getOrderDetails(orderNumber).then(function(order) {
      $scope.order = $scope.filterDeletedItems(order);
      $scope.displayMessage('success', 'Successfully retrieved latest info for order.');
    });
  };

  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        headerText: function () {
          return 'Order #' + order.ordernumber;
        },
        exportMethod: function() {
          return OrderService.exportOrderDetails;
        },
        exportConfig: function() {
          return OrderService.getDetailExportConfig(order.ordernumber);
        },
        exportParams: function() {
          return order.ordernumber;
        }
      }
    });
  };

  $scope.saveAsList = function() {
    ListService.createList($scope.order.items, { name: 'List From - ' + $scope.order.ordernumber });
  };

}]);