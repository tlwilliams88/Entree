'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProductService
 * @description
 * # ProductService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProductService', ['$http', 'UserProfileService', 'RecentlyViewedItem','ItemNotes', 'Constants', 'ExportService',
    function($http, UserProfileService, RecentlyViewedItem, ItemNotes, Constants, ExportService) {

      var defaultPageSize = Constants.infiniteScrollPageSize,
        defaultStartingIndex = 0;

      function filterNonstock(itemspecs) {
        var nonstockIndex = -1;
        itemspecs.forEach(function(spec, index) {
          if (spec === 'nonstock') {
            nonstockIndex = index;
          }
        });
        return nonstockIndex;
      }

      var Service = {
        selectedProduct: {}, 

        getFacets: function(categories, brands, manufacturers, dietary, itemspecs) {
          var facets = [];

          // handle nonstock special case
          var nonstockIndex = filterNonstock(itemspecs);
          var cleanItemspecs = angular.copy(itemspecs);
          if (nonstockIndex > -1) {
            cleanItemspecs.splice(nonstockIndex, 1);
          }

          if (categories && categories.length > 0) {
            facets.push('categories:' + categories.join('|'));
          }
          if (brands && brands.length > 0) {
            facets.push('brands:' + brands.join('|'));
          }
          if (manufacturers && manufacturers.length > 0) {
              facets.push('mfrname:' + manufacturers.join('|'));
          }
          if (dietary && dietary.length > 0) {
            facets.push('dietary:' + dietary.join('|'));
          }
          if (cleanItemspecs && cleanItemspecs.length > 0) {
            facets.push('itemspecs:' + cleanItemspecs.join('|'));
          }
          if (nonstockIndex > -1) {
            facets.push('nonstock:y');
          }

          facets = facets.join('___'); // join all facets together for query string

          if (facets === '') {
            facets = null;
          }
          return facets;
        },

        getSearchParams: function(pageSize, index, sortField, sortDirection, facets) {
          var params = {
            size: pageSize  || defaultPageSize,
            from: index || defaultStartingIndex,
            facets: facets,
            sfield: sortField,
            sdir: sortDirection
          };
          if (!params.facets) {
            delete params.facets;
          } 
          if (!params.sfield) {
            delete params.sfield;
          }
          return params;  
        },

        getSearchUrl: function(type, id) {
          var url = '/catalog/search/' + id + '/products'; // default to search url

          if (type === 'category') {
            url = '/catalog/search/category/' + id + '/products';
          } else if (type === 'brand') {
            url = '/catalog/search/brands/house/' + id;
          }
          return url;
        },

        searchCatalog: function(type, id, params) {
          
          var url = Service.getSearchUrl(type, id);
          
          var config = {
            params: params
          };

          return $http.get(url, config).then(function(response) {
            var data = response.data;

            // convert nonstock data structure to match other itemspecs
            if (data.facets.nonstock && data.facets.nonstock.length > 0) {
              data.facets.nonstock.forEach(function(nonstockItem) {
                if (nonstockItem.name === 'y') {
                  data.facets.itemspecs.push({
                    name: 'nonstock',
                    count: nonstockItem.count // yes
                  });
                }
              });
            }

            return data;
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

        /****************
        ITEM NOTES
        ****************/
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

        /****************
        RECENTLY VIEWED ITEMS
        ****************/
        saveRecentlyViewedItem: function(itemNumber) {
          return RecentlyViewedItem.save({
            itemNumber: itemNumber
          }, {}).$promise;
        },

        getRecentlyViewedItems: function() {
          return RecentlyViewedItem.query({}).$promise;
        },

        /****************
        EXPORT
        ****************/
        getExportConfig: function() {
          return $http.get('/catalog/export').then(function(response) {
            return response.data;
          });
        },

        exportProducts: function(config, url) {
          url = url.replace('search', 'export');
          ExportService.export(url, config);
        }

      };

      return Service;

    }
  ]);
