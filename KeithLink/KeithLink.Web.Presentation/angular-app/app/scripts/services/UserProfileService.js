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
      profile: {
        name: 'Maria Knabe',
        currentLocation: '',
        locations: [

        ],
        role: {
          id: 1,
          name: 'VIEW'
        }
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
