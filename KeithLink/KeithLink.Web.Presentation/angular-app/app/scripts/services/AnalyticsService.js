'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:AnalyticsService
 * @description
 * # AnalyticsService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('AnalyticsService', ['$state', 'Analytics', function($state, Analytics) {

    var Service = {

        recordTransaction: function(customerName, orderNumber, cart){
            cart.items.forEach(function(item){
                item.price = item.caseprice && item.packageprice == '0.00' ? item.caseprice : item.packageprice;

                // Add item to transaction
                Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);
            });

            // Create transaction
            Analytics.trackTransaction(orderNumber, customerName, cart.subtotal, '', '', '', cart.listid, 'Cart Submission', '');
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