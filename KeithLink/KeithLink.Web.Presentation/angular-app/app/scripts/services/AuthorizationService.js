'use strict';

angular.module('bekApp')
  .factory('AuthorizationService', ['localStorageService', 'Constants', 'AuthenticationService', 'UserProfileService', 
    function (localStorageService, Constants, AuthenticationService, UserProfileService) {

    var Service = {

      isLoggedIn: function() {
        return !!(AuthenticationService.getToken());
      },

      isCustomer: function() {
        return false;
      },

      // ROLES

      isOwner: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.OWNER );
      },

      isAccounting: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.ACCOUNTING );
      },

      isPurchasing: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.PURCHASING );
      },

      isUser: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.USER );
      },

      // PRIVILEDGES

      canBrowseCatalog: function() {
        return ( Service.isOwner() || Service.isAccounting() || Service.isPurchasing() || Service.isUser() );
      },

      canManageLists: function() {
        return ( Service.isOwner() || Service.isAccounting() || Service.isPurchasing() || Service.isUser() );
      },

      canPlaceOrder: function() {
        return ( Service.isOwner() || Service.isPurchasing() );
      },

      canPayInvoices: function() {
        return ( Service.isOwner() || Service.isAccounting() );
      },

      canManageAccount: function() {
        return ( Service.isOwner() );
      }

    };
 
    return Service;

  }]);
