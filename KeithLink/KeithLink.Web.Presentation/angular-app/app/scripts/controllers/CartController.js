'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartController', ['$scope', '$state', '$stateParams', 'Constants', 'CartService', 
    function($scope, $state, $stateParams, Constants, CartService) {
    
    $scope.loadingResults = false;
    $scope.sortBy = null;
    $scope.sortOrder = false;
    
    $scope.carts = CartService.carts;

    $scope.goToCart = function(cart) {
      $state.go('menu.cartitems', {cartId: cart.id}).then(function() {
        $scope.currentCart = cart;
      });
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
        $scope.$$childTail.$$childTail.cartForm.$setPristine();
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
        setCurrentCart();
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

    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
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

    function setCurrentCart() {
      if ($stateParams.cartId) {
        $scope.currentCart = CartService.findCartById($stateParams.cartId);
      } 
      if (!$scope.currentCart) {

        // go to active cart

        // go to first cart in list
        if (CartService.carts && CartService.carts.length > 0) {
          $scope.goToCart(CartService.carts[0]);
        } else { // display default message
          $scope.currentCart = null;
          $state.go('menu.cart');
        }
      }
    }

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      if ($scope.itemsToDisplay < $scope.currentCart.items.length) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    function renameRedirect() {
      if ($stateParams.renameCart === 'true') {
        $scope.startEditCartName($scope.currentCart.name);
        $state.go('menu.cartitems', {cartId: $scope.currentCart.id});
      }
    }
    
    setCurrentCart();
    renameRedirect();

  }]);