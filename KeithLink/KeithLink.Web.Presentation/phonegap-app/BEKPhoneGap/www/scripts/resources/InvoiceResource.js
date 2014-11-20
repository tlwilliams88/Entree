'use strict';

angular.module('bekApp')
  .factory('Invoice', [ '$resource', 
  function ($resource) {
    return $resource('/invoice', { }, {

      // defaults: GET, QUERY
      
      getOneInvoice: {
        url: '/invoice/:invoiceNumber',
        method: 'GET',
        isArray: true
      }

    });
  
  }]);
