'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', [ '$http', '$q', '$log', 'LocalStorage', function ($http, $q, $log, LocalStorage) {

    var Service = {
      getProfile: function(email) {
        var data = {
          params: {
            email: email
          }
        };

        return $http.get('/profile', data).then(function (response) {
          var profile = response.data.userProfiles[0];

          $log.debug(profile);

          profile.salesRep = {
            'id': 34234,
            'name': 'Heather Hill',
            'phone': '(888) 912-2342',
            'email': 'heather.hill@benekeith.com',
            'imageUrl': './images/placeholder-dsr.jpg'
          };

          //profile.imageUrl = 'http://testmultidocs.bekco.com/avatar/{1d521e08-62c9-4749-ad62-dfe03617acfc}';

          LocalStorage.setProfile(profile);
          // TODO: how to determine if user has customer locations, needs to match logic to display dropdowns
          if (profile.rolename === 'guest') {
            LocalStorage.setSelectedBranchInfo(profile.branchid);
          } else {
            LocalStorage.setSelectedCustomerInfo(profile.user_customers[0]);
          }
          return profile;
        });
      },

      getUserProfile: function(email) {
        var data = {
          params: {
            email: email
          }
        };

        return $http.get('/profile', data).then(function(response){
          return response.data.userProfiles[0];
        });
      },

      // accountid, customerid , email
      getAllUsers: function(params) {
        var deferred = $q.defer();
        $http.get('/profile/users', params).then(function(response) {
          var data = response.data;
          if (data.successResponse) {
            deferred.resolve(data.successResponse.userProfiles);
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;
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
            $log.debug(profile);
            LocalStorage.setProfile(profile);
            deferred.resolve(profile);
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;
      },

      updateProfile: function(userProfile) {
        var deferred = $q.defer();

        $http.put('/profile', userProfile).then(function(response) {

          var data = response.data;

          if (data.successResponse) {
            var profile = data.successResponse.userProfiles[0];
            $log.debug(profile);
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
          $log.debug(response);
          if (response.data === '"Password update successful"') {
            deferred.resolve(response.data);
          } else {
            deferred.reject(response.data);
          }
        });

        return deferred.promise;
      },

      uploadAvatar: function() {
        // TODO: add upload avatar api call
        // needs to return url so you can refresh the profile object
      },

      removeAvatar: function() {
        // TODO: add remove avatar api call
      }
    };

    return Service;

  }]);
