'use strict';

angular.module('bekApp')
  .factory('ConfigSettingsService', [ 'ConfigSettings', function (ConfigSettings) {

  var Service = {

    getAppSettings: function() {
      return ConfigSettings.get({}).$promise.then(function(resp){
        return resp.successResponse;
      });
    },

    saveAppSettings: function(setting) {
      return ConfigSettings.update(setting).$promise.then(function(resp){
        return resp.successResponse;
      });
    },

  };

  return Service;

}]);