'use strict';

angular.module('bekApp')
  .factory('AccessService', ['AuthenticationService', 'LocalStorage', 'Constants',
    function (AuthenticationService, LocalStorage, Constants) {

    var Service = {

      isLoggedIn: function() {
        return !!(LocalStorage.getToken() && LocalStorage.getProfile() && AuthenticationService.isValidToken());
      },

      isOrderEntryCustomer: function() {
        return ( Service.isLoggedIn() && ( Service.isOwner() || Service.isAccounting() || Service.isApprover() || Service.isBuyer() ) );
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

      isUser: function() {
        return ( LocalStorage.getUserRole() === Constants.roles.GUEST );
      },

      isBekAdmin: function() {
        return false;
      },

      isCustomerAdmin: function() {
        return false;
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
