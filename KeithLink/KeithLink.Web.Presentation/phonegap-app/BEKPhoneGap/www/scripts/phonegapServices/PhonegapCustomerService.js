'use strict';

angular.module('bekApp')
  .factory('PhonegapCustomerService', ['$http', '$q', 'CustomerService',
    function($http, $q, CustomerService) {

      var originalCustomerService = angular.copy(CustomerService);

      var Service = angular.extend(CustomerService, {});

      Service.getAccountBalanceInfo = function() {
        if (navigator.connection.type === 'none') {
          return $q.reject('Offline: cannot retreive account balance info.');
        } else {
          return originalCustomerService.getAccountBalanceInfo();
        }
      };

      return Service;

  }
]);