'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:AccountService
 * @description
 * # AccountService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('AccountService', [ '$http', 'UtilityService', function ($http, UtilityService) {

  var Service = {

    getAccountDetails: function(accountId) {
      var promise = $http.get('/profile/account/' + accountId);
      return UtilityService.resolvePromise(promise);
    },

    getAccountByUser: function(userid) {
      var config = {
        params: {
          userid: userid
        }
      };

      var promise = $http.get('/profile/accounts', config);
      return UtilityService.resolvePromise(promise).then(function(successResponse) {
        return successResponse.accounts[0]; // a user can only be admin on one account
      });
    },

    // getAllAccounts: function() {
    //   var promise = $http.get('/profile/accounts');
    //   return UtilityService.resolvePromise(promise).then(function(successResponse) {
    //     return successResponse.accounts;
    //   });
    // },

    searchAccounts: function(searchTerm) {
      var config = {
        params: {
          wildcard: searchTerm
        }
      };
      
      var promise = $http.get('/profile/accounts', config);
      return UtilityService.resolvePromise(promise).then(function(successResponse) {
        return successResponse.accounts;
      });
    },

    createAccount: function(account) {
      var promise = $http.post('/profile/account', account);
      return UtilityService.resolvePromise(promise).then(function(successResponse) {
        return successResponse.accounts[0];
      });
    },

    updateAccount: function(account) {
      var promise = $http.put('/profile/account', account);
      return UtilityService.resolvePromise(promise).then(function(successResponse) {
        debugger;
      });
    }
  };

  return Service;

}]);
