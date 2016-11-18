'use strict';

angular.module('bekApp')
.controller('InventoryReportExportModalController', ['$scope', '$analytics', '$modalInstance', 'items', 'ExportService',
  function ($scope, $analytics, $modalInstance, items, ExportService) {

  $scope.options = {
    category: false,
    groupByLabel: false
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
        groupByLabel = $scope.options.groupByLabel;

    if(groupByCategory == true && groupByLabel == false) {
      ExportService.exportInventoryValueReport(format, items, 'category');
    } else if(groupByCategory == true && groupByLabel == true) {
      ExportService.exportInventoryValueReport(format, items, 'categorythenlabel');
    } else if(groupByCategory == false && groupByLabel == true) {
      ExportService.exportInventoryValueReport(format, items, 'label');
    } else {
      ExportService.exportInventoryValueReport(format, items);
    }
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);