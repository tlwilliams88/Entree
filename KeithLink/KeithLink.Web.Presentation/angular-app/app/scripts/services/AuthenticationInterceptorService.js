'use strict';

angular.module('bekApp')
.factory('AuthenticationInterceptorService', ['$q', '$location', 'UserProfileService', function ($q, $location, UserProfileService) {
 
    var authInterceptorServiceFactory = {
        request: function (config) {
 
            config.headers = config.headers || {};
     
            var authData = UserProfileService.getProfile().token;
            if (authData) {
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