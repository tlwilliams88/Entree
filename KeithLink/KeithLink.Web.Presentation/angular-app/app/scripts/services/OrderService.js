'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', '$q', '$filter', 'UtilityService', 'Order', 
    function ($http, $q, $filter, UtilityService, Order) {
    
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
        // return $http.put('/order', order).then(function(response) {
        //   return response.data;
        // });
        return Order.update(params, order).$promise;
      },

      findChangeOrderByOrderNumber: function(changeOrders, orderNumber) {
        return UtilityService.findObjectByField(changeOrders, 'ordernumber', orderNumber);
      },

      cancelOrder: function(commerceId) {
        return Order.delete({
          orderNumber: commerceId
        }).$promise;
      }
    };
 
    return Service;
 
  }]);