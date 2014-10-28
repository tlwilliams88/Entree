'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:DefaultBasketResolve
 * @description
 * # DefaultBasketResolve
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ResolveService', ['ListService', 'OrderService', 'CartService', 
    function (ListService, OrderService, CartService) {
    
    var Service = {
      
      selectDefaultBasket: function(id, changeOrders) {
        var selectedBasket = {};

        // check valid cart id
        var selectedCart = CartService.findCartById(id);
        if (selectedCart) {
          selectedBasket.isChangeOrder = false;
          selectedBasket.promise = CartService.getCart(selectedCart.id);
        }

        // check valid change order number
        var selectedChangeOrder = OrderService.findChangeOrderByOrderNumber(changeOrders, id);
        if (!selectedCart && selectedChangeOrder) {
          selectedBasket.isChangeOrder = true;
          selectedBasket.promise = selectedChangeOrder;
        }

        // if invalid id, select a cart
        var defaultCart = CartService.getSelectedCart();
        if (!selectedCart && !selectedChangeOrder && defaultCart) {
          selectedBasket.isChangeOrder = false;
          selectedBasket.promise = CartService.getCart(defaultCart.id);
        }

        // default to change order if no carts exist
        var defaultChangeOrder = changeOrders[0];
        if (!selectedCart && !selectedChangeOrder && !defaultCart && defaultChangeOrder) {
          selectedBasket.isChangeOrder = true;
          selectedBasket.promise = defaultChangeOrder;
        }

        return selectedBasket;
      },

      selectDefaultList: function(listId) {
        // check for valid listId, go to favorites list by default
        var selectedList = ListService.findListById(listId);
        if (!selectedList) {
          selectedList = ListService.getFavoritesList();
        }

        return ListService.getList(selectedList.listid);
      }

    };
 
    return Service;
 
  }]);