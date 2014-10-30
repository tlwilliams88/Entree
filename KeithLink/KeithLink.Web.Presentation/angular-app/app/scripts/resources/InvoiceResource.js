'use strict';

angular.module('bekApp')
  .factory('Invoice', [ '$resource', 
  function ($resource) {
    return $resource('/invoice/:invoiceId', { }, {

      // defaults: GET, QUERY
      
    });
  
  }]);
