'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ApiService
 * @description
 * # ApiService
 * calls service to get endpoint url
 */
angular.module('bekApp')
  .factory('ApiService', ['$http', 'ApiSettings', function ($http, ApiSettings) {
    
    var Service = {
      getEndpointUrl: function() {
        return $http.get('../servicelocator').then(function(response) {
        	ApiSettings.url = location.protocol + '//' + response.data.ClientApiEndpoint;
        });
      }
    };

    return Service;

  }])
.value('ApiSettings', { url: 'http://devapi.bekco.com' } );