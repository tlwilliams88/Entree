'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:LocalStorage
 * @description
 * # LocalStorage
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('LocalStorage', ['localStorageService', 'Constants',
    function (localStorageService, Constants) {

    var Service = {

      clearAll: function() {
        localStorageService.remove(Constants.localStorage.userProfile);
        localStorageService.remove(Constants.localStorage.userToken);
        localStorageService.remove(Constants.localStorage.currentLocation);
        localStorageService.remove(Constants.localStorage.branchId);
        localStorageService.remove(Constants.localStorage.customerNumber);
      },

      /*************
      TOKEN
      *************/

      getToken: function() {
        return localStorageService.get(Constants.localStorage.userToken);
      },

      setToken: function(token) {
        localStorageService.set(Constants.localStorage.userToken, token);
      },

      /*************
      LEAD GEN
      *************/
      getLeadGenInfo: function() {
        return localStorageService.set(Constants.localStorage.leadGenInfo);
      },

      setLeadGenInfo: function(leadGenInfo) {
        localStorageService.set(Constants.localStorage.leadGenInfo, leadGenInfo);
      },


      /*************
      USER PROFILE
      *************/
      getProfile: function() {
        return localStorageService.get(Constants.localStorage.userProfile);
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
        if (Service.getProfile()) {
          return Service.getProfile().rolename;
        }
      },

      getCurrentLocation: function() {
        return localStorageService.get(Constants.localStorage.currentLocation);
      },

      setCurrentLocation: function(location) {
        localStorageService.set(Constants.localStorage.currentLocation, location);
      },

      getBranchId: function() {
        return localStorageService.get(Constants.localStorage.branchId);
      },

      setBranchId: function(branchId) {
        localStorageService.set(Constants.localStorage.branchId, branchId);
      },

      getCustomerNumber: function() {
        return localStorageService.get(Constants.localStorage.customerNumber);
      },

      setCustomerNumber: function(customerNumber) {
        localStorageService.set(Constants.localStorage.customerNumber, customerNumber);
      }
    };

    return Service;

  }]);
