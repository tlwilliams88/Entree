'use strict';

angular.module('bekApp')
  .factory('PhonegapServices', ['PhonegapAuthenticationService', 'PhonegapCartService', 'PhonegapListService', 'PhonegapOrderService', 'PhonegapNotificationService', 'PhonegapMarketingService', 'PhonegapCustomerService',
    function(PhonegapAuthenticationService, PhonegapCartService, PhonegapListService, PhonegapOrderService, PhonegapNotificationService, PhonegapMarketingService, PhonegapCustomerService) {
      return {};
    }
  ]);