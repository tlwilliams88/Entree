'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', function ($http) {
    
    var Service = {
      profile: {},

      getProfile: function() {
        return $http.get('http://localhost:9002/ws/profile').then(function(data) {
          this.profile = data.data.profile;
        });
      },

      // user can view system admin and configuration screens
      hasAdminRole: function() {
        return this.profile.role.id === 1;
      },

      // user can view order screens
      hasBuyerRole: function() {
        return this.profile.role.id === 2;
      },

      // user can view invoice screens
      hasPayerRole: function() {
        return this.profile.role.id === 3;
      }
    };

    return Service;

  });
