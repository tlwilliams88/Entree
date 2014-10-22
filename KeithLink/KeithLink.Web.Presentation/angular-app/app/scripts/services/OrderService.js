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
          // orders[0].ischangeorderallowed = true;
          // orders[0].items = orders[0].lineItems;
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
      }
    };
 
    return Service;
 
  }]);