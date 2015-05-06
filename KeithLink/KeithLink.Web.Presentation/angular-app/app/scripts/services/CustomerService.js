'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CustomerService
 * @description
 * # CustomerService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CustomerService', [ '$http', '$q', 'UtilityService', 
    function ($http, $q, UtilityService) {

    var categories;

    var Service = {
      // gets customers for current user
      getCustomers: function(searchTerm, size, from, field, order, customerGroupId) {
        if (!customerGroupId) {
          customerGroupId = '';
        }

        var data = {
          params: {
            size: size,
            from: from,
            terms: searchTerm,
            field: field,
            order: order,
            account: customerGroupId
          }
        };
        return $http.get('/profile/customer', data).then(function(response) {
          return response.data;
        });
      },

      getCustomerDetails: function(customerNumber, branchNumber) {
        var promise = $http.get('/profile/customer/' + branchNumber + '/' + customerNumber);
        return UtilityService.resolvePromise(promise);
      },

      getAccountBalanceInfo: function() {
        return $http.get('/profile/customer/balance').then(function(response) {
          return response.data;
        }, function() {
          return $q.reject('Error getting account balance information.');
        });
      }
  };

    return Service;

  }]);
