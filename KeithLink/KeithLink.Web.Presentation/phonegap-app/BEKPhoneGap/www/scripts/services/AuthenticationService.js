'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', ['$http', '$q', 'localStorageService', 'Constants', 'UserProfileService', 'ListService',
    function ($http, $q, localStorageService, Constants, UserProfileService, ListService) {

    var Service = {

      authenticateUser: function(username, password) {

        var data = 'grant_type=password&username=' + username + '&password=' + password;

        var headers = { headers : {
            'Content-Type': 'application/x-www-form-urlencoded' 
          }
        };

        return $http.post('/authen' , data, headers).then(function(response){
          var token = response.data;
          Service.setToken(token);
          return token;
        });
      },

      login: function(username, password) {
        return Service.authenticateUser(username, password).then(function(token) {
          return UserProfileService.getProfile(username).then(function(profile) {
            return profile;
          }, function(error) {
            AuthenticationService.logout();
          });
        });
      },

      logout: function() {
        localStorageService.remove(Constants.localStorage.userProfile);
        localStorageService.remove(Constants.localStorage.userToken);
      },

      getToken: function() {
        return localStorageService.get(Constants.localStorage.userToken);
      },

      setToken: function(token) {
        localStorageService.set(Constants.localStorage.userToken, token);
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
