'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'ENV', 'LocalStorage',
    function ($http, $q, ENV, LocalStorage) {

    var Service = {

      authenticateUser: function(username, password) {

        // must use query string, authen request does not work if sending data as a JSON object
        var data = 'grant_type=password&username=' + username + '&password=' + password + '&apiKey=' + ENV.apiKey + '&message=Loading...';

        var headers = { headers : {
            'Content-Type': 'application/x-www-form-urlencoded' 
          }
        };

        return $http.post('/authen', data, headers).then(function(response){
          var token = response.data;

          // calculate date time when token expires
          var expires_at = new Date();
          expires_at.setSeconds(expires_at.getSeconds() + token.expires_in);

          // save token in local storage
          token.expires_at = expires_at;
          LocalStorage.setToken(token);
          return username;
        }, function(error) {
          var errorMessage = 'Error Authenticating User';
          if (error && error.data) {
            errorMessage = error.data.error_description; 
          }
          return $q.reject(errorMessage);
        });
      },

      login: function(username, password) {
        return Service.authenticateUser(username, password);
      },

      logout: function() {
        LocalStorage.clearAll();
      }

    };
 
    return Service;

  }]);
