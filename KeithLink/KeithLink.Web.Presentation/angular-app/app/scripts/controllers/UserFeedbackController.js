'use strict';

angular.module('bekApp')
  .controller('UserFeedbackController', ['$scope', '$state', 'UserFeedbackService', 'SessionService', 'BranchService', '$stateParams',
    function ($scope, $state, UserFeedbackService, SessionService, BranchService, $stateParams) {

      $scope.formMessage;
      $scope.formType;

      switch($stateParams.audience) {
        case "Support":
          $scope.formType = 'Support';
          $scope.formMessage = 'to submit your Entree feedback.';
        break;
        case "BranchSupport":
          $scope.formType = 'Branch Support';
          $scope.formMessage = 'to contact your Local Support Staff.';
        break;
        case "SalesRep":
          $scope.formType = 'Sales Rep';
          $scope.formMessage = 'to contact your Sales Representative.'
        break;
      }

      var init = function () {
        $scope.userProfile = SessionService.userProfile; //Get current users profile from LocalStorage

        var branchId = $scope.selectedUserContext.customer.customerBranch;

        if (!$scope.branches) {
          BranchService.getBranches().then(
            function (resp) {
              $scope.branches = resp;

              findBranch(branchId);
            }
          )
        } else {
          findBranch(branchId);
        }

      };

      function findBranch(branchid){
        angular.forEach($scope.branches,
          function (branch) {
            if (branch.id.toUpperCase() === branchid.toUpperCase()) {
              $scope.branch = branch;
            }
          }
        );
      }

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
