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
    
    function all() {
      return $http.get(
          ApiService.endpointUrl +'/catalog/search/products',
          {
            responseType: 'json' 
          }
        )
        .then(function(response) {
          Service.products = response.data.products;
        });
    }

    var Service = {
      getProducts: function() {
        return all();
      },
      getProductsByCategory: function(categoryId, page, etc) {
        console.log('catalog/search/category/' + categoryId + '/products?parentCategoryId=' + categoryId);
      },
      getProduct: function(id) {
        return $http.get(ApiService.endpointUrl +'/catalog/product/' + id);
      }
    };

    return Service;

  }]);
