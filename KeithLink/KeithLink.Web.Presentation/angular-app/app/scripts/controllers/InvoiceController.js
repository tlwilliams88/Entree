'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$scope', '$filter', '$modal', 'accounts', 'InvoiceService', '$rootScope', 'LocalStorage', 'CustomerService', '$state', 'PagingModel',
    function ($scope, $filter, $modal, accounts, InvoiceService, $rootScope, LocalStorage, CustomerService, $state, PagingModel) {

  var currentUserSelectedContext = {};

  $scope.invoiceCustomerContexts = [{
    text: 'All Customers',
    isViewingAllCustomers: true
  }, {
    text: $scope.selectedUserContext.customer.customerNumber + ' - ' + $scope.selectedUserContext.customer.customerName,
    isViewingAllCustomers: false
  }];
  $scope.selectedInvoiceContext = $scope.invoiceCustomerContexts[1];

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

  var invoicePagingModel = new PagingModel( 
    InvoiceService.getInvoices, 
    setInvoices,
    appendInvoices,
    startLoading,
    stopLoading
  );

  invoicePagingModel.loadData();
    
  // different filter views for users to choose in the header dropdown
  $scope.filterViews = [{
    name: 'All Invoices',
    filterFields: []
  }, {
    name: 'Invoices to Pay',
    filterFields: [{
      field: 'ispayable',
      value: true,
      type: 'equals'
    }]
  }, {
    name: 'Open Invoices',
    filterFields: [{
      field: 'statusdescription',
      value: 'Open'
    }]
  }, {
    name: 'Invoices Pending Payment',
    filterFields: [{
      field: 'statusdescription',
      value: 'Pending'
    }]
  }, {
    name: 'Past Due Invoices',
    filterFields: [{
      field: 'statusdescription',
      value: 'Past Due'
    }]
  }, {
    name: 'Paid Invoices',
    filterFields: [{
      field: 'statusdescription',
      value: 'Paid'
    }]
  }];
  $scope.selectedFilterView = $scope.filterViews[0];

  function setInvoices(data) {
    $scope.invoices = data.pagedresults.results;
    $scope.totalInvoices = data.pagedresults.totalResults;
    $scope.hasPayableInvoices = data.haspayableinvoices;
    $scope.totalAmountDue = data.totaldue;

    // determine which invoices are payable
    data.pagedresults.results.forEach(function(invoice) {
      if (invoice.pendingtransaction && invoice.pendingtransaction.editable) {
        invoice.userCanPayInvoice = true;
        invoice.paymentAmount = invoice.pendingtransaction.amount;
        invoice.date = invoice.pendingtransaction.date.substr(0,10); // get format '2014-01-31'
      } else if (invoice.ispayable) {
        invoice.userCanPayInvoice = true;
      }
    });
  }
  function appendInvoices(data) {
    $scope.invoices = $scope.invoices.concat(data.pagedresults.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  $scope.filterInvoices = function(filterFields) {
    invoicePagingModel.filterData(filterFields, $scope.selectedFilterView.filterFields);
  };
  $scope.clearFilters = function() {
    $scope.filterFields = {};
    invoicePagingModel.clearFilters();
  };
  $scope.selectFilterView = function (filterView) {
    $scope.selectedFilterView = filterView;
    invoicePagingModel.filterData($scope.filterRowFields, $scope.selectedFilterView.filterFields);
  };
  $scope.infiniteScrollLoadMore = function() {
    invoicePagingModel.loadMoreData($scope.invoices, $scope.totalInvoices, $scope.loadingResults);
  };
  $scope.sortInvoices = function(field, sortDescending) {
    $scope.sort = {
      field: field,
      sortDescending: sortDescending
    };
    invoicePagingModel.sortData($scope.sort);
  };

  /************
  VIEWING INVOICES FOR ALL CUSTOMERS
  ************/

  function setTempContextForViewingAllCustomers() {
    //store current user context temporarily
    currentUserSelectedContext = $scope.selectedUserContext;
    //wipe user context and replace text with all customers
    var tempContext = {
      text: 'All Customers'
    };
    $scope.setSelectedUserContext(tempContext);
  }

  //toggles state between all customer invoices and single customer invoices
  $scope.setViewingAllCustomers = function (invoiceContext) {
    $scope.viewingAllCustomers = invoiceContext.isViewingAllCustomers;
    $scope.selectedInvoiceContext = invoiceContext;
    
    // clear values to reset page
    $scope.invoices = [];
    $scope.totalInvoices = 0;
    $scope.filterRowFields = {};

    if ($scope.viewingAllCustomers) {

      $scope.selectedFilterView = $scope.filterViews[2]; // default to Open Invoices filter view
      setTempContextForViewingAllCustomers();
      invoicePagingModel.getData = InvoiceService.getAllOpenInvoices;

    } else {
      
      //restore previously selected user context
      $scope.setSelectedUserContext(currentUserSelectedContext);
      invoicePagingModel.getData = InvoiceService.getInvoices;
    }

    invoicePagingModel.filterData($scope.filterRowFields, $scope.selectedFilterView.filterFields);
  };

  function changeUserContext(stateName, stateParams, customerNumber) {
    //generate and set customer context to customerNumber that user selected
    CustomerService.getCustomerDetails(customerNumber).then(function (customer) {
      var generatedUserContext = {
        id: customer.customerNumber,
        text: customer.displayname,
        customer: customer
      };

      //set the selected context to the generated one
      $scope.setSelectedUserContext(generatedUserContext);

      //persist the context change to Local Storage to prevent the stateChangeStart listener from reverting the context change
      LocalStorage.setSelectedCustomerInfo(generatedUserContext);

      //refresh the page
      $state.transitionTo(stateName, stateParams, {
        reload: true,
        inherit: false,
        notify: true
      });
    });
  }

  //change the selected user context to the one the user clicked and refresh the page
  $scope.changeCustomerOnClick = function (customerNumber) {
    changeUserContext('menu.invoice', $state.params, customerNumber);
  };

  $scope.linkToReferenceNumber = function(customerNumber, branch, invoiceNumber){
    if ($scope.viewingAllCustomers) {
      // change selected context if viewing all customers
      changeUserContext('menu.invoiceitems', { invoiceNumber: invoiceNumber }, customerNumber);
    } else {
      $state.go('menu.invoiceitems', { invoiceNumber: invoiceNumber} );
    }
  };

  $scope.selectAccount = function (account) {
    $scope.selectedAccount = account;
  };

  $scope.selectInvoice = function (invoice, isSelected) {
    if (isSelected) {
      if (!invoice.pendingtransaction) {
        invoice.paymentAmount = invoice.amount.toString();
      }
    } else {
      if (invoice.pendingtransaction && invoice.pendingtransaction.editable) {
        invoice.paymentAmount = invoice.pendingtransaction.amount; 
      } else {
        invoice.paymentAmount = '0';  
      }
    }

    $scope.invoiceForm.$setDirty();
  };

  $scope.selectAll = function (areAllSelected) {
    $scope.selectAllPayable = areAllSelected;
    angular.forEach($scope.invoices, function (item, index) {
      if (item.userCanPayInvoice) {
        item.isSelected = areAllSelected;
        $scope.selectInvoice(item, item.isSelected);
      }
    });
  };

  $scope.totalPaymentAmount = function () {
    var total = 0;
    $scope.invoices.forEach(function (invoice) {
      total += parseFloat(invoice.paymentAmount || 0);
    });
    $scope.total = total;
    return total;
  };

  var processingPayInvoices = false;
  $scope.payInvoices = function () {
    if (!processingPayInvoices) {
      processingPayInvoices = true;
      var payments = $filter('filter')($scope.invoices, {isSelected: true});
      InvoiceService.payInvoices(payments, $scope.selectedAccount).then(function() {
        $state.go('menu.transaction');
      }).finally(function () {
        processingPayInvoices = false;
      });
    }
  };

  $scope.openExportModal = function () {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        headerText: function () {
          return 'Invoices';
        },
        exportMethod: function () {
          return InvoiceService.exportInvoice;
        },
        exportConfig: function () {
          return InvoiceService.getExportConfig();
        },
        exportParams: function () {
          var params = $scope.invoiceParams;
          params.filter = createFilterObject();
          return params;
        }
      }
    });
  };

}]);
