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
exportParams  : text,obj - export specific params such as listId or search url for catalog export, passed as the second argument to the export method
headerText    : text - value displayed at the top of the export window ('Invoice', 'Invoice # 12345')
******/

angular.module('bekApp')
.controller('ExportModalController', ['$scope', '$filter', '$modalInstance', '$analytics', 'exportConfig', 'exportMethod', 'exportParams', 'headerText', 'location', 'exportType',
  function ($scope, $filter, $modalInstance, $analytics, exportConfig, exportMethod, exportParams, headerText, location, exportType) {

    $scope.headerText = headerText;
    $scope.selectedFields = [];
    $scope.unselectedFields = [];
    $scope.exportConfig = exportConfig;
    $scope.exportType = exportType;

    // set default export type if there is not a previously selectedtype
    if (!$scope.exportConfig.selectedtype) {
        $scope.exportConfig.selectedtype = $scope.exportConfig.availabletypes[0];
    }

    // build list of previously selected and unselected fields
    function selectFields() {
      exportConfig.fields.forEach(function(field) {
        if (field.selected === true) {
          $scope.selectedFields.push(field);
        } else {
          $scope.unselectedFields.push(field);
        }
      });

      // sort previously selected fields by order
      $scope.selectedFields = $filter('orderBy')($scope.selectedFields, 'order', false);  
    }
    selectFields();

    function invoiceExport() {
        var exportRequestModel = {
            fields: [],
            selectedtype: null,
            sort: null
        };
        
        var pagingExportModel = {
            size: null,
            from: null,
            sort: [],
            filter: [],
            daterange: [],
            search: null,
            terms: null,
            type: null
        };
        var exportRequest = exportRequestModel,
            pagingExport = pagingExportModel;
      
      exportRequest.selectedtype = exportConfig.selectedtype;
      exportRequest.fields = $scope.selectedFields;
      exportRequest.size = exportParams.params.size;
      exportRequest.from = exportParams.params.from;
      exportRequest.sort = exportParams.params.sort;
      
      pagingExport.filter = exportParams.params.filter.filterFields ? exportParams.params.filter.filterFields[0] : exportParams.params.filter[0].filter;
      pagingExport.daterange = exportParams.params.filter[0] && exportParams.params.filter[0].daterange ? exportParams.params.filter[0].daterange : {};
      pagingExport.isViewingAllCustomers = exportParams.isViewingAllCustomers;
      pagingExport.size = exportParams.params.size;
      pagingExport.from = exportParams.params.from;
      // This model is used for invoice exports
      $scope.invoiceExportRequest = {
          paging: pagingExport,
          export: exportRequest
      }
    }
  
    function listExport() {
      // This model is used for all list exports
      $scope.exportRequestModel = {
          fields: [],
          selectedtype: null,
          sort: null, 
          filter: null
      };
      
      $scope.exportRequestModel.selectedtype = exportConfig.selectedtype;
      $scope.exportRequestModel.fields = $scope.selectedFields;
      $scope.exportRequestModel.size = exportParams.params.size;
      $scope.exportRequestModel.from = exportParams.params.from;
    }

    function standardExport() {
        $scope.standardExport = exportParams;
    }
    
    function createExportModel() {
        switch ($scope.exportType) {
            case 'invoiceExport':
                invoiceExport();
            break;
            case 'listExport':
                listExport();
            break;
            case 'standardExport':
                standardExport();
            break;
        }
    }
    createExportModel()

    $scope.defaultExport = function() {
        $analytics.eventTrack(location.action, {  category: location.category, label: 'Default Export' });
        switch ($scope.exportType) {
            case 'invoiceExport':
                delete $scope.invoiceExportRequest.export.fields;
                exportMethod($scope.invoiceExportRequest);
            break;
            case 'listExport':
                delete $scope.exportRequest.fields;
                exportMethod($scope.exportRequest);
            break;
            case 'standardExport':
                delete $scope.standardExport.fields;
                exportMethod($scope.standardExport);
            break;
        }
    };

    $scope.customExport = function() {
        $analytics.eventTrack(location.action, {  category: location.category, label: 'Custom Export' });
        // prepare config object with user's export settings
        $scope.selectedFields.forEach(function(field, index) {
            field.selected = true;
            field.order = index + 1;
        });

        var config = {
            selectedtype: $scope.exportConfig.selectedtype,
            fields: $scope.selectedFields,
            sort: exportConfig.sort
        },
        params = {
            isViewingAllCustomers: exportParams.isViewingAllCustomers,
            filter: $scope.exportParamsFilter
        };

        if(exportParams.filter != null){
            config.filter = exportParams.filter;
        }

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