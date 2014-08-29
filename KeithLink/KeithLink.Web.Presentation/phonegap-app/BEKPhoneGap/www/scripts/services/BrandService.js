'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BrandService', function ($http) {
    
    var brands;
 
    var Service = {
      getHouseBrands: function() {
          if (!brands) {
             brands = $http.get('/brands/house').then(function (response) {
                return response.data;
            });
          }
      return brands;
      }
  };
 
    return Service;
 
  });