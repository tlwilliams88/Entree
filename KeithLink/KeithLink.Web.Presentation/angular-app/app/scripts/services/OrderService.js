'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$q', '$filter', 'UtilityService', 'ExportService', 'Order', 
    function ($http, $q, $filter, UtilityService, ExportService, Order) {
    
    var Service = {
      
      getAllOrders: function() {
        return Order.query().$promise;
      },

      getOrderDetails: function(orderNumber) {
        return Order.get({
          orderNumber: orderNumber
        }).$promise;
      },

      getOrdersByDate: function(startDate, endDate) {
        return Order.getOrdersByDate({
          from: startDate,
          to: endDate
        }).$promise;
      },

      /*************
      CHANGE ORDERS
      *************/

      getChangeOrders: function() {
        var deferred = $q.defer();
        $http.get('/order/changeorder').then(function(response) {
          var data = response.data;
          if (data.successResponse) {
            deferred.resolve(data.successResponse);
          } else {
            $q.reject(data.errorMessage);
          }
        });
        return deferred.promise;
      },

      resubmitOrder: function(orderNumber) {
        return Order.resubmitOrder({
          orderNumber: orderNumber
        }, null).$promise.then(function(order) {
          return order.ordernumber;
        });
      },

      updateOrder: function(order, params) {
        return Order.update(params, order).$promise;
      },

      findChangeOrderByOrderNumber: function(changeOrders, orderNumber) {
        return UtilityService.findObjectByField(changeOrders, 'ordernumber', orderNumber);
      },

      cancelOrder: function(commerceId) {
        return Order.delete({
          orderNumber: commerceId
        }).$promise;
      },

      /*************
      ORDER HISTORY
      *************/

      refreshOrderHistory: function() {
        return Order.getOrderHistory().$promise;
      },

      getOrderHistoryDetails: function(invoiceNumber) {
        return Order.getOrderHistoryDetails({
          invoiceNumber: invoiceNumber
        }).$promise;
      },

      pollOrderHistory: function() {
        return Order.pollOrderHistory().$promise;
      },

      /********************
      EXPORT
      ********************/

      getOrderExportConfig: function() {
        return Order.getOrderExportConfig({}).$promise;
      },

      exportOrders: function(config) {
        ExportService.export('/order/export', config);
      },

      getDetailExportConfig: function(orderNumber) {
        return Order.getDetailExportConfig({
          orderNumber: orderNumber
        }).$promise;
      },

      exportOrderDetails: function(config, orderNumber) {
        ExportService.export('/order/export/' + orderNumber, config);
      }

    };
 
    return Service;
 
  }]);