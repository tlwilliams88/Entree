'use strict';

angular.module('bekApp')
  .controller('UserProfileController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state', 'MessagePreferenceService',
    function ($scope, UserProfileService, branches, LocalStorage, $state, MessagePreferenceService) {

  var init = function(){
    $scope.branches = branches;
    
    MessagePreferenceService.getPreferencesForCustomer(null).then(function (preferences) {
      $scope.defaultPreferences = preferences;
    });
  };

  /*********
  AVATAR
  *********/
  $scope.files = [];
  $scope.onFileSelect = function($files) {
    $scope.files = [];
    for (var i = 0; i < $files.length; i++) {
      $scope.files.push($files[i]);
    }
  };

  function refreshAvatarUrl() {
    var now = new Date();
    var newUrl = $scope.userProfile.imageurl + '?d=' + now.toString();
    $scope.userProfile.imageurl = newUrl;
    $scope.$parent.userProfile.imageurl = newUrl;
  }

  $scope.uploadAvatar = function() {
    // console.log(avatarFile);
    // var file = {
    //   name: avatarFile.filename,
    //   file: avatarFile.base64
    // };
    var file = {
      name: $scope.files[0].name, 
      file: $scope.files[0]
    };
    UserProfileService.uploadAvatar(file).then(refreshAvatarUrl);
  };

  $scope.removeAvatar = function() {
    UserProfileService.removeAvatar().then(refreshAvatarUrl);
  };

  $scope.cancelChanges = function () {
    $scope.userProfile = angular.copy(LocalStorage.getProfile());
    $scope.updateProfileForm.$setPristine();
  };

  $scope.updateUserProfile = function(userProfile) {
    userProfile.email = userProfile.emailaddress;
    $scope.updateProfileErrorMessage = null;
    
    UserProfileService.updateUserProfile(userProfile).then(function(profile) {
      $scope.$parent.userProfile = profile;
      $scope.displayMessage('success', 'Successfully updated profile.');
    }, function(errorMessage) {
      $scope.updateProfileErrorMessage = errorMessage;
    });
  };

  $scope.changePassword = function (changePasswordData) {
    $scope.changePasswordErrorMessage = null;
    changePasswordData.email = $scope.userProfile.emailaddress;

    UserProfileService.changePassword(changePasswordData).then(function (success) {
      $scope.changePasswordData = {};
      $scope.changePasswordForm.$setPristine();
      $scope.displayMessage('success', 'Successfully changed password.');
    }, function (errorMessage) {
      $scope.changePasswordErrorMessage = errorMessage;
      $scope.displayMessage('error', 'Error updating profile.');
    });
  };

  $scope.savePreferences = function () {
    MessagePreferenceService.updatePreferences($scope.defaultPreferences, null).then(function(data) {
      $scope.canEditNotifications = false;
      $scope.notificationPreferencesForm.$setPristine();
    });
  };

  init();

}]);
