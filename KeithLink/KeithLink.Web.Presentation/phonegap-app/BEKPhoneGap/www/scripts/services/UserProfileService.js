'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:UserProfileService
 * @description
 * # UserProfileService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('UserProfileService', [ '$http', '$q', '$log', '$upload', 'toaster', 'LocalStorage', 'UtilityService', 'AccessService', 'SessionService',
    function ($http, $q, $log, $upload, toaster, LocalStorage, UtilityService, AccessService, SessionService) {

    var Service = {

      // gets and sets current user profile
      getCurrentUserProfile: function() {
        return Service.getUserProfile().then(function (profile) {
          SessionService.userProfile = profile;

          // check if user is Order entry customer to determine which branch/context to select
          if (AccessService.isOrderEntryCustomer()) {
            var userSelectedContext = {
              id: profile.defaultcustomer !== null ? profile.defaultcustomer.customerNumber : '',
              text: profile.defaultcustomer !== null ? profile.defaultcustomer.displayname : '',
              customer: profile.defaultcustomer
            };
            LocalStorage.setSelectedCustomerInfo(userSelectedContext);

          } else {
            LocalStorage.setSelectedBranchInfo(profile.branchid || 'FDF'); // default to DFW branch
          }

          return profile;
        }, function(error) {
          // log out
          LocalStorage.clearAll();

          var message = error;
          if (error.data.error_description) {
            message = error.data.error_description;
          }
          return $q.reject(message);
        });
      },

      validateToken: function(token) {        
        return $http.post('/profile/forgotpassword/validatetoken/', { token: token });
      },

      resetPassword: function(email) {          
        var promise = $http.post('/profile/forgotpassword?emailAddress='+email);
        return UtilityService.resolvePromise(promise);
      },

      getUserProfile: function(email) {
        var config = {};

        if (email) {
          config.params = {
            email: email
          };
        } else { // show loading screen when getting current user's profile
          config.data = {
            message: 'Loading...'
          };
        }

        return $http.get('/profile', config).then(function(response){
          if (response.data.userProfiles.length === 0) {
            return $q.reject('User profile not found.');
          }
          var profile = response.data.userProfiles[0];
          $log.debug(profile);
          Service.updateDisplayName(profile);  
          return profile;
        });
      },

      updateDisplayName: function(profile){
        // set display name for user
        if (profile.firstname === 'guest' && profile.lastname === 'account') {
          profile.displayname = profile.emailaddress;
        } else if (profile.firstname && profile.lastname) {
          profile.displayname = profile.firstname + ' ' + profile.lastname;
        } else if (profile.firstname) {
          profile.displayname = profile.firstname;
        } else {
          profile.displayname = profile.emailaddress;
        }
      },

      getAllUserCustomers: function(userId) {
        var promise = $http.get('/profile/user/' + userId + '/customers');
        return UtilityService.resolvePromise(promise).then(function(successResponse) {
          return successResponse.customers;
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

      createUser: function(userProfile) {
        userProfile.message = 'Creating user...';
        var promise = $http.post('/profile/register', userProfile);
        return UtilityService.resolvePromise(promise);
      },

      createUserFromAdmin: function(userProfile) {
        var promise = $http.post('/profile/admin/user', userProfile);
        return UtilityService.resolvePromise(promise).then(function(successResponse) {
          return successResponse.userProfiles;
        });
      },

      removeUserFromCustomerGroup: function(userId, accountId) {
        var promise = $http.delete('/profile/' + userId + '/account/' + accountId);
        return UtilityService.resolvePromise(promise);
      },

      updateUserProfile: function(userProfile) {
        userProfile.message = 'Saving profile...';
        var promise = $http.put('/profile', userProfile);
        return UtilityService.resolvePromise(promise).then(function(successResponse) {
          var loggedinprofile = SessionService.userProfile; //Get current users profile from LocalStorage

          var profile = successResponse.userProfiles[0];
          $log.debug(profile);
          //Only save updated profile back to local storage if the profile being updated is the same as
          //the currently logged in user. If this is an admin editing another user's profile, don't save
          //to local storage
          if(loggedinprofile.userid === profile.userid){
            SessionService.userProfile = profile;
          } 
          Service.updateDisplayName(profile);        
          return profile;
        });
      },

      changePassword: function(passwordData) {
        var deferred = $q.defer();

        $http.put('/profile/password', passwordData).then(function(response) {
          $log.debug(response);
          if (response.data.successResponse === true) {
            deferred.resolve(response.data);
          } else {
            deferred.reject(response.data);
          }
        });

        return deferred.promise;
      },

      changeForgottenPassword: function(passwordData) {
        var deferred = $q.defer();

        $http.post('/profile/forgotpassword/change', passwordData).then(function(response) {
          $log.debug(response);
          if (response.data.successResponse === true) {
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


    /**********
    ACCESS TO OTHER FEATURES
    **********/
    function grantAccess(url, email) {
      var promise = $http.post(url);
      return UtilityService.resolvePromise(promise).then(function(response) {
        toaster.pop('success', null, 'Successfully granted access');
        return response.data;
      }, function() {
        toaster.pop('error', null, 'Error granting access');
      });
    }
    function revokeAccess(url, email) {
      var promise = $http.delete(url);
      return UtilityService.resolvePromise(promise).then(function(response) {
        toaster.pop('success', null, 'Successfully revoked access');
        return response.data;
      }, function() {
        toaster.pop('error', null, 'Error revoking access');
      });
    }
    Service.changeProgramAccess = function(email, program, isGrantingAccess) {
      var url = '/profile/' + email + '/access/' + program;

      if (isGrantingAccess) {
        return grantAccess(url);
      } else {
        return revokeAccess(url);
      }
    };


    return Service;

  }]);
