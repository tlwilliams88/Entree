'use strict';

angular.module('BEKPhoneGap')
  .constant('Constants', {

    localStorage : {
      userProfile: 'userProfile',
      userToken: 'userToken',
      currentLocation: 'currentLocation',
      leadGenInfo: 'leadGenInfo'
    },

    servicelocatorUrl: '../servicelocator',

    roles: {
      OWNER: 'Owner',
      ACCOUNTING: 'Accounting',
      APPROVER: 'Approver',
      SHOPPER: 'Shopper',
      USER: 'User'
    }
  });