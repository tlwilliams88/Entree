'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'localStorageService', 'Constants', 'UserProfileService', 'ENV',
    function ($http, $q, localStorageService, Constants, UserProfileService, ENV) {

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
          Service.setToken(token);
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
        localStorageService.remove(Constants.localStorage.userProfile);
        localStorageService.remove(Constants.localStorage.userToken);
        localStorageService.remove(Constants.localStorage.currentLocation);
        localStorageService.remove(Constants.localStorage.branchId);
        localStorageService.remove(Constants.localStorage.customerNumber);
      },

      getToken: function() {
        return localStorageService.get(Constants.localStorage.userToken);
      },

      setToken: function(token) {
        localStorageService.set(Constants.localStorage.userToken, token);
      },

      isValidToken: function() {
        var token = Service.getToken();
        var now = new Date();
        return (now < new Date(token.expires_at));
      },

      getLeadGenInfo: function() {
        return localStorageService.set(Constants.localStorage.leadGenInfo);
      },

      setLeadGenInfo: function(leadGenInfo) {
        localStorageService.set(Constants.localStorage.leadGenInfo, leadGenInfo);
      }

    };
 
    return Service;

  }]);
