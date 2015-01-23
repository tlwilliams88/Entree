'use strict';

angular.module('bekApp')
  .factory('PhonegapAuthenticationService', ['$http', '$q', 'AuthenticationService', 'UserProfileService', 'localStorageService', 'ListService', 'CartService',
      function($http, $q, AuthenticationService, UserProfileService, localStorageService, ListService, CartService) {
       
        var Service = angular.extend(AuthenticationService, {})

        Service.login = function(username, password) {
          return Service.authenticateUser(username, password)
            .then(UserProfileService.getCurrentUserProfile)
            .then(function(profile) {
              return $q.all(
                  ListService.getAllLists(),
                  ListService.getAllLabels(),
                  CartService.getAllCarts(),
                  CartService.getShipDates()
              ).then(function(results) {
                      var lists = results[0];
                      var labels = results[1];
                      var carts = results[2];
                      var shipDates = results[3];
                      localStorageService.set('lists', lists);
                      localStorageService.set('labels', labels);
                      localStorageService.set('carts', carts);
                      localStorageService.set('shipDates', shipDates);
                      return profile;
                  },
                  function(error) {
                      console.log(error);
                      Service.logout();
                  });
            });
        }

        return Service;
    }
  ]);