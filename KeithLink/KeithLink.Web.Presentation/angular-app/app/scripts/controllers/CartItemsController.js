'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', 'Constants', 'CartService', 'OrderService', 'changeOrders',
    function($scope, $state, $stateParams, $filter, Constants, CartService, OrderService, changeOrders) {
    
    $scope.loadingResults = false;
    $scope.sortBy = null;
    $scope.sortOrder = false;
    
    $scope.carts = CartService.carts;
    $scope.shipDates = CartService.shipDates;
    // $scope.reminderList = reminderList;
    $scope.changeOrders = changeOrders;
    
    $scope.goToCart = function(cartId, isChangeOrder) {
      if (cartId) {
        $state.go('menu.cart.items', {cartId: cartId, renameCart: null, isChangeOrder: isChangeOrder});
      } else {
        $state.go('menu.cart');
      }
    };

    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.selectShipDate = function(shipDate) {
      $scope.currentCart.requestedshipdate = shipDate.shipdate;
      $scope.selectedShipDate = shipDate;
      $scope.cartForm.$setDirty();
    };

    $scope.sortByPrice = function(item) {
      if (item.price) {
        return item.price;
      } else {
        return item.each ? item.packageprice : item.caseprice;
      }
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
      $scope.saveCart(cart)
        .then(CartService.submitOrder)
        .then(function(data) {
          $state.go('menu.orderitems', { orderNumber: data.ordernumber });
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
        $state.go('menu.cart.items', {cartId: response.id, renameCart: true});
        $scope.displayMessage('success', 'Successfully created new cart.');
      }, function() {
        $scope.displayMessage('error', 'Error creating new cart.');
      });
    };

    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart.id).then(function() {
        setCurrentCart();
        $scope.displayMessage('success', 'Successfully deleted cart.');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });
    };

    $scope.getSubtotal = function(cartItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        if (item.price) {
          subtotal += (item.quantity || 0) * item.price;
        } else {
          subtotal += ( (item.quantity || 0) * (item.each ? item.packageprice : item.caseprice) );
        }
      });
      return subtotal;
    };

    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
      $scope.cartForm.$setDirty();
    };

    /**********
    CHANGE ORDERS
    **********/
    $scope.resubmitOrder = function(order) {
      var changeOrder = angular.copy(order);

      // delete items if quantity is 0
      changeOrder.lineItems = $filter('filter')(changeOrder.lineItems, {quantity: '!0'});

      OrderService.updateOrder(changeOrder).then(function() {
        OrderService.resubmitOrder(changeOrder.ordernumber).then(function() {
          $scope.displayMessage('success', 'Successfully submitted change order.');
        }, function(error) {
          $scope.displayMessage('error', 'Error re-submitting order.');
        });
      });
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
          $scope.goToCart(selectedCart.id);
        }
      } else {
        $state.go('menu.cart');
      }
      
      if ($scope.currentCart && $stateParams.renameCart === 'true') {
        $scope.startEditCartName($scope.currentCart.name);
      }

      $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);
    }

    function setCurrentChangeOrder() {
      var selectedChangeOrder = OrderService.getOrderDetails($stateParams.cartId);

      selectedChangeOrder.then(function(order) {
        if (order) {
          $scope.currentCart = angular.copy(order);
        } else {
          $state.go('menu.cart');
        }
      }, function() {
        $state.go('menu.cart');
      });
      
    }
    
    if ($stateParams.isChangeOrder === 'true') {
      setCurrentChangeOrder();
      $scope.isChangeOrder = true;
    } else {
      setCurrentCart();
      $scope.isChangeOrder = false;
    }

  }]);