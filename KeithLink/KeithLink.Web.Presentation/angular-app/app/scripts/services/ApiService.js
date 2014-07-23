'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ApiService
 * @description
 * # ApiService
 * calls service to get endpoint url
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
