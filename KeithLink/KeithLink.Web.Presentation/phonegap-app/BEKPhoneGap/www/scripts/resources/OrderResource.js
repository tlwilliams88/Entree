'use strict';

angular.module('bekApp')
  .factory('Order', [ '$resource', 
  function ($resource) {
    return $resource('/order/:orderNumber', { }, {

      // defaults: GET, QUERY, SAVE

    });
  
  }]);
