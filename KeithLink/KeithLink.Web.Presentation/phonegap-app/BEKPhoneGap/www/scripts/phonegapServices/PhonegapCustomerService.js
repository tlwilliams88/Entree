'use strict';

angular.module('bekApp')
  .factory('PhonegapCustomerService', ['$http', '$q', '$log', 'CustomerService',
    function($http, $q, $log, CustomerService) {

      var originalCustomerService = angular.copy(CustomerService);

      var Service = angular.extend(CustomerService, {});

      Service.getAccountBalanceInfo = function() {
        if (navigator.connection.type === 'none') {
          return $q.reject('Offline: cannot load account balance information.');
        } else {
          return originalCustomerService.getAccountBalanceInfo();
        }
      };

      return Service;

  }
]);