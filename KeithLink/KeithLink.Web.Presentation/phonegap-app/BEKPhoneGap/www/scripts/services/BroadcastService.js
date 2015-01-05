'use strict';

angular.module('bekApp')
  .factory('BroadcastService', ['$http', 'UtilityService', function($http, UtilityService){
    var Service = {
      broadcastMessage: function(messagePayload){
        var promise = $http.post('/messaging/mail', messagePayload);
        return UtilityService.resolvePromise(promise);
      }
    };

    return Service;
  }]);
