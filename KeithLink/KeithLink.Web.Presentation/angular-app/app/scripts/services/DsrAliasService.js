'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:DsrAliasService
 * @description
 * # DsrAliasService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('DsrAliasService', [ '$http', '$q', 'UtilityService',
    function ($http, $q, UtilityService) {

    var Service = {

      getAliasesForCurrentUser: function() {
        var promise = $http.get('/profile/dsralias');
        return UtilityService.resolvePromise(promise).then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      getAliasesForUser: function(userId) {
        var promise = $http.get('/profile/dsralias/' + userId);
        return UtilityService.resolvePromise(promise);
      },

      createAlias: function(alias) {
        var promise = $http.post('/profile/dsralias', alias);
        return UtilityService.resolvePromise(promise);
      },

      deleteAlias: function(aliasId) {
        var promise = $http.delete('/profile/dsralias', { 
          headers: {'Content-Type': 'application/json'},
          data: {
            dsrAliasId: aliasId
          }
         });
        return UtilityService.resolvePromise(promise);
      }
    
    };


    return Service;

  }]);
