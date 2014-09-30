'use strict';

angular.module('bekApp')
  .constant('Constants', {

    localStorage : {
      userProfile: 'userProfile',
      userToken: 'userToken',
      currentLocation: 'currentLocation',
      branchId: 'branchId',
      customerNumber: 'customerNumber',
      leadGenInfo: 'leadGenInfo'
    },

    servicelocatorUrl: '../servicelocator',

    roles: {
      OWNER: 'owner',
      ACCOUNTING: 'Accounting',
      APPROVER: 'Approver',
      BUYER: 'Buyer',
      GUEST: 'guest'
    },

    infiniteScrollPageSize: 50
  });