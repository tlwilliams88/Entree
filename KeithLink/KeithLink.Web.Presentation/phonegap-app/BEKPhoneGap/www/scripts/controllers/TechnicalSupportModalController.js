'use strict';

angular.module('bekApp')
.controller('TechnicalSupportModalController', ['$scope', '$modalInstance', 'branchId', 'branches',
  function ($scope, $modalInstance, branchId, branches) {

  angular.forEach(branches, function(branch) {
    if (branch.id.toUpperCase() === branchId.toUpperCase()) {
      $scope.branch = branch;
    }
  });

  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);