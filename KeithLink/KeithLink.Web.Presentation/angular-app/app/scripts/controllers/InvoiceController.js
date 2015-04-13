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
 $scope.currDate = new Date();
  $scope.datepickerOptions = {
    minDate: new Date(),
    options: {
      dateFormat: 'yyyy-MM-dd',
      showWeeks: false
    }
  };

    
  // different filter views for users to choose in the header dropdown
  $scope.filterViews = [{
    name: 'All Invoices',
    filterFields: []
  }, {
    name: 'Open Invoices',
    filterFields: [],
    specialFitler: {
      condition: 'or',
      filter: [{
        field: 'statusdescription',
        value: 'Open'
      }, {
        field: 'statusdescription',
        value: 'Past Due'
      }]
    }
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


  var invoicePagingModel = new PagingModel( 
    InvoiceService.getInvoices, 
    setInvoices,
    appendInvoices,
    startLoading,
    stopLoading
  );

    if(!InvoiceService.selectedFilterView){
  $scope.selectedFilterView = InvoiceService.selectedFilterView = $scope.filterViews[1];
}
  retrieveFilter();
  invoicePagingModel.loadData();

  function calculateInvoiceFields(invoices) {
    invoices.forEach(function(invoice) {
      // determine which invoices are payable
      if (invoice.pendingtransaction && invoice.pendingtransaction.editable) {
        invoice.userCanPayInvoice = true;
        invoice.paymentAmount = invoice.pendingtransaction.amount;
        invoice.date = invoice.pendingtransaction.date.substr(0,10); // get format '2014-01-31'
      } else if (invoice.ispayable) {
        invoice.userCanPayInvoice = true;
      }

      // calculate max payment date
      var date = moment(invoice.duedate).add(2, 'd');
      invoice.maxPaymentDate = date.format('YYYY-MM-DD');
    });
  }

  function setInvoices(data) {
    $scope.invoices = data.pagedresults.results;
    $scope.totalInvoices = data.pagedresults.totalResults;
    $scope.hasPayableInvoices = data.haspayableinvoices;
    $scope.totalAmountDue = data.totaldue;

    calculateInvoiceFields(data.pagedresults.results);
  }


    function retrieveFilter() {
        if(InvoiceService && InvoiceService.filterRowFields){
            $scope.showFilter = true;
            $scope.filterRowFields =InvoiceService.filterRowFields;
            $scope.selectedFilterView = InvoiceService.selectedFilterView;
          var filter = getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);
        }
        else{
          if(InvoiceService && InvoiceService.selectedFilterView){
          $scope.selectedFilterView = InvoiceService.selectedFilterView;
          var filter = getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);
        }
  }
}

  function appendInvoices(data) {
    $scope.invoices = $scope.invoices.concat(data.pagedresults.results);
    calculateInvoiceFields(data.pagedresults.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  function getInvoicesFilterObject(filterFields, filterView) {  
    var filter = invoicePagingModel.getFilterObject(filterFields, filterView.filterFields);
    if (filterView.specialFitler) {
      if (filter) {
        filter.filter.push(filterView.specialFitler);
      } else {
        filter = filterView.specialFitler;
      }
    }
    invoicePagingModel.filter = filter;
    invoicePagingModel.pageIndex = 0;

    return filter;
  }

  $scope.filterInvoices = function(filterFields) {
    InvoiceService.setFilters($scope.selectedFilterView , filterFields);
    getInvoicesFilterObject(filterFields, $scope.selectedFilterView);
    invoicePagingModel.loadData();
  };
  $scope.clearFilters = function() {
    $scope.filterRowFields = InvoiceService.filterRowFields = {};
    getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);    
    invoicePagingModel.loadData();
    $scope.showFilter = false;
  };
  $scope.selectFilterView = function (filterView) {
    InvoiceService.setFilters(filterView, $scope.filterRowFields);
    $scope.selectedFilterView = filterView;
    getInvoicesFilterObject($scope.filterRowFields, filterView);
    invoicePagingModel.loadData();
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

      $scope.selectedFilterView = $scope.filterViews[1]; // default to Open Invoices filter view
      setTempContextForViewingAllCustomers();
      invoicePagingModel.getData = InvoiceService.getAllOpenInvoices;

    } else {
      
      //restore previously selected user context
      $scope.setSelectedUserContext(currentUserSelectedContext);
      invoicePagingModel.getData = InvoiceService.getInvoices;
    }

    getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);
    invoicePagingModel.loadData();
  };

  function changeUserContext(stateName, stateParams, customerNumber, customerBranch) {
    //generate and set customer context to customerNumber that user selected
    CustomerService.getCustomerDetails(customerNumber, customerBranch).then(function (customer) {
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
  $scope.changeCustomerOnClick = function (customerNumber, branch) {
    changeUserContext('menu.invoice', $state.params, customerNumber, branch);
  };

  $scope.linkToReferenceNumber = function(customerNumber, branch, invoiceNumber){
    if ($scope.viewingAllCustomers) {
      // change selected context if viewing all customers
      changeUserContext('menu.invoiceitems', { invoiceNumber: invoiceNumber }, customerNumber, branch);
    } else {
      $state.go('menu.invoiceitems', { invoiceNumber: invoiceNumber} );
    }
  };

  $scope.selectAccount = function (account) {
    $scope.selectedAccount = account;
  };

  $scope.toggleSelect = function (invoice) {
    if (invoice.paymentAmount && invoice.paymentAmount != 0) { // jshint ignore:line
      invoice.isSelected = true;
    } else {
      invoice.isSelected = false;
    }
    if (invoice.pendingtransaction && invoice.pendingtransaction.amount == invoice.paymentAmount) { // jshint ignore:line
      invoice.isSelected = false;
    }
  };

  $scope.selectInvoice = function (invoice, isSelected) {
    if (isSelected) {
      if (!invoice.pendingtransaction) {
        invoice.paymentAmount = invoice.amount.toString();
      }
    } else {
      if (invoice.pendingtransaction) {
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
      if (invoice.isSelected) {
        total += parseFloat(invoice.paymentAmount || 0);
      }
    });
    $scope.total = total;
    return total;
  };

  $scope.getSelectedInvoices = function() {
    return $filter('filter')($scope.invoices, {isSelected: true});
  };

  var processingPayInvoices = false;
  $scope.payInvoices = function () {
    if (!processingPayInvoices) {
      processingPayInvoices = true;
      var payments = $scope.getSelectedInvoices();
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
          return {
            isViewingAllCustomers: $scope.viewingAllCustomers,
            paging: {
              filter: getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView)
            }
          };
        }
      }
    });
  };
}]);
