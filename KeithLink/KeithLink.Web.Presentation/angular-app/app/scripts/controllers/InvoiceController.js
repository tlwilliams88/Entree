'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$scope', '$filter', '$modal', 'accounts', 'InvoiceService', 'Constants', '$rootScope', 'LocalStorage', 'CustomerService', '$state',
    function ($scope, $filter, $modal, accounts, InvoiceService, Constants, $rootScope, LocalStorage, CustomerService, $state) {

  function loadInvoices(params) {
    var promise;
    $scope.loadingResults = true;

    params.filter = createFilterObject();

    if ($scope.viewingAllCustomers) {
      // handles loading invoices if the user wishes to view all customers at the same time
      promise = InvoiceService.getAllOpenInvoices(params);
    } else {
      // handles customer specific invoice loading
      promise = InvoiceService.getInvoices(params);
    }

    return promise.then(function(data) {
      $scope.loadingResults = false;
      $scope.totalInvoices = data.pagedresults.totalResults;
      $scope.hasPayableInvoices = data.haspayableinvoices;
      $scope.totalAmountDue = data.totaldue;

      return data.pagedresults.results;
    });
  }

  function setInvoices(invoices) {
    $scope.invoices = invoices;
  }

  /************
  VIEWING INVOICES FOR ALL CUSTOMERS
  ************/

  function setPageText(customerName, customerNumber) {
    if ($scope.viewingAllCustomers) {
      //set button and header text
      $scope.viewAllButtonText = 'Return to Invoices for: ' + customerNumber + ' - ' + customerName;
      $scope.headerText = 'Open Invoices of All Customers';
    } else {
      $scope.viewAllButtonText = 'View Open Invoices for All Customers';
      $scope.headerText = 'Invoices for: ' + customerName;
    }
  }

  //toggles state between all customer invoices and single customer invoices
  $scope.switchViewingAllCustomers = function () {
    //properly set string values for each state and change the selected user context to a placeholder to prevent confusion
    $scope.viewingAllCustomers = !$scope.viewingAllCustomers;
    
    //wipe current invoices out
    $scope.invoices = [];
    $scope.totalInvoices = 0;

    // clear filter row
    $scope.filterRowFields = {};

    if ($scope.viewingAllCustomers) {

      setPageText($scope.selectedUserContext.customer.customerName, $scope.selectedUserContext.customer.customerNumber);
      //store current user context temporarily
      temporarySelectedUserContext = $scope.selectedUserContext;

      //wipe user context and replace text with all customers
      var tempContext = {
        text: 'All Customers'
      };
      $scope.setSelectedUserContext(tempContext);

    } else {
      setPageText(temporarySelectedUserContext.customer.customerName);
      //restore previous selected user context
      $scope.setSelectedUserContext(temporarySelectedUserContext);
    }

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  //listens for state change event to restore selectedUserContext
  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    //change selected user context back to the one stored in LocalStorage here
    if (fromState.name === 'menu.invoice' && !$scope.selectedUserContext.id) {
      $scope.setSelectedUserContext(LocalStorage.getCurrentCustomer());
    }
  });

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

  $scope.linkToReferenceNumber = function(customerNumber, invoiceNumber){
    if ($scope.viewingAllCustomers) {
      // change selected context if viewing all customers
      changeUserContext('menu.invoiceitems', { invoiceNumber: invoiceNumber }, customerNumber);
    } else {
      $state.go('menu.invoiceitems', { invoiceNumber: invoiceNumber} );
    }
  };

  /******************************
  PAGING, SORTING, FILTERING
  ******************************/

  function createFilterObject() {

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

    var filterObject = {
      // field: 
      // value:
      filter: []
    };

    // if viewing all customers, set open filter
    if ($scope.viewingAllCustomers) {
      filterObject.filter.push({
        field: 'statusdescription',
        value: 'open'
      });
    }

    // if filter view is selected, set given filter
    if ($scope.selectedFilterView && !$scope.viewingAllCustomers) {
      filterObject.filter = filterObject.filter.concat($scope.selectedFilterView.filterFields);
    }

    // if filter row is used
    if ($scope.filterRowFields) {
      // var filterList = [];
      for (var propertyName in $scope.filterRowFields) {
        if ($scope.filterRowFields[propertyName] && $scope.filterRowFields[propertyName] !== '') {
          var filterField = {
            field: propertyName,
            value: $scope.filterRowFields[propertyName]
          };
          filterObject.filter.push(filterField);
        }
      }
    }

    // if no filters are selected, remove filter object
    if (filterObject.filter.length === 0) {
      filterObject = [];
    
    
    // otherwise, pop first filter
    } else {
      var firstFilter = filterObject.filter.splice(0, 1)[0];
      filterObject.field = firstFilter.field;
      filterObject.value = firstFilter.value;
      if (firstFilter.type) {
        filterObject.type = firstFilter.type;
      }
    }

    return filterObject;
  }

  $scope.filterInvoices = function (filterFields) {
    // reset paging
    $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.clearFilters = function () {
    $scope.filterRowFields = {};
    
    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.sortInvoices = function (field, order) {
    $scope.sortOrder = order;

    $scope.invoiceParams.sort = [{
      field: field,
      order: order ? 'desc' : 'asc'
    }];

    // reset paging
    $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.infiniteScrollLoadMore = function () {
    if (($scope.invoices && $scope.invoices.length >= $scope.totalInvoices) || $scope.loadingResults) {
      return;
    }

    $scope.invoiceParams.from += $scope.invoiceParams.size;

    loadInvoices($scope.invoiceParams).then(function (invoices) {
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
    filterFields: [{
      field: 'ispayable',
      value: true,
      type: 'equals'
    }]
  },
  {
    name: 'Open Invoices',
    filterFields: [{
      field: 'statusdescription',
      value: 'Open'
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

  $scope.selectFilterView = function (filterView) {
    $scope.selectedFilterView = filterView;

    $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
  };

  $scope.selectAccount = function (account) {
    $scope.selectedAccount = account;
  };

  $scope.selectInvoice = function (invoice, isSelected) {
    if (isSelected) {
      invoice.paymentAmount = invoice.amount.toString();
    } else {
      invoice.paymentAmount = '0';
    }
  };

  $scope.selectAll = function () {
    angular.forEach($scope.invoices, function (item, index) {
      if (item.ispayable) {
        item.isSelected = $scope.selectAllPayable;
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
      InvoiceService.payInvoices(payments, $scope.selectedAccount).finally(function () {
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

  //set defaults and instantiate persistent user context
  setPageText($scope.selectedUserContext.customer.customerName);
  var temporarySelectedUserContext = {};

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
