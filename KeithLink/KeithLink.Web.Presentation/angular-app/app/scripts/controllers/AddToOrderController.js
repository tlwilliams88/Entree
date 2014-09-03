'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$filter', 'carts', 'lists', 'Constants', 'CartService', 
    function ($scope, $state, $filter, carts, lists, Constants, CartService) {
    
    $scope.carts = carts;
    $scope.lists = lists;

    $scope.selectedList = lists[0];
    $scope.selectedCart = carts[0];

    $scope.sortBy = null;
    $scope.sortOrder = false;
    var itemsPerPage = Constants.infiniteScrollPageSize;

    $state.go('menu.addtoorder.items', {listId: $scope.selectedList.listid});

    $scope.getListItemsWithQuantity = function(listItems) {
      return $filter('filter')(listItems, {quantity: ''});
    };

    $scope.selectList = function(list) {
      $scope.selectedList = list;
    };

    $scope.selectCart = function(cart) {
      $scope.selectedCart = cart;
    };

    $scope.addItemsToCart = function(list, cart) {
      var itemsToAdd = $scope.getListItemsWithQuantity(list.items);

      if (cart && cart.id) {
        // add items to existing cart
        CartService.updateCart(cart, {deleteomitted: false}).then(function() {
          $scope.addToOrderForm.$setPristine();
        });

      } else { 
        // create new cart
        CartService.createCart(itemsToAdd).then(function() {
          $scope.addToOrderForm.$setPristine();
        });
      }
    };

    $scope.getSubtotal = function(cartItems, listItems) {
      var subtotal = 0;
      angular.forEach(cartItems, function(item, index) {
        subtotal += ( item.quantity * (item.each ? item.packageprice : item.caseprice) );
      });
      angular.forEach(listItems, function(item, index) {
        if (item.quantity) {
          subtotal += parseFloat( item.quantity * (item.each ? item.packageprice : item.caseprice) );
        }
      });
      return subtotal;
    };

    $scope.openDatepicker = function($event) {
      $event.preventDefault();
      $event.stopPropagation();

      $scope.openedDatepicker = !$scope.openedDatepicker;
    };

  }]);