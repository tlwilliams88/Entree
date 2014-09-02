'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartController', ['$scope', '$state', 'CartService', function($scope, $state,  CartService) {
    
    $scope.loadingResults = true;

    $scope.carts = CartService.carts;
    
    CartService.getAllCarts().then(function(data) {
      $scope.currentCart = CartService.carts[0];
      $scope.goToCart();
      $scope.sortBy = null;
      $scope.sortOrder = false;
      $scope.loadingResults = false;
    });

    $scope.goToCart = function() {
      angular.forEach($scope.currentCart.items, function(item, index) {
        item.packageprice = 4;
        item.caseprice = 16;
      });
      $state.transitionTo('menu.cartitems', {cartId: $scope.currentCart.id}, {notify: false});
    };

    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.saveCart = function(cart) {
      CartService.updateCart(cart).then(function() {
        $scope.currentCart.isRenaming = false;
        $scope.sortBy = null;
        $scope.sortOrder = false;
        $scope.currentCart = cart;
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

    $scope.getSubtotal = function(cartItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        subtotal +=( item.quantity * (item.each ? item.packageprice : item.caseprice) );
      });
      return subtotal;
    };

  }]);