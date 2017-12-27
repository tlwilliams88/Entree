'use strict';

angular.module('bekApp')
.controller('OrderItemsController', ['$scope', '$modal', '$filter', 'order', 'OrderService', 'ListService', 'Constants', 'AnalyticsService',
  function ($scope, $modal, $filter, order, OrderService, ListService, Constants, AnalyticsService) {

  $scope.sortOrder = false;
  $scope.sortBy = 'linenumber';

  $scope.filterDeletedItems = function(order){ 
    order.items =  OrderService.filterDeletedOrderItems(order);
    return order;
  };


  $scope.getOrder = function(orderNumber) {
    OrderService.getOrderDetails(orderNumber).then(function(order) {
      AnalyticsService.recordCheckout(order, 
                                      Constants.checkoutSteps.CheckStatus, // step
                                      ""); //option

      $scope.setOrder(order);
      $scope.displayMessage('success', 'Successfully retrieved latest info for order.');
    });
  };

  function calculatePieces(order){   
    var pieceCount = 0;
    if(order.items && order.items.length > 0){
      order.items.forEach(function(item){
        pieceCount = pieceCount + item.quantityordered;
      });
    }
    order.piececount = pieceCount;
  }

  $scope.setOrder = function(order){
    calculatePieces(order);
    if(order.catalogtype === 'UNFI,' || order.catalogtype === 'UNFI'){
      $scope.UNFIOrder = $scope.filterDeletedItems(order);
    }
    else{
      $scope.order = $scope.filterDeletedItems(order);
    }
    if(order.relatedordernumbers && $scope.getRelatedOrder){
      $scope.getOrder(order.relatedordernumbers);    
      $scope.getRelatedOrder = false;
    }
  };

  $scope.getRelatedOrder = true;
  $scope.setOrder(order);


  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        location: function() {
          return {category:'Orders', action:'Export Order Items'};
        },
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
        },
        exportType: function() {
           return Constants.exportType.orderItemsExport;
        }
      }
    });
  };

  $scope.saveAsList = function() {
    ListService.createList($scope.order.items, { name: 'List From - ' + $scope.order.ordernumber });
  };

}]);