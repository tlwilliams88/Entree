'use strict';

angular.module('bekApp')
  .factory('MessagePreferenceService', [ '$http', 'UtilityService', function ($http, UtilityService) {
    var Service = {
      updatePreferences: function(preferences){
        var promise = $http.put('/messaging/preferences', preferences);
        return UtilityService.resolvePromise(promise);
      }
    };

    return Service;
  }]);
