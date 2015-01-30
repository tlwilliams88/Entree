'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'items', 'name',
  function ($scope, $modalInstance, PrintService, items, name) {

  $scope.items = items;
  $scope.name = name;
  $scope.labelOptions = [{
    type: '5160',
    numberOnPage: 30,
    columns: 3
  }];
  $scope.selectedLabelOption = $scope.labelOptions[0]; // pre-select first option

  $scope.printLabels = function(items, labelOption) {
    var data = {
      labelOption: labelOption,
      items: items
    };
    PrintService.print('views/printTemplates/productlabels.html', data);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);