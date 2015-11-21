'use strict';

angular.module('bekApp')
.controller('InvoiceTransactionSummaryModalController', ['$scope', '$modalInstance', 'invoice', 'InvoiceService', 
  function ($scope, $modalInstance, invoice, InvoiceService) {


  InvoiceService.getInvoice(invoice.invoicenumber).then(function(invoice){
	  $scope.invoice = invoice;
  });


  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);