'use strict';

angular.module('bekApp')
.controller('ExportModalController', ['$scope', '$filter', '$modalInstance', 'exportConfig', 'exportMethod', 'exportParams', 'headerText',
  function ($scope, $filter, $modalInstance, exportConfig, exportMethod, exportParams, headerText) {

  $scope.headerText = headerText;
  $scope.selectedFields = [];
  $scope.unselectedFields = [];
  $scope.exportConfig = exportConfig;

  // get previously selected fields
  exportConfig.fields.forEach(function(field) {
    if (field.selected === true) {
      $scope.selectedFields.push(field);
    } else {
      $scope.unselectedFields.push(field);
    }
  });

  // sort previously selected fields
  $scope.selectedFields = $filter('orderBy')($scope.selectedFields, 'order', false);

  // set default export type
  if (!$scope.exportConfig.selectedtype) {
    $scope.exportConfig.selectedtype = $scope.exportConfig.availabletypes[0];
  }

  $scope.defaultExport = function() {
    var config = {
      selectedtype: $scope.exportConfig.selectedtype
    };
    exportMethod(config, exportParams);
  };

  $scope.customExport = function() {
    // prepare config object with user's export settings
    $scope.selectedFields.forEach(function(field, index) {
      field.selected = true;
      field.order = index + 1;
    });

    var config = {
      selectedtype: $scope.exportConfig.selectedtype,
      fields: $scope.selectedFields
    };

    exportMethod(config, exportParams);
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.selectAll = function() {
    $scope.selectedFields = $scope.selectedFields.concat($scope.unselectedFields);
    $scope.unselectedFields = [];
  };

  $scope.unselectAll = function() {
    $scope.unselectedFields = $scope.unselectedFields.concat($scope.selectedFields);
    $scope.selectedFields = [];
  };
  
}]);