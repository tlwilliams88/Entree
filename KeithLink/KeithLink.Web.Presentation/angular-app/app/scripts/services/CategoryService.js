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
    
    function all() {
      return $http.get(
          ApiService.endpointUrl +'/catalog/categories',
          {
            responseType: 'json' 
          }
        )
        .then(function(response) {
          Service.categories = response.data.categories;
        });
    }

    var Service = {
      categories: [],
      getCategories: function() {
        return all();
      }
    };

    return Service;

  }]);
