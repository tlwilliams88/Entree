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
    
    var id = $stateParams.itemId;
    if ($stateParams.item) {
      $scope.item = $stateParams.item;
    } else {
      ProductService.getProductDetails($scope.currentUser.currentLocation.branchId, id).then(function(response) {
        $scope.item = response.data;
      });
    }

    $scope.print = function () {
      window.print(); 
    };

  }]);