'use strict';

/**
 * roleName filter
 * Replace role names with user facing titles
 */
angular.module('bekApp')
.filter('roleName', [ '$filter', function($filter) {
  return function(role) {

  if(role === 'buyer'){
      role = 'Shopper';
  }

  if(role === 'approver'){
    role = 'Buyer';
  } 
    return role;
  };
}]);