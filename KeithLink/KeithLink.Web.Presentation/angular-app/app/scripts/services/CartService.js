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
      return UserProfileService.getCurrentBranchId().toLowerCase();
    }

    var Service = {
      carts: [],

      getAllCarts: function(requestParams) {
        return Cart.query({
          branchId: getBranch()
        }).$promise.then(function(response) {
          var allCarts = response;
          angular.copy(allCarts, Service.carts);
          return allCarts;

        });
      },

      getCart: function(cartId) {
        return Cart.get({ 
          cartId: cartId,
          branchId: getBranch()
        }).$promise.then(function(response) {
          return response;
        });
      },

      createCart: function(items, shipDate) {
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
          items: items,
          requestedshipdate: shipDate
        };

        return Cart.save({
          branchId: getBranch()
        }, newCart).$promise.then(function(response) {
          newCart.id = response.listitemid;
          Service.carts.push(newCart);
          return newCart;
        });
      },

      updateCart: function(cart, params) {
        return Cart.update(params, cart).$promise.then(function(response) {
          return Service.getCart(response.id).then(function(cart) {
              var updatedCart = Service.findCartById(cart.id);
              // var idx = Service.carts.indexOf(updatedCart);
              angular.copy(cart, updatedCart);
              return cart;
          });
        });
      },

      deleteCart: function(cart) {
        var deletedCart = cart;
        return Cart.delete({ cartId: cart.id }).$promise.then(function(response) {
          var cartDeleted = Service.findCartById(deletedCart.id);
          var idx = Service.carts.indexOf(cartDeleted);
          Service.carts.splice(idx, 1);
          return response;
        });
      },

      addItemToCart: function(cartId, item) {
        if (!item.quantity) {
          item.quantity = 1;
        }
        
        return Cart.addItem({ cartId: cartId }, item).$promise.then(function(response) {
          return response;
        });
      },

      updateItem: function(cartId, item) {
        return Cart.updateItem({ cartId: cartId }).$promise.then(function(response) {
          return response;
        });
      },

      deleteItem: function(cartId, itemId) {
        return Cart.deleteItem({ cartId: cartId }).$promise.then(function(response) {
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
      },

      getSelectedCart: function(cartId) {
        var selectedCart;
        if (cartId) {
          selectedCart = Service.findCartById(cartId);
        }
        // go to active cart
        if (!selectedCart) {
          angular.forEach(Service.carts, function(cart, index) {
            if (cart.active) {
              selectedCart = cart;
            }
          });
        }
        // go to first cart in list
        if (!selectedCart && Service.carts && Service.carts.length > 0) {
          selectedCart = Service.carts[0];
        }

        return selectedCart;
      }

    };

    return Service;
 
  }]);