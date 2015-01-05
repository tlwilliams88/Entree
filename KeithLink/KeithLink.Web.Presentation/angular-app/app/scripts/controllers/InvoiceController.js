'use strict';

angular.module('bekApp')
.controller('InvoiceController', ['$scope', '$filter', '$modal', 'accounts', 'InvoiceService', 'Constants',
  function ($scope, $filter, $modal, accounts, InvoiceService, Constants) {

  function loadInvoices(params) {
    $scope.loadingResults = true;
    return InvoiceService.getInvoices(params).then(function(data) {
      $scope.loadingResults = false;
      $scope.totalInvoices = data.pagedresults.totalResults;
      $scope.hasPayableInvoices = data.haspayableinvoices;

      return data.pagedresults.results; // return array of invoices
    });
  }

  function setInvoices(invoices) {
    $scope.invoices = invoices;
  }

    //set text defaults
    $scope.headerText = 'Your Invoices';
    $scope.viewAllButtonText = 'View Open Invoices for All Customers';

    //toggles state between all invoices and single customer invoices
  $scope.viewAllOpenInvoices = function() {
    console.log($scope.selectedUserContext);
    $scope.viewingAllCustomers = !$scope.viewingAllCustomers;
    if($scope.viewingAllCustomers){
      $scope.viewAllButtonText = 'Return to Invoices for: ' + $scope.selectedUserContext.customer.customerNumber +  ' - '  + $scope.selectedUserContext.customer.customerName;
      $scope.headerText = 'Open Invoices of All Customers';
      // $scope.$parent.selectedUserContext.text = 'All Customers';
      $scope.$parent.selectedUserContext = {
        text: 'All',
        id: 0
      }
    } else{
      $scope.viewAllButtonText = 'View Open Invoices for All Customers';
      $scope.headerText = 'Your Invoices';
      $scope.$parent.selectedUserContext.text = $scope.selectedUserContext.customer.customerNumber +  ' - '  + $scope.selectedUserContext.customer.customerName;
    }
  };

    //ideally this will change the invoice page to the specific customer that the user clicked
    $scope.changePageContext = function (customerNumber) {
      console.log('changePageContext: ' + customerNumber);
    };

  /******************************
  PAGING, SORTING, FILTERING
  ******************************/

  $scope.filterInvoices = function(filterFields) {

    // create array of filter fields
    // example filter object
    // filter: {
    //   field: 'subject', // this contains the info for the first filter
    //   value: 'value',
    //   filter: [ // use this array if filtering by more than one field
    //      {
    //        field: 'name',
    //        value: 'value'
    //      }
    //   ]
    // }
    var filterList = [];
    for(var propertyName in filterFields) {
      if (filterFields[propertyName] && filterFields[propertyName] !== '') {
        var filterObject = {
          field: propertyName,
          value: filterFields[propertyName]
        };
        filterList.push(filterObject);
      }
    }

    var firstFilter = filterList[0];
    filterList.splice(0,1);

    var filterParamObject = {
      field: firstFilter.field,
      value: firstFilter.value,
      filter: filterList
    };

    $scope.invoiceParams.filter = filterParamObject;

    // reset paging
    $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.clearFilters = function() {
    $scope.filterFields = {};
    $scope.invoiceParams.filter = [];
    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.sortInvoices = function(field, order) {
    $scope.sortOrder = order;

    $scope.invoiceParams.sort = [{
      field: field
    }];

    if (order === true) {
      $scope.invoiceParams.sort[0].order = 'desc';
    } else {
      $scope.invoiceParams.sort[0].order = 'asc';
    }

    // reset paging
    $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.invoices && $scope.invoices.length >= $scope.totalInvoices) || $scope.loadingResults) {
      return;
    }

    $scope.invoiceParams.from += $scope.invoiceParams.size;

    loadInvoices($scope.invoiceParams).then(function(invoices) {
      $scope.invoices = $scope.invoices.concat(invoices);
    });
  };


  /************
  Filter Views
  ************/

  // different filter views for users to choose in the header dropdown
  $scope.filterViews = [{
    name: 'All Invoices',
    filterFields: []
  }, {
    name: 'Invoices to Pay',
    filterFields: {
      field: 'ispayable',
      value: true,
      type: 'equals'
    }
  },
   {
    name: 'Open Invoices',
    filterFields: {
      field: 'statusdescription',
      value: 'Open'
    }
  }, {
    name: 'Past Due Invoices',
    filterFields: {
      field: 'statusdescription',
      value: 'Past Due'
    }
  }, {
    name: 'Paid Invoices',
    filterFields: {
      field: 'statusdescription',
      value: 'Paid'
    }
  }];

  $scope.selectFilterView = function(filterView) {
    $scope.selectedFilterView = filterView;

    $scope.invoiceParams.filter = filterView.filterFields;
    $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.selectAccount = function(account) {
    $scope.selectedAccount = account;
  };

  $scope.selectInvoice = function(invoice, isSelected) {
    if (isSelected) {
      invoice.paymentAmount = invoice.amount.toString();
    } else {
      invoice.paymentAmount = '0';
    }
  };

  $scope.selectAll = function() {
    angular.forEach($scope.invoices, function(item, index) {
      if (item.ispayable) {
        item.isSelected = $scope.selectAllPayable;
        $scope.selectInvoice(item, item.isSelected);
      }
    });
  };

  $scope.totalPaymentAmount = function() {
    var total = 0;
    $scope.invoices.forEach(function(invoice) {
      total += parseFloat(invoice.paymentAmount || 0);
    });
    $scope.total = total;
    return total;
  };

  var processingPayInvoices = false;
  $scope.payInvoices = function() {
    if (!processingPayInvoices) {
      processingPayInvoices = true;
      var payments = $filter('filter')($scope.invoices, { isSelected: true});
      InvoiceService.payInvoices(payments, $scope.selectedAccount).finally(function() {
        processingPayInvoices = false;
      });
    }
  };

  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        headerText: function () {
          return 'Invoices';
        },
        exportMethod: function() {
          return InvoiceService.exportInvoice;
        },
        exportConfig: function() {
          return InvoiceService.getExportConfig();
        },
        exportParams: function() {
          return;
        }
      }
    });
  };


  $scope.invoices = [];
  $scope.accounts = accounts;
  $scope.selectedAccount = accounts[0];

  $scope.datepickerOptions = {
    minDate: new Date(),
    maxDate: '2015-06-22',
    options: {
      dateFormat: 'yyyy-MM-dd',
      showWeeks: false
    }
  };

  $scope.sortBy = 'invoicenumber';
  $scope.sortOrder = true;

  $scope.invoiceParams = {
    size: Constants.infiniteScrollPageSize,
    from: 0
    // sort: [{
    //   field: 'messagecreatedutc',
    //   order: 'desc'
    // }]
    // filter: {
    //   field: 'subject',
    //   value: 'value',
    //   filter: [
    //      {
    //        field: 'name',
    //        value: 'value'
    //      }
    //   ]
    // }
  };

  $scope.selectedFilterView = $scope.filterViews[0];
  loadInvoices($scope.invoiceParams).then(setInvoices);

}]);
