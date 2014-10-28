'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$filter', 'UtilityService', 'Order', 
    function ($http, $filter, UtilityService, Order) {
    
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
        }, null).$promise.then(function(order) {
          return order.ordernumber;
        });
      },

      updateOrder: function(order) {
        var params = {};
        return Order.update(params, order).$promise.then(function(order) {
          console.log(order);
          return order;
        });
      },

      findChangeOrderByOrderNumber: function(changeOrders, orderNumber) {
        return UtilityService.findObjectByField(changeOrders, 'ordernumber', orderNumber);
      },
    };
 
    return Service;
 
  }]);