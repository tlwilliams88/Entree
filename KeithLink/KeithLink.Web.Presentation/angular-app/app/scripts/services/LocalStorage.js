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

      /*************
      SELECTED USER CONTEXT
      *************/

      getBranchId: function() {
        return localStorageService.get(Constants.localStorage.branchId);
      },

      getCustomerNumber: function() {
        return localStorageService.get(Constants.localStorage.customerNumber);
      },

      getCurrentCustomer: function() {
        return localStorageService.get(Constants.currentCustomer);
      },

      setSelectedBranchInfo: function(branchId) { // for guest users
        setBranchId(branchId);
      },

      setSelectedCustomerInfo: function(customer) { // for order entry users
        setBranchId(customer.customerBranch);
        setCustomerNumber(customer.customerNumber);
        setCurrentCustomer(customer);
      }
    };

    function setCustomerNumber(customerNumber) {
      localStorageService.set(Constants.localStorage.customerNumber, customerNumber);
    }

    function setBranchId(branchId) {
      localStorageService.set(Constants.localStorage.branchId, branchId);
    }

    function setCurrentCustomer(customer) {
      localStorageService.set(Constants.currentCustomer, customer);
    }


    return Service;

  }]);
