'use strict';

angular.module('bekApp')
.controller('InvoiceConfirmationModalController', ['$scope', '$state', '$modalInstance', '$filter', 'InvoiceService', 'payments',
  function ($scope, $state, $modalInstance, $filter, InvoiceService, payments) {

  $scope.payments = payments;

    $scope.payInvoicesFromModal = function(){
        InvoiceService.payInvoices(payments).then(function(invoiceNumber) {
            $scope.displayMessage('success', 'Successfully submitted payment(s)');
            $modalInstance.close(true);
          }, function(error) {
            $scope.displayMessage('error', error);
          }).finally(function () {
             processingPayInvoices = false;
          });
    };

    $scope.invoiceModalCancel = function () {
      $modalInstance.close(false);
    };
}]);