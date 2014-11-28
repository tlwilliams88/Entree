'use strict';

angular.module('bekApp')
  .constant('Constants', {

    localStorage : {
      userProfile: 'userProfile',
      userToken: 'userToken',
      branchId: 'branchId',
      customerNumber: 'customerNumber',
      leadGenInfo: 'leadGenInfo',
      currentCustomer: 'currentCustomer'
    },

    servicelocatorUrl: '../servicelocator',

    roles: {
      // external
      OWNER: 'owner',
      ACCOUNTING: 'Accounting',
      APPROVER: 'Approver',
      BUYER: 'Buyer',
      GUEST: 'guest',

      // internal
      DSR: 'dsr',
      SYS_ADMIN: 'beksysadmin',
      BRANCH_MANAGER: 'branchismanager'
    },

    infiniteScrollPageSize: 50,
    promoItemsSize: 6
  });