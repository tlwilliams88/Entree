'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$stateParams', '$filter', 'carts', 'lists', 'Constants', 'CartService', 'ListService', 
    function ($scope, $state, $stateParams, $filter, carts, lists, Constants, CartService, ListSerivce) {
    
    $scope.carts = carts;
    $scope.lists = lists;

    $scope.sortBy = 'position';
    $scope.sortOrder = false;
    var itemsPerPage = Constants.infiniteScrollPageSize;

    $scope.getListItemsWithQuantity = function(listItems) {
      return $filter('filter')(listItems, function(value, index) {
        return value.quantity > 0;
      });
    };

    $scope.selectList = function(list) {
      $scope.selectedList = angular.copy(list);
      $state.go('menu.addtoorder.items', { listId: list.listid, cartId: $scope.selectedCart.id, useParlevel: $scope.useParlevel });
      if ($scope.addToOrderForm) {
        $scope.addToOrderForm.$setPristine();
      }
    };

    $scope.selectCart = function(cart) {
      $scope.selectedCart = cart;
      $scope.showCarts = false;
    };

    $scope.createNewCart = function() {
      var cart = {};
      cart.items = [];
      cart.id = 'New';
      $scope.selectCart(cart);
    };

    $scope.setQuantityValueFromDropdown = function(item, qty) {
      $scope.addToOrderForm.$setDirty();
      item.quantity = qty;
    };

    function deleteFieldInList(items, field) {
      angular.forEach(items, function(item, index) {
        delete item[field];
      });
      return items;
    }

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

    function updateCart(cart) {
      CartService.updateCart(cart, {deleteomitted: false}).then(function(cart) {
        
        // // add quantities to existing item numbers
        // angular.forEach(cart.items, function(item, index) {
        //   var matchingCartItems = $filter('filter')($scope.selectedCart.items, {itemnumber: item.itemnumber});
        //   if (matchingCartItems.length === 0) {
        //     $scope.selectedCart.items.push(item);
        //   } else {
        //     matchingCartItems[0].quantity += item.quantity;
        //   }
        // });

        $scope.selectedCart = cart;

        // reset quantities
        $scope.selectedList.items = deleteFieldInList($scope.selectedList.items, 'quantity');

        $scope.addToOrderForm.$setPristine();
        $scope.displayMessage('success', 'Successfully added ' + cart.items.length + ' Items to Cart ' + cart.name + '.');
      }, function() {
        $scope.displayMessage('error', 'Error adding items to cart.');
      });
    }

    function saveNewCart(items, shipDate) {
      CartService.createCart(items, shipDate).then(function(cart) {
        $scope.selectCart(cart);
        $scope.addToOrderForm.$setPristine();
        $scope.selectList($scope.selectedList);
        $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to New Cart.');
      }, function() {
        $scope.displayMessage('error', 'Error adding items to cart.');
      });
    }

    $scope.addItemsToCart = function(list, cart) {
      var itemsToAdd = angular.copy($scope.getListItemsWithQuantity(list.items));
      itemsToAdd = combineDuplicateItemNumbers(itemsToAdd);

      if (itemsToAdd && itemsToAdd.length > 0) {
        // remove cart item ids
        itemsToAdd = deleteFieldInList(itemsToAdd, 'listitemid');

        // add items to existing cart
        if (cart && cart.id && cart.id !== 'New') {
          var updatedCart = angular.copy(cart);
          updatedCart.items = itemsToAdd;
          updateCart(updatedCart);
        
        // create new cart
        } else { 
          saveNewCart(itemsToAdd, cart.requestedshipdate);
        }
      }
    };

    $scope.getSubtotal = function(cartItems, listItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        subtotal += ( item.quantity * (item.each ? parseFloat(item.packageprice) : parseFloat(item.caseprice)) );
      });
      angular.forEach(listItems, function(item, index) {
        if (item.quantity) {
          subtotal += parseFloat( item.quantity * (item.each ? item.packageprice : item.caseprice) );
        }
      });
      return subtotal;
    };

    $scope.getItemCount = function(cartItems, listItems) {
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
    };

    $scope.openDatepicker = function($event) {
      $event.preventDefault();
      $event.stopPropagation();

      $scope.openedDatepicker = !$scope.openedDatepicker;
    };

    // select default cart
    var cart = CartService.getSelectedCart($stateParams.cartId);
    if ($stateParams.cartId === 'New' || !cart) {
      $scope.createNewCart();
    } else {
      $scope.selectCart(cart);
    }

    // select list
    if ($stateParams.listId) {
      $scope.selectedList = ListSerivce.findListById($stateParams.listId);
    }
    if (!$scope.selectedList) {
      $scope.selectList(angular.copy(lists[0]));
    }

    $scope.useParlevel = $stateParams.useParlevel === 'true' ? true : false;
  }]);