'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$scope', '$filter', '$modal', 'accounts', 'InvoiceService', 'Constants', '$rootScope', 'LocalStorage', 'CustomerService', '$state',
    function ($scope, $filter, $modal, accounts, InvoiceService, Constants, $rootScope, LocalStorage, CustomerService, $state) {

  $scope.customerText = $scope.selectedUserContext.customer.customerNumber + ' - ' + $scope.selectedUserContext.customer.customerName;
  var currentUserSelectedContext = {};

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

      data.pagedresults.results.forEach(function(invoice) {
        if (invoice.pendingtransaction && invoice.pendingtransaction.editable) {
          invoice.userCanPayInvoice = true;
          invoice.paymentAmount = invoice.pendingtransaction.amount;
          invoice.date = invoice.pendingtransaction.date.substr(0,10); // get format '2014-01-31'
        } else if (invoice.ispayable) {
          invoice.userCanPayInvoice = true;
        }
      });

      return data.pagedresults.results;
    });
  }

  function setInvoices(invoices) {
    $scope.invoices = invoices;
  }

  /************
  VIEWING INVOICES FOR ALL CUSTOMERS
  ************/

  function setPageText() {
    if ($scope.viewingAllCustomers) {
      //set button and header text
      $scope.viewAllButtonText = 'All Customers';
    } else {
      $scope.viewAllButtonText = $scope.customerText;
    }
  }

  function setContextForViewingAllCustomers() {
    //store current user context temporarily
    currentUserSelectedContext = $scope.selectedUserContext;
    //wipe user context and replace text with all customers
    var tempContext = {
      text: 'All Customers'
    };
    $scope.setSelectedUserContext(tempContext);
  }

  //toggles state between all customer invoices and single customer invoices
  $scope.switchViewingAllCustomers = function (isViewingAllCustomers) {
    $scope.viewingAllCustomers = isViewingAllCustomers;
    
    // clear values to reset page
    $scope.invoices = [];
    $scope.totalInvoices = 0;
    $scope.filterRowFields = {};
    setPageText();

    if ($scope.viewingAllCustomers) {

      $scope.selectedFilterView = $scope.filterViews[2]; // default to Open Invoices filter view
      setContextForViewingAllCustomers();

    } else {
      
      //restore previously selected user context
      $scope.setSelectedUserContext(currentUserSelectedContext);
    }

    loadInvoices($scope.invoiceParams).then(setInvoices);
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

    // if filter view is selected, set given filter
    if ($scope.selectedFilterView) {
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

  $scope.selectFilterView = function (filterView) {
    $scope.selectedFilterView = filterView;

    $scope.invoiceParams.from = 0;

    loadInvoices($scope.invoiceParams).then(setInvoices);
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
  setPageText();
  
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
