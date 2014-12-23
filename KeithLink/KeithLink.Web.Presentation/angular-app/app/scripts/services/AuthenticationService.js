'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'UserProfileService', 'ENV', 'LocalStorage',
    function ($http, $q, UserProfileService, ENV, LocalStorage) {

    var Service = {

      authenticateUser: function(username, password) {

        // must use query string, authen request does not work if sending data as a JSON object
        var data = 'grant_type=password&username=' + username + '&password=' + password + '&apiKey=' + ENV.apiKey;

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
          return token;
        });
      },

      login: function(username, password) {
        return Service.authenticateUser(username, password).then(function(token) {
          return UserProfileService.getProfile(username).then(function(profile) {
            return profile;
          }, function(error) {
            Service.logout();
          });
        });
      },

      logout: function() {
        LocalStorage.clearAll();
      },

      // validates the token is not expired
      isValidToken: function() {
        var token = LocalStorage.getToken();
        var now = new Date();
        return (now < new Date(token.expires_at));
      }

    };
 
    return Service;

  }]);
