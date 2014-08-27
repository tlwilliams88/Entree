'use strict';

angular.module('bekApp')
  .controller('AccountDetailsController', ['$scope', 'UserProfileService', function ($scope, UserProfileService) {
    
    $scope.userProfile = UserProfileService.profile();

  }]);
