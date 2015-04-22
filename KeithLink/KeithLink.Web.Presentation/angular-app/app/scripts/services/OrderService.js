'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$q', 'UtilityService', 'ExportService', 'PricingService', 'Order', 
    function ($http, $q, UtilityService, ExportService, PricingService, Order) {
    
    var Service = {

      changeOrderHeaders: [],
      
      getAllOrders: function() {
        return Order.query().$promise;
      },

      getOrders: function(params) {
        return $http.post('/order', params).then(function(response) {
          return response.data;
        });
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

      getChangeOrders: function(params) {
        var promise = $http.get('/order/changeorder', { params: {
          header: true
        }});
        return UtilityService.resolvePromise(promise).then(function(changeOrders) {
          changeOrders.forEach(function(changeOrder) {
            PricingService.updateCaculatedFields(changeOrder.items);
          });
          angular.copy(changeOrders, Service.changeOrderHeaders);
          return changeOrders;
        });
      },

      resubmitOrder: function(orderNumber) {
        return Order.resubmitOrder({
          orderNumber: orderNumber
        }, { message: 'Submitting changes...' }).$promise.then(function(order) {
          return order.ordernumber;
        });
      },

      updateOrder: function(order, params) {
        order.message = 'Saving order...';
        return Order.update(params, order).$promise.then(function(changeOrder) {
          PricingService.updateCaculatedFields(changeOrder.items);
          return changeOrder;
        });
      },

      findChangeOrderByOrderNumber: function(changeOrders, orderNumber) {
        return UtilityService.findObjectByField(changeOrders, 'ordernumber', orderNumber);
      },

      cancelOrder: function(commerceId) {
        return Order.delete({
          orderNumber: commerceId
        }).$promise.then(function(orderNumber) {
          // delete change order from cache
          
          var deletedChangeOrder;
          Service.changeOrderHeaders.forEach(function(changeOrder) {
            if (changeOrder.ordernumber === orderNumber) {
              deletedChangeOrder = changeOrder;
            }
          });
          var idx = Service.changeOrderHeaders.indexOf(deletedChangeOrder);
          Service.changeOrderHeaders.splice(idx, 1);
        });
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