'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProductService
 * @description
 * # ProductService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProductService', ['$http', 'UserProfileService', 'RecentlyViewedItem','ItemNotes', 
    function($http, UserProfileService, RecentlyViewedItem, ItemNotes) {

      var defaultPageSize = 50,
        defaultStartingIndex = 0;

      function concatenateNestedParameters(name, list) {
        var filters = name + ':';
        angular.forEach(list, function(item, index) {
          if (name === 'brands' || name === 'dietary' || name==='itemspecs') {
            filters += item + '|';
          }
          if (name === 'nonstock') {
            filters += 'y|';
          }
        });
        return filters.substr(0, filters.length - 1);
      }

      var Service = {
        selectedProduct: {}, 

        getProducts: function(searchTerm, pageSize, index, brands, facetCategory, dietary, itemspecs, nonstock, sortfield, sortdirection) {
          pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
          index = typeof index !== 'undefined' ? index : defaultStartingIndex;

          var facets = ''; // todo: convert this to json
          var facetSeparator = '___';
          if (brands && brands.length > 0) {
            facets += concatenateNestedParameters('brands', brands);
            facets += facetSeparator;
          }
          if (facetCategory) {
            facets += 'categories:' + facetCategory.name;
            facets += facetSeparator;
          }
          if (dietary && dietary.length > 0) {
            facets += concatenateNestedParameters('dietary', dietary);
            facets += facetSeparator;
          }
          if (itemspecs && itemspecs.length > 0) {
            facets += concatenateNestedParameters('itemspecs', itemspecs);
            facets += facetSeparator;
          }
          if (nonstock && nonstock.length > 0) {
            facets += concatenateNestedParameters('nonstock', nonstock);
            facets += facetSeparator;
          }

          if (facets === '') {
            facets = null;
          }
          else {
            facets = facets.substr(0, facets.length - 3);
          }

          return $http.get('/catalog/search/' + searchTerm + '/products', {
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

        getProductsByCategory: function(categoryName, pageSize, index, brands, facetCategory, dietary, itemspecs, nonstock, sortfield, sortdirection) {
          pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
          index = typeof index !== 'undefined' ? index : defaultStartingIndex;

          var facets = '';
          var facetSeparator = '___';
          if (brands && brands.length > 0) {
            facets += concatenateNestedParameters('brands', brands);
            facets += facetSeparator;
          }
          if (facetCategory) {
            //categoryName = facetCategory.name;
          }
          if (dietary && dietary.length > 0) {
            facets += concatenateNestedParameters('dietary', dietary);
            facets += facetSeparator;
          }
          if (itemspecs && itemspecs.length > 0) {
            facets += concatenateNestedParameters('itemspecs', itemspecs);
            facets += facetSeparator;
          }
          if (nonstock && nonstock.length > 0) {
            facets += concatenateNestedParameters('nonstock', nonstock);
            facets += facetSeparator;
          }
          
          if (facets === '') {
            facets = null;
          }
          else {
            facets = facets.substr(0, facets.length - 3);
          }

          return $http.get('/catalog/search/category/' + categoryName + '/products', {
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

        getProductsByHouseBrand: function(houseBrandId, pageSize, index, brands, facetCategory, dietary, itemspecs, nonstock, sortfield, sortdirection) {
          pageSize = typeof pageSize !== 'undefined' ? pageSize : defaultPageSize;
          index = typeof index !== 'undefined' ? index : defaultStartingIndex;
          
          var facets = '';
          var facetSeparator = '___';
          if (brands && brands.length > 0) {
            facets += concatenateNestedParameters('brands', brands);
            facets += facetSeparator;
          }
          if (facetCategory) {
            facets += 'categories:' + facetCategory.name;
            facets += facetSeparator;
          }
          if (dietary && dietary.length > 0) {
            facets += concatenateNestedParameters('dietary', dietary);
            facets += facetSeparator;
          }
          if (itemspecs && itemspecs.length > 0) {
            facets += concatenateNestedParameters('itemspecs', itemspecs);
            facets += facetSeparator;
          }
          if (nonstock && nonstock.length > 0) {
            facets += concatenateNestedParameters('nonstock', nonstock);
            facets += facetSeparator;
          }

          if (facets === '') {
            facets = null;
          }
          else {
            facets = facets.substr(0, facets.length - 3);
          }

          return $http.get('/catalog/search/brands/house/' + houseBrandId, {
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
          var returnProduct;
          if (!Service.selectedProduct.name) {
            returnProduct = $http.get('/catalog/product/' + id).then(function(response) {
              return response.data;
            });
          } else {
            returnProduct = Service.selectedProduct;
            Service.selectedProduct = {};
          }
          return returnProduct;
        },

        // ITEM NOTES
        updateItemNote: function(itemNumber, note) {
          var itemNote = {
            itemnumber: itemNumber,
            note: note
          };
          return ItemNotes.save(null, itemNote).$promise;
        },

        deleteItemNote: function(itemNumber) {
          return ItemNotes.delete({
            itemNumber: itemNumber
          }).$promise;
        },

        // RECENTLY VIEWED ITEMS
        saveRecentlyViewedItem: function(itemNumber) {
          return RecentlyViewedItem.save({
            itemNumber: itemNumber
          }, {}).$promise;
        },

        getRecentlyViewedItems: function() {
          return RecentlyViewedItem.query({}).$promise;
        }
      };

      return Service;

    }
  ]);
