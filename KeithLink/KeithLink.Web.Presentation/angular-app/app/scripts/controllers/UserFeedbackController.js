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
