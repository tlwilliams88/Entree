'use strict';

angular.module('bekApp')
  .factory('AccessService', ['LocalStorage', 'Constants', 'SessionService',
    function (LocalStorage, Constants, SessionService) {

  function getRole() {
    return SessionService.userProfile.rolename;
  }

  // EXTERNAL
  function isOwner() {
    return ( getRole() === Constants.roles.OWNER );
  }
  function isAccounting() {
    return ( getRole() === Constants.roles.ACCOUNTING );
  }
  function isApprover() {
    return ( getRole() === Constants.roles.APPROVER );
  }
  function isBuyer() {
    return ( getRole() === Constants.roles.BUYER );
  }
  function isGuest() {
    return ( getRole() === Constants.roles.GUEST );
  }
  // INTERNAL
  function isSysAdmin() {
    return ( getRole() === Constants.roles.SYS_ADMIN );
  }
  function isBranchManager() {
    return ( getRole() === Constants.roles.BRANCH_MANAGER );
  }
  function isPowerUser() {
    return ( getRole() === Constants.roles.POWER_USER );
  }
  function isDsr() {
    return ( getRole() === Constants.roles.DSR );
  }
  function isDsm() {
    return ( getRole() === Constants.roles.DSM );
  }
  function isMarketing() {
    return ( getRole() === Constants.roles.MARKETING );
  }
  function isKbitAdmin() {
    return ( getRole() === Constants.roles.KBIT_ADMIN );
  }

  function isDemo() {
    return SessionService.userProfile.isdemo;
  }

  function isKeithNetCustomer() {
    return LocalStorage.getCurrentCustomer().customer.isKeithnetCustomer;
  }

  function isValidRole() {
    return ( isOwner() || isAccounting() || isApprover() || isBuyer() || isGuest() || isSysAdmin() || isBranchManager() || isPowerUser() || isDsr() || isDsm() || isKbitAdmin() || isMarketing() );
  }

  var Service = {

    clearLocalStorage: function() {
      LocalStorage.clearAll();
    },

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
        case Constants.roles.KBIT_ADMIN:
          displayRole = 'KBIT Admin';
          break;
      }
      return displayRole;
    },

    isValidToken: function() {
      var token = LocalStorage.getToken();
      var now = new Date();
      return (token && now < new Date(token.expires_at));
    },

    isLoggedIn: function() {
      return !!(SessionService.userProfile && Service.isValidToken() && isValidRole());
    },

    isPasswordExpired: function() {
        return (Service.isLoggedIn() && SessionService.userProfile.passwordexpired);
    },

    isOrderEntryCustomer: function() {
      return ( Service.isLoggedIn() && ( isOwner() || isAccounting() || isApprover() || isBuyer() || Service.isInternalAccountAdminUser() ) );
    },

    isInternalAccountAdminUser: function() {
      return ( Service.isLoggedIn() && ( isDsr() || isDsm() || isSysAdmin() || isKbitAdmin() || isBranchManager() || isPowerUser()) );
    },

    isInternalUser: function() {
      return ( Service.isLoggedIn() && ( isDsr() || isDsm() || isSysAdmin() || isKbitAdmin() || isBranchManager() || isPowerUser() || isMarketing() ) );
    },

    // PRIVILEDGES

    canBrowseCatalog: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() || isMarketing() || isGuest() );
    },

    canSeePrices: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() );
    },

    canManageLists: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() );
    },

    canViewOrders: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner()  || isApprover() || isBuyer() );
    },

    canCreateOrders: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner()  || isApprover() || isBuyer() ) && isKeithNetCustomer();
    },

    canSubmitOrders: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isApprover() && isKeithNetCustomer());
    },

    canPayInvoices: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() );
    },

    canGrantAccessToOtherServices: function() {
      return ( isSysAdmin() || isKbitAdmin() );
    },
	
	  canGrantAccessToKbit: function() {
      return ( isSysAdmin() || isKbitAdmin() || isBranchManager() );
    },
	
	  canGrantAccessToEmenuManage: function() {
      return ( isSysAdmin() );
    },

    canViewCustomerGroups: function() {
      return ( Service.isInternalAccountAdminUser() );
    },

    // create, delete, assign customers, add admin users
    canManageCustomerGroups: function() {
      return ( isSysAdmin() || isKbitAdmin() || isBranchManager() );
    },

    canViewCustomerGroupDashboard: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() );
    },

    // moving a user with customers to a different customer group
    canMoveUserToAnotherGroup: function() {
      return ( isSysAdmin() );
    },

    // changing user info and role on the edit user details page
    canEditUsers: function() {
      return ( isSysAdmin() || isKbitAdmin() || isBranchManager() || isOwner() );
    },
    
    // editing DSR Aliases
    canEditInternalUsers: function() {
      return ( isSysAdmin() || isBranchManager() );
    },

    canViewMarketing: function() {
      return ( isSysAdmin() || isMarketing() );
    },

    isDemo: function() {
      return isDemo();
    },

    //used primarily for Inventory Valuation and Item Usage Reports.  KBIT access is handled differently.  
    canRunReports: function() {
      return ( Service.isInternalAccountAdminUser() || isOwner() || isAccounting() || isApprover() || isBuyer() );
    }

  };

  return Service;

}]);
