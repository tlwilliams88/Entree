'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartController', ['$scope', 'CartService', function($scope,  CartService) {
    
    $scope.loadingResults = true;

    $scope.carts = CartService.carts;
    
    CartService.getAllCarts().then(function(data) {
      $scope.setCurrentCart(CartService.carts[0]);
      $scope.loadingResults = false;
    });

    $scope.setCurrentCart = function(cart) {
      angular.forEach(cart.items, function(item, itemIndex) {
        item.editNotes = item.notes;
        item.editQuantity = item.quantity;
        item.editEach = item.each;
        // item.editPosition = item.position;
      });
      $scope.currentCart = cart;
    };

    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.saveCart = function(cart) {

      var updatedCart = angular.copy(cart);
      angular.forEach(cart.items, function(item, itemIndex) {
        item.notes = item.editNotes;
        item.quantity = item.editQuantity;
        item.each = item.editEach;
        // item.editPosition = item.position;
      });

      CartService.updateCart(cart).then(function() {
        $scope.currentCart.isRenaming = false;
        $scope.sortBy = 'position';
        $scope.sortCart = false;
        $scope.currentCart = updatedCart;
        console.log('Successfully saved cart ' + cart.name);
      }, function() {
        console.log('Error saving cart ' + cart.name);
      });
    };

    $scope.renameCart = function (cartId, cartName) {
      var cart = angular.copy($scope.currentCart);
      cart.name = cartName;

      CartService.updateCart(cart).then(function(data) {
        $scope.currentCart.isRenaming = false;
        $scope.currentCart.name = cartName;
        console.log('Successfully renamed cart to ' + cartName + '.');
      }, function() {
        console.log('Error renaming cart.');
      });
    };

    $scope.deleteCart = function(cartId) {
      CartService.deleteCart(cartId).then(function() {
        console.log('Successfully deleted cart.');
      }, function() {
        console.log('Error deleting cart.');
      });
    };

  }]);