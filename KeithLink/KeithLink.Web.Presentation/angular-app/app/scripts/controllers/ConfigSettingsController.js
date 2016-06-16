'use strict';

angular.module('bekApp')
  .controller('ConfigSettingsController', ['$scope', 'ConfigSettingsService',
    function($scope, ConfigSettingsService) {

        $scope.settingsVerified = false;

        ConfigSettingsService.getAppSettings().then(function(resp){
            $scope.configSettings = resp;
        });

        $scope.setSettingsVerified = function(setting){
            if(!$scope.isverified){
                $scope.isverified = !$scope.isverified;
            }
            setting.isverified = !setting.isverified;
        }

        $scope.saveConfig = function(settings){
            if($scope.configSettingsForm.$dirty){
                var settingValues = [];
                settings.forEach(function(setting){
                    if(setting.newvalue && setting.isverified){
                        setting.value = setting.newvalue;
                        settingValues.push(setting);
                    }
                })
                ConfigSettingsService.saveAppSettings(settingValues).then(function(resp){
                    $scope.configSettings = resp;
                    $scope.configSettings.forEach(function(setting){
                        if(setting.newvalue){
                            setting.newvalue = '';
                        }
                    })
                    $scope.configSettingsForm.$setPristine();
                    $scope.displayMessage('success', 'Successfully saved config settings');
                });
                $scope.verifySettings = false;
            } else {
                $scope.verifySettings = false;
                return;
            }

        }
  }]);