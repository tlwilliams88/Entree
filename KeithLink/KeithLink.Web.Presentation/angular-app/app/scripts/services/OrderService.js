'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$filter', 'Order', function ($http, $filter, Order) {
    
    var Service = {
      
      getAllOrders: function() {
        return Order.query().$promise;
      },

      getOrderDetails: function(orderNumber) {
        return Order.get({
          orderNumber: orderNumber
        }).$promise;
      },

      /*************
      CHANGE ORDERS
      *************/

      getChangeOrders: function() {
        return Service.getAllOrders().then(function(orders) {
          return $filter('filter')(orders, {ischangeorderallowed: true});
        });
      },

      resubmitOrder: function(orderNumber) {
        return Order.resubmitOrder({
          orderNumber: orderNumber
        }, null).$promise;
      },

      updateOrder: function(order) {
        var params = {};
        return Order.update(params, order).$promise;
      },

      findChangeOrderByOrderNumber: function(changeOrders, orderNumber) {
        var itemsFound = $filter('filter')(changeOrders, {ordernumber: orderNumber});
        if (itemsFound.length === 1) {
          return itemsFound[0];
        }
      },
    };
 
    return Service;
 
  }]);