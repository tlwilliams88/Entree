'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', 'item', 'ProductService', 'ListService', 'CartService', function ($scope, item, ProductService, ListService, CartService) {
    
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
  }]);