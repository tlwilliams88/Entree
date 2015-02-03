'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$filter', 'UtilityService', 'ExportService', 'Order', 
    function ($http, $filter, UtilityService, ExportService, Order) {
    
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
        var promise = $http.get('/order/changeorder', {
          header: true
        });
        return UtilityService.resolvePromise(promise);
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
      },

      getUnconfirmedOrders: function() {
        var promise = $http.get('/order/admin/submittedUnconfirmed');
        return UtilityService.resolvePromise(promise);
      },

      resubmitUnconfirmedOrder: function(controlNumber) {
        var promise = $http.put('/order/admin/resubmitUnconfirmed/' + controlNumber );
        return UtilityService.resolvePromise(promise);
      }

    };
 
    return Service;
 
  }]);