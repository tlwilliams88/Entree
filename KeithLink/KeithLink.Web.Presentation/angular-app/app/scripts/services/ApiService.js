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
        if (ApiSettings.url) {
          return ApiSettings.url;
        } else {
          return $http.get('../servicelocator').then(function(response) {
          	ApiSettings.url = location.protocol + '//' + response.data.ClientApiEndpoint;
          }, function() {
            // running locally, use dev
            ApiSettings.url = 'http://devapi.bekco.com';
          });
        }
      }
    };

    return Service;

  }])
.value('ApiSettings', { });