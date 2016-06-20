'use strict';

angular.module('bekApp')
  .controller('ConfigSettingsController', ['$scope', 'ConfigSettingsService',
    function($scope, ConfigSettingsService) {

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
                        setting.isverified = !setting.isverified;
                        setting.value = setting.newvalue;
                        settingValues.push(setting);
                    }
                })

                ConfigSettingsService.saveAppSettings(settingValues).then(function(resp){
                    settings.forEach(function(setting){
                        if(setting.newvalue && resp){
                            setting.newvalue = '';
                        }
                    })
                    if(resp && settingValues.length){
                        $scope.displayMessage('success', 'Successfully saved config settings');
                        $scope.configSettingsForm.$setPristine();
                        $scope.verifySettings = false;
                    } else if(!resp) {
                        $scope.displayMessage('error', 'Error saving config settings');
                    } else {
                        return;
                    }
                });
            } else {
                return;
            }

        }
  }]);