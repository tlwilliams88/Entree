'use strict';

angular.module('bekApp')
  .controller('AccountDetailsController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state', 'MessagePreferenceService',
    function ($scope, UserProfileService, branches, LocalStorage, $state, MessagePreferenceService) {
      /*---init---*/
      $scope.userProfile = angular.copy(LocalStorage.getProfile());
      $scope.branches = branches;
      $scope.customers = $scope.userProfile.user_customers;

      /*---process user preferences---*/
      var prefArray = [];
      //for every topic of notification build a preference object
      $scope.userProfile.messagingpreferences[0].preferences.forEach(function(preference){
        var newPreference = {};
        //set description and default to false
        newPreference.description = preference.description;
        newPreference.notificationType = preference.notificationType;
        newPreference.channels = [false,false,false];
        //override defaulted false with true if it is sent by the pref object
        preference.selectedChannels.forEach(function (selectedChannel) {
          if(selectedChannel.channel === 1){
            newPreference.channels[0] = true;
          } else if(selectedChannel.channel === 2){
            newPreference.channels[1] = true;
          } else if(selectedChannel.channel === 4){
            newPreference.channels[2] = true;
          }
        });
        //add new pref object with booleans to the temporary array
        prefArray.push(newPreference);
      });

      //persist temp array to scope for use in DOM
      $scope.defaultPreferences = prefArray;

      $scope.cancelChanges = function () {
        $scope.userProfile = angular.copy(LocalStorage.getProfile());
        $scope.updateProfileForm.$setPristine();
      };

      $scope.changePassword = function (changePasswordData) {
        $scope.changePasswordErrorMessage = null;
        changePasswordData.email = $scope.userProfile.emailaddress;

        UserProfileService.changePassword(changePasswordData).then(function (successMessage) {
          $scope.changePasswordData = {};
          $scope.changePasswordForm.$setPristine();
          $scope.displayMessage('success', 'Successfully changed password.');
        }, function (errorMessage) {
          $scope.changePasswordErrorMessage = errorMessage;
          $scope.displayMessage('error', 'Error updating profile.');
        });
      };

      $scope.savePreferences = function () {
        //creates new payload object
        var preferencePayload = {};

        //initializes properties
        preferencePayload.customerNumber = null;
        preferencePayload.preferences = [];

        //for each topic format object accordingly by pushing only trues to the array
        $scope.defaultPreferences.forEach(function (preference) {
          var newTopic = {};
          newTopic.description = preference.description;
          newTopic.notificationType = preference.notificationType;
          newTopic.selectedChannels = [];
          if(preference.channels[0] === true){
            newTopic.selectedChannels.push({"Channel": 1});
          }
          if(preference.channels[1] === true){
            newTopic.selectedChannels.push({"Channel": 2});
          }
          if(preference.channels[2] === true){
            newTopic.selectedChannels.push({"Channel": 4});
          }
          preferencePayload.preferences.push(newTopic);
        });

        //make ajax call
        MessagePreferenceService.updatePreferences(preferencePayload).then(function (success) {
          $scope.displayMessage('success', 'Your preferences were successfully updated');
          //probably should refresh profile preferences here <----------TODO
        }, function (error) {
          $scope.displayMessage('error', 'An error occurred while updating user preferences' + error)
        });
      };
    }]);
