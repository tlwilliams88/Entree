'use strict';

/******
used for all export modals (invoice, order, catalog, lists)

takes 4 resolve values

exportConfig  : obj - config object which gives the available export types and fields

{
  availabletypes: ["CSV", "TAB", "EXCEL"],
  fields: [{
    field: "Notes",
    label: "Note",
    order: 0,
    selected: false,
  }, {
    field: "ItemNumber",
    label: "Item",
    order: 1,
    selected: true,
  }],
  selectedtype: "CSV"
}

exportMethod  : function - export function which takes the config obj and optional params as arguments
exportParams  : text,obj - export specific params such as listId or search url for catalog export, passed as the second arguement to the export method
headerText    : text - value displayed at the top of the export window ('Invoice', 'Invoice # 12345')
******/

angular.module('bekApp')
.controller('ExportModalController', ['$scope', '$filter', '$modalInstance', 'exportConfig', 'exportMethod', 'exportParams', 'headerText',
  function ($scope, $filter, $modalInstance, exportConfig, exportMethod, exportParams, headerText) {

  $scope.headerText = headerText;
  $scope.selectedFields = [];
  $scope.unselectedFields = [];
  $scope.exportConfig = exportConfig;

  // build list of previously selected and unselected fields
  exportConfig.fields.forEach(function(field) {
    if (field.selected === true) {
      $scope.selectedFields.push(field);
    } else {
      $scope.unselectedFields.push(field);
    }
  });

  // sort previously selected fields by order
  $scope.selectedFields = $filter('orderBy')($scope.selectedFields, 'order', false);

  // set default export type if there is not a previously selectedtype
  if (!$scope.exportConfig.selectedtype) {
    $scope.exportConfig.selectedtype = $scope.exportConfig.availabletypes[0];
  }

  $scope.defaultExport = function() {
    var config = {
      selectedtype: $scope.exportConfig.selectedtype
      // do not set fields for default export
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