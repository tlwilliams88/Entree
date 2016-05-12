'use strict';

angular.module('bekApp')
  .controller('ConfigSettingsController', ['$scope', 'ConfigSettingsService',
    function($scope, ConfigSettingsService) {

    	ConfigSettingsService.getAppSettings().then(function(resp){
    		$scope.configSettings = resp;
    	});

    	$scope.saveConfig = function(setting){
    		if($scope.configSettingsForm.$dirty){
    			var key = setting.key;
    			ConfigSettingsService.saveAppSettings(setting).then(function(resp){
    				$scope.configSettingsForm.$setPristine();
    				$scope.displayMessage('success', 'Successfully saved config setting ' + key);
    			});
    		}else {
    			return;
    		}

    	}
  }]);