'use strict';

angular.module('bekApp')
.controller('InvoiceController', ['$scope', '$filter', 'invoices', 'accounts',
  function ($scope, $filter, invoices, accounts) {

  $scope.invoices = invoices;
  $scope.accounts = accounts;
  $scope.selectedAccount = accounts[0];

  $scope.hasPayableInvoices = $filter('filter')(invoices, { ispayable: true }).length > 0;

  // different filter views for users to choose in the header dropdown
  $scope.filterViews = [{
    name: 'All Invoices'
  }, {
    name: 'Invoices to Pay',
    filter: function(invoice) {
      return invoice.ispayable;
    }
  }, {
    name: 'Open Invoices',
    filter: function(invoice) {
      return invoice.statusdescription === 'Open';
    }
  }, {
    name: 'Past Due Invoices',
    filter: function(invoice) {
      return invoice.statusdescription === 'Late';
    }
  }, {
    name: 'Paid Invoices',
    filter: function(invoice) {
      return invoice.statusdescription === 'Paid';
    }
  }];

  $scope.selectFilterView = function(filterView) {
    $scope.selectedFilterView = filterView;
  };

  $scope.selectFilterView($scope.filterViews[0]);

  $scope.selectAccount = function(account) {
    $scope.selectedAccount = account;
  };

  $scope.isTypeInvoice = function(invoice) {
    return invoice.typedescription === 'IN ';
  };

  //logic for proper select filtering, allows user to disable filter instead of showing only true or only false
  $scope.filterFields = {};
  $scope.setSelectedFilter = function(selectedFilter) {
    if (selectedFilter) {
      delete $scope.filterFields.isSelected;
    } else {
      $scope.filterFields.isSelected = true;
    }
  };

  $scope.selectAll = function() {
    angular.forEach($scope.filteredItems, function(item, index) {
      if (item.ispayable) {
        item.isSelected = $scope.selectAllPayable;
      }
    });
  };

  $scope.totalPaymentAmount = function() {
    var total = 0;
    $scope.filteredItems.forEach(function(invoice) {
      total += parseFloat(invoice.paymentAmount || 0);
    });
    return total;
  };

}]);