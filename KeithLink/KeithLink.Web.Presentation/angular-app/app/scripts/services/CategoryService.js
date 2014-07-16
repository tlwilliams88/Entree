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
        return $http.get(ApiService.endpointUrl + '/ws/categories');
      }
      //,
      //getSubcategories = function(categoryId) {
      //	return $http.get('http://localhost:9002/ws/category/FS940/categories')
      //}
    };

    return Service;

  }]);
