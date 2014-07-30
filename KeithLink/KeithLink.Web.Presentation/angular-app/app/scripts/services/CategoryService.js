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
    
    var Service = {
      getCategories: function() {
        return $http.get('/catalog/categories');
      }
    };

    return Service;

  });
