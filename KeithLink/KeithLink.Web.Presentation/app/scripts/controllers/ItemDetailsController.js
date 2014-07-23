'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$stateParams', 'ProductService', function ($scope, $stateParams, ProductService) {
    
    // ProductService.getProduct($stateParams.itemId).then(function(response) {
    //   $scope.item = response.data;
    // });

  $scope.item = {'categoryId':'FS490','id':'101285','description':'Shrimp Raw Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'5 LB','upc':'00000000000000','manufacturer_number':'B-W-26/30','manufacturer_name':'Ellington Farms Seafood','cases':'0','kosher':'true','price':'325.00'};

  $scope.print = function () {
    window.print(); 
  };

  }]);