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
        localStorageService.remove(Constants.localStorage.currentCustomer);
        localStorageService.remove(Constants.localStorage.branchId);
        localStorageService.remove(Constants.localStorage.customerNumber);
        localStorageService.remove(Constants.localStorage.tempContext);
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
      TEMP
      *************/

      getTempContext: function() {
        return localStorageService.get(Constants.localStorage.tempContext);
      },

      setTempContext: function(context) {
        localStorageService.set(Constants.localStorage.tempContext, context);
      },

      getTempBranch: function() {
        return localStorageService.get(Constants.localStorage.tempBranch);
      },

      setTempBranch: function(branch) {
        localStorageService.set(Constants.localStorage.tempBranch, branch);
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
        return localStorageService.get(Constants.localStorage.currentCustomer);
      },

      setSelectedBranchInfo: function(branchId) { // for guest users
        setBranchId(branchId);
      },

      setSelectedCustomerInfo: function(customer) { // for order entry users
        setBranchId(customer.customer === null ? '' : customer.customer.customerBranch);
        setCustomerNumber(customer.customer === null ? '' : customer.customer.customerNumber);
        setCurrentCustomer(customer);
      },

      /*************
        Last List
      *************/

      getLastList: function() {
        return localStorageService.get(Constants.localStorage.lastList);
      },

      getLastOrderList: function() {
        return localStorageService.get(Constants.localStorage.lastOrderList);
      },

      setLastList: function(list) { // for guest users
      localStorageService.set(Constants.localStorage.lastList, list);
      },

      setLastOrderList: function(allSets) { // for order entry users       
      localStorageService.set(Constants.localStorage.lastOrderList, allSets);
      }

    };

    function setCustomerNumber(customerNumber) {
      localStorageService.set(Constants.localStorage.customerNumber, customerNumber);
    }

    function setBranchId(branchId) {
      localStorageService.set(Constants.localStorage.branchId, branchId);
    }

    function setCurrentCustomer(customer) {
      localStorageService.set(Constants.localStorage.currentCustomer, customer);
    }


    return Service;

  }]);
