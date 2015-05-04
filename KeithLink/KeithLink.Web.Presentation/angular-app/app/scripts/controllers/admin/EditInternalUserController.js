'use strict';

angular.module('bekApp')
  .controller('EditInternalUserController', ['$scope', '$state', 'userProfile', 'DsrAliasService',
    function ($scope, $state, userProfile, DsrAliasService) {

  $scope.profile = userProfile;

  // get dsr aliases for internal users
  DsrAliasService.getAliasesForUser(userProfile.userid).then(function(aliases) {
    $scope.dsrAliases = aliases;
  });
  
  /**********
  DSR ALIAS EVENTS
  **********/

  $scope.addDsrAlias = function(dsrNumber) {
    $scope.dsrAliasErrorMessage = '';
    var alias = {
      userId: userProfile.userid,
      email: userProfile.emailaddress,
      branchId: $scope.selectedUserContext.customer.customerBranch,
      dsrNumber: dsrNumber
    };

    DsrAliasService.createAlias(alias).then(function(newAlias) {
      alias.id = newAlias.id;
      $scope.dsrAliasNumber = '';
      $scope.dsrAliases.push(alias);
    }, function(errorMessage) {
      $scope.dsrAliasErrorMessage = errorMessage;
    });
  };

  $scope.removeDsrAlias = function(alias, email) {
    DsrAliasService.deleteAlias(alias.id, email).then(function() {
      $scope.dsrAliases.splice($scope.dsrAliases.indexOf(alias), 1);
    });
  };

}]);
