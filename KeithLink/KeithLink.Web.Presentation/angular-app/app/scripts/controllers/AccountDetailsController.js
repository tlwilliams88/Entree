'use strict';

angular.module('bekApp')
  .controller('AccountDetailsController', ['$scope', 'UserProfileService', 'branches',
    function ($scope, UserProfileService, branches) {
    
    $scope.userProfile = angular.copy(UserProfileService.profile());
    $scope.branches = branches;

    $scope.updateUserProfile = function(userProfile) {
      userProfile.email = userProfile.emailaddress;
      UserProfileService.updateUser(userProfile).then(function(response) {
        if (response.successResponse) {
          UserProfileService.setProfile(userProfile);
          $scope.$parent.userProfile = userProfile;
          $scope.displayMessage('success', 'Successfully updated profile.');
        } else {
          $scope.displayMessage('error', 'Error updating profile.');
        }
      });
    };

    $scope.cancelChanges = function() {
      $scope.userProfile = angular.copy(UserProfileService.profile());
    };

    $scope.changePassword = function(changePasswordData) {
      $scope.changePasswordErrorMessage = null;

      UserProfileService.changePassword(changePasswordData).then(function(response) {
        console.log(response);

        if (response.data === '"Password update successful"') {
          $scope.changePasswordData = {};
          $scope.displayMessage('success', 'Error updating profile.');
        } else {
          $scope.changePasswordErrorMessage = response.data;
          $scope.displayMessage('error', 'Error updating profile.');
        }
      });
    };

  }]);
