'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$stateParams', 'ProductService', 'ListService', function ($scope, $stateParams, ProductService, ListService) {
    
    var itemNumber = $stateParams.itemNumber;
    $scope.loadingDetails = true;
    ProductService.getProductDetails(itemNumber).then(function(response) {
      $scope.item = response.data;
      $scope.loadingDetails = false;
    });


    ListService.getAllLists({'header': true}).then(function(data) {
     $scope.lists = data;
    }); 

    $scope.highlightRow = function(index, totalItems) {
      return Boolean((index + totalItems) % 2);
    };

  }]);