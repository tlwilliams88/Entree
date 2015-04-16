'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ChangePasswordController
 * @description
 * ChangePasswordController
 * Controller of the bekApp
 */
angular.module('bekApp')
    .controller('ChangePasswordController', ['$scope', '$state', 'SessionService','AuthenticationService', 'UserProfileService',
        function ($scope, $state, SessionService, AuthenticationService, UserProfileService) {
           var profile = SessionService.userProfile;

           $scope.passwordData = { 
               email: profile.emailaddress,
               originalPassword: '',
               newPassword: ''
           };

           $scope.changePassword = function(passwordData) {
               $scope.changePasswordErrorMessage = '';

               UserProfileService.changePassword(passwordData).then(function(response) {
                   profile.passwordexpired = false;
                   SessionService.userProfile = profile;
                   $scope.redirectUserToCorrectHomepage();
               }, function(errorMessage) {
                   $scope.changePasswordErrorMessage = errorMessage.errorMessage;
               });
           }; 

           $scope.signOut = function() {
               AuthenticationService.logout();
               $state.go('register');
           };

     }]); // Controller Closing
