'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:AccountService
 * @description
 * # AccountService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('AccountService', [ '$q', '$filter', '$http', function ($q, $filter, $http) {
    
    var filter = $filter('filter');
 
    var Service = {
      accounts: [],

      getAccount: function() {
        $http.get('/profile/account');
      },

      getAllAccounts: function() {
        var deferred = $q.defer();
        $http.get('/profile/accounts').then(function(response) {
          var data = response.data;
          console.log(data);
          if (data.successResponse) {
            angular.copy(data.successResponse.accounts, Service.accounts);
            deferred.resolve(data.successResponse.accounts);
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;
      },

      // findAccountById: function(accountId) {
      //   var accountsFound = filter(Service.accounts, {id: accountId});
      //   if (accountsFound.length === 1) {
      //     return accountsFound[0];
      //   }
      // },

      createAccount: function(account) {
        var deferred = $q.defer();
        $http.post('/profile/account', account).then(function(response) {
          var data = response.data;
          console.log(data);
          if (data.successResponse) {
            // TODO: return just new account id
            // TODO: add new account to cache lists
            // angular.copy(data.successResponse.accounts, Service.accounts);
            deferred.resolve(data.successResponse.accounts);
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;
      },

      updateAccount: function(account) {
        $http.put('/profile/account', account);
      }
  };
 
    return Service;
 
  }]);