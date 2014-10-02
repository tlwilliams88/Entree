'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$modal', 'item', 'ProductService', 'ListService', 'CartService', 
    function ($scope, $modal, item, ProductService, ListService, CartService) {
    
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;
    
    ProductService.getProductDetails(item.itemnumber).then(function(item) {
      $scope.item.productimages = item.productimages;
    });

    // TODO: move into context menu controller
    $scope.lists = ListService.lists;
    ListService.getAllLists({
      'header': true
    });

    $scope.carts = CartService.carts;
    CartService.getAllCarts({
      'header': true
    });

    $scope.canOrderProduct = function(item) {
      return ProductService.canOrderProduct(item);
    };

    $scope.saveNote = function(itemNumber, note) {
      ProductService.updateItemNote(itemNumber, note).then(function() {
        $scope.itemNotesForm.$setPristine();
        originalItemNotes = note;
        $scope.displayMessage('success', 'Successfully updated note.');
      }, function() {
        $scope.displayMessage('error', 'Error updating note.');
      });
    };

    $scope.deleteNote = function(itemNumber) {
      ProductService.deleteItemNote(itemNumber).then(function() {
        $scope.itemNotesForm.$setPristine();
        item.notes = null;
        $scope.item.notes = null;
        $scope.displayMessage('success', 'Successfully deleted note.');
      }, function() {
        $scope.displayMessage('error', 'Error deleting note.');
      });
    };

    $scope.cancelChanges = function() {
      $scope.item.notes = originalItemNotes;
      $scope.itemNotesForm.$setPristine();
    };


    $scope.open = function (item) {

      var modalInstance = $modal.open({
        templateUrl: 'views/modal.html',
        controller: 'ModalInstanceCtrl',
        resolve: {
          lists: function () {
            return $scope.lists;
          },
          carts: function () {
            return $scope.carts;
          },
          item: function() {
            return item;
          }
        }
      });

      modalInstance.result.then(function (object) {
        $scope.selected = object;
      }, function () {
        console.log('Modal dismissed at: ' + new Date());
      });
    };
  }]);

angular.module('bekApp')
.controller('ModalInstanceCtrl', function ($scope, $modalInstance, lists, carts) {

  $scope.lists = lists;
  $scope.carts = carts;

  $scope.selectList = function(list) {
    $modalInstance.close(list);
  };

  $scope.selectCart = function(cart) {
    $modalInstance.close(cart);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
});