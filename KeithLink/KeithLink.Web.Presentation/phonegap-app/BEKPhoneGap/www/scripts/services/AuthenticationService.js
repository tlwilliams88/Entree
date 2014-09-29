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

        // var token = {
        //   access_token: "TmOeDkVBybstnlJFy22VBvgLea9D-j6H_XM_0fR395BImDm-oPxhVo3Aw-cfMbZUPs2EgMI4TETkMR2hoeKaeAEafyHlhelOOCqM63fBkf_2g9Nkf1BTlOXQ3opR5bpYiRHtCQBxi1jY2irxEpxyu1sbHC_hto7kTrWypOAGH5T6GlBN1mluGS1LpZr5CclUoePdZXjJU2QiO230FA-Kk03rMyziqL9PwVo5Od4fYwclGb31tRE8V7w15KM7kvcUUdVH-Q",
        //   token_type: "bearer",
        //   expires_in: 86399
        // };

        // return token;

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
