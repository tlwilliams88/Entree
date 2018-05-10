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

      getRecommendedCategories: function() {

        // This call is a post to allow for supporting any changes to the available parameters in the future
        // Available Parameters:
        // pagesize: num,
        // getimages: bool
        recommendedCategories = $http.post('/catalog/growthandrecovery').then(function(response) {
          return response.data.successResponse.items;
        })
        return recommendedCategories;
      }
  };

    return Service;

  });
