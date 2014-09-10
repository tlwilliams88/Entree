'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$filter', 'toaster', 'carts', 'lists', 'Constants', 'CartService', 
    function ($scope, $state, $filter, toaster, carts, lists, Constants, CartService) {
    
    $scope.carts = carts;
    $scope.lists = lists;

    $scope.selectedList = angular.copy(lists[0]);
    $scope.selectedCart = CartService.getSelectedCart();

    $scope.sortBy = null;
    $scope.sortOrder = false;
    var itemsPerPage = Constants.infiniteScrollPageSize;

    $state.go('menu.addtoorder.items', {listId: $scope.selectedList.listid});

    $scope.getListItemsWithQuantity = function(listItems) {
      return $filter('filter')(listItems, {quantity: ''});
    };

    $scope.selectList = function(list) {
      $scope.selectedList = angular.copy(list);
      $state.go('menu.addtoorder.items', { listId: list.listid });
      $scope.addToOrderForm.$setPristine();
    };

    $scope.selectCart = function(cart) {
      $scope.selectedCart = cart;
    };

    $scope.addItemsToCart = function(list, cart) {
      var itemsToAdd = $scope.getListItemsWithQuantity(list.items);

      if (itemsToAdd && itemsToAdd.length > 0) {
        // remove cart item ids
        angular.forEach(itemsToAdd, function(item, index) {
          delete item.cartitemid;
        });

        var addItemsPromise;
        if (cart && cart.id) {
          // add items to existing cart
          var updatedCart = angular.copy(cart);
          updatedCart.items = itemsToAdd;
          addItemsPromise = CartService.updateCart(updatedCart, {deleteomitted: false});
        } else { 
          // create new cart
          addItemsPromise = CartService.createCart(itemsToAdd);
        }

        // resolve promise
        addItemsPromise.then(function() {
          // update cart items
          var itemsAdded = angular.copy(itemsToAdd);
          angular.forEach(itemsAdded, function(item, index) {
            var matchingCartItems = $filter('filter')($scope.selectedCart.items, {itemnumber: item.itemnumber});
            if (matchingCartItems.length === 0) {
              $scope.selectedCart.items.push(item);
            } else {
              matchingCartItems[0].quantity += item.quantity;
            }
          });

          // set empty quantities for list
          angular.forEach($scope.selectedList.items, function(item, index) {
            delete item.quantity;
          });

          $scope.addToOrderForm.$setPristine();
          toaster.pop('success', null, 'Successfully added ' + itemsToAdd.length + ' Items from List ' + list.name + ' to Cart ' + cart.name + '.');
        }, function() {
          toaster.pop('error', null, 'Error adding items to cart.');
        });
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

      listItems = $scope.getListItemsWithQuantity(listItems);
      angular.forEach(listItems, function(item, index) {
        // check if item already exists in the cart
        var itemsWithSameItemNumber = $filter('filter')(cartItems, {itemnumber: item.itemnumber});
        if (itemsWithSameItemNumber.length === 0) {
          total++;
        }
      });
      return total;
    };

    $scope.openDatepicker = function($event) {
      $event.preventDefault();
      $event.stopPropagation();

      $scope.openedDatepicker = !$scope.openedDatepicker;
    };

  }]);