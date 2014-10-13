'use strict';

angular.module('bekApp')
  .factory('Order', [ '$resource', 
  function ($resource) {
    return $resource('/order/:orderNumber', { }, {

      // defaults: GET, QUERY, SAVE

      submit: {
        url: '/order/:cartId',
        method: 'POST'
      }

    });
  
  }]);
