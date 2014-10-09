'use strict';

angular.module('bekApp')
.factory('AuthenticationInterceptor', ['$q', '$location', 'ENV', 'LocalStorage',
  function ($q, $location, ENV, LocalStorage) {

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
          config.headers['apiKey'] = ENV.apiKey;
        }

        // add branch and customer information header
        var urlsWithoutCustomerInfo = ['/profile', '/authen'];
        if (doesUrlRequireHeader(config.url, urlsWithoutCustomerInfo)) {
          var catalogInfo = {
            customerid: LocalStorage.getCustomerNumber(), //'020348', //
            branchid: LocalStorage.getBranchId()
          };
          config.headers['userSelectedContext'] = JSON.stringify(catalogInfo);
        }


        // add api url to request url
        config.url = ENV.apiEndpoint + config.url;
        console.log(config.url);
      }

      return config;
    },

     responseError: function (rejection) {
      if (rejection.status === 401) {
        LocalStorage.clearAll();
        $location.path('/register');
      }
      return $q.reject(rejection);
    }

  };

  function doesUrlRequireHeader(url, invalidUrls) {
    return invalidUrls.indexOf(url) === -1;
  }

  return authInterceptorServiceFactory;
}]);