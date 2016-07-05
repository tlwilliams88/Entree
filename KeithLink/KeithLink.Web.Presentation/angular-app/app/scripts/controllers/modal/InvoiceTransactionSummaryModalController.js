'use strict';

angular.module('bekApp')
.controller('InvoiceTransactionSummaryModalController', ['$scope', '$modalInstance', 'invoiceNumber', 'customerNumber', 'branchId', 'InvoiceService', 'SessionService',
  function ($scope, $modalInstance, invoiceNumber, customerNumber, branchId, InvoiceService, SessionService) {


  InvoiceService.getInvoiceTransactions(branchId, customerNumber, invoiceNumber).then(function(invoiceTransactions){
	  $scope.invoiceTransactions = invoiceTransactions;
  });


  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);