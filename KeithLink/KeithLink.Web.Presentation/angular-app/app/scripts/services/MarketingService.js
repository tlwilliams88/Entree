'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:MarketingService
 * @description
 * # MarketingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('MarketingService', [ '$q', '$http', 'LocalStorage', 'Constants', function ($q, $http, LocalStorage, Constants) {

  var Service = {
  
    getPromoItems: function() {
      var deferred = $q.defer();
      $http.get('/cms/promoitems/' + LocalStorage.getBranchId() + '/' + Constants.promoItemsSize).then(function(response) {
        var data = response.data;
        if (data.successResponse) {
          deferred.resolve(data.successResponse);
        } else {
          deferred.reject(data.errorMessage);
        }
      });
      return deferred.promise;
    }

  };

    return Service;

  }]);
