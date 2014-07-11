'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProfileService
 * @description
 * # ProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProfileService', function ($http) {
    
    var Service = {
      getPro: function() {
        return $http.get('http://localhost:9002/ws/products');
      }
    };

    return Service;

  });
