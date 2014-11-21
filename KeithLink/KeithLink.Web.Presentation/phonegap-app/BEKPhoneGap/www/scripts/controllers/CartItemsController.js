'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', '$modal', 'Constants', 'CartService', 'OrderService', 'UtilityService', 'changeOrders', 'originalBasket', 'criticalItemsLists',
    function($scope, $state, $stateParams, $filter, $modal, Constants, CartService, OrderService, UtilityService, changeOrders, originalBasket, criticalItemsLists) {

    $scope.loadingResults = false;
    $scope.sortBy = null;
    $scope.sortOrder = false;
    
    $scope.carts = CartService.carts;
    $scope.shipDates = CartService.shipDates;
    $scope.changeOrders = changeOrders;
    $scope.isChangeOrder = originalBasket.hasOwnProperty('ordernumber') ? true : false;
    $scope.currentCart = angular.copy(originalBasket);
    $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);

    // set mandatory and reminder lists
    criticalItemsLists.forEach(function(list) {
      if (list.ismandatory) {
        $scope.mandatoryList = list;
      } else if (list.isreminder) {
        $scope.reminderList = list;
      }
    });

    // set default selected critical items list
    if ($scope.mandatoryList) {
      $scope.mandatoryList.active = true;
    } else {
      $scope.reminderList.active = true;
    }

    $scope.goToCart = function(cartId, isChangeOrder) {
      $state.go('menu.cart.items', {cartId: cartId, renameCart: null}).then(function() {
        if (!isChangeOrder) {
          CartService.setActiveCart(cartId);
        }
      });
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
      CartService.createCart().then(function(cartId) {
        CartService.setActiveCart(cartId);
        $state.go('menu.cart.items', {cartId: cartId, renameCart: true});
        $scope.displayMessage('success', 'Successfully created new cart.');
      }, function() {
        $scope.displayMessage('error', 'Error creating new cart.');
      });
    };

    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart.id).then(function() {
        $scope.goToCart();
        $scope.displayMessage('success', 'Successfully deleted cart.');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });
    };

    $scope.getSubtotal = function(cartItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        subtotal += ( (item.quantity || 0) * (item.each ? item.packageprice : item.caseprice) );
      });
      return subtotal;
    };

    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
      $scope.cartForm.$setDirty();
    };

    /************
    CHANGE ORDERS
    ************/

    $scope.saveChangeOrder = function(order) {
      var changeOrder = angular.copy(order);
      changeOrder.items = $filter('filter')(changeOrder.items, {quantity: '!0'});

      return OrderService.updateOrder(changeOrder).then(function(order) {
        $scope.currentCart = order;
        $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);
        return order.ordernumber;
      });
    };

    $scope.resubmitOrder = function(order) {

      $scope.saveChangeOrder(order)
        .then(OrderService.resubmitOrder)
        .then(function(orderNumber) {
          // update changeOrders object
          angular.forEach($scope.changeOrders, function(changeOrder) {
            if (changeOrder.ordernumber === $scope.currentCart.ordernumber) {
              changeOrder.ordernumber = orderNumber;
            }
          });
          $scope.currentCart.ordernumber = orderNumber;
          $scope.displayMessage('success', 'Successfully submitted change order.');
        }, function(error) {
          $scope.displayMessage('error', 'Error re-submitting order.');
        });
    };

    $scope.cancelOrder = function(changeOrder) {
      OrderService.cancelOrder(changeOrder.commerceid).then(function() {
        var changeOrderFound = UtilityService.findObjectByField($scope.changeOrders, 'commerceid', changeOrder.commerceid);
        var idx = $scope.changeOrders.indexOf(changeOrderFound);
        $scope.changeOrders.splice(idx, 1);
        $scope.goToCart();
        $scope.displayMessage('success', 'Successfully cancelled order ' + changeOrder.ordernumber + '.');
      }, function(error) {
        $scope.displayMessage('error', 'Error cancelling order ' + changeOrder.ordernumber + '.');
      });
    };

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      var items = $scope.currentCart.items.length;

      if ($scope.currentCart && $scope.itemsToDisplay < items) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    $scope.openOrderImportModal = function () {

      var modalInstance = $modal.open({
        templateUrl: 'views/modals/orderimportmodal.html',
        controller: 'ImportModalController'
      });
    };

    /*************************
    REMINDER / MANDATORY ITEMS
    *************************/

    $scope.changeAllSelectedItems = function(items, selectAll) {
      angular.forEach(items, function(item, index) {
        item.isSelected = selectAll;
      });
    };

    $scope.addSelectedToCart = function(items) {
      if (items && items.length > 0) {
        items = $filter('filter')(items, {isSelected: true});
       
        // set quantity
        items.forEach(function(item) {
          item.quantity = Math.ceil(item.parlevel - item.qtyInCart) || 1;
        });

        $scope.currentCart.items = $scope.currentCart.items.concat(items);

        $scope.cartForm.$setDirty();
        $scope.reminderList.allSelected = false;
        $scope.mandatoryList.allSelected = false;
        $scope.changeAllSelectedItems(items, false);
      }    
    };

    // on page load
    if ($stateParams.renameCart === 'true' && !$scope.isChangeOrder) {
      $scope.startEditCartName(originalBasket.name);
    }


  }]);