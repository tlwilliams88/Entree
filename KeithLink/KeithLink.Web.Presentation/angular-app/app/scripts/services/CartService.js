'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CartService
 * @description
 * # CartService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('CartService', ['$http', '$q', '$upload', 'ENV', 'toaster', 'UtilityService', 'PricingService', 'ExportService', 'Cart', 'DateService', 'SessionService', 'Constants', 'LocalStorage', '$rootScope', 'AnalyticsService',
    function ($http, $q, $upload, ENV, toaster, UtilityService, PricingService, ExportService, Cart, DateService, SessionService, Constants, LocalStorage, $rootScope, AnalyticsService) {

    var Service = {

      renameCart: false,
      cartContainsSpecialItems: false,
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
        return Cart.get({header: true}).$promise
          .then(function(resp) {
            var cartHeaders = resp.successResponse;
            angular.copy(cartHeaders, Service.cartHeaders);
            Service.updateCartHeaders(cartHeaders);
            return cartHeaders;
          });
      },

      updateCartHeaders: function(headers) {
        $rootScope.cartsHeaders = headers;
      },

      // accepts cartId (guid)
      // returns cart object
      getCart: function(cartId) {
        return Cart.get({
          cartId: cartId,
        }).$promise.then(function(resp) {
          var cart = resp.successResponse;
            Service.cartContainsSpecialItems = false;
            var i;
            if(cart != null && cart.items && cart.items.length > 0){
              for (i = 0; i < cart.items.length; i++) {
                if (cart.items[i].is_specialty_catalog) {
                  Service.cartContainsSpecialItems = true;
                }
              }
              PricingService.updateCaculatedFields(cart.items);
            }

          return cart;
        });
      },

      printOrder: function(list, cartId, landscape, showparvalues, options, shownotes) {

          var printparams = {
            landscape: landscape,
            showparvalues: showparvalues,
            shownotes: shownotes,
            paging: options
          };


        var promise = $http.post('/cart/print/' + cartId + '/' + list.type + '/' + list.listid, printparams, {
          responseType: 'arraybuffer'
        });
        return ExportService.print(promise);
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

      beforeCreateCart: function(items, shipDate, name, ponumber) {
        var newCart = {};

        if (!items) { // if null
          newCart.items = [];
        } else if (Array.isArray(items))  { // if multiple items
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

          AnalyticsService.recordAddToCart(
            item,
            LocalStorage.getCustomerNumber(),
            LocalStorage.getBranchId());
        });

        if (name && name !== 'New') {
          newCart.name = name;
        } else {
          newCart.name = UtilityService.generateName(SessionService.userProfile.firstname, Service.cartHeaders);
        }

        newCart.requestedshipdate = shipDate;
        // default to next ship date
        if (!newCart.requestedshipdate && Service.shipDates.length > 0) {
          newCart.requestedshipdate = Service.shipDates[0].shipdate;
        }
        if (!newCart.subtotal) {
          newCart.subtotal = PricingService.getSubtotalForItems(newCart.items);
        }

        if(ponumber){
          newCart.ponumber = ponumber;
        }

        return newCart;
      },

      // accepts null, item object, or array of item objects and shipDate
      // returns promise and new cart object
      createCart: function(items, shipDate, name, ponumber, listid, listtype) {
        var newCart = Service.beforeCreateCart(items, shipDate, name, ponumber);

        newCart.message = 'Creating cart...';
        newCart.listid = listid;
        newCart.listtype = listtype;
        return Cart.save({}, newCart).$promise.then(function(response) {
          if(response.successResponse){
            newCart.id = response.successResponse.listitemid;
            newCart.createddate = new Date();
            newCart.itemcount = newCart.items.length;
            newCart.items = [];
            Service.cartHeaders.push(newCart);
            Service.getCartHeaders();
            newCart.valid = true;
            return newCart;
          } else {
            newCart.valid = false;
            return newCart;
          }
        });
      },

      /********************
      IMPORT CART
      ********************/

      importCart: function(file, options, selectedCart) {
        var deferred = $q.defer(),
            cart;

        if(selectedCart){
          options.cartname = selectedCart.name;
          options.shipdate = selectedCart.requestedshipdate;
          options.ponumber = selectedCart.ponumber;
          if(selectedCart.items){
            selectedCart.items = [];
          }
          cart = selectedCart;
        }

        $upload.upload({
          url: '/import/order',
          method: 'POST',
          data: { options: options },
          file: file, // or list of files ($files) for html5 only
        }).then(function(response) {
          var data = response.data.successResponse;
          if (response.data.isSuccess && data.success) {
            if(!selectedCart){
              cart.name = 'Imported Cart';
            }
            cart.id = data.listid;
            Service.cartHeaders.push(cart);

            // display messages
            toaster.pop('success', null, data.successmsg);
            if (data.warningmsg) {
              toaster.pop('error', null, data.warningmsg);
            }

            deferred.resolve(data);
          } else {
            var errorMessage = response.data.errorMessage;
            if(data && data.errormsg){
              toaster.pop('error', null, data.errormsg);
              errorMessage = data.errormsg;
            }
            deferred.reject(errorMessage);
          }
        });

        return deferred.promise;
      },

      /********************
      QUICK ADD
      ********************/

      validateQuickAdd: function(items) {
        return $http.post('/cart/quickadd/validate', items).then(function(response) {
          return response.data.successResponse;
        });
      },

      quickAdd: function(items) {
        return Cart.quickAdd({}, items).$promise.then(function(resp) {
          var response = resp.successResponse;
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
      EXPORT
      ********************/

      getCartExportConfig: function(cartid) {
        return $http.get('/cart/export/' + cartid).then(function(response){
          return response.data.successResponse;
        });
      },

      exportCart: function(cartid, config) {
        ExportService.export('/cart/export/' + cartid, config);
      },

      /********************
      EDIT CART
      ********************/

      // accepts cart object and params (deleteOmitted?)
      // returns promise and updated cart object
      updateCart: function(cart, params, list) {

        if(list){
          cart.listid = list.listid;
          cart.listtype = list.type;
          var lastList ={
            listId: cart.listid,
            listType: cart.listtype
          };

          LocalStorage.setLastList(lastList);
          LocalStorage.setLastOrderList(lastList);
        }

        return Cart.update(params, cart).$promise.then(function(response) {
          cart = response.successResponse;

          Service.getCartHeaders();

          var cartId = cart.id ? cart.id : cart.commerceid;
          return Service.getCart(cartId);
        });
      },

      updateCartFromQuickAdd: function(cart) {
        cart.message = 'Saving cart...';
        return Cart.updateFromQuickAdd(cart).$promise.then(function(response) {

          // update cache
          Service.cartHeaders.forEach(function(cartHeader, index) {
            if (cartHeader.id === cart.id) {
              var cartHeaderToUpdate = Service.cartHeaders[index];
              cartHeaderToUpdate.requestedshipdate = cart.requestedshipdate;
              cartHeaderToUpdate.subtotal = cart.subtotal;
              cartHeaderToUpdate.name = cart.name;
            }
          });

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

          Service.updateCartHeaders(Service.cartHeaders);

          return response.successResponse;
        });
      },

      deleteMultipleCarts: function(cartGuidArray) {
        return $http.delete('/cart', {
          headers:{'Content-Type': 'application/json'},
          data: cartGuidArray
        }).then(function() {

          // update cart headers cache
          var cartsKept = [];
          Service.cartHeaders.forEach(function(cart, index) {
            if (cartGuidArray.indexOf(cart.id) === -1) {
              cartsKept.push(cart);
            }
          });
          angular.copy(cartsKept, Service.cartHeaders);

          Service.updateCartHeaders(Service.cartHeaders);

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

        AnalyticsService.recordAddToCart(
          item,
          LocalStorage.getCustomerNumber(),
          LocalStorage.getBranchId());

        return Cart.addItem({ cartId: cartId }, item).$promise.then(function(response) {
          Service.getCartHeaders();
          return response.successResponse;
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
            var dates = data.successResponse;
            if(dates.shipdates.length > 0){
            var cutoffDate = DateService.momentObject(dates.shipdates[0].cutoffdatetime).format();
            var now = DateService.momentObject().tz('America/Chicago').format();

            var invalidSelectedDate = (now > cutoffDate) ? true : false;
            if(invalidSelectedDate){
             dates.shipdates = dates.shipdates.slice(1,dates.shipdates.length);
            }
            angular.copy(dates.shipdates, Service.shipDates);
            deferred.resolve(dates.shipdates);
            return dates.shipdates;
        }
          });
        }
        return deferred.promise;
      },

      findCutoffDate: function(cart) {
        var shipDateFound;
        if (cart && cart.requestedshipdate) {
          var selectedShipDate = DateService.momentObject(cart.requestedshipdate.substr(0, 10)).format(Constants.dateFormat.yearMonthDayDashes);
          angular.forEach(Service.shipDates, function(shipDate) {
            if (selectedShipDate == DateService.momentObject(shipDate.shipdate).format(Constants.dateFormat.yearMonthDayDashes)) {
              shipDateFound = shipDate;
            }
          });
        }
        return shipDateFound;
      },

      isSubmitted: function(cartId) {
        return $http.get('/cart/issubmitted/' + cartId).then(function(resp){
          return (resp && resp.data) ? resp.data.successResponse : true;
        });
      },

      submitOrder: function(cartId) {
        return Cart.submit({
          cartId: cartId
        }, { message: 'Submitting Order...' }).$promise.then(function(data) {
          var deletedCart = Service.findCartById(cartId);
          var idx = Service.cartHeaders.indexOf(deletedCart);
          Service.cartHeaders.splice(idx, 1);
          return data.successResponse;
        });
      },

      setActiveCart: function(cartId) {
        return Cart.setActive({
          cartId: cartId
        }, null).$promise.then(function(resp){
          return resp.successResponse;
        });
      }
    };

    return Service;

  }]);
