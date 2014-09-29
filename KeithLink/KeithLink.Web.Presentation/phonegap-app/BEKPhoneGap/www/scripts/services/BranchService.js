'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BranchService', function ($http) {
    
    var branches;
 
    var Service = {
      getBranches: function() {
          if (!branches) {
             branches = $http.get('/catalog/divisions').then(function (response) {
                return response.data;
            });
          }
      return branches;
      }
  };
 
    return Service;
 
  });