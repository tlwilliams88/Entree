'use strict';

angular.module('bekApp')
  .controller('UserFeedbackController', ['$scope', '$state', 'UserFeedbackService', 'SessionService', 'BranchService',
    function ($scope, $state, UserFeedbackService, SessionService, BranchService) {

      var init = function () {
        $scope.userProfile = SessionService.userProfile; //Get current users profile from LocalStorage

        var branchId = $scope.selectedUserContext.customer.customerBranch;

        if (!$scope.branches) {
          BranchService.getBranches().then(
            function (resp) {
              $scope.branches = resp;
            }
          )
        };

        angular.forEach($scope.branches,
          function (branch) {
            if (branch.id.toUpperCase() === branchId.toUpperCase()) {
              $scope.branch = branch;
            }
          }
        );

      };

      $scope.goBack = function () {
        $state.go('menu.home');
      };

      $scope.submitUserFeedback = function (userFeedback) {
        userFeedback.audience = $state.params.audience;
        userFeedback.browserUserAgent = navigator.userAgent;
        userFeedback.browserVendor = navigator.vendor;

        $scope.submitUserFeedbackErrorMessage = null;

        UserFeedbackService.submitUserFeedback(userFeedback).then(
          function (profile) {
            $scope.userFeedback = {};
            $scope.userFeedbackForm.$setPristine();
            $scope.displayMessage('success', 'Successfully sent feedback.');
          },
          function (errorMessage) {
            $scope.submitUserFeedbackErrorMessage = errorMessage;
          }
        );
      };

      init();

    }]);
