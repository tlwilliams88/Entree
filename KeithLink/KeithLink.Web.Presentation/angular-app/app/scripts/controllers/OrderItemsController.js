'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$modal', 'order', 'OrderService', 'ListService',
  function ($scope, $modal, order, OrderService, ListService) {

  $scope.order = order;

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
    ListService.createList($scope.order.items, { name: "List From -" + $scope.order.ordernumber })
  };

}]);