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
        if (!basketFound && CartService.cartHeaders.length) {
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
          CartService.renameCart = true;
          return CartService.createCart().then(function(newCart) {
            return newCart.id;
          });
        }
      },

      // determines selected basket from id
      selectValidBasket: function(basketId, changeOrders) {
        var selectedBasket;
        // check valid cart id
        var selectedCart = CartService.findCartById(basketId);
        if (selectedCart) {
          selectedBasket = CartService.getCart(selectedCart.id);
        }

        // check valid change order number
        var selectedChangeOrder = OrderService.findChangeOrderByOrderNumber(changeOrders, basketId);
        if (!selectedCart && selectedChangeOrder) {
          selectedBasket = OrderService.getOrderDetails(basketId);
        }
        return selectedBasket;
      },

      // returns valid list id
      validateList: function(listId, defaultList) {
        var selectedList;

        if(listId){
          ListService.getListWithItems(listId).then(function(resp){
            return selectedList = resp.listid;
          });
        } else if(!listId && defaultList){
          ListService.getListWithItems(defaultList).then(function(resp){
            return selectedList = resp.listid;
          });
        } else {
          ListService.getFavoritesList().then(function(resp){
            return selectedList = resp.listid;
          });
        }
        
      }

    };
 
    return Service;
 
  }]);