'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:SessionService
 * @description
 * # SessionService
 * Service of the bekApp
 */
angular.module('bekApp').factory('SessionService', function() {
  var storage = {

    userProfile: null,

  };
  return storage;
});
