'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', '$modal', 'Constants', 'CartService', 'OrderService', 'UtilityService', 'PricingService', 'changeOrders', 'originalBasket', 'criticalItemsLists',
    function($scope, $state, $stateParams, $filter, $modal, Constants, CartService, OrderService, UtilityService, PricingService, changeOrders, originalBasket, criticalItemsLists) {

    var basketId = originalBasket.id || originalBasket.ordernumber;
    if ($stateParams.cartId !== basketId.toString()) {
      $state.go('menu.cart.items', {cartId: basketId, renameCart: null}, {location:'replace', inherit:false, notify: false});
    }

    function onQuantityChange(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var item = $scope.currentCart.items[idx];
      if (item) {
        item.extPrice = PricingService.getPriceForItem(item);
      }

      $scope.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.currentCart.items);
    }

    $scope.loadingResults = false;
    $scope.sortBy = null;
    $scope.sortOrder = false;
    
    $scope.carts = CartService.carts;
    $scope.shipDates = CartService.shipDates;
    $scope.changeOrders = angular.copy(changeOrders);
    $scope.isChangeOrder = originalBasket.hasOwnProperty('ordernumber') ? true : false;
    $scope.currentCart = angular.copy(originalBasket);
    $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);

    if (!$scope.isChangeOrder) {
      CartService.setActiveCart($scope.currentCart.id);
    }

    for (var i = 0; i < $scope.currentCart.items.length; i++) {
      $scope.$watch('currentCart.items[' + i + '].quantity', onQuantityChange);
      $scope.$watch('currentCart.items[' + i + '].each', onQuantityChange);
    }

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
    } else if ($scope.reminderList) {
      $scope.reminderList.active = true;
    } else {
      $scope.mandatoryList = {};
      $scope.reminderList = {};
    }


    function selectNextCartId() {
      var redirectId;
      if ($scope.carts.length > 0) {
        redirectId = $scope.carts[0].id;
      } else if ($scope.changeOrders.length > 0) {
        redirectId = $scope.changeOrders[0].ordernumber;
      }
      return redirectId;
    }

    $scope.goToCart = function(cartId, isChangeOrder) {
      if (!cartId) {
        cartId = selectNextCartId();
      }
      $state.go('menu.cart.items', {cartId: cartId, renameCart: null} );
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
      // if (item.price) {
      //   return item.price;
      // } else {
        return item.each ? item.packageprice : item.caseprice;
      // }
    };

    var processingSaveCart = false;
    $scope.saveCart = function(cart) {
      if (!processingSaveCart) {
        processingSaveCart = true;
        var updatedCart = angular.copy(cart);

        // delete items if quantity is 0 or price is 0
  		  updatedCart.items = $filter('filter')( updatedCart.items, function(item){ 
          return item.quantity > 0 && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item)); 
        });

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
        }).finally(function() {
          processingSaveCart = false;
        });
      }
    };
	
	var processingSubmitOrder = false;
    $scope.submitOrder = function(cart) {
      if (!processingSubmitOrder) {
        processingSubmitOrder = true;

        $scope.saveCart(cart)
          .then(CartService.submitOrder)
          .then(function(data) {
            $state.go('menu.orderitems', { orderNumber: data.ordernumber });
            $scope.displayMessage('success', 'Successfully submitted order.');
          }, function(error) {
            $scope.displayMessage('error', 'Error submitting order.');
          }).finally(function() {
            processingSubmitOrder = false;
          });
      }
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
      CartService.createCart().then(function(newCart) {
        $state.go('menu.cart.items', {cartId: newCart.id, renameCart: true});
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

    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
      $scope.cartForm.$setDirty();
    };

    /************
    CHANGE ORDERS
    ************/

    var processingSaveChangeOrder = false;
    $scope.saveChangeOrder = function(order) {
      if (!processingSaveChangeOrder) {
        processingSaveChangeOrder = true;

        var changeOrder = angular.copy(order);
        changeOrder.items = $filter('filter')( changeOrder.items, function(item){ 
          return item.quantity > 0 && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item)); 
        });

        return OrderService.updateOrder(changeOrder).then(function(order) {
          $scope.currentCart = order;
          $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);
          $scope.displayMessage('success', 'Successfully updated change order.');
          return order.ordernumber;
        }, function(error) {
          $scope.displayMessage('error', 'Error updating change order ' + order.ordernumber + '.');
        }).finally(function() {
          processingSaveChangeOrder = false;
        });
      }
    };

    var processingResubmitOrder = false;
    $scope.resubmitOrder = function(order) {
      if (!processingResubmitOrder) {
        processingResubmitOrder = true;

        $scope.saveChangeOrder(order)
          .then(OrderService.resubmitOrder)
          .then(function(orderNumber) {
            // update changeOrders object in cache
            angular.forEach($scope.changeOrders, function(changeOrder) {
              if (changeOrder.ordernumber === $scope.currentCart.ordernumber) {
                changeOrder.ordernumber = orderNumber;
              }
            });
            $scope.currentCart.ordernumber = orderNumber;
            $scope.displayMessage('success', 'Successfully submitted change order.');
          }, function(error) {
            $scope.displayMessage('error', 'Error re-submitting order.');
          }).finally(function() {
            processingResubmitOrder = false;
          });
      }
      
    };

    var processingCancelOrder = false;
    $scope.cancelOrder = function(changeOrder) {
      if (!processingCancelOrder) {
        processingCancelOrder = true;
        
        OrderService.cancelOrder(changeOrder.commerceid).then(function() {
          var changeOrderFound = UtilityService.findObjectByField($scope.changeOrders, 'commerceid', changeOrder.commerceid);
          var idx = $scope.changeOrders.indexOf(changeOrderFound);
          $scope.changeOrders.splice(idx, 1);
          $scope.goToCart();
          $scope.displayMessage('success', 'Successfully cancelled order ' + changeOrder.ordernumber + '.');
        }, function(error) {
          $scope.displayMessage('error', 'Error cancelling order ' + changeOrder.ordernumber + '.');
        }).finally(function() {
          processingCancelOrder = false;
        });
      }
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

        var originalItemCount = $scope.currentCart.items.length;
        $scope.currentCart.items = $scope.currentCart.items.concat(items);
        for (var i = originalItemCount; i < $scope.currentCart.items.length; i++) {
          $scope.$watch('currentCart.items[' + i + '].quantity', onQuantityChange);
          $scope.$watch('currentCart.items[' + i + '].each', onQuantityChange);
        }

        $scope.cartForm.$setDirty();
        if ($scope.reminderList) {
          $scope.reminderList.allSelected = false;
        }
        if ($scope.mandatoryList) {
          $scope.mandatoryList.allSelected = false;
        }
        $scope.changeAllSelectedItems(items, false);
      }    
    };

    // on page load
    if ($stateParams.renameCart === 'true' && !$scope.isChangeOrder) {
      $scope.startEditCartName(originalBasket.name);
    }
  }]);