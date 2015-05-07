'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'ListService', 'list', 'pagingModelOptions',
  function ($scope, $modalInstance, PrintService, ListService, list, pagingModelOptions) {

  $scope.list = list;
  $scope.labelOptions = [{
    type: '5160',
    numberOnPage: 30,
    columns: 3
  }];
  $scope.selectedLabelOption = $scope.labelOptions[0]; // pre-select first option
  $scope.landscape = false;

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

  $scope.printList = function(list, landscape) {
    ListService.printList(list.listid, landscape, pagingModelOptions);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);