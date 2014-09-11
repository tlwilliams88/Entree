'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', 'toaster', 'Constants', 'CartService', 
    function($scope, $state, $stateParams, $filter, toaster, Constants, CartService) {
    
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

      CartService.updateCart(updatedCart).then(function() {
        $scope.currentCart.isRenaming = false;
        $scope.sortBy = null;
        $scope.sortOrder = false;
        $scope.currentCart = updatedCart;
        $scope.cartForm.$setPristine();
        addSuccessAlert('Successfully saved cart ' + cart.name);
      }, function() {
        addErrorAlert('Error saving cart ' + cart.name);
      });
    };

    $scope.renameCart = function (cartId, cartName) {
      var cart = angular.copy($scope.currentCart);
      cart.name = cartName;

      CartService.updateCart(cart).then(function(data) {
        $scope.currentCart.isRenaming = false;
        $scope.currentCart.name = cartName;
        addSuccessAlert('Successfully renamed cart to ' + cartName + '.');
      }, function() {
        addErrorAlert('Error renaming cart.');
      });
    };

    $scope.createNewCart = function() {
      CartService.createCart().then(function(response) {
        $state.go('menu.cart.items', {cartId: response.listitemid, renameCart: true});
        addSuccessAlert('Successfully created new cart.');
      }, function() {
        addErrorAlert('Error creating new cart.');
      });
    };

    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart).then(function() {
        setCurrentCart();
        addSuccessAlert('Successfully deleted cart.');
      }, function() {
        addErrorAlert('Error deleting cart.');
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

    $scope.dateOptions = {
      formatYear: 'yy',
      startingDay: 1,
      showWeeks: false
    };
    $scope.datepicker = {};

    $scope.openDatepicker = function($event) {
      $event.preventDefault();
      $event.stopPropagation();

      $scope.datepicker.opened = true;
    };

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      if ($scope.currentCart && $scope.itemsToDisplay < $scope.currentCart.items.length) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    // ALERTS
    function addSuccessAlert(message) {
      addAlert('success', message);
    }
    function addErrorAlert(message) {
      addAlert('error', message);
    }
    function addAlert(alertType, message) {
      toaster.pop(alertType, null, message);
    }


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