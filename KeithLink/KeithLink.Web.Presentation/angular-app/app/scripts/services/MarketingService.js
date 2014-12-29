'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:MarketingService
 * @description
 * # MarketingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('MarketingService', ['$http', 'LocalStorage', 'Constants', 'UtilityService', function ($http, LocalStorage, Constants, UtilityService) {

  var Service = {
  
    getPromoItems: function() {
      var promise = $http.get('/cms/promoitems/' + LocalStorage.getBranchId() + '/' + Constants.promoItemsSize);
      return UtilityService.resolvePromise(promise);
    }

  };

  return Service;

}]);
