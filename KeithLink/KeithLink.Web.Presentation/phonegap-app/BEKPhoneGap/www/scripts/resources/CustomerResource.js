'use strict';

angular.module('bekApp')
  .factory('Customer', [ '$resource', 
  function ($resource) {
    return $resource('/customer', { }, {

      query: {
        url: '/profile/customers',
        method: 'GET',
        isArray: true
      }

    });
  
  }]);
