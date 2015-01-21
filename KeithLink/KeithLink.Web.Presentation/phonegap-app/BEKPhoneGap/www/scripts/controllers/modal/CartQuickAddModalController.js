'use strict';

angular.module('bekApp')
.controller('CartQuickAddModalController', ['$scope', '$modalInstance', '$filter', 'CartService',
  function ($scope, $modalInstance, $filter, CartService) {

  $scope.addRow = function() {
    $scope.items.push({
      itemnumber: '',
      quantity: 0,
      each: false
    });
  };

  $scope.saveCart = function(items) {
    // filter items where quantity is greater than 0 and item number is valid
    var newItems = $filter('filter')( items, function(item) {
      return item.quantity > 0 && item.itemnumber.length === 6; 
    });
    CartService.quickAdd(newItems).then(function(cartId) {
      $modalInstance.close(cartId);
      $scope.displayMessage('success', 'Successfully added new cart.');
    }, function(error) {
      $scope.displayMessage('error', error);
    });
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.items = [];
  $scope.addRow();

}]);