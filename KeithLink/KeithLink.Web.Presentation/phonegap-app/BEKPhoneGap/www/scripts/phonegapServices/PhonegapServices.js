'use strict';

angular.module('bekApp')
    .factory('PhonegapServices', ['PhonegapAuthenticationService', 'PhonegapCartService', 'PhonegapListService', 'PhonegapOrderService', 'PhonegapNotificationService',
        function(PhonegapAuthenticationService, PhonegapCartService, PhonegapListService, PhonegapOrderService, PhonegapNotificationService) {
            return {};
        }
    ]);