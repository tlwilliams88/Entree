'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CategoryService', function ($http, $filter) {

    var categories = {},
        recommendedCategories = {};

    var Service = {
      getCategories: function(catalogType) {
        if (!categories.hasOwnProperty(catalogType)) {
           categories[catalogType] = $http.get('/catalog/' + catalogType + '/categories').then(function (response) {
              Service.categories = response.data.successResponse.categories;
              return response.data.successResponse.categories;
          });
        }
        return categories[catalogType];
      },

      getRecommendedCategories: function(pagesize, getimages) {

        var params = {
          pagesize: pagesize,
          getimages: getimages
        }

        recommendedCategories = $http.post('/catalog/growthandrecovery', params).then(function(response) {
          return response.data.successResponse.items;
        })
        return recommendedCategories;
      }
  };

    return Service;

  });
