'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', [ '$http', '$q', 'localStorageService', 'Constants',
    function ($http, $q, localStorageService, Constants) {

    var Service = {

      profile: function() {
        return localStorageService.get(Constants.localStorage.userProfile);
      },

      getProfile: function(email) {
        var data = { 
          params: {
            email: email
          }
        };

        return $http.get('/profile', data).then(function (response) {
          var profile = response.data.userProfiles[0];

          console.log(profile);

          // TEMP: to show different roles
          if (profile.emailaddress === 'guestuser@gmail.com') {
            profile.role = 'Guest';
          } else {
            profile.role = 'Owner';
            profile.stores = [{
              'name': 'Dallas-Ft. Worth',
              'customerNumber': 453234,
              'id': 'FDF'
            }, {
              'name': 'San Antonio',
              'customerNumber': 534939,
              'id': 'FSA'
            }, {
              'name': 'Amarillo',
              'customerNumber': 534939,
              'id': 'FAM'
            }, {
              'name': 'Arkansas',
              'customerNumber': 534939,
              'id': 'FAR'
            }];
          }

          profile.salesRep = {
            'id': 34234,
            'name': 'Heather Hill',
            'phone': '(888) 912-2342',
            'email': 'heather.hill@benekeith.com',
            'imageUrl': '../images/placeholder-dsr.jpg'
          };

          profile.imageUrl = '../images/placeholder-user.png';

          Service.setProfile(profile);
          return profile;
        });
      },

      setProfile: function(profile) {
        // set display name for user
        if (profile.firstname === 'guest' && profile.lastname === 'account') {
          profile.displayname = profile.emailaddress;
        } else {
          profile.displayname = profile.firstname + ' ' + profile.lastname;
        }

        localStorageService.set(Constants.localStorage.userProfile, profile);
      },

      getUserRole: function() {
        if (Service.profile()) {
          return Service.profile().role;
        }
      },

      getCurrentLocation: function() {
        return localStorageService.get(Constants.localStorage.currentLocation);
      },

      setCurrentLocation: function(locationId) {
        localStorageService.set(Constants.localStorage.currentLocation, locationId);
      },

      getCurrentBranchId: function() {
        return Service.getCurrentLocation();
      },

      createUser: function(userProfile) {
        var deferred = $q.defer();

        $http.post('/profile/register', userProfile).then(function(response) {
          var data = response.data;
          if (data.successResponse) {
            deferred.resolve(data.successResponse);
          } else {
            deferred.reject(data.errorMessage);
          }
        });

        return deferred.promise;
      },

      updateUser: function(userProfile) {
        var deferred = $q.defer();

        $http.put('/profile', userProfile).then(function(response) {

          var data = response.data;

          if (data.successResponse) {
            var profile = data.successResponse.userProfiles[0];
            profile.role = 'Owner';
            console.log(profile);
            Service.setProfile(profile);
            deferred.resolve(profile);  
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;
      },

      changePassword: function(passwordData) {
        var deferred = $q.defer();

        $http.put('/profile/password', passwordData).then(function(response) {
          console.log(response);
          if (response.data === '"Password update successful"') {
            deferred.resolve(response.data);
          } else {
            deferred.reject(response.data);
          }
        });

        return deferred.promise;
      }
    };

    return Service;

  }]);
