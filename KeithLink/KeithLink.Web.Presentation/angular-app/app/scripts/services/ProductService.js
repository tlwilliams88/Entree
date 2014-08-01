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
   
    // var Product = $resource('/catalog/search/:branchId/:searchTerm/products', {
    //   userId:123, cardId:'@id'
    // }, {
    //  search: { method:'GET', params: { branchId: '@branchId', searchTerm: '@searchTerm', size: 15, index: 0 } }
    // });


    var Service = {
      getProducts: function(branchId, searchTerm, pageSize, index) {
        pageSize = typeof pageSize !== 'undefined' ? pageSize : 15;
        index = typeof index !== 'undefined' ? index : 0;

        return $http.get('/catalog/search/' + branchId + '/' + searchTerm + '/products', {
          params: {
            size: pageSize,
            from: index
          }
        });
      },
      getProductsByCategory: function(branchId, categoryId) {
        return $http.get('/catalog/search/category/' + branchId + '/' + categoryId + '/products');
      },
      getProductDetails: function(branchId, id) {
        return $http.get('/catalog/product/' + branchId + '/' + id);
      }
    };

    return Service;

  });
