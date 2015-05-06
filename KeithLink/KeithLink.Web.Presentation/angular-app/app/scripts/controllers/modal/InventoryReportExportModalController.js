'use strict';

angular.module('bekApp')
.controller('InventoryReportExportModalController', ['$scope', '$modalInstance', 'items', 'ExportService',
  function ($scope, $modalInstance, items, ExportService) {

  $scope.formats = [{
    text: 'Excel',
    value: 'excel'
  },{
    text: 'PDF',
    value: 'pdf'
  }]

  $scope.export = function(format) {
    ExportService.exportInventoryValueReport(format, items);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);