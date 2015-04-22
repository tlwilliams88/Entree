'use strict';

angular.module('bekApp')
.controller('CartQuickAddModalController', ['$scope', '$modalInstance', '$filter', 'CartService',
  function ($scope, $modalInstance, $filter, CartService) {

  $scope.enableSubmit = false;

  $scope.addRow = function() {
    $scope.items.push({
      itemnumber: '',
      quantity: 0,
      each: false
    });
  };

  $scope.removeRow = function(item) {
    var idx = $scope.items.indexOf(item);
    $scope.items.splice(idx, 1);
  };

  function getRowsWithQuantity(items) {
    return $filter('filter')( items, function(item) {
      return item.quantity > 0 && item.itemnumber.length === 6; 
    });
  }

  $scope.validateItems = function(items) {
    var invalidItemsExist = false;
    $scope.items = getRowsWithQuantity(items);
    CartService.validateQuickAdd($scope.items).then(function(validatedItems) {
      $scope.items.forEach(function(item, index) {
        var validatedItem = validatedItems[index];
        item.valid = validatedItem.valid;

        if (validatedItem.valid === false) {
          invalidItemsExist = true;
        }

        if (validatedItem.reason === 0) {
          item.reason = 'Invalid item number';
        } else if (validatedItem.reason === 1) {
          item.reason = 'Each not allowed for this item';
        }
      });

      $scope.enableSubmit = !invalidItemsExist;
    });
  };

  $scope.saveCart = function(items) {
    // filter items where quantity is greater than 0 and item number is valid
    var newItems = getRowsWithQuantity(items);
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