'use strict';

angular.module('bekApp')
  .controller('ForgotPasswordController', ['$scope', 'UserProfileService', '$state', 'validToken', '$stateParams', 'toaster',
    function ($scope, UserProfileService, $state, validToken, $stateParams, toaster) {

    var Token = validToken;

    $scope.isTokenValid = (Token && Token.email && Token.email.length > 0) ? true : false;
    
    //If token is valid, variable validToken will be the email address of the current user. Otherwise, it will be null.
    if($scope.isTokenValid){
      $scope.emailAddress = Token.email;
      $scope.firstName = Token.firstName;
      $scope.lastName = Token.lastName;
      $scope.userName = Token.userName;
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