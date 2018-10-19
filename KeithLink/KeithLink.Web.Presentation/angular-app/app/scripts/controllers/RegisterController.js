'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:RegisterController
 * @description
 * RegisterController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('RegisterController', ['$scope', '$state', 'ENV', 'toaster', 'AuthenticationService', 'BranchService', 'UserProfileService', 'PhonegapPushService', 'LocalStorage', 'blockUI', '$interval', 'ApplicationSettingsService', 'TutorialService',
    function ($scope, $state, ENV, toaster, AuthenticationService, BranchService, UserProfileService, PhonegapPushService, LocalStorage, blockUI, $interval, ApplicationSettingsService, TutorialService) {

  $scope.isMobileApp = ENV.mobileApp;
  $scope.signUpBool = false;
  $scope.isInternalEmail = false;
  $scope.defaultUserName = ENV.username;
  $scope.saveUserName = $scope.defaultUserName ? true : false;

  var branchCheck;
  function checkForBranches() {
    BranchService.getBranches().then(function(resp){
      if(resp == -1) {
        return;
      } else {
        blockUI.stop();
        $interval.cancel(branchCheck);
        $scope.branches = resp.successResponse;
      }
    })
  }

  BranchService.getBranches().then(function(resp) {
    var branches = [],
        maintenanceMessage = 'We\'re currently undergoing maintenance for an extended period today.\n We\'ll be back soon.\n Thank you for your patience.';
    if(resp == -1 && $scope.isMobileApp) {
      blockUI.start(maintenanceMessage).then(function() {
        branchCheck = $interval(checkForBranches, 30000);
      })
    } else {
      if(resp && resp.length > 0) {
        branches = resp;
      }
      $scope.branches = branches;
    }
  });

  // Biometrics Tutorial
  var getHideTutorial = LocalStorage.getHideTutorialRegisterPage(),
      runTutorial =  getHideTutorial ? false : true,
      message = $scope.autheMethod == 'Touch ID' ? 
            "After entering your credentials click here to register for " + $scope.authenMethod + ". After registering you will be able to use this method going forward.  <br/><br/>If you would like to unregister at any time you can do so in your profile settings.  <br/><br/>Please be aware that after registering any " + $scope.authenMethod + " entry stored on this device will be able to access Entree." :
            "After entering your credentials click here to register for " + $scope.authenMethod + ". After registering you will be able to use this method going forward.  <br/><br/>If you would like to unregister at any time you can do so in your profile settings.",
      overlay = true,
      offset = {left: -70, top: 64.11},
      width = 300,
      highlight = true;

  if($scope.isMobileApp == true) {

    window.plugins.touchid.isAvailable(function(biometryType) {

      $scope.authenMethod = biometryType == 'touch' && biometryType != 'OK' ? 'Touch ID' : 'Face ID'; // iOS
      if($scope.authenMethod == 'OK') {
        $scope.authenMethod = 'Fingerprint'; // Android
      }
      window.plugins.touchid.has("Entree_Credential_User", function() {
        $scope.keyAvailable = true;

        window.plugins.touchid.verify("Entree_Credential_User", "Use " + $scope.authenMethod + " to login", entreeCredentialFound, entreeBiometricResponse);
        
      }, function() {
        $scope.keyAvailable = false;
      });

      if(runTutorial) 
      {

        TutorialService.setTutorial(
          "register_tutorial", 
          "Biometric Register Location", 
          message,
          [{name: "Close", onclick: setTutorialHidden}],
          overlay,
          "#bioRegister",
          "top",
          offset,
          width,
          highlight
        );
      } 
    }, function(msg) {
        $scope.authenMethod = 'standard'
    });

  };

  function setTutorialHidden(){
    TutorialService.setDisplayTutorial('hide', LocalStorage.setHideTutorialRegisterPage);
  }

  // gets prepopulated login info for dev environment
  if(ENV.username) {
    $scope.loginInfo = {
      username: $scope.defaultUserName,
      password: ''
    };
  } else {
    $scope.loginInfo = {
      username: '',
      password: ''
    };
  }

  $scope.setSavingUserName = function(username) {
    $scope.saveUserName = !$scope.saveUserName;
    $scope.enteredUserName = username;
  };

  $scope.login = function(loginInfo) {
    $scope.loginErrorMessage = '';

    if(loginInfo.key) {
      loginInfo.username = loginInfo.key.slice(22);
      delete loginInfo.key;

      loginInfo.password = '';
      delete loginInfo.value; 
    }

    if($scope.saveUserName){
      LocalStorage.setDefaultUserName(loginInfo.username);
    } else {
      LocalStorage.setDefaultUserName('');
    }

    AuthenticationService.login(loginInfo.username, loginInfo.password)
      .then(UserProfileService.getCurrentUserProfile)
      .then(function(profile) {

        if (ENV.mobileApp) { // ask to allow push notifications
          storeUserKeyForBiometricLogin(profile);
          PhonegapPushService.register();
        }
        $scope.redirectUserToCorrectHomepage();
      }, function(errorMessage) {
        $scope.loginErrorMessage = errorMessage;
      });

  };

  $scope.displayBiometricsLogin = function() {

    window.plugins.touchid.verify("Entree_Credential_User", "Use " + $scope.authenMethod + " to login", entreeCredentialFound, entreeBiometricResponse);

  };

  function entreeCredentialFound(storedKey) {

    var credentials = {},
        key = {
          key: storedKey,
          value: device.uuid
        };

    ApplicationSettingsService.getUserKey(key).then(function(resp) {
      credentials = resp;

      LocalStorage.setBiometryEnabled(true);
      LocalStorage.setBiometryType($scope.authenMethod);

      $scope.login(credentials);
    })
  }

  function entreeBiometricResponse(msg) {
    var message = msg && msg.ErrorMessage ? msg.ErrorMessage : msg;

    switch(message) 
    {

      case 'Canceled by user.':
        $scope.userCanceledBiometric = true;
      break;

      case 'Fallback authentication mechanism selected.':
        $scope.displayAlternateAuthentication(false);
      break;

      case 'Biometry is locked out.':
        biometryUnavailable(msg.ErrorMessage);
      break;

      case 'User has denied the use of biometry for this app.':
        biometryUnavailable(msg.ErrorMessage);
      break;

      case '-1':
        window.plugins.touchid.verify("Entree_Credential_User", "Verifying" + $scope.authenMethod + "authentication", entreeCredentialFound, saveCredentialLocally);
      break;

    }
    
  }

  function saveCredentialLocally() {
    window.plugins.touchid.save("Entree_Credential_User", $scope.loginInfo.username, true, successfullySavedCredential, errorSavingCredential);
  }

  function successfullySavedCredential() {
    window.plugins.touchid.verify("Entree_Credential_User", "Use " + $scope.authenMethod + " to login", entreeCredentialFound, entreeBiometricResponse);
  }

  function errorSavingCredential() {
    return;
  }

  function biometryUnavailable() {
    $scope.displayBiometrics = false;
  }

  function storeUserKeyForBiometricLogin(user) {
    var userDevice = device.uuid.toString(),
        userKey = {userid: user.userid, key: $scope.loginInfo.username, value: userDevice};

    ApplicationSettingsService.setUserKey(userKey);
  }

  $scope.displayAlternateAuthentication = function(set) {
    $scope.displayBiometrics = set;
  };

  $scope.forgotPassword = function(email) {
    $scope.checkForInternalEmail(email);
    if(!$scope.isInternalEmail){
      UserProfileService.resetPassword(email).then(function(data){
       toaster.pop('success', null, 'We have sent a password reset request if the email was verified');
      },function(error) {
      toaster.pop('error', null, 'Error resetting password.');
      });
    } else {
      return false;
    }

   };

  $scope.setSignUpBool = function(signUpBool) {
    $scope.registerUser.password = null;

    $scope.registerUser.email = null;

    $scope.signUpBool = !signUpBool;
  };

  $scope.checkForInternalEmail = function(email) {
    if(email.slice(email.indexOf('@'),(email.indexOf('@') + 14)).toLowerCase(0) === "@benekeith.com"){
      $scope.isInternalEmail = true;
      $scope.$digest();
    }else{
      $scope.isInternalEmail = false;
    }
  };

  var processingRegistration = false;
  $scope.registerNewUser = function(userProfile) {
    if (!processingRegistration) {
      processingRegistration = true;
      $scope.registrationErrorMessage = null;

      UserProfileService.createUser(userProfile).then(function(data) {

        // log user in
        AuthenticationService.login(userProfile.email, userProfile.password)
          .then(UserProfileService.getCurrentUserProfile)
          .then(function(profile) {
            // redirect to account details page
            $state.go('menu.userprofile');
          }, function(error) {
            $scope.loginErrorMessage = error.data.error_description;
            $scope.clearForm();
            $scope.loginInfo = {};
          });
        }, function(error) {
          $scope.registrationErrorMessage = error;
        }).finally(function() {
          processingRegistration = false;
        });
    }
  };

  $scope.clearForm = function() {
    $scope.registerUser = {
      email: null,
      confirmEmail: null,
      password: null,
      confirmPassword: null,
      existingcustomer: false,
      marketingflag: true,
      branch: null
    };
    $scope.registrationForm.$setPristine();
    $scope.registrationErrorMessage = null;
    $scope.signUpBool = false;
  };

}]);
