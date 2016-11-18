'use strict';

angular.module('bekApp')
.controller('InventoryReportExportModalController', ['$scope', '$analytics', '$modalInstance', 'items', 'ExportService',
  function ($scope, $analytics, $modalInstance, items, ExportService) {

  $scope.options = {
    category: false,
    groupByLabel: false,
    groupBySupplier: false
  };

  $scope.formats = [{
    text: 'Excel',
    value: 'excel'
  },{
    text: 'PDF',
    value: 'pdf'
  },{
    text: 'TAB',
    value: 'tab'
  },{
    text: 'CSV',
    value: 'csv'
  }];

  $scope.selectFormat = function(format){
    $scope.selectedFormat = format;
    $scope.options.category = false;
    $scope.options.groupByLabel = false;
  };

  $scope.export = function(format, category) {
    $analytics.eventTrack('Export Inventory Valuation', {category: 'Reports'});

    var groupByCategory = $scope.options.category,
        groupByLabel = $scope.options.groupByLabel,
        groupBySupplier = $scope.options.groupBySupplier;

    if(groupByCategory == true && groupByLabel == false && groupBySupplier == false) {
      ExportService.exportInventoryValueReport(format, items, 'category');
    } else if(groupByCategory == true && groupByLabel == true && groupBySupplier == false) {
      ExportService.exportInventoryValueReport(format, items, 'categorythenlabel');
    } else if(groupByCategory == false && groupByLabel == true && groupBySupplier == false) {
      ExportService.exportInventoryValueReport(format, items, 'label');
    } else if(groupBySupplier == true) {
      ExportService.exportInventoryValueReport(format, items, 'supplier');
    } else {
      ExportService.exportInventoryValueReport(format, items);
    }
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);