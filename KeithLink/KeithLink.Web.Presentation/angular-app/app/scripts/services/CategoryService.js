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

    var categories = {};

    var Service = {
      getCategories: function(catalogType) {
        if (!categories.hasOwnProperty(catalogType)) {
           categories[catalogType] = $http.get('/catalog/' + catalogType + '/categories').then(function (response) {
              var categories = $filter('filter')(response.data.successResponse.categories, {name: '!All'});
              return categories;
          });
        }
        return categories[catalogType];
      }
  };

    return Service;

  });
