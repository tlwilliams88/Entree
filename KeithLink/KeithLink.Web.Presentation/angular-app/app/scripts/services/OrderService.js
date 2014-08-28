'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', 'UserProfileService', 'NameGeneratorService', function ($http, UserProfileService, NameGeneratorService) {

    function getBranch() {
      return UserProfileService.getCurrentBranchId();
    }

    var Service = {
      orders: [],

      getAllOrders: function(requestParams) {
        return $http.get('/cart/' + getBranch(), {
          params: requestParams
        }).then(function(response) {
          var allOrders = response.data;
          angular.copy(allOrders, Service.orders);

          console.log(allOrders);
          return allOrders;
        });
      },

      getOrder: function(orderId) {
        return $http.get('/cart/' + getBranch() + '/' + orderId).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      addItemToOrder: function(orderId, item) {
        return $http.post('/cart/' + orderId + '/item', item).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      updateItem: function(orderItemId, item) {
        return $http.put('/cart/' + orderItemId + '/item', item).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      deleteItem: function(orderId, itemId) {
        return $http({
          method: 'DELETE', 
          url: '/cart/' + orderId + '/item/' + itemId
        }).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      createOrder: function(items) {
        if (!items) {
          items = [];
        }

        var newOrder = {
          name: NameGeneratorService.generateName('Order', Service.orders),
          items: items
        };

        return $http.post('/cart/' + getBranch(), newOrder).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      createOrderWithItem: function(item) {
        // delete item.listitemid;
        var items = [item];

        return Service.createOrder(items);
      },

      updateOrder: function(order) {
        return $http.put('/cart', order).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      deleteOrder: function(orderId) {
        return $http({
          method: 'DELETE', 
          url: '/cart/' + orderId
        }).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      }

    };

    return Service;
 
  }]);