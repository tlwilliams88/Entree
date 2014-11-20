'use strict';

angular.module('bekApp')
    .factory('PhonegapServices', ['PhonegapAuthenticationService', 'PhonegapCartService', 'PhonegapListService', 'PhonegapPushService',
        function(PhonegapAuthenticationService, PhonegapCartService, PhonegapListService, PhonegapPushService) {
            return {};
        }
    ]);