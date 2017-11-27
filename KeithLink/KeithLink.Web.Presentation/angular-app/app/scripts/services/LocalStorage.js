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
        var last = localStorageService.get(Constants.localStorage.lastOrderList) || [];
        return last;
      },

      setLastList: function(list) {
        localStorageService.set(Constants.localStorage.lastList, list);
      },

      setLastOrderList: function(allSets) {        
        localStorageService.set(Constants.localStorage.lastOrderList, allSets);
      },

      /*************
        System Variables
      *************/

      getPageSize: function() {
        var pageSize = localStorageService.get(Constants.localStorage.pageSize) || 50;
        return parseInt(pageSize);
      },

      setPageSize: function(size) { 
        localStorageService.set(Constants.localStorage.pageSize, size);
      },

      getDefaultSort: function() {
        return localStorageService.get(Constants.localStorage.defaultSort);
      },

      setDefaultSort: function(sort) { 
        localStorageService.set(Constants.localStorage.defaultSort, sort);
      },

      getDefaultView: function() {
        return localStorageService.get(Constants.localStorage.defaultView);
      },
      
      setDefaultView: function(view) {
        localStorageService.set(Constants.localStorage.defaultView, view);
      },

      setDefaultUserName: function(username) {
        localStorageService.set(Constants.localStorage.userName, username);
      },

      getDefaultUserName: function() {
        return localStorageService.get(Constants.localStorage.userName);
      },

      setUserId: function(userId) {
        localStorageService.set(Constants.localStorage.userId, userId);
      },

      getUserId: function() {
        return localStorageService.get(Constants.localStorage.userId);
      },

      setRoleName: function(roleName) {
        localStorageService.set(Constants.localStorage.roleName, roleName);
      },

      getRoleName: function() {
        return localStorageService.get(Constants.localStorage.roleName);
      },

      setIsInternalUser: function(isInternalUser) {
        localStorageService.set(Constants.localStorage.isInternalUser, isInternalUser);
      },

      getIsInternalUser: function() {
        return localStorageService.get(Constants.localStorage.isInternalUser);
      },

      setIsKbitCustomer: function(isKbitCustomer) {
        localStorageService.set(Constants.localStorage.isKbitCustomer, isKbitCustomer);
      },

      getIsKbitCustomer: function() {
        return localStorageService.get(Constants.localStorage.isKbitCustomer);
      },

      setIsPowerMenuCustomer: function(isPowerMenuCustomer) {
        localStorageService.set(Constants.localStorage.isPowerMenuCustomer, isPowerMenuCustomer);
      },

      getIsPowerMenuCustomer: function() {
        return localStorageService.get(Constants.localStorage.isPowerMenuCustomer);
      },

      setHideTutorialHomePage: function(hideTutorial){
        localStorageService.set(Constants.localStorage.hideTutorialHome, hideTutorial);
      },

      getHideTutorialHomePage: function(){
        return localStorageService.get(Constants.localStorage.hideTutorialHome);
      },

      setHideTutorialSearch: function(hideTutorial){
        localStorageService.set(Constants.localStorage.hideTutorialSearch, hideTutorial);
      },

      getHideTutorialSearch: function(){
        return localStorageService.get(Constants.localStorage.hideTutorialSearch);
      },

      setHideTutorialAddToOrder: function(hideTutorial){
        localStorageService.set(Constants.localStorage.hideTutorialAddToOrder, hideTutorial);
      },

      getHideTutorialAddToOrder: function(){
        return localStorageService.get(Constants.localStorage.hideTutorialAddToOrder);
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
