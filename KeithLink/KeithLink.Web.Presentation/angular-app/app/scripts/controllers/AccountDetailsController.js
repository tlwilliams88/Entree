'use strict';

angular.module('bekApp')
  .controller('AccountDetailsController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage',
    function ($scope, UserProfileService, branches, LocalStorage) {
    
    $scope.userProfile = angular.copy(LocalStorage.getProfile());
    $scope.branches = branches;

    $scope.updateUserProfile = function(userProfile) {
      userProfile.email = userProfile.emailaddress;
      $scope.updateProfileErrorMessage = null;
      
      UserProfileService.updateUser(userProfile).then(function(profile) {
        $scope.$parent.userProfile = profile;
        $scope.displayMessage('success', 'Successfully updated profile.');
      }, function(errorMessage) {
        $scope.updateProfileErrorMessage = errorMessage;
      });
    };

    $scope.cancelChanges = function() {
      $scope.userProfile = angular.copy(LocalStorage.getProfile());
      $scope.updateProfileForm.$setPristine();
    };

    $scope.changePassword = function(changePasswordData) {
      $scope.changePasswordErrorMessage = null;
      changePasswordData.email = $scope.userProfile.emailaddress;

      UserProfileService.changePassword(changePasswordData).then(function(successMessage) {
        $scope.changePasswordData = {};
        $scope.changePasswordForm.$setPristine();
        $scope.displayMessage('success', 'Successfully changed password.');
      }, function(errorMessage) {
        $scope.changePasswordErrorMessage = errorMessage;
        $scope.displayMessage('error', 'Error updating profile.');
      });
    };

  }]);
