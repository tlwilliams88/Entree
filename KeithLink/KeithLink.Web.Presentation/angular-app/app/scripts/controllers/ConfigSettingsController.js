'use strict';

angular.module('bekApp')
  .controller('ConfigSettingsController', ['$scope', 'ConfigSettingsService',
    function($scope, ConfigSettingsService) {

        ConfigSettingsService.getAppSettings().then(function(resp){
            $scope.configSettings = resp;
        });

        $scope.saveConfig = function(settings){
            if($scope.configSettingsForm.$dirty){
                var settingValues = [];
                settings.forEach(function(setting){
                    if(setting.newvalue){
                        setting.value = setting.newvalue;
                        settingValues.push(setting);
                    }
                })
                ConfigSettingsService.saveAppSettings(settingValues).then(function(resp){
                    $scope.configSettingsForm.$setPristine();
                    $scope.displayMessage('success', 'Successfully saved config settings');
                });
            } else {
                return;
            }

        }
  }]);