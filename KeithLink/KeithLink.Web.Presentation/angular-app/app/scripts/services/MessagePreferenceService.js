'use strict';

angular.module('bekApp')
  .factory('MessagePreferenceService', [ '$http', '$q', '$log', function ($http, $q, $log) {
    var Service = {
      updatePreferences: function(preferences){
        /*var deferred = $q.defer();

        $http.put('/messagingpreferences', preferences).then(function(response) {

          var data = response.data;

          if (data.successResponse) {
            $log.debug(data.successResponse);
            deferred.resolve(data.successResponse);
          } else {
            deferred.reject(data.errorMessage);
          }
        });
        return deferred.promise;*/
        console.log(preferences);
      }
    };

    return Service;
  }]);
