'use strict';

angular.module('bekApp')
.controller('InvoiceItemsController', ['$scope', '$stateParams', 'invoice',
  function ($scope, $stateParams, invoice) {

  $scope.invoice = invoice;

}]);