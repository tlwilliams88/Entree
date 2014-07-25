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
    
    var itemNumber = $stateParams.itemNumber;
    if ($stateParams.item) {
      $scope.item = $stateParams.item;
    } else {
      $scope.loadingDetails = true;
      ProductService.getProductDetails($scope.currentUser.currentLocation.branchId, itemNumber).then(function(response) {
        $scope.item = response.data;
        $scope.loadingDetails = false;
      });
    }

    $scope.print = function () {
      window.print(); 
    };

  }]);