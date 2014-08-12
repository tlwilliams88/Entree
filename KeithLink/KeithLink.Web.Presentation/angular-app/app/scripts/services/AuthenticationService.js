'use strict';

angular.module('bekApp')
  .factory('AuthenticationService', function ($http) {
    
    var categories;
 
    var Service = {
      login: function(email, password) {
        var params = {
          email: 'sabroussard@somecompany.com',
          password: 'L1ttleStev1e'
        }

        categories = $http.get('/profile/login').then(function (response) {
          debugger;
          return response.data;
        });
      }
  };
 
    return Service;

  });
