'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:MarketingService
 * @description
 * # MarketingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('MarketingService', ['$http', '$q', 'LocalStorage', 'Constants', 'UtilityService', function ($http, $q, LocalStorage, Constants, UtilityService) {

  var Service = {
  
    getPromoItems: function() {
      var promise = $http.get('/cms/promoitems/' + LocalStorage.getBranchId() + '/' + Constants.promoItemsSize);
      return UtilityService.resolvePromise(promise).then(function(data) {
        return data;
      }, function() {
        return $q.reject('Error loading promo items.');
      });
    }

  };

  return Service;

}]);
