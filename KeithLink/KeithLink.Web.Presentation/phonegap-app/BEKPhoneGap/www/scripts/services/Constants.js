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

    servicelocatorUrl: '../servicelocator', // DEPRECATED

    roles: {
      // external
      OWNER: 'owner',
      ACCOUNTING: 'accounting',
      APPROVER: 'approver',
      BUYER: 'buyer',
      GUEST: 'guest',

      // internal
      DSR: 'dsr',
      DSM: 'dsm',
      SYS_ADMIN: 'beksysadmin',
      BRANCH_MANAGER: 'branchismanager'
    },

    infiniteScrollPageSize: 50,
    promoItemsSize: 6
  });