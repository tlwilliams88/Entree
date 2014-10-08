'use strict';

angular.module('bekApp')
    .factory('PhonegapCartService', ['$http', '$q', 'CartService', 'localStorageService', 'UserProfileService', 'UtilityService', 'Cart',
        function($http, $q, CartService, localStorageService, UserProfileService, UtilityService, Cart) {

            var originalCartService = angular.copy(CartService);

            var Service = angular.extend(CartService, {});

            Service.getAllCarts = function() {
                if (navigator.connection.type === 'none') {
                    if (Service.carts) {
                        localStorageService.set('carts', Service.carts);
                    }
                    var localCarts = localStorageService.get("carts");
                    angular.copy(localCarts, Service.carts);
                    return localCarts;
                } else {
                    originalCartService.getAllCarts().then(function(allCarts) {
                        localStorageService.set('carts', allCarts);
                        return allCarts;
                    });

                }

            };

            Service.getCart = function(cartId) {
                if (navigator.connection.type === 'none') {
                    return Service.findCartById(cartId);
                } else {
                    return originalCartService.getCart(cartId);
                }
            };

            Service.createCart = function(items, shipDate) {
                if (navigator.connection.type === 'none') {
                    var newCart = {};
                    if (!items) { // if null
                        newCart.items = [];
                    } else if (Array.isArray(items)) { // if multiple items
                        newCart.items = items;
                    } else if (typeof items === 'object') { // if one item
                        newCart.items = [items];
                    }

                    // set default quantity to 1
                    angular.forEach(newCart.items, function(item, index) {
                        if (!item.quantity || item.quantity === 0) {
                            item.quantity = 1;
                        }
                    });

                    newCart.name = UtilityService.generateName('Cart', Service.carts);
                    newCart.requestedshipdate = shipDate;
                    newCart.isNew = true;

                    var localCarts = localStorageService.get('carts');
                    newCart.cartid = newCart.name;
                    localCarts.push(newCart);
                    localStorageService.set('carts', localCarts);
                    Service.carts.push(newCart);

                    //return a promise
                    var deferred = $q.defer();
                    deferred.resolve(newCart);
                    return deferred.promise;

                } else {
                    return originalCartService.createCart(items, shipDate).then(function(response) {
                        localStorageService.set('carts', Service.carts);
                        return response;
                    });
                }

            };

            Service.updateCart = function(cart, params) {
                if (navigator.connection.type === 'none') {
                    var localCarts = localStorageService.get('carts');
                    angular.forEach(localCarts, function(item, index) {
                        if (item.id === cart.id) {
                            cart.isUpdated = true;
                            localCarts[index] = cart;
                        }
                    });
                    localStorageService.set('carts', localCarts);
                    angular.copy(localCarts, Service.carts);
                    var deferred = $q.defer();
                    deferred.resolve(cart.id);
                    return deferred.promise;
                } else {
                    return originalCartService.updateCart(cart, params).then(function(response) {
                        localStorageService.set('carts', Service.carts);
                        return response;
                    });
                }
            };

            Service.deleteCart = function(cartId) {
                if (navigator.connection.type === 'none') {
                    var localCarts = localStorageService.get('carts');

                    angular.forEach(localCarts, function(item, index) {
                        if (item.id === cartId) {
                            localCarts.splice(index, 1);
                            if (item.isNew) {
                                var isNew = true;
                            }

                            localStorageService.set('carts', localCarts);
                            angular.copy(localCarts, Service.carts);
                            if (!isNew) {
                                var deletedCartGuids = localStorageService.get('deletedCartGuids');
                                if (deletedCartGuids) {
                                    deletedCartGuids.push(cartId);
                                    localStorageService.set('deletedCartGuids', deletedCartGuids);
                                } else {
                                    var deletedArray = [];
                                    deletedArray.push(cartId);
                                    localStorageService.set('deletedCartGuids', deletedCartGuids);
                                }
                            }
                        }
                    });
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

                    var updatedCart = Service.findCartById(cartId);
                    if (updatedCart && updatedCart.items) {
                        updatedCart.items.push(item);
                        updatedCart.isChanged = true;
                    }
                    localStorageService.set('carts', Service.carts);

                } else {
                    return originalCartService.addItemToCart(cartId, item);
                }
            };

            Service.updateCartsFromLocal = function() {
                var promises = [];
                var localCarts = localStorageService.get('carts');
                angular.forEach(localCarts, function(cart, index) {
                    if (cart.isNew) {
                        delete cart.id;
                        delete cart.isNew;
                        promises.push(Service.createCartFromLocal(cart));
                    }
                    if (cart.isChanged) {
                        delete cart.isChanged;
                        promises.push(Service.updateCart(cart).then(null, function(rejection) {
                            console.log(rejection);
                        }));
                    }

                });
                var deletedCartGuids = localStorageService.get('deletedCartGuids');
                if (deletedCartGuids) {
                    promises.push(Service.deleteMultipleCarts(deletedCartGuids));
                }

                $q.all(promises).then(function() {
                    //update from server and remove deleted array
                    Service.getAllCarts();
                    console.log('carts updated!');
                    localStorageService.remove('deletedCartGuids');
                });



            };

            Service.createCartFromLocal = function(newCart) {
                return Cart.save({}, newCart).$promise.then(function(response) {
                    return Service.getCart(response.listitemid);
                });
            };



        }
    ]);