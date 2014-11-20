'use strict';

angular.module('bekApp')
.controller('InvoiceItemsController', ['$scope', '$stateParams', 'invoice', 'order',
  function ($scope, $stateParams, invoice, order) {

  invoice.items = order.items;
  $scope.invoice = invoice;

}]);