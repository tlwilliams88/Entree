'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CartService
 * @description
 * # CartService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CartService', ['$http', '$filter', 'UserProfileService', 'UtilityService', 'Cart', 
    function ($http, $filter, UserProfileService, UtilityService, Cart) {

    var filter = $filter('filter');

    var Service = {
      carts: [],

      // accepts "header: true" params to get only names
      // return array of cart objects
      getAllCarts: function(requestParams) {
        if (!requestParams) {
          requestParams = {};
        }

        return Cart.query(requestParams).$promise.then(function(response) {
          var allCarts = response;
          angular.copy(allCarts, Service.carts);
          return allCarts;
        });
      },

      getCartHeaders: function() {
        return Service.getAllCarts({ header: true });
      },

      // accepts cartId (guid)
      // returns cart object
      getCart: function(cartId) {
        return Cart.get({ 
          cartId: cartId,
        }).$promise.then(function(cart) {
          // update cart in cache
          var existingCart = UtilityService.findObjectByField(Service.carts, 'id', cart.id);
          if (existingCart) {
            var idx = Service.carts.indexOf(existingCart);
            angular.copy(cart, Service.carts[idx]);
          } else {
            Service.carts.push(cart);
          }
          return cart;
        });
      },

      findCartById: function(cartId) {
        var itemsFound = filter(Service.carts, {id: cartId});
        if (itemsFound.length === 1) {
          return itemsFound[0];
        }
      },

      /********************
      EDIT CART
      ********************/

      // accepts null, item object, or array of item objects and shipDate
      // returns promise and new cart object
      createCart: function(items, shipDate) {
        var newCart = {};

        if (!items) { // if null
          newCart.items = [];
        } else if (Array.isArray(items)) { // if multiple items
          newCart.items = items;
        } else if (typeof items === 'object') { // if one item
          newCart.items = [items];
        }

        // set default quantity to 1
        angular.forEach(newCart.items, function (item, index) {
          if (!item.quantity || item.quantity === 0) {
            item.quantity = 1;
          }
        });

        newCart.name = UtilityService.generateName('Cart', Service.carts);
        newCart.requestedshipdate = shipDate;

        return Cart.save({}, newCart).$promise.then(function(response) {
          return Service.getCart(response.listitemid);
        });
      },

      // accepts cart object and params (deleteOmitted?)
      // returns promise and updated cart object
      updateCart: function(cart, params) {
        return Cart.update(params, cart).$promise.then(function(response) {
          return Service.getCart(response.id);
        });
      },

      // accepts cartId (guid)
      deleteCart: function(cartId) {
        return Cart.delete({ 
          cartId: cartId 
        }).$promise.then(function(response) {
          // TODO: can I clean this up?
          var deletedCart = Service.findCartById(cartId);
          var idx = Service.carts.indexOf(deletedCart);
          Service.carts.splice(idx, 1);
          return response;
        });
      },

      deleteMultipleCarts: function(cartGuidArray)
        {
          return $http.delete('/cart', {
            headers:{'Content-Type': 'application/json'},
            data: cartGuidArray
          });
        },

      /********************
      EDIT SINGLE ITEM
      TODO: currently I am not keeping the cached object in sync
      ********************/

      addItemToCart: function(cartId, item) {
        if (!item.quantity || item.quantity === 0) {
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

      // gets the default selected cart
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