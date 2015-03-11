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

  // ROLES

  // EXTERNAL
  function isOwner() {
    return ( LocalStorage.getUserRole() === Constants.roles.OWNER );
  }
  function isAccounting() {
    return ( LocalStorage.getUserRole() === Constants.roles.ACCOUNTING );
  }
  function isApprover() {
    return ( LocalStorage.getUserRole() === Constants.roles.APPROVER );
  }
  function isBuyer() {
    return ( LocalStorage.getUserRole() === Constants.roles.BUYER );
  }
  function isGuest() {
    return ( LocalStorage.getUserRole() === Constants.roles.GUEST );
  }
  // INTERNAL
  function isSysAdmin() {
    return ( LocalStorage.getUserRole() === Constants.roles.SYS_ADMIN );
  }
  function isBranchManager() {
    return ( LocalStorage.getUserRole() === Constants.roles.BRANCH_MANAGER );
  }
  function isPowerUser() {
    return ( LocalStorage.getUserRole() === Constants.roles.POWER_USER );
  }
  function isDsr() {
    return ( LocalStorage.getUserRole() === Constants.roles.DSR );
  }
  function isDsm() {
    return ( LocalStorage.getUserRole() === Constants.roles.DSM );
  }

  var Service = {

    getRoleDisplayString: function(role) {
      var displayRole = role;
      switch (role) {
        case Constants.roles.DSR:
          displayRole = 'DSR';
          break;
        case Constants.roles.DSM: 
          displayRole = 'DSM';
          break;
        case Constants.roles.BRANCH_MANAGER:
          displayRole = 'Branch IS Manager';
          break;
        case Constants.roles.POWER_USER:
          displayRole = 'Power User';
          break;
        case Constants.roles.SYS_ADMIN:
          displayRole = 'Sys Admin';
          break;
      }
      return displayRole;
    },

    isLoggedIn: function() {
      return !!(LocalStorage.getToken() && LocalStorage.getProfile() && isValidToken());
    },

    isPasswordExpired: function() {
        return (Service.isLoggedIn() && LocalStorage.getProfile().passwordexpired);
    },

    isOrderEntryCustomer: function() {
      return ( Service.isLoggedIn() && ( isOwner() || isAccounting() || isApprover() || isBuyer() || Service.isInternalAccountAdminUser() ) );
    },

    isInternalAccountAdminUser: function() {
      return ( Service.isLoggedIn() && ( isDsr() || isDsm() || isSysAdmin() || isBranchManager() || isPowerUser() ) );
    },

    // PRIVILEDGES

    canBrowseCatalog: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() || isGuest() );
    },

    canSeePrices: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() );
    },

    canManageLists: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() );
    },

    canCreateOrders: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner()  || isApprover() || isBuyer() );
    },

    canSubmitOrders: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isApprover() );
    },

    canPayInvoices: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() );
    },

    canGrantAccessToOtherServices: function() {
      return ( isSysAdmin() );
    },

    canViewCustomerGroups: function() {
      return ( Service.isInternalAccountAdminUser() );
    },

    canManageCustomerGroups: function() {
      return ( isSysAdmin() || isBranchManager() );
    },

    canViewCustomerGroupDashboard: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() );
    },

    canEditUsers: function() {
      return ( isSysAdmin() || isBranchManager() || isOwner() );
    }

  };

  return Service;

}]);
