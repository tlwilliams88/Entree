'use strict';

angular.module('bekApp')
  .factory('AccessService', ['localStorageService', 'Constants', 'AuthenticationService', 'UserProfileService', 
    function (localStorageService, Constants, AuthenticationService, UserProfileService) {

    var Service = {

      isLoggedIn: function() {
        return !!(AuthenticationService.getToken() && UserProfileService.profile() && AuthenticationService.isValidToken());
      },

      isOrderEntryCustomer: function() {
        return ( Service.isLoggedIn() && ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() ) );
      },

      // ROLES

      isOwner: function() {
        return ( UserProfileService.getUserRole() === Constants.roles.OWNER );
      },

      isAccounting: function() {
        return ( UserProfileService.getUserRole() === Constants.roles.ACCOUNTING );
      },

      isApprover: function() {
        return ( UserProfileService.getUserRole() === Constants.roles.APPROVER );
      },

      isBuyer: function() {
        return ( UserProfileService.getUserRole() === Constants.roles.BUYER );
      },

      isUser: function() {
        return ( UserProfileService.getUserRole() === Constants.roles.GUEST );
      },

      // PRIVILEDGES

      canBrowseCatalog: function() {
        return ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() || Service.isUser() );
      },

      canManageLists: function() {
        return ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() || Service.isUser() );
      },

      canCreateOrders: function() {
        return ( Service.isOwner()  || Service.isApprover() || Service.isBuyer() );
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
