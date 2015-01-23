'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'list',
  function ($scope, $modalInstance, PrintService, list) {

  $scope.list = list;
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