'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProductService
 * @description
 * # ProductService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProductService', ['$http', 'UserProfileService',
    function($http, UserProfileService) {

      var defaultPageSize = 30,
        defaultStartingIndex = 0;

      function concatenateNestedParameters(name, list) {
        var filters = name + ':';
        angular.forEach(list, function(item, index) {
          if (name === "brands" || name === "allergens" || name === "dietary" || name==="itemspecs") {
            filters += item + '|';
          }
          // if (name === "itemspecs") {
          //   filters += item.name + '|';
          // }
        });
        return filters.substr(0, filters.length - 1);
      }

      var Service = {
        getProducts: function(searchTerm, pageSize, index, brands, facetCategory, allergens, dietary, itemspecs, sortfield, sortdirection) {
          pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
          index = typeof index !== 'undefined' ? index : defaultStartingIndex;

          var branchId = UserProfileService.getCurrentLocation().branchId;

          var facets = '';
          if (brands && brands.length > 0) {
            facets += concatenateNestedParameters('brands', brands);
            facets += ',';
          }
          if (facetCategory) {
            facets += 'categories:' + facetCategory.name;
            facets += ',';
          }
          if (allergens && allergens.length > 0) {
            facets += concatenateNestedParameters('allergens', allergens);
            facets += ',';
          }
          if (dietary && dietary.length > 0) {
            facets += concatenateNestedParameters('dietary', dietary);
            facets += ',';
          }
          if (itemspecs && itemspecs.length > 0) {
            facets += concatenateNestedParameters('itemspecs', itemspecs);
            facets += ',';
          }

          if (facets === '') {
            facets = null;
          }
          else {
            facets = facets.substr(0, facets.length - 1);
          }

          return $http.get('/catalog/search/' + branchId + '/' + searchTerm + '/products', {
            params: {
              size: pageSize,
              from: index,
              facets: facets,
              sfield: sortfield,
              sdir: sortdirection
            }
          }).then(function(response) {
            return response.data;
          });
        },

        getProductsByCategory: function(categoryId, pageSize, index, brands, facetCategory, allergens, dietary, itemspecs, sortfield, sortdirection) {
          pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
          index = typeof index !== 'undefined' ? index : defaultStartingIndex;

          var branchId = UserProfileService.getCurrentLocation().branchId;

          var facets = '';
          if (brands && brands.length > 0) {
            facets += concatenateNestedParameters('brands', brands);
            facets += ',';
          }
          if (facetCategory) {
            categoryId = facetCategory.name;
          }
          if (allergens && allergens.length > 0) {
            facets += concatenateNestedParameters('allergens', allergens);
            facets += ',';
          }
          if (dietary && dietary.length > 0) {
            facets += concatenateNestedParameters('dietary', dietary);
            facets += ',';
          }
          if (itemspecs && itemspecs.length > 0) {
            facets += concatenateNestedParameters('itemspecs', itemspecs);
            facets += ',';
          }
          
          if (facets === '') {
            facets = null;
          }
          else {
            facets = facets.substr(0, facets.length - 1);
          }

          return $http.get('/catalog/search/category/' + branchId + '/' + categoryId + '/products', {
            params: {
              size: pageSize,
              from: index,
              facets: facets,
              sfield: sortfield,
              sdir: sortdirection
            }
          }).then(function(response) {
            return response.data;
          });
        },

        getProductDetails: function(id) {
          var branchId = UserProfileService.getCurrentLocation().branchId;
          return $http.get('/catalog/product/' + branchId + '/' + id);
        }
      };

      return Service;

    }
  ]);
