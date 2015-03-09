'use strict';

angular.module('bekApp')
.factory('PhonegapAuthenticationService', ['$http', '$q', 'AuthenticationService', 'UserProfileService', 'PhonegapDbService', 'PhonegapLocalStorageService', 'ListService', 'CartService',
  function($http, $q, AuthenticationService, UserProfileService, PhonegapDbService, PhonegapLocalStorageService, ListService, CartService) {

    var Service = angular.extend(AuthenticationService, {})

    var db_table_name_lists = 'lists',
      db_table_name_labels = 'labels';

    function saveAllLists(lists) {
      lists.forEach(function(list) {
        PhonegapDbService.setItem(db_table_name_lists, list.listid, list);
      });
    }
    function clearTables(profile) {
      return $q.all([
        PhonegapDbService.dropTable(db_table_name_lists),
        PhonegapDbService.dropTable(db_table_name_labels)
      ]).then(function() {
        return profile;
      });
    }

    Service.login = function(username, password) {
      return Service.authenticateUser(username, password)
      .then(UserProfileService.getCurrentUserProfile)
      .then(clearTables)
      .then(function(profile) {
        return $q.all([
          ListService.getAllLists(),
          ListService.getAllLabels()//,
          // CartService.getAllCarts(),
          // CartService.getShipDates()
          ]).then(function(results) {
            var lists = results[0];
            var labels = results[1];
            var carts = results[2];
            var shipDates = results[3];

            debugger;
            saveAllLists(lists);
            PhonegapLocalStorageService.setLabels(labels);
            
            // localStorageService.set('carts', carts);
            // localStorageService.set('shipDates', shipDates);
            return profile;
          },
          function(error) {
            console.log(error);
            Service.logout();
          });
        });
    };

    // Service.logout = function() {
    //   LocalStorage.clearAll();
    //   PhonegapDbService.dropDatabase();
    // };

    return Service;
  }
  ]);