'use strict';

angular.module('bekApp')
  .factory('Cart', [ '$resource', 'UserProfileService', 
  function ($resource, UserProfileService) {
    
    function getBranch() {
      return UserProfileService.getCurrentBranchId();
    }

    return $resource('/cart/:branchId/:cartId', {
      branchId: getBranch()
    }, {

      // defaults: GET, QUERY, SAVE

      update: {
        url: '/cart',
        method: 'PUT'
      },

      delete: {
        url: '/cart/:cartId',
        method: 'DELETE'
      },

      addItem: {
        url: '/cart/:cartId/item',
        method: 'POST'
      },

      updateItem: {
        url: '/cart/:cartId/item',
        method: 'PUT'
      },

      deleteItem: {
        url: '/cart/:cartId/item',
        method: 'DELETE'
      }

    });
  
  }]);