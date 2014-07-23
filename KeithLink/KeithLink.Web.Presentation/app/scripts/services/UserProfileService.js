'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', ['$http', 'ApiService', function ($http, ApiService) {
    
    function profile() {
      return $http.get(ApiService.endpointUrl + '/ws/profile').then(function(data) {
        Service.profile = data.data.profile;
      });
    }


    var Service = {
      profile: null,

      getProfile: function() {
        return profile();
      }
    };

    return Service;

  }]);
