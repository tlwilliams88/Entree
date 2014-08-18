'use strict';

angular.module('bekApp')
	.value('Constants', {
		
		localStorage : {
			userProfile: 'userProfile',
			userToken: 'userToken',
			currentLocation: 'currentLocation'
		},

		servicelocatorUrl: '../servicelocator'
	});