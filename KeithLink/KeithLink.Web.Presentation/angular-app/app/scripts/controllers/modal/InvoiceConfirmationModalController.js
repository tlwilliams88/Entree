'use strict';

angular.module('bekApp')
.controller('InvoiceTransactionSummaryModalController', ['$scope', '$modalInstance', 'invoiceNumber', 'InvoiceService', 
  function ($scope, $modalInstance, invoiceNumber, InvoiceService) {


  InvoiceService.getInvoiceTransactions(invoiceNumber).then(function(invoiceTransactions){
	  $scope.invoiceTransactions = invoiceTransactions;
  });


  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);