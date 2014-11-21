'use strict';

angular.module('bekApp')
  .factory('Invoice', [ '$resource', 
  function ($resource) {
    return $resource('/invoice', { }, {

      // defaults: GET, QUERY
      
      getTransactions: {
        url: '/invoice/:invoiceNumber',
        method: 'GET',
        isArray: true
      }

    });
  
  }]);
