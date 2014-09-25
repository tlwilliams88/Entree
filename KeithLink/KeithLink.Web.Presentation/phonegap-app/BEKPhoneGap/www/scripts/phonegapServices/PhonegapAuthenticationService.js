'use strict';

angular.module('bekApp')
    .factory('PhonegapAuthenticationService', ['$http', '$q', 'AuthenticationService', 'UserProfileService', 'localStorageService', 'ListService', 'CartService',
        function($http, $q, AuthenticationService, UserProfileService, localStorageService, ListService, CartService) {
            var Service = angular.extend(AuthenticationService, {})

            Service.login = function(username, password) {
                return Service.authenticateUser(username, password).then(function(token) {

                    return $q.all([
                        UserProfileService.getProfile(username),
                        ListService.getAllLists(),
                        ListService.getAllLabels(),
                        CartService.getAllCarts()
                    ]).then(function(results) {
                        var profile = results[0];
                        var lists = results[1];
                        var labels = results[2];
                        var carts = results[3];
                        localStorageService.set('lists', lists);
                        localStorageService.set('labels', labels);
                        localStorageService.set('carts', carts);
                        return profile;
                    },
                    function(error){
                    	console.log(error);
                    	Service.logout();
                    });
                });
            }
            return Service;
        }
    ]);