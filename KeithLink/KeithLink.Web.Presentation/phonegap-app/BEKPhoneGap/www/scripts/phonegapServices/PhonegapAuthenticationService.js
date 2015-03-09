'use strict';

angular.module('bekApp')
.factory('PhonegapAuthenticationService', ['$http', '$q', 'AuthenticationService', 'UserProfileService', 'PhonegapDbService', 'PhonegapLocalStorageService', 'ListService', 'CartService',
  function($http, $q, AuthenticationService, UserProfileService, PhonegapDbService, PhonegapLocalStorageService, ListService, CartService) {

    var Service = angular.extend(AuthenticationService, {})

    // Service.logout = function() {
    //   LocalStorage.clearAll();
    //   PhonegapDbService.dropDatabase();
    // };

    return Service;
  }
  ]);