'use strict';

angular.module('bekApp')
  .factory('Account', [ '$resource', 
  function ($resource) {
    return $resource('/account', { }, {

      query: {
        url: '/profile/accounts',
        method: 'GET',
        isArray: true
      },

      save: {
        url: '/profile/account',
        method: 'POST'
      },

      // update: {
      //   url: '/profile/account',
      //   method: 'PUT'
      // }

      addCustomer: {
        url: '/profile/account/customer',
        method: 'PUT'
      },

      // addUser: {
      //   url: '/profile/account/user',
      //   method: 'PUT'
      // },

      // deleteCustomer: {
      //   url: '/profile/account/customer',
      //   method: 'DELETE'
      // },

      // deleteUser: {
      //   url: '/profile/account/user',
      //   method: 'DELETE'
      // }

    });
  
  }]);
