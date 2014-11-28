'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$stateParams', '$filter', 'carts', 'lists', 'changeOrders', 'selectedList', 'selectedCart', 'Constants', 'CartService', 'ListService', 'OrderService', 'UtilityService',
    function ($scope, $state, $stateParams, $filter, carts, lists, changeOrders, selectedList, selectedCart, Constants, CartService, ListService, OrderService, UtilityService) {
    
    $scope.carts = carts;
    $scope.lists = lists;
    $scope.shipDates = CartService.shipDates;
    $scope.changeOrders = changeOrders;
    $scope.selectedList = selectedList;
    $scope.selectedCart = selectedCart;
    $scope.useParlevel = $stateParams.useParlevel === 'true' ? true : false;
    $scope.isChangeOrder = selectedCart.hasOwnProperty('ordernumber') ? true : false;

    $scope.sortBy = 'position';
    $scope.sortOrder = false;

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

    function redirect(list, cart) {
      $state.go('menu.addtoorder.items', { listId: list.listid, cartId: cart.ordernumber || cart.id, useParlevel: $scope.useParlevel });
    }

    $scope.goToList = function(list, cart) {
      if (cart.id) {
        CartService.setActiveCart(cart.id).then(function() {
          redirect(list, cart);
        });
      } else {
        redirect(list, cart);
      }
    };

    // $scope.selectList = function(list) {
    //   $state.go('menu.addtoorder.items', { listId: list.listid, cartId: $scope.selectedCart.ordernumber || $scope.selectedCart.id, useParlevel: $scope.useParlevel });
    // };

    // $scope.selectCart = function(cart) {
    //   CartService.getCart(cart.id).then(function(cart) {
    //     $scope.selectedCart = cart;
    //     $scope.selectList($scope.selectedList);
    //   });
    // };

    // $scope.selectChangeOrder = function(changeOrder) {
    //   $scope.selectedCart = OrderService.findChangeOrderByOrderNumber(changeOrders, changeOrder.ordernumber);
    // };

    $scope.sortByPrice = function(item) {
      return item.each ? item.packageprice : item.caseprice;
    };

    $scope.createNewCart = function() {
      var cart = {};
      cart.items = [];
      cart.id = 'New';
      $scope.selectedCart = cart;
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

    function updateCart(cart, items) {
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
      });
    }

    function saveNewCart(items, shipDate) {
      CartService.createCart(items, shipDate).then(function(cartId) {
        // $scope.selectedCart = cart;
        $scope.addToOrderForm.$setPristine();

        var cart = {
          id: cartId
        };

        $scope.goToList($scope.selectedList, cart);
        $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to New Cart.');
      }, function() {
        $scope.displayMessage('error', 'Error adding items to cart.');
      });
    }

    function addItemsToChangeOrder(items, order) {
      order.items = order.items.concat(items);
      OrderService.updateOrder(order).then(function(cart) {
        $scope.selectedCart = cart;
        $scope.addToOrderForm.$setPristine();
        $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to Order # ' + order.ordernumber + '.');
      }, function() {
        $scope.displayMessage('error', 'Error adding items to Order # ' + order.ordernumber + '.');
      });
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

    $scope.getSubtotal = function(cartItems, listItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        if (item.price) {
          subtotal += ( item.quantity * item.price );
        } else {
          subtotal += ( item.quantity * (item.each ? parseFloat(item.packageprice) : parseFloat(item.caseprice)) );
        }
      });
      angular.forEach(listItems, function(item, index) {
        if (item.quantity) {
          subtotal += parseFloat( item.quantity * (item.each ? item.packageprice : item.caseprice) );
        }
      });
      return subtotal;
    };

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

  }]);