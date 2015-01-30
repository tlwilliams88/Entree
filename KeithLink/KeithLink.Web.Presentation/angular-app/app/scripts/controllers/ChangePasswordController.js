'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ChangePasswordController
 * @description
 * ChangePasswordController
 * Controller of the bekApp
 */
angular.module('bekApp')
    .controller('ChangePasswordController', ['$scope', '$state', 'ENV', 'LocalStorage','AuthenticationService', 'AccessService', 'UserProfileService',
        function ($scope, $state, ENV, LocalStorage, AuthenticationService, AccessService, UserProfileService) {
           var profile = LocalStorage.getProfile();

           $scope.passwordData = { 
               email: profile.emailaddress,
               originalPassword: '',
               newPassword: ''
           };

           $scope.changePassword = function(passwordData) {
               $scope.changePasswordErrorMessage = '';

               UserProfileService.changePassword(passwordData).then(function(response) {
                   profile.passwordexpired = false;
                   LocalStorage.setProfile(profile);
                   if ( AccessService.isOrderEntryCustomer() || AccessService.isInternalAccountAdminUser() ) {
                        $state.go('menu.home');
                   } else {
                        $state.go('menu.catalog.home');
                   }
               }, function(errorMessage) {
                   $scope.changePasswordErrorMessage = errorMessage.errorMessage;
               });
           }; 

           $scope.signOut = function() {
               AuthenticationService.logout();
               $state.go('register');
           };

     }]); // Controller Closing
