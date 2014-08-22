'use strict';

angular.module('bekApp')
  .factory('AccessService', ['localStorageService', 'Constants', 'AuthenticationService', 'UserProfileService', 
    function (localStorageService, Constants, AuthenticationService, UserProfileService) {

    var Service = {

      isLoggedIn: function() {
        return !!(AuthenticationService.getToken() && UserProfileService.profile());
      },

      isOrderEntryCustomer: function() {
        return ( Service.isLoggedIn() && ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isShopper() ) );
      },

      // ROLES

      isOwner: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.OWNER );
      },

      isAccounting: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.ACCOUNTING );
      },

      isApprover: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.APPROVER );
      },

      isShopper: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.SHOPPER );
      },

      isUser: function() {
        return ( UserProfileService.getCurrentRole() === Constants.roles.USER );
      },

      // PRIVILEDGES

      canBrowseCatalog: function() {
        return ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isShopper() || Service.isUser() );
      },

      canManageLists: function() {
        return ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isShopper() || Service.isUser() );
      },

      canCreateOrders: function() {
        return ( Service.isOwner()  || Service.isApprover() || Service.isShopper() );
      },

      canSubmitOrders: function() {
        return ( Service.isOwner() || Service.isApprover() );
      },

      canPayInvoices: function() {
        return ( Service.isOwner() || Service.isAccounting() );
      },

      canManageAccount: function() {
        return ( Service.isOwner() );
      },

      canManageeMenu: function() {
        return ( Service.isOwner() );
      }

    };
 
    return Service;

  }]);
