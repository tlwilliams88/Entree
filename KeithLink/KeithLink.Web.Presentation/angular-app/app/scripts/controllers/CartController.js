'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartController', ['$scope', '$state', '$stateParams', 'CartService', 
    function($scope, $state, $stateParams, CartService) {
    
    $scope.loadingResults = false;
    $scope.sortBy = null;
    $scope.sortOrder = false;
    
    $scope.carts = CartService.carts;

    $scope.goToCart = function(cart) {
      $scope.currentCart = cart;
      $state.transitionTo('menu.cart.items', {cartId: cart.id}, {notify: false});
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

    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart).then(function() {
        if ($scope.carts.length > 0) {
          $scope.goToCart($scope.carts[0]);
        }
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

    $scope.dateOptions = {
      formatYear: 'yy',
      startingDay: 1,
      showWeeks: false
    };

    $scope.openDatepicker = function($event) {
      $event.preventDefault();
      $event.stopPropagation();

      $scope.openedDatepicker = true;
    };

    function initPage() {
      if ($state.params.cartId) {
        $scope.currentCart = CartService.findCartById($state.params.cartId);
      } else {
        $scope.currentCart = CartService.carts[0];
        $state.go('menu.cart.items', { cartId: CartService.carts[0].id });
      }
      
      if ($stateParams.renameCart === 'true') {
        $scope.startEditCartName($scope.currentCart.name);
        $state.transitionTo('menu.cart.items', {cartId: $scope.currentCart.id}, {notify: false});
      }
    }
    initPage();

  }]);