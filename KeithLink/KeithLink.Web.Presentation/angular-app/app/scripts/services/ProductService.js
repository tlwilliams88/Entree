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
   
    var defaultPageSize = 15,
      defaultStartingIndex = 0;
 
    function concatenateNestedParameters(name, list) {
      var filters = name + ':';
      angular.forEach(list, function (item, index) {
        filters += item + '|';
      });
      return filters.substr(0, filters.length-1);
    }
 
    var Service = {
      getProducts: function(branchId, searchTerm, pageSize, index, brands) {
        pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
        index = typeof index !== 'undefined' ? index : defaultStartingIndex;
 
        concatenateNestedParameters(brands);
 
        return $http.get('/catalog/search/' + branchId + '/' + searchTerm + '/products', {
          params: {
            size: pageSize,
            from: index
          }
        }).then(function(response) {
          return response.data;
        });
      },
 
      getProductsByCategory: function(branchId, categoryId, pageSize, index, brands, filterCategoryId) {
        pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
        index = typeof index !== 'undefined' ? index : defaultStartingIndex;
 
        var facets = '';
        if (!brands || brands.length > 0) {
          facets += concatenateNestedParameters('brands', brands);
        }
        if (brands && brands.length > 0 && filterCategoryId) {
          facets += '&';
        }
        if (filterCategoryId) {
          facets += 'category:' + categoryId;
        }
        
        return $http.get('/catalog/search/category/' + branchId + '/' + categoryId + '/products', {
          params: {
            size: pageSize,
            from: index,
            facets: facets
          }
        }).then(function(response) {
          return response.data;
        });
      },
 
      getProductDetails: function(branchId, id) {
        return $http.get('/catalog/product/' + branchId + '/' + id);
      }
    };
 
    return Service;
 
  });