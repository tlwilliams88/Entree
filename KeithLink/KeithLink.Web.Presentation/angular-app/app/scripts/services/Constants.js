'use strict';

angular.module('bekApp')
  .constant('Constants', {

    localStorage : {
      userProfile: 'userProfile',
      userToken: 'userToken',
      branchId: 'branchId',
      customerNumber: 'customerNumber',
      leadGenInfo: 'leadGenInfo',
      currentCustomer: 'currentCustomer',
      tempContext: 'tempContext',
      tempBranch: 'tempBranch',
      lastList: 'lastList',
      lastOrderList: 'lastOrderList',
      pageSize: 'pageSize',
      defaultSort: 'defaultSort'
    },

    offlineLocalStorage: {
      labels: 'labels',
      deletedListGuids: 'deletedListGuids',
      shipDates: 'shipDates',
      deletedCartGuids: 'deletedCartGuids'
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
      POWER_USER: 'poweruser',
      BRANCH_MANAGER: 'branchismanager',
      KBIT_ADMIN: 'kbitadmin',
      MARKETING: 'marketing'
    },

    infiniteScrollPageSize: 50,
    promoItemsSize: 6
  });