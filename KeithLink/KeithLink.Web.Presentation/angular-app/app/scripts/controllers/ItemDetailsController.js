'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', function ($scope, $stateParams) {
    
    $scope.itemId = $stateParams.itemId;

    $scope.item = {
      'id':'101285',
      'description':'Shrimp Raw Hdls 26/30',
      'ext_description':'Premium Wild Texas White',
      'brand':'Philly',
      'size':'5 LB',
      'upc':'00000000000000',
      'manufacturer_number':'B-W-26/30',
      'manufacturer_name':'Philly Seafood',
      'cases':'0',
      'categoryId':'FS490',
      'kosher':'true',
      'price':'325.00'
    };


  });
