'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CategoryService', function ($http) {
    
    var categories;
 
    var Service = {
      getCategories: function() {
          if (!categories) {
             categories = $http.get('/catalog/categories').then(function (response) {
                return response.data;
            });
          }
      return categories;
      }
  };
 
    return Service;
 
  });