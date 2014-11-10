'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CartService
 * @description
 * # CartService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CartService', ['$http', '$filter', '$q', '$upload', 'toaster', 'UtilityService', 'Cart',
    function ($http, $filter, $q, $upload, toaster, UtilityService, Cart) {

    var filter = $filter('filter');

    var Service = {
      carts: [],
      shipDates: [],

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
            delete existingCart.items;
            angular.copy(cart, Service.carts[idx]);
          } else {
            var newCart = angular.copy(cart);
            delete newCart.items;
            Service.carts.push(newCart);
          }
          return cart;
        });
      },

      findCartById: function(cartId) {
        return UtilityService.findObjectByField(Service.carts, 'id', cartId);
      },

      importCart: function(file, options) {
        var deferred = $q.defer();

        $upload.upload({
          url: 'import/order',
          method: 'POST',
          data: { options: options },
          file: file, // or list of files ($files) for html5 only
        }).then(function(response) {
          var data = response.data;
          debugger;
          if (data.success) {
            var cart = {
              id: data.listid, // ****
              name: 'Imported Cart'
            };
            Service.carts.push(cart);

            // display messages
            if (data.warningmsg) {
              toaster.pop('success', null, data.warningmsg);
            } else {
              toaster.pop('success', null, 'Successfully imported a new cart.');
            }

            deferred.resolve(data);
          } else {
            toaster.pop('error', null, data.errormsg);
            deferred.reject(data.errormsg);
          }
        });

        return deferred.promise;
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
        // default to next ship date
        if (!newCart.requestedshipdate && Service.shipDates.length > 0) {
          newCart.requestedshipdate = Service.shipDates[0].shipdate;
        }

        return Cart.save({}, newCart).$promise.then(function(response) {
          newCart.id = response.listitemid;
          Service.carts.push(newCart);
          return response.listitemid;
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
      },

      /********************
      SUBMIT ORDERS
      ********************/

      getShipDates: function() {
        var deferred = $q.defer();
        
        if (Service.shipDates.length > 0) {
          deferred.resolve(Service.shipDates);
        } else {
          Cart.getShipDates().$promise.then(function(data) {
            angular.copy(data.shipdates, Service.shipDates);
            deferred.resolve(data.shipdates);
            return data.shipdates;
          }); 
        }
        return deferred.promise;
      },

      findCutoffDate: function(obj) {
        var cutoffdate;
        if (obj && obj.requestedshipdate) {
          angular.forEach(Service.shipDates, function(shipDate) {
            var requestedShipDateString = new Date(obj.requestedshipdate).toDateString(),
              shipDateString = new Date(shipDate.shipdate + ' 00:00').toDateString();
            if (requestedShipDateString === shipDateString) {
              cutoffdate = shipDate;
            }
          });
        }
        return cutoffdate;
      },

      submitOrder: function(cartId) {
        return Cart.submit({
          cartId: cartId
        }, null).$promise;
      },

      setActiveCart: function(cartId) {
        return Cart.setActive({
          cartId: cartId
        }, null).$promise;
      }
    };

    return Service;
 
  }]);