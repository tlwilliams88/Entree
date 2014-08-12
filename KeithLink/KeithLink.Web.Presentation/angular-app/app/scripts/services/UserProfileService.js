'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', ['$http', function ($http) {

    var Service = {
      profile: {},

      getProfile: function() {
        return Service.profile;
      },

      setProfile: function(newProfile) {
        angular.copy(newProfile, Service.profile);
      },

      getCurrentLocation: function() {
        return Service.profile.currentLocation;
      },

      createUser: function() {

      },

      updateUser: function() {

      }
    };

    return Service;

  }]);
