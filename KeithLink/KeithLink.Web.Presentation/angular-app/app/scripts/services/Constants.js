'use strict';

angular.module('bekApp')
	.value('Constants', {
		
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
      PURCHASING: 'Purchasing',
      USER: 'User'
    }
	});