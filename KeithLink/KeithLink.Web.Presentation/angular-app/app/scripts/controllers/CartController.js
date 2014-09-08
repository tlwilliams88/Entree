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
    
    $scope.alerts = [];

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
      addWaitAlert('Waiting for cart to save...');
      CartService.updateCart(cart).then(function() {
        $scope.currentCart.isRenaming = false;
        $scope.sortBy = null;
        $scope.sortOrder = false;
        $scope.currentCart = cart;
        $scope.$$childTail.$$childTail.cartForm.$setPristine();
        $scope.$$childTail.$$childTail.cartItemsForm.$setPristine();
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
        subtotal +=( item.quantity * (item.each ? item.packageprice : item.caseprice) );
      });
      return subtotal;
    };

    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
      $scope.$$childTail.$$childTail.cartForm.$setDirty();
      $scope.$$childTail.$$childTail.cartItemsForm.$setDirty();
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

    // ALERTS
    function addSuccessAlert(message) {
      growl.addSuccessMessage(message);
      // addAlert('success', message);
    }
    function addErrorAlert(message) {
      growl.addErrorMessage(message);
      // addAlert('error', message);
    }
    function addWaitAlert(message) {
      growl.addInfoMessage(message, {ttl: -1});
      // addAlert('wait', message);
    }
    function addAlert(alertType, message) {
      toaster.pop(alertType, null, message);
    }
    $scope.closeAlert = function(index) {
      $scope.alerts.splice(index, 1);
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