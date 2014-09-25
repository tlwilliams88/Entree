'use strict';

angular.module('bekApp')
    .factory('PhonegapCartService', ['$http', '$q', 'CartService', 'localStorageService', 'UserProfileService', 'UtilityService', 'Cart',
        function($http, $q, CartService, localStorageService, UserProfileService, UtilityService, Cart) {
            var Service = angular.extend(CartService, {});

            // function getBranch() {
            //     //return UserProfileService.getCurrentBranchId();
            //     return 'fdf';
            // }

            // Service.getAllCarts2 = function() {
            //     if (navigator.connection.type === 'none') {
            //         if (Service.carts) {
            //             localStorageService.set('carts', Service.carts);
            //         }
            //         var localCarts = localStorageService.get("carts");
            //         angular.copy(localCarts, Service.carts);
            //         return localCarts;
            //     } else {
            //         return Cart.query({
            //             branchId: getBranch()
            //         }).$promise.then(function(response) {
            //             var allCarts = response;
            //             angular.copy(allCarts, Service.carts);
            //             return allCarts;

            //         });
            //     }

            // };

            // Service.addItemToCart2 = function(cartId, item) {
            //     if (!item.quantity) {
            //         item.quantity = 1;
            //     }

            //     if (navigator.connection.type === 'none') {
            //         var cart = Service.findCartById(cartId);
            //         cart.items.push(item);
            //         localStorageService.set('carts', Service.carts);
            //         //return a promise
            //         var deferred = $q.defer();
            //         deferred.resolve();
            //         return deferred.promise;
            //     } else {
            //         return Cart.addItem({
            //             cartId: cartId
            //         }, item).$promise.then(function(response) {
            //             return response;
            //         });
            //     }
            // };

            // Service.updateCartFromLocal2 = function() {
            //     var params = null;
            //     var localCarts = localStorageService.get('carts');
            //     angular.forEach(localCarts, function(cart, index){
            //         return Cart.update(params, cart).$promise.then(function(response) {
            //             return response.data;
            //         });
            //     });
            // };

        }
    ]);