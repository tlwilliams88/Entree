'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:MarketingService
 * @description
 * # MarketingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('MarketingService', ['$http', '$q', 'LocalStorage', 'Constants', 'UtilityService', 'ExportService', function ($http, $q, LocalStorage, Constants, UtilityService, ExportService) {

  var Service = {
  
    getPromoItems: function() {
      var promise = $http.get('/cms/promoitems/' + LocalStorage.getBranchId());
      return UtilityService.resolvePromise(promise).then(function(data) {
        return data;
      }, function() {
        return $q.reject('Error loading promo items.');
      });
    },

    /**
     * Gets array of users who registered for the site
     * @param  {String} fromDate String date in the format YYYY-MM-DD for the start of the date range
     * @param  {String} toDate   String date in the format YYYY-MM-DD for the end of the date range
     * @return {Promise}          
     */
    getUsersAndMarketingInfo: function(fromDate, toDate) {
      var data = { 
        params: {
          from: fromDate,
          to: toDate
        }
      };
      return $http.get('/profile/marketinginfo', data).then(function(response) {
        return response.data;
      });
    },

    getMarketingInfoExportConfig: function() {
      return $http.get('/profile/export/marketinginfo').then(function(response) {
        return response.data;
      });
    }
  };

  return Service;

}]);
