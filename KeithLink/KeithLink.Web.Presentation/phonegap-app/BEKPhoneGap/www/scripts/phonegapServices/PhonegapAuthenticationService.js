'use strict';

angular.module('bekApp')
    .factory('PhonegapAuthenticationService', ['$http', '$q', 'AuthenticationService', 'UserProfileService', 'localStorageService', 'ListService',
        function($http, $q, AuthenticationService, UserProfileService, localStorageService, ListService) {
            var Service = angular.extend(AuthenticationService, {})

            Service.login = function(username, password) {
                return Service.authenticateUser(username, password).then(function(token) {

                    return $q.all([
                        UserProfileService.getProfile(username),
                        ListService.getAllLists()
                    ]).then(function(results) {
                        var profile = results[0];
                        var lists = results[1];
                        localStorageService.set('lists', lists);
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