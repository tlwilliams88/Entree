'use strict';

/**********
AuthenticationInterceptor

adds Authorization, apiKey, and userSelectedContext headers to http requests
***********/

angular.module('bekApp')
.factory('AuthenticationInterceptor', ['$q', '$location', '$log', 'ENV', 'LocalStorage', 'toaster',
  function ($q, $location, $log, ENV, LocalStorage, toaster) {

  var userWasLoggedOut = false;
  function logout(showMessage) {
    LocalStorage.clearAll();
    $location.path('/register');

    if (showMessage === true && userWasLoggedOut === false) {
      toaster.pop('error', null, 'An error occured: no branch ID was selected.');
      userWasLoggedOut = true;
    }
  }

  var authInterceptorServiceFactory = {
    request: function (config) {

      // do not alter requests for html templates, all api requests start with a '/'
      if (config.url.indexOf('/') === 0) {
        config.headers = config.headers || {};

        // Authorization - add authorization token header if token is present and endpoint requires authorization
        var authData = LocalStorage.getToken();
        if (authData) {
          var urlsWithoutToken = ['/authen', '/catalog/divisions'];
          if (doesUrlRequireHeader(config.url, urlsWithoutToken)) {
            config.headers.Authorization = 'Bearer ' + authData.access_token;
          }
        }

        // apiKey - add api key to request headers
        var urlsWithoutApiKey = ['/authen'];
        if (doesUrlRequireHeader(config.url, urlsWithoutApiKey)) {
          config.headers.apiKey = ENV.apiKey;
        }

        // userSelectedContext - add branch and customer information header based on the customer dropdown
        var urlsWithoutCustomerInfo = ['/profile/users', '/profile', '/authen', '/profile/customer'];
        if (doesUrlRequireHeader(config.url, urlsWithoutCustomerInfo)) {

          if (!LocalStorage.getBranchId()) {
            logout(true);
          } else {
            userWasLoggedOut = false;
          }

          var catalogInfo = {
            customerid: LocalStorage.getCustomerNumber(),
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
        logout();
      }

      // force hard refresh if api key is outdated
      // if (rejection.message === 'Invalid Api') {
      //   window.location.href = "http://localhost:9000";
      // }

      if (!rejection.config) {
        rejection.config = {};
      }

      return $q.reject(rejection);
    }

  };

  function doesUrlRequireHeader(url, invalidUrls) {
    return invalidUrls.indexOf(url) === -1;
  }

  return authInterceptorServiceFactory;
}]);
