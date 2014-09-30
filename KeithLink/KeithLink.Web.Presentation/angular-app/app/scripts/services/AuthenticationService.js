'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'UserProfileService', 'ENV', 'LocalStorage',
    function ($http, $q, UserProfileService, ENV, LocalStorage) {

    var Service = {

      authenticateUser: function(username, password) {

        var data = 'grant_type=password&username=' + username + '&password=' + password + '&api-key=' + ENV.apiKey; // move api-key here!

        var headers = { headers : {
            'Content-Type': 'application/x-www-form-urlencoded' 
          }
        };

        return $http.post('/authen', data, headers).then(function(response){
          var token = response.data;

          // set date time when token expires
          var expires_at = new Date();
          expires_at.setSeconds(expires_at.getSeconds() + token.expires_in);

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

      isValidToken: function() {
        var token = LocalStorage.getToken();
        var now = new Date();
        return (now < new Date(token.expires_at));
      }

    };
 
    return Service;

  }]);
