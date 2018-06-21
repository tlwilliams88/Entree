'use strict';

angular.module('bekApp')
  .controller('UserFeedbackController', ['$scope', '$state', 'UserFeedbackService', 'SessionService',
    function ($scope, $state, UserFeedbackService, SessionService) {

      var init = function () {
        $scope.userProfile = SessionService.userProfile; //Get current users profile from LocalStorage
      };

      $scope.goBack = function () {
        $state.go('menu.home');
      };

      $scope.cancelChanges = function () {
        $scope.userFeedbackForm.$setPristine();
      };

      $scope.submitUserFeedback = function (userFeedback) {
        userFeedback.Audience = $state.params.audience;
        userFeedback.BrowserUserAgent = navigator.userAgent;
        userFeedback.BrowserVendor = navigator.vendor;

       $scope.submitUserFeedbackErrorMessage = null;

        UserFeedbackService.submitUserFeedback(userFeedback).then(
          function (profile) {
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
