'use strict';

angular.module('bekApp')
  .controller('ForgotPasswordController', ['$scope', 'UserProfileService', '$state', 'validToken', '$stateParams', 'toaster',
    function ($scope, UserProfileService, $state, validToken, $stateParams, toaster) {

    $scope.isTokenValid = validToken.data;

    $scope.resetPassword = function(passwordData) {
               $scope.changePasswordErrorMessage = '';

               UserProfileService.changeForgottenPassword({ password: passwordData.confirmPassword, token: $stateParams.t }).then(function(response) {
               	    toaster.pop('success', null, 'You may now login with your new password');
                   $scope.redirectUserToCorrectHomepage();
               }, function(errorMessage) {
                   $scope.changePasswordErrorMessage = errorMessage.errorMessage;
               });
           }; 

}]);