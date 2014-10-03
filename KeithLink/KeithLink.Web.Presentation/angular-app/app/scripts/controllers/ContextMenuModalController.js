'use strict';

angular.module('bekApp')
.controller('ContextMenuModalController', ['$scope', '$modalInstance', '$state', '$q', 'ListService', 'CartService', 'lists', 'carts', 'item', 
  function ($scope, $modalInstance, $state, $q, ListService, CartService, lists, carts, item) {

  $scope.lists = lists;
  $scope.carts = carts;
  $scope.item = item;

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.addItemToFavorites = function(item) {
    var newItem = angular.copy(item);
    ListService.addItemToFavorites(newItem).then(function(data) {
      item.favorite = true;
      $modalInstance.close(item);
      $scope.displayMessage('success', 'Successfully added item to Favorites List.');
    }, function() {
      $scope.displayMessage('error', 'Error adding item to Favorites List.');
    });
  };

  $scope.addItemToList = function(listId, item) {
    var newItem = angular.copy(item);

    $q.all([
      ListService.addItem(listId, item),
      ListService.addItemToFavorites(item)
    ]).then(function(data) {
      item.favorite = true;
      $modalInstance.close(item);
      $scope.displayMessage('success', 'Successfully added item to list.');
    }, function() {
      $scope.displayMessage('error', 'Error adding item to list.');
    });
  };

  $scope.createListWithItem = function(item) {
    $q.all([
      ListService.createList(item),
      ListService.addItemToFavorites(item)
    ]).then(function(data) {
      $modalInstance.close(item);
      $state.go('menu.lists.items', { listId: data[0].listid, renameList: true });
      $scope.displayMessage('success', 'Successfully created new list.');
    }, function() {
      $scope.displayMessage('error', 'Error creating new list.');
    });
  };

  $scope.addItemToCart = function(cartId, item) {
    CartService.addItemToCart(cartId, item).then(function(data) {
      $modalInstance.close(item);
      $scope.displayMessage('success', 'Successfully added item to cart.');
    }, function() {
      $scope.displayMessage('error', 'Error adding item to cart.');
    });
  };

  $scope.createCartWithItem = function(item) {
    var items = [item];
    CartService.createCart(items).then(function(data) {
      $modalInstance.close(item);
      $state.go('menu.cart.items', { cartId: data.id, renameCart: true });
      $scope.displayMessage('success', 'Successfully created new cart.');
    }, function() {
      $scope.displayMessage('error', 'Error creating new cart.');
    });
  };
}]);