'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'ListService', 'list',
  function ($scope, $modalInstance, PrintService, ListService, list) {

  $scope.list = list;
  $scope.labelOptions = [{
    type: '5160',
    numberOnPage: 30,
    columns: 3
  }];
  $scope.selectedLabelOption = $scope.labelOptions[0]; // pre-select first option

  // $scope.printLabels = function(items, labelOption) {
  //   var data = {
  //     labelOption: labelOption,
  //     items: items
  //   };
  //   PrintService.print('views/printTemplates/productlabels.html', data);
  // };

  $scope.printLabels = function(list, labelOption) {
    ListService.printBarcodes(list.listid);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);