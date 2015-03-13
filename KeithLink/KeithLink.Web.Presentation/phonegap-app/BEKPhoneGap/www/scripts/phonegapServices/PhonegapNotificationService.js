'use strict';

angular.module('bekApp')
  .factory('PhonegapNotificationService', ['$http', '$q', 'NotificationService',
    function($http, $q, NotificationService) {

      var originalNotificationService = angular.copy(NotificationService);

      var Service = angular.extend(NotificationService, {});

      Service.getUnreadMessageCount = function() {
        if (navigator.connection.type === 'none') {
          var deferred = $q.defer();
          deferred.resolve(0);
          return deferred.promise;
        } else {
          return originalNotificationService.getUnreadMessageCount();
        }
      }

      return Service;

    }
  ]);
