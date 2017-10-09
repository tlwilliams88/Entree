'use strict';

angular.module('bekApp')
.controller('InvoiceItemsController', ['$scope', '$stateParams', '$modal', 'invoice', 'InvoiceService', 'Constants',
  function ($scope, $stateParams, $modal, invoice, InvoiceService, Constants) {

  $scope.sortOrder = false;
  $scope.sortBy = 'linenumber';

  $scope.invoice = invoice;
  
  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        location: function() {
          return {category:'Invoices', action:'Export Invoice Items'};
        },
        headerText: function () {
          return 'Invoice #' + invoice.invoicenumber;
        },
        exportMethod: function() {
          return InvoiceService.exportInvoiceDetails;
        },
        exportConfig: function() {
          return InvoiceService.getDetailExportConfig(invoice.invoicenumber);
        },
        exportParams: function() {
          return invoice.invoicenumber;
        },
        exportType: function() {
           return Constants.exportType.listExport;
        }
      }
    });
  };

}]);
