'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:CartService
 * @description
 * # CartService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CartService', ['$http', '$q', '$upload', 'ENV', 'toaster', 'UtilityService', 'PricingService', 'Cart',
    function ($http, $q, $upload, ENV, toaster, UtilityService, PricingService, Cart) {
 
    var Service = {
      
      cartHeaders: [],
      shipDates: [],
 
      // accepts "header: true" params to get only names
      // return array of cart objects
      getAllCarts: function(requestParams) {
        if (!requestParams) {
          requestParams = {};
        }
 
        return Cart.query(requestParams).$promise;
      },
 
      getCartHeaders: function() {
        return Cart.query({header: true}).$promise
          .then(function(cartHeaders) {
            angular.copy(cartHeaders, Service.cartHeaders);
            return cartHeaders;
          });
      },
 
      // accepts cartId (guid)
      // returns cart object
      getCart: function(cartId) {
        return Cart.get({ 
          cartId: cartId,
        }).$promise.then(function(cart) {
          PricingService.updateCaculatedFields(cart.items);
          return cart;
        });
      },
 
      findCartById: function(cartId) {
        return UtilityService.findObjectByField(Service.cartHeaders, 'id', cartId);
      },
 
      // gets the default selected cart
      getSelectedCart: function(cartId) {
        var selectedCart;
        if (cartId) {
          selectedCart = Service.findCartById(cartId);
        }
        // go to active cart
        if (!selectedCart) {
          angular.forEach(Service.cartHeaders, function(cart, index) {
            if (cart.active) {
              selectedCart = cart;
            }
          });
        }
        // go to first cart in list
        if (!selectedCart && Service.cartHeaders && Service.cartHeaders.length > 0) {
          selectedCart = Service.cartHeaders[0];
        }
 
        return selectedCart;
      },
 
      /********************
      CREATE CART
      ********************/
 
      beforeCreateCart: function(items, shipDate) {
        var newCart = {};
    
        if (!items) { // if null
          newCart.items = [];
        } else if (Array.isArray(items)) { // if multiple items
          newCart.items = items;
        } else if (typeof items === 'object') { // if one item
          newCart.items = [items];
        }
 
        // TODO: move this out of here
        // set default quantity to 1
        angular.forEach(newCart.items, function (item, index) {
          if (!item.quantity || item.quantity === 0) {
            item.quantity = 1;
          }
        });
 
        newCart.name = UtilityService.generateName('Cart', Service.cartHeaders);
 
        newCart.requestedshipdate = shipDate;
        // default to next ship date
        if (!newCart.requestedshipdate && Service.shipDates.length > 0) {
          newCart.requestedshipdate = Service.shipDates[0].shipdate;
        }
        return newCart;
      },
 
      // accepts null, item object, or array of item objects and shipDate
      // returns promise and new cart object
      createCart: function(items, shipDate) {
        var newCart = Service.beforeCreateCart(items, shipDate);        
 
        return Cart.save({}, newCart).$promise.then(function(response) {
          newCart.id = response.listitemid;
          newCart.items = [];
          Service.cartHeaders.push(newCart);
          return newCart;
        });
      },
 
      importCart: function(file, options) {
        var deferred = $q.defer();
 
        $upload.upload({
          url: '/import/order',
          method: 'POST',
          data: { options: options },
          file: file, // or list of files ($files) for html5 only
        }).then(function(response) {
          var data = response.data;
          if (data.success) {
            var cart = {
              id: data.listid,
              name: 'Imported Cart'
            };
            Service.cartHeaders.push(cart);
 
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
 
      quickAdd: function(items) {
        return Cart.quickAdd({}, items).$promise.then(function(response) {
 
          if (response.success) {
            return response.id;
          } else {
            return $q.reject(response.errormessage);
          }
        }, function(error){
 
         return $q.reject('An error has occurred with this Quick Add order.');
        });
      },
 
      /********************
      EDIT CART
      ********************/
 
      // accepts cart object and params (deleteOmitted?)
      // returns promise and updated cart object
      updateCart: function(cart, params) {
        return Cart.update(params, cart).$promise.then(function(response) {
          return Service.getCart(response.id);
        });
      },
 
      addItemsToCart: function(cart, items) {
           cart.items = items;
        return Service.updateCart(cart, {deleteomitted: false});
      },
 
      // accepts cartId (guid)
      deleteCart: function(cartId) {
        return Cart.delete({ 
          cartId: cartId 
        }).$promise.then(function(response) {
          
          // updte cart headers cache
          var deletedCart = Service.findCartById(cartId);
          var idx = Service.cartHeaders.indexOf(deletedCart);
          Service.cartHeaders.splice(idx, 1);
          
          return response;
        });
      },
 
      deleteMultipleCarts: function(cartGuidArray) {
        return $http.delete('/cart', {
          headers:{'Content-Type': 'application/json'},
          data: cartGuidArray
        }).then(function() {
 
          // update cart headers cache
          var tempCarts = angular.copy(Service.cartHeaders);
          tempCarts.forEach(function(cart, index) {
            if (cartGuidArray.indexOf(cart.id) > -1) {
              Service.cartHeaders.splice(index, 1);
            }
          });
 
          return;
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
 
      /********************
      SUBMIT ORDERS
      ********************/
 
      updateNetworkStatus: function() {
        if ( ENV.mobileApp === true && navigator.connection.type === 'none') {
          Service.isOffline = true;
        }
        else{
          Service.isOffline = false;
        } 
      },
 
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
 
      findCutoffDate: function(cart) {
        var shipDateFound;
        if (cart && cart.requestedshipdate) {
          var selectedShipDate = cart.requestedshipdate.substr(0, 10);
          angular.forEach(Service.shipDates, function(shipDate) {
            if (selectedShipDate === shipDate.shipdate) {
              shipDateFound = shipDate;
            }
          });
        }
        return shipDateFound;
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