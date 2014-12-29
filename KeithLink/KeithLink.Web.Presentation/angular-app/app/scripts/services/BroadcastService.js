'use strict';

angular.module('bekApp')
  .factory('BroadcastService', ['$http', '$q', function($http, $q){
    var Service = {
      broadcastMessage: function(messagePayload){
        var deferred = $q.defer();
        $http.post('/messaging/mail', messagePayload).then(function(response) {
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
