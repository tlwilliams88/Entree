'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', [ '$http', 'localStorageService', 'Constants',
    function ($http, localStorageService, Constants) {

    var Service = {
      
      profile: function() {
        return localStorageService.get(Constants.localStorage.userProfile);
      },

      getProfile: function(email) {
        var data = { 
          params: {
            emailAddress: email
          }
        };

        return $http.get('/profile', data).then(function (response) {
          var profile = response.data.userProfiles[0];

          profile.stores = [{
            'name': 'Dallas Ft Worth',
            'customerNumber': 453234,
            'branchId': 'fdf'
          }, {
            'name': 'San Antonio',
            'customerNumber': 534939,
            'branchId': 'fsa'
          }, {
            'name': 'Amarillo',
            'customerNumber': 534939,
            'branchId': 'fam'
          }];

          profile.salesRep = {
            'id': 34234,
            'name': 'Heather Hill',
            'phone': '(888) 912-2342',
            'email': 'heather.hill@ben.e.keith.com',
            'imageUrl': '../images/placeholder-dsr.jpg'
          };

          profile.role = 'Owner';

          Service.setProfile(profile);

          return profile;
        });
      },

      setProfile: function(profile) {
        localStorageService.set(Constants.localStorage.userProfile, profile);
      },

      getCurrentBranchId: function() {
        return Service.getCurrentLocation().branchId;
      },

      getCurrentRole: function() {
        if (Service.profile()) {
          return Service.profile().role;
        }
      },

      getCurrentLocation: function() {
        return localStorageService.get(Constants.localStorage.currentLocation);
      },

      setCurrentLocation: function(location) {
        localStorageService.set(Constants.localStorage.currentLocation, location);
      },

      createUser: function() {

      },

      updateUser: function() {

      }
    };

    return Service;

  }]);
