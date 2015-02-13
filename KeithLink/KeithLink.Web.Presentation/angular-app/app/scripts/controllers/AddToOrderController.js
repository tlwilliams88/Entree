'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$stateParams', '$filter', 'carts', 'lists', 'changeOrders', 'selectedList', 'selectedCart', 'Constants', 'CartService', 'ListService', 'OrderService', 'UtilityService', 'PricingService',
    function ($scope, $state, $stateParams, $filter, carts, lists, changeOrders, selectedList, selectedCart, Constants, CartService, ListService, OrderService, UtilityService, PricingService) {
    

    function init() {
      
      $scope.carts = carts;
      $scope.lists = lists;
      $scope.changeOrders = changeOrders;
      $scope.shipDates = CartService.shipDates;
      $scope.useParlevel = $stateParams.useParlevel === 'true' ? true : false;
      
      $scope.selectedList = selectedList;
      if ($stateParams.cartId === 'New') { 
        $scope.createNewCart();
      } else if (selectedCart) {
        $scope.selectedCart = selectedCart;
        $scope.isChangeOrder = selectedCart.hasOwnProperty('ordernumber') ? true : false;
      } else {
        // create new cart if no cart was selected
        $scope.createNewCart();
      }

      $scope.sortBy = 'position';
      $scope.sortOrder = false;

      for (var i = 0; i < $scope.selectedList.items.length - 1; i++) {
        $scope.$watch('selectedList.items[' + i + '].quantity', function(newVal, oldVal) {
          var idx = this.exp.substr(this.exp.indexOf('[') + 1, this.exp.indexOf(']') - this.exp.indexOf('[') - 1);
          var item = $scope.selectedList.items[idx];
          item.extPrice = PricingService.getPriceForItem(item);

          refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
        });
      }
    }

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      if ($scope.itemsToDisplay < $scope.selectedList.items.length) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    $scope.getListItemsWithQuantity = function(listItems) {
      return $filter('filter')(listItems, function(value, index) {
        return value.quantity > 0;
      });
    };

    function redirect(listId, cartId) {
      $state.go('menu.addtoorder.items', { listId: listId, cartId: cartId, useParlevel: $scope.useParlevel });
    }

    $scope.goToList = function(list, cart) {
      if (cart.id) { // make sure cart is not a change order 

        if (cart.id === 'New') {
          redirect(list.listid, cart.id);
        } else {
          // TODO: unsaved changes warning happens AFTER selected cart is already set to active
          CartService.setActiveCart(cart.id).then(function() {
            // wait until cart is successfuly set to 'active' to redirect so item.quantityincart is updated
            redirect(list.listid, cart.id);
          });
        }
      } else {
        redirect(list.listid, cart.ordernumber);
      }
    };

    $scope.sortByPrice = function(item) {
      return item.each ? item.packageprice : item.caseprice;
    };

    $scope.createNewCart = function() {
      var cart = {};
      cart.items = [];
      cart.id = 'New';
      $scope.selectedCart = cart;
      $scope.isChangeOrder = false;
      refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
    };

    function combineDuplicateItemNumbers(items) {

      // combine quantities if itemnumber is a duplicate
      var newItems = [];
      angular.forEach(items, function(item, index) {
        var duplicateItem = findItemInList(newItems, item.itemnumber);
        if (duplicateItem) {
          duplicateItem.quantity += item.quantity;
        } else {
          newItems.push(item);
        }
      });
      return newItems;
    }

    function findItemInList(items, itemNumber) {
      var itemFound;
      angular.forEach(items, function(item, index) {
        if (item.itemnumber === itemNumber) {
          itemFound = item;
        }
      });
      return itemFound;
    }

    var processingUpdateCart = false; 
    function updateCart(cart, items) {
      if (!processingUpdateCart) {
        processingUpdateCart = true;
        var updatedCart = angular.copy(cart);
        CartService.addItemsToCart(updatedCart, items).then(function(cart) {
          $scope.selectedCart = cart;

          // reset quantities
          angular.forEach($scope.selectedList.items, function(item) {
            item.quantityincart += item.quantity; 
            item.quantity = 0;
          });

          $scope.addToOrderForm.$setPristine();
          $scope.displayMessage('success', 'Successfully added ' + updatedCart.items.length + ' Items to Cart ' + cart.name + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function() {
          processingUpdateCart = false;
        });
      }
    }

    var processingSaveCart = false;
    function saveNewCart(items, shipDate) {
      if (!processingSaveCart) {
        processingSaveCart = true;
        CartService.createCart(items, shipDate).then(function(cart) {
          // $scope.selectedCart = cart;
          $scope.addToOrderForm.$setPristine();

          $scope.goToList($scope.selectedList, cart);
          $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to New Cart.');
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function(){
          processingSaveCart = false;
        });
      }
    }

    var processingSaveChangeOrder = false;
    function addItemsToChangeOrder(items, order) {
      if (!processingSaveChangeOrder) {
        processingSaveChangeOrder = true;
        order.items = order.items.concat(items);
        OrderService.updateOrder(order).then(function(cart) {
          $scope.selectedCart = cart;
          $scope.addToOrderForm.$setPristine();
          $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to Order # ' + order.ordernumber + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding items to Order # ' + order.ordernumber + '.');
        }).finally(function() {
          processingSaveChangeOrder = false;
        });
      }
    }

    $scope.addItemsToCart = function(list, cart) {
      var itemsToAdd = angular.copy($scope.getListItemsWithQuantity(list.items));
      itemsToAdd = combineDuplicateItemNumbers(itemsToAdd);

      if (itemsToAdd && itemsToAdd.length > 0) {

        if (cart.ordernumber) {

          addItemsToChangeOrder(itemsToAdd, cart);
        } else {
          
          // remove cart item ids
          UtilityService.deleteFieldFromObjects(itemsToAdd, ['listitemid']);

          // add items to existing cart
          if (cart && cart.id && cart.id !== 'New') {
            updateCart(cart, itemsToAdd);
          
          // create new cart
          } else { 
            saveNewCart(itemsToAdd, cart.requestedshipdate);
          }
        }
      }
    };

    function refreshSubtotal(cartItems, listItems) {
      var listItemsWithQuantity = $scope.getListItemsWithQuantity(listItems);

      // get subtotal for cart items and list items with quantity > 0
      $scope.subtotal = PricingService.getSubtotalForItems(cartItems) + PricingService.getSubtotalForItemsWithPrice(listItemsWithQuantity);
      return $scope.subtotal;
    }

    $scope.getItemCount = function(cart, list) {
      if (cart && list) {
        var cartItems = cart.items;
        var listItems = list.items;

        var total = cartItems.length;
        var uniqueItemNumbers = [];

        listItems = $scope.getListItemsWithQuantity(listItems);
        angular.forEach(listItems, function(item, index) {
          // check if item already exists in the cart
          var cartItemsWithSameItemNumber = $filter('filter')(cartItems, {itemnumber: item.itemnumber});
          if (cartItemsWithSameItemNumber.length === 0 && uniqueItemNumbers.indexOf(item.itemnumber)) {
            total++;
            uniqueItemNumbers.push(item.itemnumber);
          }
        });
        return total;
      }
    };

    // update quantity from on hand amount and par level
    $scope.updateQuantity = function(item) {
      if (!isNaN(item.onhand)) {
        var quantity = Math.ceil(item.parlevel - item.onhand);
        if (quantity > -1) {
          item.quantity = quantity;
        } else {
          item.quantity = 0;
        }
      }
    };

    init();

  }]);