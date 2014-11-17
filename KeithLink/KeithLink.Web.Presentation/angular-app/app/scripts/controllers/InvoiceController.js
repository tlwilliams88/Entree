'use strict';

angular.module('bekApp')
.controller('InvoiceController', ['$scope', '$state', 'invoices', 'accounts',
  function ($scope, $state, invoices, accounts) {

  $scope.invoices = invoices;
  $scope.accounts = accounts;
  $scope.selectedAccount = accounts[0];

  $scope.selectAccount = function(account) {
    $scope.selectedAccount = account;
  };

  $scope.totalPaymentAmount = function() {
    var total = 0;
    $scope.invoices.forEach(function(invoice) {
      total += invoice.paymentAmount;
    });
    return total;
  };

}]);