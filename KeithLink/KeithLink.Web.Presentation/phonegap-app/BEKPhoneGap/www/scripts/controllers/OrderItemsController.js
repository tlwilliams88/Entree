'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$modal', 'order', 'OrderService',
  function ($scope, $modal, order, OrderService) {

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
          return OrderService.getDetailExportConfig();
        },
        exportParams: function() {
          return order.ordernumber;
        }
      }
    });
  };

}]);