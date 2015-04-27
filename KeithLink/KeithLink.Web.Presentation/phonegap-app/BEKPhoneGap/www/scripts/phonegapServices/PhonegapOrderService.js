'use strict';

angular.module('bekApp')
  .factory('PhonegapOrderService', ['$http', '$q', '$log', 'OrderService',
    function($http, $q, $log, OrderService) {

      var originalOrderService = angular.copy(OrderService);

      var Service = angular.extend(OrderService, {});

      Service.getChangeOrders = function() {
        if (navigator.connection.type === 'none') {
          $log.debug('getting change orders from DB');            
          var deferred = $q.defer();
          deferred.resolve([]);
          return deferred.promise;
        } else {
          $log.debug('getting all change orders from server');
          return originalOrderService.getChangeOrders();
        }
      };

      Service.getOrders = function (params) {
        if (navigator.connection.type === 'none') {
          return $q.reject('Offline: cannot load orders.');
        } else {
          return originalOrderService.getOrders(params);
        }
      };

      return Service;

  }
]);