'use strict';

angular.module('bekApp')
    .factory('PhonegapServices', ['PhonegapAuthenticationService', 'PhonegapCartService', 'PhonegapListService',
        function(PhonegapAuthenticationService, PhonegapCartService, PhonegapListService) {
            return {};
        }
    ]);