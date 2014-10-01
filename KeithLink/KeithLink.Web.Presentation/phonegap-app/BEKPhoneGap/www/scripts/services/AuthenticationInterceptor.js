'use strict';

angular.module('bekApp')
.factory('AuthenticationInterceptor', ['$q', '$location', 'localStorageService', 'Constants', 'ENV',
  function ($q, $location, localStorageService, Constants, ENV) {

  var authInterceptorServiceFactory = {
    request: function (config) {

      // do not alter requests for html templates or the service locator, all api requests start with a '/'
      if (config.url.indexOf('/') === 0) {
        config.headers = config.headers || {};

        // add authorization token header if token is present and endpoint requires authorization
        var authData = localStorageService.get(Constants.localStorage.userToken);
        if (authData) {
          if (endpointRequiresToken(config.url)) {
              config.headers.Authorization = 'Bearer ' + authData.access_token;
          }
        }
        // add api key to request headers, do not add to /authen request
        if (config.url.indexOf('/authen') === -1) {
          config.headers['api-key'] = ENV.apiKey;
        }

        // add api url to request url
        config.url = ENV.apiEndpoint + config.url;
        console.log(config.url);
      }

      return config;
    },

     responseError: function (rejection) {
      if (rejection.status === 401) {
        localStorageService.remove(Constants.localStorage.userProfile);
        localStorageService.remove(Constants.localStorage.userToken);
        $location.path('/register');
      }
      return $q.reject(rejection);
    }

  };

  return authInterceptorServiceFactory;



  // check if requestUrl requires authentication token
  function endpointRequiresToken(requestUrl) {

    // do not need to authenticate these urls
    var authorizedApiUrls = [
      '/authen',
      '/catalog/divisions'
    ];

    var isSecure = true;
    angular.forEach(authorizedApiUrls, function(url, index) {
      // checks if requestUrl starts with one of the authorizedApiUrls
      if (requestUrl.indexOf(url) === 0) {
        isSecure = false;
      }
    });

    return isSecure;
  }
}]);