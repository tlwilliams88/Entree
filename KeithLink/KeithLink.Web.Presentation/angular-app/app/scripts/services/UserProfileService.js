'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', [ '$http', '$q', '$log', '$upload', 'toaster', 'LocalStorage', 'UtilityService',
    function ($http, $q, $log, $upload, toaster, LocalStorage, UtilityService) {

    var Service = {

      // TODO: getProfile and getUserProfile are duplicates

      // gets and sets current user profile
      getProfile: function(email) { 
        return Service.getUserProfile(email).then(function (profile) {
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
            var userSelectedContext = {
              id: profile.defaultcustomer.customerNumber,
              text: profile.defaultcustomer.displayname,
              customer: profile.defaultcustomer
            };
            LocalStorage.setSelectedCustomerInfo(userSelectedContext);
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
          $log.debug(response.data);
          return response.data.userProfiles[0];
        });
      },

      searchUserCustomers: function(searchTerm, size, from) {
        var data = {
          params: {
            size: size,
            from: from,
            terms: searchTerm
          }
        };
        return $http.get('/profile/customer', data).then(function(response) {
          return response.data;
        });
      },

      // accountid, customerid , email
      getAllUsers: function(params) {
        var config = {
          params: params
        };
        
        var promise = $http.get('/profile/users', config);

        return UtilityService.resolvePromise(promise).then(function(successResponse) {
          return successResponse.userProfiles;
        });
      },

      getUsersForAccount: function(accountId) {
        var promise = $http.get('/profile/account/' + accountId + '/users');
        return UtilityService.resolvePromise(promise);
      },

      createUser: function(userProfile) {
        var promise = $http.post('/profile/register', userProfile);
        return UtilityService.resolvePromise(promise);
      },

      // TODO: updateUser and updateProfile are duplicates

      updateUser: function(userProfile) {
        var promise = $http.put('/profile', userProfile);

        return UtilityService.resolvePromise(promise).then(function(successResponse) {
          var profile = successResponse.userProfiles[0];
          $log.debug(profile);
          LocalStorage.setProfile(profile);
          return profile;
        });
      },

      updateProfile: function(userProfile) {
        return Service.updateUser(userProfile);
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

      /**********
      AVATAR
      **********/
      uploadAvatar: function(file) {
        var promise = $upload.upload({
          url: '/profile/avatar',
          method: 'POST',
          file: file.file,
          data: { name: file.name },
        });

        return UtilityService.resolvePromise(promise).then(function(successResponse) {
          // TODO: update url locally 
          toaster.pop('success', null, 'Successfully uploaded avatar');
        }, function(error) {
          toaster.pop('error', null, 'Error uploading avatar.');
          return $q.reject(error);
        });
      },

      removeAvatar: function() {
        // TODO: add remove avatar api call
      }
    };

    return Service;

  }]);
