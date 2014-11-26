'use strict';

angular.module('bekApp')
.controller('InvoiceItemsController', ['$scope', '$stateParams', 'transactions', 'order',
  function ($scope, $stateParams, transactions, order) {

  $scope.invoice = transactions[0];
  $scope.invoice.items = order.items;
  $scope.transactions = transactions;

}]);