'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'items', 'name',
  function ($scope, $modalInstance, PrintService, items, name) {

  $scope.items = items;
  $scope.name = name;
  $scope.labelOptions = {
    type: '5160'
  };

  $scope.printLabels = function(items, labelType) {
    var data = {
      type: labelType,
      items: items
    };
    PrintService.print('views/printTemplates/productlabels.html', data);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);