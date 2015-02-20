'use strict';

angular.module('bekApp')
.controller('InvoiceItemsController', ['$scope', '$stateParams', '$modal', 'invoice', 'InvoiceService',
  function ($scope, $stateParams, $modal, invoice, InvoiceService) {

  $scope.sortOrder = false;
  $scope.sortBy = 'linenumber';

  $scope.invoice = invoice;
  
  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
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
        }
      }
    });
  };

}]);
