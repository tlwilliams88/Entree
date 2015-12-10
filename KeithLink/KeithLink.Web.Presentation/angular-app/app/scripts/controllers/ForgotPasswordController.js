'use strict';

angular.module('bekApp')
  .controller('ForgotPasswordController', ['$scope', 'UserProfileService', '$state', 'validToken', '$stateParams', 'toaster',
    function ($scope, UserProfileService, $state, validToken, $stateParams, toaster) {

    //If token is valid, validToken.data will be the email address of the current user. Otherwise, it will be null.
    if(validToken.data){
      $scope.emailAddress = validToken.data;
      $scope.isTokenValid = true;
    }
    else{
      $scope.isTokenValid = false;
    }
    
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