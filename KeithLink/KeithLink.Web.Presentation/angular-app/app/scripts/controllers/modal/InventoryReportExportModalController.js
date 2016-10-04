'use strict';

angular.module('bekApp')
.controller('InventoryReportExportModalController', ['$scope', '$analytics', '$modalInstance', 'items', 'ExportService',
  function ($scope, $analytics, $modalInstance, items, ExportService) {

  $scope.category = false;

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
  }]

  $scope.selectFormat = function(format){
    $scope.selectedFormat = format;
    $scope.category = false;
  }

  $scope.setTotalByContractCategory = function(){
    $scope.category = !$scope.category;
  }

  $scope.export = function(format, category) {
    $analytics.eventTrack('Export Inventory Valuation', {category: 'Reports'});
    ExportService.exportInventoryValueReport(format, items, category);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);