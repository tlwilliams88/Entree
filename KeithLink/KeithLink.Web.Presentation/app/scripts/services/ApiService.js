'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ApiService', function ($http) {
    
    var Service = {
      endpointUrl: undefined,
      getEndpointUrl: function() {
        return $http.get('../servicelocator');
      }
    };

    return Service;

  });
