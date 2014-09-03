'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CartService
 * @description
 * # CartService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CartService', ['$http', 'UserProfileService', 'NameGeneratorService', 'Cart', function ($http, UserProfileService, NameGeneratorService, Cart) {

    function getBranch() {
      return UserProfileService.getCurrentBranchId();
    }

    var Service = {
      carts: [],

      getAllCarts: function(requestParams) {
        return Cart.query(requestParams).$promise.then(function(response) {
          var allCarts = response;
          angular.copy(allCarts, Service.carts);

          console.log(allCarts);
          return allCarts;

        });
      },

      getCart: function(cartId) {
        return Cart.get({ cartId: cartId }).$promise.then(function(response) {
          console.log(response);
          return response;
        });
      },


      createCart: function(items) {
        if (!items) {
          items = [];
        } else {
          angular.forEach(items, function (item, index) {
            if (!item.quantity || item.quantity === 0) {
              item.quantity = 1;
            }
          });
        }

        var newCart = {
          name: NameGeneratorService.generateName('Cart', Service.carts),
          items: items
        };

        return Cart.save(null, newCart).$promise.then(function(response) {
          console.log(response);
          return response;
        });
      },

      updateCart: function(cart, params) {
        return Cart.update(params, cart).$promise.then(function(response) {
          console.log(response.data);
          return response.data;
        });
      },

      deleteCart: function(cart) {
        var deletedCart = cart;
        return Cart.delete({ cartId: cart.id }).$promise.then(function(response) {
          var idx = Service.carts.indexOf(deletedCart);
          Service.carts.splice(idx, 1);
          console.log(response);
          return response;
        });
      },

      addItemToCart: function(cartId, item) {
        if (!item.quantity) {
          item.quantity = 1;
        }
        
        return Cart.addItem({ cartId: cartId }, item).$promise.then(function(response) {
          console.log(response);
          return response;
        });
      },

      updateItem: function(cartId, item) {
        return Cart.updateItem({ cartId: cartId }).$promise.then(function(response) {
          console.log(response);
          return response;
        });
      },

      deleteItem: function(cartId, itemId) {
        return Cart.deleteItem({ cartId: cartId }).$promise.then(function(response) {
          console.log(response);
          return response;
        });
      },

      findCartById: function(cartId) {
        var cartFound;
        angular.forEach(Service.carts, function(cart, index) {
          if (cart.id === cartId) {
            cartFound = cart;
          }
        });
        return cartFound;
      }

    };

    return Service;
 
  }]);