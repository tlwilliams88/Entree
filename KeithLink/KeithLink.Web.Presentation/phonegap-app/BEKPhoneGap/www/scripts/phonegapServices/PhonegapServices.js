'use strict';

angular.module('bekApp')
    .factory('PhonegapServices', ['PhonegapAuthenticationService', 'PhonegapCartService', 'PhonegapListService', 'PhonegapOrderService',
        function(PhonegapAuthenticationService, PhonegapCartService, PhonegapListService, PhonegapOrderService) {
            return {};
        }
    ]);