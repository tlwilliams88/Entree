'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProductService
 * @description
 * # ProductService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProductService', function ($http) {
    
    var Service = {
      getProducts: function() {
        return $http.get('http://localhost:9002/ws/products');
      },
      getProductsByCategory: function(categoryId, page, etc) {
        console.log('catalog/search/category/' + categoryId + '/products?parentCategoryId=' + categoryId);
      }
    };

    return Service;

  });
