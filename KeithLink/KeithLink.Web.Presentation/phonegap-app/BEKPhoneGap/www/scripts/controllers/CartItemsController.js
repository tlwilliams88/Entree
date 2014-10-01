'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', 'Constants', 'CartService', 'OrderService',
    function($scope, $state, $stateParams, $filter, Constants, CartService, OrderService) {
    
    $scope.loadingResults = false;
    $scope.sortBy = null;
    $scope.sortOrder = false;
    
    $scope.alerts = [];

    $scope.carts = CartService.carts;

    $scope.goToCart = function(cart) {
      if (cart) {
        $state.go('menu.cart.items', {cartId: cart.id, renameCart: null});
      } else {
        $state.go('menu.cart');
      }
    };

    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.saveCart = function(cart) {
      var updatedCart = angular.copy(cart);

      // delete items if quantity is 0
      updatedCart.items = $filter('filter')(updatedCart.items, {quantity: '!0'});

      return CartService.updateCart(updatedCart).then(function() {
        $scope.currentCart.isRenaming = false;
        $scope.sortBy = null;
        $scope.sortOrder = false;
        $scope.currentCart = updatedCart;
        $scope.cartForm.$setPristine();
        $scope.displayMessage('success', 'Successfully saved cart ' + cart.name);
        return updatedCart.id;
      }, function() {
        $scope.displayMessage('error', 'Error saving cart ' + cart.name);
      });
    };

    $scope.submitOrder = function(cart) {
      $scope.saveCart(cart).then(OrderService.submitOrder).then(function(data) {
        $scope.displayMessage('success', 'Successfully submitted order.');
      }, function(error) {
        $scope.displayMessage('error', 'Error submitting order.');
      });
    };

    $scope.renameCart = function (cartId, cartName) {
      var cart = angular.copy($scope.currentCart);
      cart.name = cartName;

      CartService.updateCart(cart).then(function(data) {
        $scope.currentCart.isRenaming = false;
        $scope.currentCart.name = cartName;
        $scope.displayMessage('success', 'Successfully renamed cart to ' + cartName + '.');
      }, function() {
        $scope.displayMessage('error', 'Error renaming cart.');
      });
    };

    $scope.createNewCart = function() {
      CartService.createCart().then(function(response) {
        $state.go('menu.cart.items', {cartId: response.listitemid, renameCart: true});
        $scope.displayMessage('success', 'Successfully created new cart.');
      }, function() {
        $scope.displayMessage('error', 'Error creating new cart.');
      });
    };

    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart).then(function() {
        setCurrentCart();
        $scope.displayMessage('success', 'Successfully deleted cart.');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });
    };

    $scope.getSubtotal = function(cartItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        subtotal +=( (item.quantity || 0) * (item.each ? item.packageprice : item.caseprice) );
      });
      return subtotal;
    };

    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
      $scope.cartForm.$setDirty();
    };

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      if ($scope.currentCart && $scope.itemsToDisplay < $scope.currentCart.items.length) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    function setCurrentCart() {
      var selectedCart = CartService.getSelectedCart($stateParams.cartId);
      if (selectedCart) {
        if (selectedCart.id === $stateParams.cartId) {
          $scope.currentCart = angular.copy(selectedCart);
        } else {
          $scope.goToCart(selectedCart);
        }
      } else {
        $state.go('menu.cart');
      }
      
      if ($scope.currentCart && $stateParams.renameCart === 'true') {
        $scope.startEditCartName($scope.currentCart.name);
      }
    }
    
    setCurrentCart();

  }]);