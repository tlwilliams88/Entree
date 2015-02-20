'use strict';

angular.module('bekApp')
  .controller('ViewUserDetailsController', ['$scope', '$state', '$stateParams', 'UserProfileService', 'userProfile', 'userCustomers', 'CustomerPagingModel',
    function ($scope, $state, $stateParams, UserProfileService, userProfile, userCustomers, CustomerPagingModel) {

  $scope.groupId = $stateParams.groupId;
  $scope.profile = userProfile;
  $scope.profile.customers = userCustomers;

}]);
