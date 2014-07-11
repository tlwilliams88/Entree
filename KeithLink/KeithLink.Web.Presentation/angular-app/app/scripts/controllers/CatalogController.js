'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CatalogController', ['$scope', 'CategoryService', function ($scope, CategoryService) {
    
    CategoryService.getCategories().then(function(data) {
      $scope.categories = data.data.categories;
    });

  }]);
