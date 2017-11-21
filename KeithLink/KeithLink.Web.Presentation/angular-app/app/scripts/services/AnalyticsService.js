'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:AnalyticsService
 * @description
 * # AnalyticsService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('AnalyticsService', ['$window', '$state', 'Analytics', function($window, $state, Analytics) {

    var Service = {

        setUserProperties: function(userName, roleName, isInternalUser, isKbitCustomer, isPowerMenuCustomer){
            //Angulartics Google Analytics provider doesn't support custom dimensions
            //so we have to inject our settings in the main pipe and send that way
            $window.ga('set', 'dimension2', roleName);
            $window.ga('set', 'dimension3', isInternalUser);
            $window.ga('set', 'dimension4', isKbitCustomer);
            $window.ga('set', 'dimension5', isPowerMenuCustomer);
            $window.ga('set', 'userId', roleName + '.' + userName);
            //we also have to "light a beacon" and send our settings in with it
            $window.ga('send', 'event', 'Process', 'Set user properties');
        },
        
        recordTransaction: function(customerName, orderNumber, cart, customerNumber, customerBranch){
            cart.items.forEach(function(item){
                item.price = item.caseprice && item.packageprice == '0.00' ? item.caseprice : item.packageprice;

                // Add item to transaction
                Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);
            });

            // Create transaction
            Analytics.trackTransaction(orderNumber, 
                                       customerBranch + '.' + customerNumber + '.' + customerName, 
                                       cart.subtotal, 
                                       '', 
                                       '', 
                                       '', 
                                       cart.listid, 
                                       'Cart Submission', 
                                       '');
        },
        
        recordCheckout: function(cart){
            cart.items.forEach(function(item){
                item.price = item.caseprice && item.packageprice == '0.00' ? item.caseprice : item.packageprice;

                // Add item to cart
                Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);
            });

            // Create Checkout Record
            Analytics.trackCheckout(1, cart.requestedshipdate);
        },

        recordAddToCart: function(item){
            var addedFrom = $state.current.name;

            // Add item being added
            Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);

            // Create Cart Record
            Analytics.trackCart('add', addedFrom);
        },
        
        recordRemoveItem: function(item){
            var removedFrom = $state.current.name;

            // Add item being removed
            Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);

            // Create Cart Record
            Analytics.trackCart('remove', removedFrom);
        },
        
        recordViewDetail: function(item){
            // Add Item Viewed
            Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);

            Analytics.trackDetail();
        }

    };
    return Service;

}]);