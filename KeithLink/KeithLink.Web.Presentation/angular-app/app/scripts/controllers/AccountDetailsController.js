'use strict';

angular.module('bekApp')
  .controller('AccountDetailsController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state',
    function ($scope, UserProfileService, branches, LocalStorage, $state) {
      /*---init---*/
      $scope.userProfile = angular.copy(LocalStorage.getProfile());
      $scope.branches = branches;
      $scope.customers = $scope.userProfile.user_customers;

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
        console.log('savePreferences');
      };
    }]);
