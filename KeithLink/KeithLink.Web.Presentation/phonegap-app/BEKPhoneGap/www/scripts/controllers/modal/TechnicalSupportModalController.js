'use strict';

angular.module('bekApp')
.controller('TechnicalSupportModalController', ['$scope', '$modalInstance', 'branch',
  function ($scope, $modalInstance, branch) {

  $scope.branch = branch;

  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);