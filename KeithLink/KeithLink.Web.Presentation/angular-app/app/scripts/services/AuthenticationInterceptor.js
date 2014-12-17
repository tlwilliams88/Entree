'use strict';

angular.module('bekApp')
.factory('AuthenticationInterceptor', ['$q', '$location', '$log', 'ENV', 'LocalStorage',
  function ($q, $location, $log, ENV, LocalStorage) {

  var authInterceptorServiceFactory = {
    request: function (config) {

      // do not alter requests for html templates, all api requests start with a '/'
      if (config.url.indexOf('/') === 0) {
        config.headers = config.headers || {};

        // add authorization token header if token is present and endpoint requires authorization
        var authData = LocalStorage.getToken();
        if (authData) {
          var urlsWithoutToken = ['/authen', '/catalog/divisions'];
          if (doesUrlRequireHeader(config.url, urlsWithoutToken)) {
            config.headers.Authorization = 'Bearer ' + authData.access_token;
          }
        }

        // add api key to request headers
        var urlsWithoutApiKey = ['/authen'];
        if (doesUrlRequireHeader(config.url, urlsWithoutApiKey)) {
          config.headers.apiKey = ENV.apiKey;
        }

        // add branch and customer information header
        var urlsWithoutCustomerInfo = ['/profile/users', '/profile', '/authen', '/profile/customer'];
        if (doesUrlRequireHeader(config.url, urlsWithoutCustomerInfo)) {
          var catalogInfo = {
            customerid: LocalStorage.getCustomerNumber(), //'020348', //
            branchid: LocalStorage.getBranchId()
          };
          config.headers.userSelectedContext = JSON.stringify(catalogInfo);
        }


        // add api url to request url
        config.url = ENV.apiEndpoint + config.url;
        $log.debug(config.url);
      }

      return config;
    },

     responseError: function (rejection) {
      // log user out if token has expired or they don't have access to a url
      // should we log the user out if they tried to hit an endpoint they don't have access? I think it should go to the homepage
      if (rejection.status === 401) {
        LocalStorage.clearAll();
        $location.path('/register');
      }

      // force hard refresh if api key is outdated
      // if (rejection.message === 'Invalid Api') {
      //   window.location.href = "http://localhost:9000";
      // }

      return $q.reject(rejection);
    }

  };

  function doesUrlRequireHeader(url, invalidUrls) {
    return invalidUrls.indexOf(url) === -1;
  }

  return authInterceptorServiceFactory;
}]);
