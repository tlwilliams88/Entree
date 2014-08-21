'use strict';

angular.module('bekApp')
  .factory('AccessService', ['localStorageService', 'Constants', 'AuthenticationService', 'UserProfileService', 
    function (localStorageService, Constants, AuthenticationService, UserProfileService) {

    var Service = {

      isLoggedIn: function() {
        return !!(AuthenticationService.getToken());
      },

      isOrderEntryCustomer: function() {
        return ( Service.isLoggedIn() && ( Service.isOwner() || Service.isAccounting() || Service.isPurchasing() ) );
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

      canCreateOrders: function() {
        return ( Service.isOwner() || Service.isPurchasing() );
      },

      canSubmitOrders: function() {
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
