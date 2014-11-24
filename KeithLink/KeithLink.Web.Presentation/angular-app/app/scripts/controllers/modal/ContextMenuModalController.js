'use strict';

angular.module('bekApp')
.controller('ContextMenuModalController', ['$scope', '$modalInstance', 'item', 
  function ($scope, $modalInstance, item) {

  $scope.item = item;

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);