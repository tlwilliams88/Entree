'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ChangePasswordController
 * @description
 * ChangePasswordController
 * Controller of the bekApp
 */
angular.module('bekApp')
    .controller('ChangePasswordController', ['$scope', '$state', 'ENV', 'LocalStorage', 'AccessService', 'UserProfileService',
        function ($scope, $state, ENV, LocalStorage, AccessService, UserProfileService) {

           $scope.passwordData = { 
               email: LocalStorage.getProfile().emailaddress,
               originalPassword: '',
               newPassword: ''
           };

           $scope.changePassword = function(passwordData) {
               $scope.changePasswordErrorMessage = '';

               UserProfileService.changePassword(passwordData).then(function(response) {
                   UserProfileService.getCurrentUserProfile($scope.passwordData.email)
                    .then(function(response) {
                       if ( AccessService.isOrderEntryCustomer() || AccessService.isInternalUser() ) {
                           $state.go('menu.home');
                       } else {
                           $state.go('menu.catalog.home');
                       }
                    });
               }, function(errorMessage) {
                   $scope.changePasswordErrorMessage = errorMessage.errorMessage;
               });
           }; 

       }]);
