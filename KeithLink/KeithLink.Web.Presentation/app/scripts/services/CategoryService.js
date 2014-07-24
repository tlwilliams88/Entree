'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CategoryService', ['$http', 'ApiService', function ($http, ApiService) {
    
    var Service = {
      getCategories: function() {
        console.log(ApiService.endpointUrl +'/catalog/categories');
        return $http.get(ApiService.endpointUrl +'/catalog/categories');
      }
    };

    return Service;

  }]);
