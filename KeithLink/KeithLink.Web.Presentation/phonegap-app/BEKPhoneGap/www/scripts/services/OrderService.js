'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:OrderService
 * @description
 * # OrderService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('OrderService', ['$http', function ($http) {
    
    var Service = {
      
      submitOrder: function(cartId) {
        return $http.post('/order/' + cartId);
      }
    };
 
    return Service;
 
  }]);