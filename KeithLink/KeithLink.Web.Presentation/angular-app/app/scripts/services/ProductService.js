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
      getProducts: function(branchId, searchTerm) {
        console.log(ApiService.endpointUrl +'/catalog/search/' + branchId + '/' + searchTerm + '/products');
        return $http.get(ApiService.endpointUrl +'/catalog/search/' + branchId + '/' + searchTerm + '/products');
      },
      getProductsByCategory: function(branchId, categoryId) {
        console.log(ApiService.endpointUrl +'/catalog/search/category/' + branchId + '/' + categoryId + '/products');
        return $http.get(ApiService.endpointUrl +'/catalog/search/category/' + branchId + '/' + categoryId + '/products');
      },
      getProductDetails: function(branchId, id) {
        console.log(ApiService.endpointUrl +'/catalog/product/' + branchId + '/' + id);
        return $http.get(ApiService.endpointUrl +'/catalog/product/' + branchId + '/' + id);
      }
    };

    return Service;

  }]);
