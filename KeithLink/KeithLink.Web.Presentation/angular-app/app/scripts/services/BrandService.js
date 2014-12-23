'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CategoryService
 * @description
 * # CategoryService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('BrandService', ['$http', function ($http) {
    
  var brands;

  var Service = {

    // gets all house brands and saves them locally
    /*
    {
      "brands": [
        {
          "brand_control_label":"EF",
          "extended_description":"ELLINGTON FARMS",
          "imageurl":"http://devkeithlink.bekco.com/assets/brands/ef.jpg"
        },{
          "brand_control_label":"KC",
          "extended_description":"KEITH'S CHOICE",
          "imageurl":"http://devkeithlink.bekco.com/assets/brands/kc.jpg"
        }
      ]
    }
    */
    getHouseBrands: function() {
      if (!brands) {
        brands = $http.get('/brands/house').then(function (response) {
          return response.data.brands;
        });
      }
      return brands;
    }
  };

  return Service;
 
}]);