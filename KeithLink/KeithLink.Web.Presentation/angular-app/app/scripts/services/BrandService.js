'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BrandService', ['$http', '$q', function ($http, $q) {
    
    var brands;
 
    var Service = {
      getHouseBrands: function() {
        var deferred = $q.defer();

        if (brands) {
          deferred.resolve(brands);
        } else {
          $http.get('/brands/house').then(function (response) {
            var brands = response.data.brands;
            deferred.resolve(brands);
            return brands;
          });
        }
        return deferred.promise;
      }
    };
 
    return Service;
 
  }]);