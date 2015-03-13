'use strict';

angular.module('bekApp')
  .factory('PhonegapCartService', ['$http', '$q', 'CartService', 'PhonegapLocalStorageService', 'PhonegapDbService', 'PricingService', 'Cart',
    function($http, $q, CartService, PhonegapLocalStorageService, PhonegapDbService, PricingService, Cart) {

      var originalCartService = angular.copy(CartService);

      var Service = angular.extend(CartService, {});

      var db_table_name_carts = 'carts';

      function generateId() {
        return 'a' + Math.floor((1 + Math.random()) * 0x10000).toString(16); // generate 5 digit id
      }

      function updateCachedCarts(updatedCart) {
        Service.carts.forEach(function(cart, index) {
          if (cart.id === updatedCart.id) {
            Service.carts[index] = updatedCart;
          }
        });
      }

      Service.getAllCarts = function() {
        if (navigator.connection.type === 'none') {
          console.log('getting carts from DB');
          // TEST: keep db carts up to date while online
          return PhonegapDbService.getAllItems(db_table_name_carts).then(function(data) {
            angular.copy(data, Service.carts);
            return data;
          });
        } else {
          console.log('getting all carts from server');
          return originalCartService.getAllCarts();
        }
      };

      Service.getCart = function(cartId) {
        if (navigator.connection.type === 'none') {
          return PhonegapDbService.getItem(db_table_name_carts, cartId).then(function(cart) {
            console.log('found cart');
            console.log(cart);
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

      Service.createCart = function(items, shipDate) {
        debugger;
        if (navigator.connection.type === 'none') {

          var newCart = originalCartService.beforeCreateCart(items, shipDate);
          newCart.id = generateId();
          newCart.isNew = true;

          newCart.items.forEach(function(item) {
            item.cartitemid = generateId();
            item.isNew = true;
          });

          PhonegapDbService.setItem(db_table_name_carts, newCart.id, newCart);
          Service.carts.push(newCart);

          //return a promise
          var deferred = $q.defer();
          deferred.resolve(newCart);
          return deferred.promise;

        } else {
          return originalCartService.createCart(items, shipDate);
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
          updateCachedCarts(cart); // update Service.carts

          deferred.resolve(cart);
          return deferred.promise;
        } else {
          return originalCartService.updateCart(cart, params);
        }
      };

      Service.deleteCart = function(cartId) {
        if (navigator.connection.type === 'none') {
          console.log('deleting cart offline');

          Service.carts.forEach(function(cart, index) {
            if (cart.id === cartId) {
              Service.carts.splice(index, 1);
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
          return originalCartService.deleteCart(cartId);
        }
      };

      Service.addItemToCart = function(cartId, item) {
        if (navigator.connection.type === 'none') {
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
          return item;
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
            })
            cartFound.items = cartFound.items.concat(items);
            PhonegapDbService.setItem(db_table_name_carts, cartFound.id, cartFound);
            deferred.resolve(cartFound);
          });
          return deferred.promise;
        } else {
          return originalCartService.addItemToCart(cart, items);
        }
      }

      Service.updateCartsFromLocal = function() {
        console.log('updating carts after back online');

        PhonegapDbService.getAllItems(db_table_name_carts).then(function(storedCarts) {
          debugger;
          var promises = [];
          angular.forEach(storedCarts, function(cart, index) {

            if (cart.isNew) { // create carts
              debugger;
              var newItems = [];
              cart.items.forEach(function(item) {
                if (item.itemnumber) {
                  newItems.push({
                    itemnumber: item.itemnumber,
                    quantity: item.quantity,
                    each: item.each
                  });
                }
              });
              var newCart = {
                name: cart.name,
                items: newItems
              };
              promises.push(Service.createCartFromLocal(newCart));
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

          debugger;
          $q.all(promises).then(function() {
            console.log('carts updated!');

            //update from server and remove deleted array
            PhonegapDbService.dropTable(db_table_name_carts)
              .then(Service.getAllCarts);

            PhonegapLocalStorageService.removeDeletedCartGuids();
          }, function() {
            console.log('error updating carts');
          });
        });
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
