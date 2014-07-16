'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProductService
 * @description
 * # ProductService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProductService', ['$http', 'ApiService', function ($http, ApiService) {
    
    var Service = {
      getProducts: function() {
        return $http.get(ApiService.endpointUrl + '/ws/products');
      },
      getProductsByCategory: function(categoryId, page, etc) {
        console.log('catalog/search/category/' + categoryId + '/products?parentCategoryId=' + categoryId);
      }
    };

    return Service;

  }]);
