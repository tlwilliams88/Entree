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
.controller('ExportModalController', ['$scope', '$filter', '$modalInstance', '$analytics', 'exportConfig', 'exportMethod', 'exportParams', 'headerText', 'location', 'exportType', 'Constants',
  function ($scope, $filter, $modalInstance, $analytics, exportConfig, exportMethod, exportParams, headerText, location, exportType, Constants) {

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
            filter: null,
            daterange: null,
            sort: null,
            selectedtype: null,
        },
        exportRequest = exportRequestModel;

      if(exportParams.params.filter.name) {
          if(exportParams.params.filter.name == "Open Invoices") {
              exportRequest.filter = exportParams.params.filter.filterFields;
          } else {
              exportRequest.filter = exportParams.params.filter.filterFields[0];
          }
      } else {
          exportRequest.filter = exportParams.params.filter[0].filter;
      }
      exportRequest.daterange = exportParams.params.filter[0] && exportParams.params.filter[0].daterange ? exportParams.params.filter[0].daterange : null;
      exportRequest.sort = exportParams.params.sort ? exportParams.params.sort : null;
            
      // This model is used for invoice exports
      $scope.invoiceExportRequest = exportRequest;
    }
  
    function listExport() {
      // This model is used for all list exports
      var exportRequestModel = {
          fields: null,
          selectedtype: null,
          sort: null, 
          filter: null
      };
      
      $scope.exportRequest = exportRequestModel;
      
      $scope.exportRequest.selectedtype = exportConfig.selectedtype;
      $scope.exportRequest.sort = exportParams && exportParams.sort ? exportParams.sort : null;
      $scope.exportRequest.filter = exportParams && exportParams.filter ? exportParams.filter : null;
      
      return $scope.exportRequest;
    }
    
    function standardExport() {
        $scope.staticParam = exportParams;
        listExport();
    }
    
    function createExportModel() {
        switch ($scope.exportType) {
            case Constants.exportType.invoiceExport:
                invoiceExport();
            break;
            case Constants.exportType.listExport:
            case Constants.exportType.orderItemsExport:
            case Constants.exportType.searchExport:
            case Constants.exportType.marketingExport:
            case Constants.exportType.itemUsageExport:
            case Constants.exportType.cartItemsExport:
            case Constants.exportType.invoiceItemsExport:
            case Constants.exportType.ordersExport:
                standardExport();
            break;
        }
    }
    createExportModel()

    $scope.defaultExport = function() {
        $analytics.eventTrack(location.action, {  category: location.category, label: 'Default Export' });
        switch ($scope.exportType) {
            case Constants.exportType.invoiceExport:
                if($scope.invoiceExportRequest.fields && $scope.invoiceExportRequest.fields.length > 0) {
                    delete $scope.invoiceExportRequest.fields;
                };
                $scope.invoiceExportRequest.selectedtype = $scope.exportConfig.selectedtype;
                exportMethod($scope.invoiceExportRequest);
            break;
            case Constants.exportType.listExport:
            case Constants.exportType.orderItemsExport:
            case Constants.exportType.searchExport:
            case Constants.exportType.marketingExport:
            case Constants.exportType.itemUsageExport:
            case Constants.exportType.cartItemsExport:
            case Constants.exportType.invoiceItemsExport:
                if($scope.exportRequest.fields && $scope.exportRequest.fields.length > 0) {
                    delete $scope.exportRequest.fields;
                };
                $scope.exportRequest.selectedtype = $scope.exportConfig.selectedtype;
                exportMethod($scope.staticParam, $scope.exportRequest);
            break;
            case Constants.exportType.ordersExport:
                if($scope.exportRequest.fields && $scope.exportRequest.fields.length > 0) {
                    delete $scope.exportRequest.fields;
                };
                $scope.exportRequest.selectedtype = $scope.exportConfig.selectedtype;
                exportMethod($scope.exportRequest);
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
        
        switch ($scope.exportType) {
            case Constants.exportType.invoiceExport:
                $scope.invoiceExportRequest.fields = $scope.selectedFields;
                $scope.invoiceExportRequest.selectedtype = $scope.exportConfig.selectedtype;
                exportMethod($scope.invoiceExportRequest);
            break;
            case Constants.exportType.listExport:
            case Constants.exportType.orderItemsExport:
            case Constants.exportType.searchExport:
            case Constants.exportType.marketingExport:
            case Constants.exportType.itemUsageExport:
            case Constants.exportType.cartItemsExport:
            case Constants.exportType.invoiceItemsExport:
                $scope.exportRequest.fields = $scope.selectedFields;
                $scope.exportRequest.selectedtype = $scope.exportConfig.selectedtype;
                exportMethod($scope.staticParam, $scope.exportRequest);
            break;
            case Constants.exportType.ordersExport:
                $scope.exportRequest.fields = $scope.selectedFields;
                $scope.exportRequest.selectedtype = $scope.exportConfig.selectedtype;
                exportMethod($scope.exportRequest);
            break;
        }
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