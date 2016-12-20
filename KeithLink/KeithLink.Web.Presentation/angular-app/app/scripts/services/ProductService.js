'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ProductService
 * @description
 * # ProductService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ProductService', ['$http', '$q', '$analytics', 'UserProfileService', 'RecentlyViewedItem','ItemNotes', 'Constants', 'ExportService',
    function($http, $q, $analytics, UserProfileService, RecentlyViewedItem, ItemNotes, Constants, ExportService) {

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

        getFacets: function(brands, manufacturers, dietary, itemspecs, temp_zone, parentcategories, subcategories) {
          var facets = [];

          // handle nonstock special case
          var nonstockIndex = filterNonstock(itemspecs);
          var cleanItemspecs = angular.copy(itemspecs);
          if (nonstockIndex > -1) {
            cleanItemspecs.splice(nonstockIndex, 1);
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
          if (temp_zone && temp_zone.length > 0){
            facets.push('temp_zone:' + temp_zone.join('|'));
          }
          if (parentcategories && parentcategories.length > 0){
            facets.push('parentcategories:' + parentcategories.join('|'));
          }
          if (subcategories && subcategories.length > 0){
            facets.push('categories:' + subcategories.join('|'));
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

        getSearchParams: function(pageSize, index, sortField, sortDirection, facets, department) {
          var params = {
            size: pageSize,
            from: index || defaultStartingIndex,
            dept: department,
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

        getSearchUrl: function(type, id, catalogType) {
          var url = '/catalog/' + catalogType +'/search/' + id + '/products'; // default to search url

          if (type === 'category') {
            url = '/catalog/search/category/' + catalogType + '/' + id + '/products';
          } else if (type === 'housebrand') {
            url = '/catalog/search/brands/house/' + id;
          } else if(type === 'brand') {
            url = '/catalog/' + catalogType + '/search/' + type + '/' + id + '/products';
          }
          return url;
        },

        searchCatalog: function(type, id, catalogType, params, department) {
          if(type === 'search'){

            var dept = (params.dept === '') ? 'All' : params.dept;
       
            $analytics.eventTrack('Search Department', {  category: 'Search', label: department });
            $analytics.eventTrack('Search Terms', {  category: 'Search', label: id });
          }

          var url = Service.getSearchUrl(type, id, catalogType);
          
          var config = {
            params: params
          };

          return $http.get(url, config).then(function(response) {
            if(response.data.successResponse){
                var dataTotalCount = response.data.successResponse.totalcount;
                config.params.size = dataTotalCount;    
            }

            // return $http.get(url, config).then(function(response){
              var data = response.data.successResponse;

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
            // });
          });
        },

        getProductDetails: function(itemNumber, catalogType) {
          var returnProduct;
          if (!Service.selectedProduct.name) {
            returnProduct = $http.get('/catalog/' + catalogType + '/product/' + itemNumber).then(function(response) {
              if(response.data.successResponse){
                return response.data.successResponse;
              }
              else{
                return null;
              }             
            });
          } else {
            returnProduct = Service.selectedProduct;
            Service.selectedProduct = {};
          }
          return returnProduct;
        },

        scanProduct: function(itemNumber) {
          return $http.get('/catalog/product/scan/' + itemNumber).then(function(response) {
            if (response.data) {
              return response.data.successResponse; // item found, return item object
            } else {
              return;
            }
          });
        },

        /****************
        ITEM NOTES
        ****************/
        updateItemNote: function(itemNumber, note, catalogid) {
          var itemNote = {
            itemnumber: itemNumber,
            note: note,
            catalog_id: catalogid
          };
          return ItemNotes.save(null, itemNote).$promise.then(function(resp){
            return resp.successResponse;
          });
        },

        deleteItemNote: function(itemNumber) {
          return ItemNotes.delete({
            itemNumber: itemNumber
          }).$promise.then(function(resp){
            return resp.successResponse;
          });
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
          return RecentlyViewedItem.get({}).$promise.then(function(response){
            return response.successResponse;
          });
        },

        clearRecentlyViewedItems: function() {
          return RecentlyViewedItem.delete({}).$promise;
        },

        /****************
        EXPORT
        ****************/
        getExportConfig: function() {
          return $http.get('/catalog/export').then(function(response) {
            return response.data.successResponse;
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
