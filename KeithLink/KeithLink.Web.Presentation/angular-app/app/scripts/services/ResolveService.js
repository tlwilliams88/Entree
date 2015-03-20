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
      
      // returns valid basket id
      validateBasket: function(basketId, changeOrders) {
        var basketFound = CartService.findCartById(basketId);
        if (basketFound) {
          return basketFound.id;
        }
        if (!basketFound && changeOrders.length) {
          basketFound = OrderService.findChangeOrderByOrderNumber(changeOrders, basketId);
          if (basketFound) {
            return basketFound.ordernumber;
          }
        }
        if (!basketFound && CartService.carts.length) {
          basketFound = CartService.getSelectedCart();
          if (basketFound) {
            return basketFound.id;
          }
        }
        if (!basketFound && changeOrders.length) {
          basketFound = changeOrders[0];
          if (basketFound) {
            return basketFound.ordernumber;
          }
        }
        if (!basketFound) {
          return CartService.createCart().then(function(newCart) {
            return newCart.id;
          });
        }
      },

      // determines selected basket from id
      selectValidBasket: function(basketId) {
        var selectedBasket;
        // check valid cart id
        var selectedCart = CartService.findCartById(basketId);
        if (selectedCart) {
          selectedBasket = CartService.getCart(selectedCart.id);
        }

        // check valid change order number
        var selectedChangeOrder = OrderService.getOrderDetails(basketId);
        if (!selectedCart && selectedChangeOrder) {
          selectedBasket = selectedChangeOrder;
        }
        return selectedBasket;
      },

      // returns valid list id
      validateList: function(listId, defaultList) {
        var selectedList = ListService.findListById(listId);
        if (!selectedList && defaultList) {
          selectedList = ListService.findList(defaultList, true);
        }
        if (!selectedList) {
          selectedList = ListService.getFavoritesList();
        }

        return selectedList.listid;
      }

      // selectDefaultBasket: function(id, changeOrders) {
      //   var selectedBasket;

      //   // check valid cart id
      //   var selectedCart = CartService.findCartById(id);
      //   if (selectedCart) {
      //     selectedBasket = CartService.getCart(selectedCart.id);
      //   }

      //   // check valid change order number
      //   var selectedChangeOrder = OrderService.findChangeOrderByOrderNumber(changeOrders, id);
      //   if (!selectedCart && selectedChangeOrder) {
      //     selectedBasket = selectedChangeOrder;
      //   }

      //   // // if invalid id, select a cart
      //   // var defaultCart = CartService.getSelectedCart();
      //   // if (!selectedCart && !selectedChangeOrder && defaultCart) {
      //   //   selectedBasket = CartService.getCart(defaultCart.id);
      //   // }

      //   // // default to change order if no carts exist
      //   // var defaultChangeOrder = changeOrders[0];
      //   // if (!selectedCart && !selectedChangeOrder && !defaultCart && defaultChangeOrder) {
      //   //   selectedBasket = defaultChangeOrder;
      //   // }

      //   return selectedBasket;
      // },

      // selectDefaultList: function(listId) {
      //   // check for valid listId, go to favorites list by default
      //   var selectedList = ListService.findListById(listId);
      //   if (!selectedList) {
      //     selectedList = ListService.getFavoritesList();
      //   }

      //   return ListService.getList(selectedList.listid);
      // }

    };
 
    return Service;
 
  }]);