'use strict';

angular.module('bekApp')
  .factory('PhonegapCartService', ['$http', '$q', '$log', 'CartService', 'PhonegapLocalStorageService', 'PhonegapDbService', 'PricingService', 'Cart',
    function($http, $q, $log, CartService, PhonegapLocalStorageService, PhonegapDbService, PricingService, Cart) {

      var originalCartService = angular.copy(CartService);

      var Service = angular.extend(CartService, {});

      var db_table_name_carts = 'carts';

      function generateId() {
        // generate random id that has a '-' in it
        return 'a' + Math.floor((1 + Math.random()) * 0x10000).toString(16) + '-' + Math.floor((1 + Math.random()) * 0x10000).toString(16);
      }

      function updateCachedCarts(updatedCart) {
        Service.cartHeaders.forEach(function(cart, index) {
          if (cart.id === updatedCart.id) {
            Service.cartHeaders[index] = updatedCart;
          }
        });
      }

      function getAllCartDetails(cartHeaders) {
        cartHeaders.forEach(function(cart) {
          originalCartService.getCart(cart.id).then(function(cartWithItems) {
            PhonegapDbService.setItem(db_table_name_carts, cartWithItems.id, cartWithItems);
          });
        });
      }

      Service.getAllCartsForOffline = function() {
        var cartPromise = PhonegapDbService.dropTable(db_table_name_carts)
          .then(originalCartService.getCartHeaders)
          .then(getAllCartDetails);
        var shipDatesPromise = originalCartService.getShipDates()
          .then(PhonegapLocalStorageService.setShipDates);

        return $q.all([
          cartPromise,
          shipDatesPromise
        ]);
      };

      Service.getCartHeaders = function() {
        if (navigator.connection.type === 'none') {
          $log.debug('getting carts from DB');
          return PhonegapDbService.getAllItems(db_table_name_carts).then(function(data) {
            angular.copy(data, Service.cartHeaders);
            return data;
          });
        } else {
          $log.debug('getting all carts from server');
          return originalCartService.getCartHeaders();
        }
      };

      Service.getCart = function(cartId) {
        if (navigator.connection.type === 'none') {
          return PhonegapDbService.getItem(db_table_name_carts, cartId).then(function(cart) {
            $log.debug('found cart');
            $log.debug(cart);
            PricingService.updateCaculatedFields(cart.items);
            return cart;
          });
        } else {
          return originalCartService.getCart(cartId);
        }
      };

      Service.setActiveCart = function(cartId) {
        if (navigator.connection.type === 'none') {
          var deferred = $q.defer();
          deferred.resolve();
          return deferred.promise;
        } else {
          return originalCartService.setActiveCart(cartId);
        }
      };

      Service.createCart = function(items, shipDate, name) {
       
        if (navigator.connection.type === 'none') {

          var newCart = originalCartService.beforeCreateCart(items, shipDate);
          newCart.id = generateId();
          newCart.isNew = true;

          newCart.items.forEach(function(item) {
            item.cartitemid = generateId();
            item.isNew = true;
          });
          //newCart.name = name;
          PhonegapDbService.setItem(db_table_name_carts, newCart.id, newCart);
          Service.cartHeaders.push(newCart);

          Service.renameCart = true;

          //return a promise
          var deferred = $q.defer();
          deferred.resolve(newCart);
          return deferred.promise;

        } else {
          return originalCartService.createCart(items, shipDate, name).then(function(newCart){
            newCart.items = items;
            PhonegapDbService.setItem(db_table_name_carts, newCart.id, newCart);
            return newCart;
          });
        }
      };

      Service.updateCart = function(cart, params) {
        if (navigator.connection.type === 'none') {
          var deferred = $q.defer();

          if (!cart.isNew) {
            cart.isChanged = true;
          }

          // flag new items and give them a temp id 
          cart.items.forEach(function(item) {
            if (!item.cartitemid) {
              item.cartitemid = generateId();
              item.isNew = true;
            }
          });

          PhonegapDbService.setItem(db_table_name_carts, cart.id, cart);
          updateCachedCarts(cart);

          deferred.resolve(cart);
          return deferred.promise;
        } else {
          return originalCartService.updateCart(cart, params).then(function(updatedCart){
            PhonegapDbService.setItem(db_table_name_carts, updatedCart.id, updatedCart);
            return updatedCart;
          });
        }
      };

      Service.deleteCart = function(cartId) {
        if (navigator.connection.type === 'none') {
          $log.debug('deleting cart offline');

          Service.cartHeaders.forEach(function(cart, index) {
            if (cart.id === cartId) {
              Service.cartHeaders.splice(index, 1);
              PhonegapDbService.removeItem(db_table_name_carts, cartId);

              // add cartId to cart of deleted carts for deleting on the server when back online
              var deletedCartGuids = PhonegapLocalStorageService.getDeletedCartGuids();
              if (!deletedCartGuids) {
                deletedCartGuids = [];
              }
              if (!cart.isNew) { // carts created while offline don't need to be sent to the server
                deletedCartGuids.push(cartId);
                PhonegapLocalStorageService.setDeletedCartGuids(deletedCartGuids);
              }
            }
          });

          // what do i need to return??? 
          var deferred = $q.defer();
          deferred.resolve();
          return deferred.promise;

        } else {
          PhonegapDbService.removeItem(db_table_name_carts, cartId);
          return originalCartService.deleteCart(cartId);
        }
      };

      Service.addItemToCart = function(cartId, item) {
        if (navigator.connection.type === 'none') {
          var deferred = $q.defer();
          if (!item.quantity || item.quantity === 0) {
            item.quantity = 1;
          }
          delete item.cartitemid;
          item.cartitemid = generateId();

          var updatedCart = Service.findCartById(cartId);
          if (updatedCart && updatedCart.items) {
            updatedCart.items.push(item);
            updatedCart.isChanged = true;
          }

          PhonegapDbService.setItem(db_table_name_carts, cartId, updatedCart);
          deferred.resolve(item);
          return deferred.promise;
        } else {
          return originalCartService.addItemToCart(cartId, item);
        }
      };

      Service.addItemsToCart = function(cart, items) {
        if (navigator.connection.type === 'none') {
          var deferred = $q.defer();
          PhonegapDbService.getItem(db_table_name_carts, cart.id).then(function(cartFound) {
            cartFound.isChanged = true;
            
            items.forEach(function(item) {
              item.cartitemid = generateId();
              item.isNew = true;
            });

            cartFound.items = cartFound.items.concat(items);
            PhonegapDbService.setItem(db_table_name_carts, cartFound.id, cartFound);
            deferred.resolve(cartFound);
          });
          return deferred.promise;
        } else {
          return originalCartService.addItemToCart(cart, items).then(function(updatedCart){
            PhonegapDbService.setItem(db_table_name_carts, updatedCart.id, updatedCart);
            return updatedCart;
          });
        }
      }

      Service.updateCartsFromLocal = function() {
        $log.debug('updating carts after back online');
        debugger;

        PhonegapDbService.getAllItems(db_table_name_carts).then(function(storedCarts) {
          
          var promises = [];
          angular.forEach(storedCarts, function(cart, index) {

            if (cart.isNew) { // create carts
              
              // var newItems = [];
              // cart.items.forEach(function(item) {
              //   if (item.itemnumber) {
              //     newItems.push({
              //       itemnumber: item.itemnumber,
              //       quantity: item.quantity,
              //       each: item.each
              //     });
              //   }
              // });
              // var newCart = {
              //   name: cart.name,
              //   requestedshipdate: cart.requestedshipdate,
              //   items: cart.items
              // };
              promises.push(Service.createCartFromLocal(cart));
            } else if (cart.isChanged) { // update carts
              delete cart.isChanged;
              cart.items.forEach(function(item) {
                if (item.isNew) {
                  delete item.cartitemid;
                }
              });
              promises.push(originalCartService.updateCart(cart));
            }

          });

          // delete carts
          var deletedCartGuids = PhonegapLocalStorageService.getDeletedCartGuids();
          if (deletedCartGuids) {
            promises.push(Service.deleteMultipleCarts(deletedCartGuids));
          }
         
          $q.all(promises).then(function() {
            $log.debug('carts updated!');
            originalCartService.updateNetworkStatus();
            //update from server and remove deleted array
            Service.getAllCartsForOffline();

            PhonegapLocalStorageService.removeDeletedCartGuids();
          }, function() {
            $log.debug('error updating carts');
          });
        })
      };

      Service.createCartFromLocal = function(newCart) {
        return Cart.save({}, newCart).$promise;
      };

      Service.getShipDates = function() {
        if (navigator.connection.type === 'none') {
          var shipDates = PhonegapLocalStorageService.getShipDates();
          var deferred = $q.defer();
          deferred.resolve(shipDates);
          return deferred.promise;
        } else {
          return originalCartService.getShipDates().then(function(response) {
            PhonegapLocalStorageService.setShipDates(response);
            return response;
          });
        }
      };

      return Service;

    }
  ]);