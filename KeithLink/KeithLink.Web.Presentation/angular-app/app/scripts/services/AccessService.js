'use strict';

angular.module('bekApp')
  .factory('AccessService', ['LocalStorage', 'Constants',
    function (LocalStorage, Constants) {

  // validates the token is not expired
  function isValidToken() {
    var token = LocalStorage.getToken();
    var now = new Date();
    return (now < new Date(token.expires_at));
  }

  var Service = {

    isLoggedIn: function() {
      return !!(LocalStorage.getToken() && LocalStorage.getProfile() && isValidToken());
    },

    isPasswordExpired: function() {
        return (Service.isLoggedIn() && LocalStorage.getProfile().passwordexpired);
    },

    isOrderEntryCustomer: function() {
      return ( Service.isLoggedIn() && ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() || Service.isInternalAccountAdminUser() ) );
    },

    isInternalAccountAdminUser: function() {
      return ( Service.isLoggedIn() && ( Service.isDsr() || Service.isDsm() || Service.isSysAdmin() || Service.isBranchManager() ) );
    },

    // ROLES

    // EXTERNAL
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

    // INTERNAL
    isSysAdmin: function() {
      return ( LocalStorage.getUserRole() === Constants.roles.SYS_ADMIN );
    },

    isBranchManager: function() {
      return ( LocalStorage.getUserRole() === Constants.roles.BRANCH_MANAGER );
    },

    isDsr: function() {
      return ( LocalStorage.getUserRole() === Constants.roles.DSR );
    },

    isDsm: function() {
      return ( LocalStorage.getUserRole() === Constants.roles.DSM );
    },


    // PRIVILEDGES

    canBrowseCatalog: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() || Service.isGuest() );
    },

    canSeePrices: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() );
    },

    canManageLists: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() );
    },

    canCreateOrders: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner()  || Service.isApprover() || Service.isBuyer() );
    },

    canSubmitOrders: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner() || Service.isApprover() );
    },

    canPayInvoices: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner() || Service.isAccounting() );
    },

    canManageAccount: function() {
      return ( Service.isInternalAccountAdminUser() || Service.isOwner() );
    },

    canManageAccounts: function() {
      return ( Service.isInternalAccountAdminUser() );
    }

  };

  return Service;

}]);
