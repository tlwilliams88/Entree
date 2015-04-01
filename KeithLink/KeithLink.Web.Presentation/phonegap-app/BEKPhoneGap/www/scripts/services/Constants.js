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
      tempContext: 'tempContext'
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
      KBIT_ADMIN: 'kbitadmin'
    },

    infiniteScrollPageSize: 50,
    promoItemsSize: 6
  });