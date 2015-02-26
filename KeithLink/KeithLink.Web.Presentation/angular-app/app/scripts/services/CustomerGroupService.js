'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CustomerGroupService
 * @description
 * # CustomerGroupService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CustomerGroupService', [ '$http', 'UtilityService', function ($http, UtilityService) {

  var Service = {

    getGroupDetails: function(groupId) {
      var promise = $http.get('/profile/account/' + groupId);
      return UtilityService.resolvePromise(promise);
    },

    getGroupByUser: function(userid) {
      var config = {
        params: {
          userid: userid
        }
      };

      var promise = $http.get('/profile/accounts', config);
      return UtilityService.resolvePromise(promise).then(function(successResponse) {
        return successResponse.accounts[0]; // a user can only be on one account
      });
    },

    // getAllAccounts: function() {
    //   var promise = $http.get('/profile/accounts');
    //   return UtilityService.resolvePromise(promise).then(function(successResponse) {
    //     return successResponse.accounts;
    //   });
    // },

    getGroups: function(params) {
      return $http.post('/profile/accounts', params).then(function(response) {
        return response.data;
      });
    },

    getUsersForGroup: function(accountId) {
      var promise = $http.get('/profile/account/' + accountId + '/users');
      return UtilityService.resolvePromise(promise);
    },

    // searchGroups: function(searchTerm) {
    //   var config = {
    //     params: {
    //       wildcard: searchTerm
    //     }
    //   };
      
    //   var promise = $http.get('/profile/accounts', config);
    //   return UtilityService.resolvePromise(promise).then(function(successResponse) {
    //     return successResponse.accounts;
    //   });
    // },

    createGroup: function(customerGroup) {
      var promise = $http.post('/profile/account', customerGroup);
      return UtilityService.resolvePromise(promise).then(function(successResponse) {
        return successResponse.accounts[0];
      });
    },

    updateGroup: function(customerGroup) {
      var promise = $http.put('/profile/account', customerGroup);
      return UtilityService.resolvePromise(promise);
    }
  };

  return Service;

}]);
