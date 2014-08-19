'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ApiService
 * @description
 * # ApiService
 * calls service to get endpoint url
 */
angular.module('bekApp')
  .factory('ApiService', ['$http', 'Constants', function ($http, Constants) {
    
    var Service = {
      endpointUrl: undefined,
      getEndpointUrl: function() {
        return $http.get(Constants.servicelocatorUrl);
      }
    };

    return Service;

  }]);
