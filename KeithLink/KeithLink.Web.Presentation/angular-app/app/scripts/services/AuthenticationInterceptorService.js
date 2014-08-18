'use strict';

angular.module('bekApp')
.factory('AuthenticationInterceptorService', ['$q', '$location', 'localStorageService', 'Constants', function ($q, $location, localStorageService, Constants) {
 
    var authInterceptorServiceFactory = {
        request: function (config) {
 
            var authorizedUrls = [
                '/authen'
            ];

            config.headers = config.headers || {};
     
            var authData = localStorageService.get(Constants.localStorage.userToken);
            // add auth data if present and if url accepts an authorization token
            if (authData && authorizedUrls.indexOf(config.url) === -1) {
                config.headers.Authorization = 'Bearer ' + authData.access_token;
            }
     
            return config;
        },

         responseError: function (rejection) {
            if (rejection.status === 401) {
                // $location.path('/login');
            }
            return $q.reject(rejection);
        }

    };
  
    return authInterceptorServiceFactory;
}]);