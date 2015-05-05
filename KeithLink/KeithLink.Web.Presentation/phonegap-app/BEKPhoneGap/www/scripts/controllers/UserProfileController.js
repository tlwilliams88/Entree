'use strict';

angular.module('bekApp')
  .controller('UserProfileController', ['$scope', 'UserProfileService', 'branches', 'SessionService', '$state', 'AccessService', 'MessagePreferenceService', 'DsrAliasService',
    function ($scope, UserProfileService, branches, SessionService, $state, AccessService, MessagePreferenceService, DsrAliasService) {

  var init = function(){
    $scope.branches = branches;

    $scope.isInternalUser = $scope.userProfile.emailaddress.indexOf('@benekeith.com') > -1;

    if (AccessService.isOrderEntryCustomer()){
      MessagePreferenceService.getPreferencesAndFilterByCustomerNumber(null).then(function (preferences) {
        $scope.defaultPreferences = preferences;
      });
    } else {
      $scope.hideNotificationPreferences = true;
    }

    if ($scope.isInternalUser) {
      DsrAliasService.getAliasesForCurrentUser().then(function(aliases) {
        $scope.dsrAliases = aliases;
      });
    }
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

  $scope.goBack = function(){
    $state.go('menu.home');
  };

  $scope.cancelChanges = function () {
    $scope.userProfile = angular.copy(SessionService.userProfile);
    $scope.updateProfileForm.$setPristine();
  };

  $scope.updateUserProfile = function(userProfile) {     
    userProfile.email = userProfile.emailaddress;
    $scope.updateProfileErrorMessage = null;
    
    UserProfileService.updateUserProfile(userProfile).then(function(profile) {
      $scope.$parent.userProfile = profile;
      $scope.updateProfileForm.$setPristine();
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
