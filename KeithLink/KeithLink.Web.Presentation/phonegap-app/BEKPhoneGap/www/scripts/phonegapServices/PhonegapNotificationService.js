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
      };

      Service.getMessages = function(params) {
        if (navigator.connection.type === 'none') {
          return $q.reject('Offline: cannot load recent activity.');
        } else {
          return originalNotificationService.getMessages(params);
        }
      };

      return Service;

    }
  ]);
