'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$q', '$filter', 'UtilityService', 'ExportService', 'PricingService', 'Order', 
    function ($http, $q, $filter, UtilityService, ExportService, PricingService, Order) {
    
    var Service = {

      changeOrderHeaders: [],
      
      getAllOrders: function() {
        return Order.query().$promise;
      },

      getOrders: function(params) {
        return $http.post('/order', params).then(function(response) {
          return response.data.successResponse;
        }, function() {
          return $q.reject('Error retrieving orders.');
        });
      },

      getOrderDetails: function(orderNumber) {
        return Order.get({
          orderNumber: orderNumber
        }).$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      getOrdersByDate: function(startDate, endDate) {
        return Order.getOrdersByDate({
          from: startDate,
          to: endDate
        }).$promise.then(function(resp){
          return resp.successResponse;
        });
      },      

      getMonthTotals: function(numberOfMonths) {
       return $http.get('/order/totalbymonth/'+ numberOfMonths).then(function(resp){        
          return resp.data.successResponse;     
        });
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
        }, { message: 'Submitting changes...' }).$promise.then(function(resp) {
          return resp.successResponse.ordernumber;
        });
      },

      updateOrder: function(order, params) {
        order.message = 'Saving order...';
        order.items.forEach(function(item){
          if(item.quantity == 0 && item.status && item.status.toUpperCase() === 'OUT OF STOCK'){
            item.quantity = item.quantityordered;
          }
        })
        return Order.update(params, order).$promise.then(function(changeOrder) {
          changeOrder = changeOrder.successResponse;
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
        }).$promise.then(function(resp) {
          // delete change order from cache
          var orderNumber = resp.successResponse.ordernumber
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

      filterDeletedOrderItems: function(order) {
        return $filter('filter')(order.items , {changeorderstatus: '!deleted'}); 
      },

      /*************
      ORDER HISTORY
      *************/

      refreshOrderHistory: function() {
        return Order.getOrderHistory().$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      getOrderHistoryDetails: function(invoiceNumber) {
        return Order.getOrderHistoryDetails({
          invoiceNumber: invoiceNumber
        }).$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      pollOrderHistory: function() {
        return Order.pollOrderHistory().$promise.then(function(resp){
          return resp.successResponse;
        });
      },

      /****************************
      RECENTLY ORDERED UNFI ITEMS
      ****************************/

      getRecentlyOrderedUNFIItems: function() {
        return $http.get('/recent/order/UNFI').then(function(response){
          return response.data.successResponse;
      });
      },

      clearRecentlyOrderedUNFIItems: function() {
        return $http.delete('/recent/order/UNFI').then(function(response){
          return response.data.successResponse;
        });
      },

      UpdateRecentlyOrderedUNFIItems: function(recentlyOrdered) {
        var payload = {
          catalog:"UNFI",
          items:recentlyOrdered
        }
        return $http.post('/recent/order', payload).then(function(response) {
          return response.data.successResponse;
        });
      },

      /********************
      EXPORT
      ********************/

      getOrderExportConfig: function() {
        return Order.getOrderExportConfig({}).$promise.then(function(response){
          return response.successResponse;
        });
      },

      exportOrders: function(config) {
        ExportService.export('/order/export', config);
      },

      getDetailExportConfig: function(orderNumber) {
        return Order.getDetailExportConfig({
          orderNumber: orderNumber
        }).$promise.then(function(response){
          return response.successResponse;
        });;
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
