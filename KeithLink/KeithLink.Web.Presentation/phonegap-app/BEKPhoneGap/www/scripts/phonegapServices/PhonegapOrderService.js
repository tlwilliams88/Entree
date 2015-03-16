'use strict';

angular.module('bekApp')
  .factory('PhonegapOrderService', ['$http', '$q', 'OrderService',
    function($http, $q, OrderService) {

      var originalOrderService = angular.copy(OrderService);

      var Service = angular.extend(OrderService, {});

      Service.getChangeOrders = function() {
        if (navigator.connection.type === 'none') {
          console.log('getting change orders from DB');            
          var deferred = $q.defer();
          deferred.resolve([]);
          return deferred.promise;
        } else {
          console.log('getting all change orders from server');
          return originalOrderService.getChangeOrders();
        }
      };

      return Service;

  }
]);