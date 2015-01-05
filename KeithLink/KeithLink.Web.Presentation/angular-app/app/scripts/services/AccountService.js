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
        return successResponse.accounts[0];
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
          // wildcard: searchTerm
          userid: '4065067c-bae0-41cd-a2f9-e89f377d4386'
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
