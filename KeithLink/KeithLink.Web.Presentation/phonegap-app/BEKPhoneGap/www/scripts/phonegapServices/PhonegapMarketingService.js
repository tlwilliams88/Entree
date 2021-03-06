'use strict';

angular.module('bekApp')
  .factory('PhonegapMarketingService', ['$http', '$q', 'MarketingService',
    function($http, $q, MarketingService) {

      var originalMarketingService = angular.copy(MarketingService);

      var Service = angular.extend(MarketingService, {});

      Service.getPromoItems = function() {
        if (navigator.connection.type === 'none') {
          return $q.reject('Offline: cannot load promotional items.');
        } else {
          return originalMarketingService.getPromoItems();
        }
      };

      return Service;

  }
]);