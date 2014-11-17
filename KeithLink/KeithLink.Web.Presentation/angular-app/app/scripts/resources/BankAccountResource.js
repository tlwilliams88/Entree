'use strict';

angular.module('bekApp')
  .factory('BankAccount', [ '$resource', 
  function ($resource) {
    return $resource('/banks/:bankAccountNumber', { }, {

      // defaults, GET, QUERY

    });
  
  }]);
