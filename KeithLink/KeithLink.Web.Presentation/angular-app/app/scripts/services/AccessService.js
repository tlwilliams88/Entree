'use strict';

angular.module('bekApp')
  .factory('AccessService', ['AuthenticationService', 'LocalStorage', 'Constants',
    function (AuthenticationService, LocalStorage, Constants) {

    var Service = {

      isLoggedIn: function() {
        return !!(LocalStorage.getToken() && LocalStorage.getProfile() && AuthenticationService.isValidToken());
      },

      isOrderEntryCustomer: function() {
        return ( Service.isLoggedIn() && ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() || Service.isDsr() ) );
      },

      isInternalUser: function() {
        return ( Service.isLoggedIn() && ( Service.isDsr() ) );
      },

      // ROLES

      isOwner: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.OWNER );
      },

      isAccounting: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.ACCOUNTING );
      },

      isApprover: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.APPROVER );
      },

      isBuyer: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.BUYER );
      },

      isGuest: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.GUEST );
      },

      isSysAdmin: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.SYS_ADMIN );
      },

      isBranchManager: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.BRANCH_MANAGER );
      },

      isDsr: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.DSR );
      },

      // PRIVILEDGES

      canBrowseCatalog: function() {
        return ( Service.isDsr() || Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() || Service.isGuest() );
      },

      canSeePrices: function() {
        return ( Service.isDsr() || Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() );
      },

      canManageLists: function() {
        return ( Service.isDsr() || Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() );
      },

      canCreateOrders: function() {
        return ( Service.isDsr() || Service.isOwner()  || Service.isApprover() || Service.isBuyer() );
      },

      canSubmitOrders: function() {
        return ( Service.isDsr() || Service.isOwner() || Service.isApprover() );
      },

      canPayInvoices: function() {
        return ( Service.isDsr() || Service.isOwner() || Service.isAccounting() );
      },

      canManageAccount: function() {
        return ( Service.isDsr() || Service.isOwner() );
      },

      canManageAccounts: function() {
        return ( true );
      }

    };
 
    return Service;

  }]);
