'use strict';

angular.module('bekApp')
.controller('InventoryReportExportModalController', ['$scope', '$analytics', '$modalInstance', 'items', 'ExportService',
  function ($scope, $analytics, $modalInstance, items, ExportService) {

  $scope.formats = [{
    text: 'Excel',
    value: 'excel'
  },{
    text: 'PDF',
    value: 'pdf'
  }]

  $scope.export = function(format) {
    $analytics.eventTrack('Export Inventory Valuation', {category: 'Reports'});
    ExportService.exportInventoryValueReport(format, items);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);