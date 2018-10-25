'use strict';

angular.module('bekApp')
  .controller('UserProfileController', ['$scope', 'UserProfileService', 'branches', 'SessionService', '$state', 'AccessService', 'ApplicationSettingsService', 'DsrAliasService', 'LocalStorage', 'ENV', 'toaster', 'Constants',
    function ($scope, UserProfileService, branches, SessionService, $state, AccessService, ApplicationSettingsService, DsrAliasService, LocalStorage, ENV, toaster, Constants) {

  var init = function(){
    $scope.isMobileApp = ENV.mobileApp;

    $scope.biometryEnabled = LocalStorage.getBiometryEnabled() ? true : false;

    $scope.branches = branches;

    if($scope.isMobileApp == true)
    {
      $scope.authenMethod = LocalStorage.getBiometryType();
      if($scope.biometryEnabled == 'true')
      {
        $scope.displayBiometricMessage = $scope.biometryEnabled == 'true' && ($scope.authenMethod == 'Touch ID' || $scope.authenMethod == 'Fingerprint');
      }
    }
    
    $scope.isInternalUser = $scope.userProfile.emailaddress.indexOf('@benekeith.com') > -1;

    if (AccessService.isOrderEntryCustomer()){
      ApplicationSettingsService.getNotificationPreferencesAndFilterByCustomerNumber(null).then(function (preferences) {
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

  $scope.biometricLogin = function(register) {
    switch(register) {
      case true :
        registerBiometricLogin();
      break;

      case false:
        deleteStoredBiometricLogin();
      break;
    }
  };

  function registerBiometricLogin() {
    saveCredentialLocally(); // Key will store locally and on backend if successful
  };

  function saveCredentialLocally() {
    window.plugins.touchid.save(Constants.biometricKeyName.keyName, $scope.userProfile.emailaddress, true, storeUserKeyForBiometricLogin);
  }

  function storeUserKeyForBiometricLogin() {
    var userDevice = device.uuid.toString(),
        userKey = {userid: $scope.userProfile.userid, key: $scope.userProfile.emailaddress, value: userDevice};

    ApplicationSettingsService.setUserKey(userKey).then( function() {
      toaster.pop('success', 'User registered for ' + $scope.authenMethod);
    },
    function() {
      toaster.pop('error', 'Unable to register user for ' + $scope.authenMethod + '.' + ' Please try again.');
    });
  }

  function deleteStoredBiometricLogin() {
    var config = {
      userid: '',
      key: Constants.biometricKeyName.keyName+$scope.userProfile.emailaddress,
      value: device.uuid
    };

    ApplicationSettingsService.deleteUserKey(config).then(function() {
      
      window.plugins.touchid.delete(Constants.biometricKeyName.keyName, function() {
        toaster.pop('success', null, $scope.authenMethod + ' has been unregistered for this device.');
      });
    },
    function() {
      toaster.pop('error', 'Unable to delete ' + $scope.authenMethod + ' login.  Please try again.');
    })
  };

  $scope.changePassword = function (changePasswordData) {
    $scope.changePasswordErrorMessage = null;
    changePasswordData.email = $scope.userProfile.emailaddress;

    UserProfileService.changePassword(changePasswordData).then(function (success) {
      $scope.changePasswordData = {};
      $scope.changePasswordForm.$setPristine();
      $scope.displayMessage('success', 'Successfully changed password.');
    }, function (error) {
      $scope.changePasswordErrorMessage = error.errorMessage;
      $scope.displayMessage('error', 'Error updating profile.');
    });
  };

  $scope.savePreferences = function () {
    ApplicationSettingsService.updateNotificationPreferences($scope.defaultPreferences, null).then(function(data) {
      $scope.canEditNotifications = false;
      $scope.notificationPreferencesForm.$setPristine();
    });
  };

  init();

}]);
