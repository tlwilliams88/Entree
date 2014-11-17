'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:MarketingService
 * @description
 * # MarketingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('MarketingService', [ 'Marketing', function (Marketing) {

  var Service = {
  
    createItem: function(contentItem) {
      return Marketing.save(contentItem).$promise;
    }

  };

    return Service;

  }]);
