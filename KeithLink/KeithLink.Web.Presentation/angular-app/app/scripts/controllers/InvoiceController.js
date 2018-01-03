  'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$q', '$scope', '$filter', '$modal', 'Constants', 'InvoiceService', '$rootScope', 'DateService', 'LocalStorage', 'CartService', 'CustomerService', '$state', 'PagingModel', 'BankAccountService', 'blockUI', 'canPayInvoices',
    function ($q, $scope, $filter, $modal, Constants, InvoiceService, $rootScope, DateService, LocalStorage, CartService, CustomerService, $state, PagingModel, BankAccountService, blockUI, canPayInvoices) {

  CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
  });

  $scope.loadingResults = true;

  var currentUserSelectedContext = {};
  var customers = [];
  $scope.errorMessage = '';
  $scope.invoiceCustomers = {};
  $scope.areAllSelected = false;
  $scope.selectedSortParameter = 'Invoice Date';
  $scope.sortParametervalue = 'invoicedate';
  $scope.sortDirection = 'Asc';
  $scope.userCanPayInvoice = canPayInvoices;
  $scope.collapsed = false;
  $scope.customerTotalToPay = [];

  $scope.invoiceCustomerContexts = [{
    text: 'All Customers',
    isViewingAllCustomers: true
  }, {
    text: $scope.selectedUserContext.customer.customerNumber + ' - ' + $scope.selectedUserContext.customer.customerName,
    isViewingAllCustomers: false
  }];
  $scope.selectedInvoiceContext = $scope.invoiceCustomerContexts[1];
 
  $scope.currDate = DateService.momentObject().format(Constants.dateFormat.yearMonthDayDashes);

  if(DateService.momentObject().utc().format(Constants.dateFormat.hourMinuteSecond) < 200000){
    $scope.mindate = DateService.momentObject($scope.currDate);
  }
  else{
    $scope.mindate = DateService.momentObject($scope.currDate).add(1,'d');
  }
  $scope.tomorrow = $scope.mindate.format(Constants.dateFormat.yearMonthDayDashes);

  $scope.datepickerOptions = {
    minDate: $scope.mindate,
    options: {    
      showWeeks: false,
      defaultDate: false
    }
  };

  // Setting Date Range Years
  var dateRangeStartYear = 2014,
      dateRangeEndYear = parseInt(DateService.momentObject().format(Constants.dateFormat.year));
  $scope.dateRangeYears = [];

  while(dateRangeEndYear >= dateRangeStartYear){
    $scope.dateRangeYears.push(dateRangeStartYear);
    dateRangeStartYear ++;
  }

  $scope.dateRangeMonths = moment.months();

  //Scope variable for credit memo filter on invoices page
  $scope.invoiceFilter = false;
    
  // different filter views for users to choose in the header dropdown
  $scope.filterViews = [{
    name: 'Open Invoices',
    filterFields: {
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
  }];

  $scope.invoiceFilters = [{
    name: 'Invoice #',
    field: 'invoicenumber'
  }, {
    name: 'PO Number',
    field: 'ponumber'
  }, {
    name: 'Invoice Type',
    field: 'typedescription'
  }];

  $scope.selectedInvoiceFilter = $scope.invoiceFilters[0];

  if(!InvoiceService.selectedFilterView){
    $scope.selectedFilterView = InvoiceService.selectedFilterView = $scope.filterViews[0];
    $scope.selectedFilterViewName = $scope.selectedFilterView.name;
  }

  $scope.selectInvoiceFilter = function(filter){
    $scope.selectedInvoiceFilter = [{
      name:filter.name,
      field:filter.field
    }];

    $scope.selectedInvoiceFilter = $scope.selectedInvoiceFilter[0];
  };

  $scope.selectSortParameter = function(parametername, parametervalue){
    $scope.selectedSortParameter = parametername;
    $scope.sortParametervalue = parametervalue;
    $scope.sortDirection = 'Asc';
    $scope.sortInvoices($scope.sortDirection, parametervalue);
  };

  $scope.sortParameters = [{
    name: 'Invoice Date',
    value: 'invoicedate'
  }, {
    name: 'Due Date',
    value: 'duedate'
  }, {
    name: 'Status',
    value: 'statusdescription'
  }, {
    name: 'PO Number',
    value: 'ponumber'
  }, {
    name: 'Invoice Amount',
    value: 'invoiceamount'
  }, {
    name: 'Amount Due',
    value: 'amount'
  }];

  var invoicePagingModel = new PagingModel( 
    InvoiceService.getAllCustomersWithInvoices,
    setCustomers,
    stopLoading,
    startLoading
  );

  // Fixes dropdown touch issue on mobile
  $('body').on('click', '.dateRangeDropdown', function (e) { 
    e.stopPropagation(); 
  });

  $('body').on('click', '.dropdown-dateRange', function() {
    $('.dropdown').removeClass('open');
  });

  function calculateInvoiceFields(invoices) {
    invoices.forEach(function(invoice) {
      // determine which invoices are payable
      if (invoice.pendingtransaction && invoice.pendingtransaction.editable) {
          $scope.canpayinvoice = true;  
          invoice.userCanPayInvoice = true;
          invoice.paymentAmount = invoice.pendingtransaction.amount;
          invoice.date = invoice.pendingtransaction.date.substr(0,10); // get format '2014-01-31'
      } else if (invoice.ispayable) {
        $scope.canpayinvoice = true;
        invoice.userCanPayInvoice = true;
      } else {
        return;
      }

      // calculate max payment date
      var date = {};
      if(invoice.amount < 0){
        date = DateService.momentObject(invoice.duedate.substr(0,10)).add(1, 'year');
      }
      else{
        date = DateService.momentObject(invoice.duedate.substr(0,10)).add(1, 'd');
      }
       invoice.maxPaymentDate = date.format(Constants.dateFormat.yearMonthDayDashes);
    });
  }
  
  var customerDetailsKey;
  function setCustomers(data) {
      $scope.customers = data.customers;
      
      $scope.customers.forEach(function(customer) {
          var isOpen = false,
              currentCustomer = customer;
          
          customerDetailsKey = getCustomerDetailsKey(currentCustomer.customerNumber, currentCustomer.customerBranch);
              
          $scope.customerTotalToPay[customerDetailsKey] = 0;
              
          Object.defineProperties(customer, {
              'isOpen': {
                  get: function() {
                    return isOpen;
                  },
                  set: function(newValue) {
                    isOpen = newValue;
                    if (isOpen) {
                      $scope.getInvoicesForCustomer(customer);
                    }
                  }
              },
              'currentCustomer': {
                  value: currentCustomer
              },
              'customerDetails': {
                  value: customerDetailsKey
              }
          });
        });
      
      $scope.totalInvoices = data.totalnumberinvoices;
      $scope.hasPayableInvoices = true;
      $scope.totalAmountDue = data.totaldue;
      
      blockUI.stop();
  }
  
  function getCustomerDetailsKey(customerNumber, customerBranch) {
      var customerDetailsString = customerNumber + '_' + customerBranch;
      return customerDetailsString;
  }
  
  $scope.getInvoicesForCustomer = function(customer){
      customerDetailsKey = getCustomerDetailsKey(customer.customerNumber, customer.customerBranch);
      var customerInvoices = $scope.invoices[customerDetailsKey] || [];
      
      if(customer.isOpen == true && customerInvoices && customerInvoices.length == 0) {
          $scope.customerNumberAndBranch = {
              customerNumber: customer.customerNumber,
              customerBranch: customer.customerBranch
          }
          setTempContextForViewingAllCustomers();
          blockUI.start('Loading Invoices...').then(function(){
              invoicePagingModel.setCustomerNumber($scope.customerNumberAndBranch.customerNumber);
              invoicePagingModel.setCustomerBranch($scope.customerNumberAndBranch.customerBranch);
              invoicePagingModel.getData = InvoiceService.getCustomerInvoices;
              invoicePagingModel.setData = setInvoices;
              
              getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);
              invoicePagingModel.loadData();
          });
      } else {
          return false;
      }

  };

  function setInvoices(invoices) {
    $scope.invoices[customerDetailsKey] = invoices;

    $scope.invoices[customerDetailsKey].forEach(function(invoice){
        defaultAccount(invoice);
        
        invoice.amountdue = invoice.amount;
        
        invoice.failedBatchValidation = false;  
    });

    calculateInvoiceFields($scope.invoices[customerDetailsKey]);
    $scope.loadingResults = false;
    
    blockUI.stop();

  }

  //Combines bank account information with customers and invoices received from api
  function customerBanks(customers, callback){
    $scope.customersWithInvoices = [];
    var deferredPromises = [];
    customers.forEach(function(customer, index){
      customer.invoices.results.forEach(function(invoice){
        var deferred = $q.defer();
        BankAccountService.getAllBankAccounts(invoice.customernumber, invoice.branchid).then(function(banks){
          invoice.banks = banks;
          defaultAccount(invoice);
          deferred.resolve(invoice);
        });
        deferredPromises.push(deferred.promise);
      });
      $scope.customersWithInvoices.push(customer);
    });
    $q.all(deferredPromises).then(function(){
      return callback($scope.customersWithInvoices);
    });
  }

  function retrieveFilter() {
    if(InvoiceService && InvoiceService.filterRowFields){
      $scope.showFilter = true;
      $scope.filterRowFields =InvoiceService.filterRowFields;
      $scope.selectedFilterView = InvoiceService.selectedFilterView;
    }
    else{
      if(InvoiceService && InvoiceService.selectedFilterView){
      $scope.selectedFilterView = InvoiceService.selectedFilterView;
      }
    }
    $scope.selectedFilterViewName = $scope.selectedFilterView.name;
  }
  retrieveFilter();

  function appendInvoices(data) {
    $scope.invoices = $scope.invoices.concat(data.pagedresults.results);
    calculateInvoiceFields(data.pagedresults.results);
  }

  function startLoading() {
    $scope.loadingResults = true;
  }

  function stopLoading() {
    $scope.loadingResults = false;
    blockUI.stop();
  }

  function getInvoicesFilterObject(filterFields, filterView) {  
    var filter = invoicePagingModel.getFilterObject(filterFields, filterView.filterFields);
    if (filterView.filterFields) {
      if (filter) {
        filter.filter.push(filterView.filterFields);
      } else {
        filter = filterView.filterFields;
      }
    }
    invoicePagingModel.filter = filter;
    invoicePagingModel.pageIndex = 0;

    $scope.invoicesFilters = filter ? filter : filterView;

    return filter;
  }
  
  $scope.openTransactionSummaryModal = function(invoice) {
      var modalInstance = $modal.open({
      templateUrl: 'views/modals/invoicetransactionsummarymodal.html',
      controller: 'InvoiceTransactionSummaryModalController',
      windowClass: 'color-background-modal',
      scope: $scope,
      resolve: {
        invoiceNumber: function() {
          return invoice.invoicenumber;
        },
        customerNumber: function() {
          return invoice.customernumber;
        },
        branchId: function() {
          return invoice.branchid;
        }
      }
    });
  };

  $scope.filterInvoices = function(filter, input) {      
    var invoicesFilter;
    
    $scope.invoices = [];
    $scope.searchFilter = {
        field: filter,
        value: input
    };

    if(input && filter == 'hascreditmemos') {
      var invoiceFilterInput = document.getElementById('invoiceFilterInput');
      if(invoiceFilterInput){
        invoiceFilterInput.value = '';
      }

      if($scope.selectedFilterView.filterFields || $scope.selectedFilterView[0]){

        if($scope.selectedFilterViewName == 'Open Invoices' || $scope.selectedFilterViewName == 'Past Due Invoices' || $scope.selectedFilterViewName == 'Invoices Pending Payment'){
          invoicesFilter = $filter('filter')($scope.filterViews, {name: $scope.selectedFilterViewName});
          constructInvoiceFilterObject(invoicesFilter[0].filterFields, true);

        } else {

          constructInvoiceFilterObject($scope.selectedFilterView[0], true, true);

        }
      }
    } else if(input && filter == 'invoicenumber') {

      invoicesFilter = [{
        filter: {
          field: filter,
          value: input
        }
      }];
      loadFilteredInvoices(invoicesFilter[0]);

    } else if(input && filter != 'invoicenumber') {

      invoicesFilter = [{
        search: {
          field: filter,
          value: input
        }
      }];
      loadFilteredInvoices(invoicesFilter[0]);

    } else {
      blockUI.start('Loading Invoices...').then(function(){

        invoicePagingModel.additionalParams = {};
        if($scope.selectedFilterView[0]){
          loadFilteredInvoices($scope.selectedFilterView[0]);
        } else {
          loadFilteredInvoices($scope.selectedFilterView);
        }
        
      });
    }
  };

  function constructInvoiceFilterObject(statusfilter, searchfilter, datefilter){
    var invoicesFilter;
    if(searchfilter && !datefilter){
      invoicesFilter = [{
        filterFields: statusfilter,
        search: $scope.searchFilter
      }];
  } else if(searchfilter && datefilter && statusfilter && statusfilter.filter != undefined){
      invoicesFilter = [{
        daterange: statusfilter.daterange,
        filter: statusfilter.filter,
        search: $scope.searchFilter
      }];
    } else if(searchfilter && datefilter && statusfilter.filter == undefined){
      invoicesFilter = [{
        daterange: statusfilter.daterange,
        search: $scope.searchFilter
      }];
    }

    loadFilteredInvoices(invoicesFilter[0]);
  }

  function loadFilteredInvoices(filter){
    invoicePagingModel.setCustomerNumber(null);
    invoicePagingModel.setCustomerBranch(null);
    invoicePagingModel.getData = InvoiceService.getAllCustomersWithInvoices;
    invoicePagingModel.setData = setCustomers;
    
    InvoiceService.setFilters(filter, $scope.filterRowFields);
    getInvoicesFilterObject($scope.filterRowFields, filter);
    invoicePagingModel.setAdditionalParams(filter);
    blockUI.start('Loading Invoices...').then(function(){
      invoicePagingModel.loadData().then(function() {
        stopLoading();
      });
    });
  }

  $scope.clearFilters = function(filter, from) {
    if(from == 'selectFilterView'){
      $scope.filterRowFields = InvoiceService.filterRowFields = {};
    }
    blockUI.start('Clearing Filter...').then(function(){
    if(filter){
      $('#invoiceFilterInput').val('');
    }
    invoicePagingModel.clearFiltersWithoutReload();
    var hasCreditMemos = document.getElementById('invoiceHasCreditMemos');
    if(hasCreditMemos){
      hasCreditMemos.checked = false;
    }
    var invoiceFilterInput = document.getElementById('invoiceFilterInput');
    if(invoiceFilterInput){
      invoiceFilterInput.value = '';
      $scope.invoices = [];
      $scope.selectedFilterView = $scope.filterViews[0];
      
      invoicePagingModel.setCustomerNumber(null);
      invoicePagingModel.setCustomerBranch(null);
      invoicePagingModel.getData = InvoiceService.getAllCustomersWithInvoices;
      invoicePagingModel.setData = setCustomers;
    }
    $scope.filterRowFields = InvoiceService.filterRowFields = {};
    getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);    
    invoicePagingModel.loadData();
    $scope.showFilter = false;
    });
  };

  $scope.selectFilterView = function (filterView, rangeYear, rangeMonth) {
    $scope.invoices = [];
    var hasCreditMemos = document.getElementById('invoiceHasCreditMemos');
    if(hasCreditMemos){
      hasCreditMemos.checked = false;
    }
    var invoiceFilterInput = document.getElementById('invoiceFilterInput');
    if(invoiceFilterInput){
      invoiceFilterInput.value = '';
    }
    invoicePagingModel.clearFiltersWithoutReload();
    if($scope.selectedFilterViewName === filterView && !rangeYear){
      return;
    } else if(rangeYear) {
      var dateFilterView;
      dateFilterView = [{
        name: 'Invoices By Month',
        daterange: {
          field: 'yearmonth',
          value: rangeYear + ',' + ($scope.dateRangeMonths.indexOf(rangeMonth) + 1)
        }
      }];

      if(filterView == 'Paid Date Range'){
        dateFilterView = [{
          filter: {
            field: 'statusdescription',
            value: 'Paid'
          },
          daterange: {
          field: 'yearmonth',
          value: rangeYear + ',' + ($scope.dateRangeMonths.indexOf(rangeMonth) + 1)
        }
        }];
      }

      if($scope.selectedFilterViewName == dateFilterView){
        return;
      } else {
        blockUI.start('Loading Invoices...').then(function(){
          $scope.errorMessage = '';
          $scope.selectedFilterViewName = rangeMonth + ', ' + rangeYear;
          $scope.selectedFilterView = dateFilterView;
          invoicePagingModel.setAdditionalParams(dateFilterView[0]);
          loadFilteredInvoices(dateFilterView[0]);
        });
      }
    } else {
      blockUI.start('Loading Invoices...').then(function(){
      $scope.errorMessage = '';
      $scope.selectedFilterViewName = filterView.name;
      $scope.selectedFilterView = filterView;
      loadFilteredInvoices(filterView);
      });
    }

  };

  var sortDescending = false;
  $scope.sortInvoices = function(sortDirection, sortField) {
    sortDescending = !sortDescending;

    $scope.sortDirection = sortDirection == 'Asc' ? 'Desc' : 'Asc';

    blockUI.start('Sorting Invoices...').then(function(){
      $scope.sort = {
        field: sortField,
        sortDescending: sortDescending
      };
      invoicePagingModel.sortData($scope.sort);
    });

  };

  $scope.setDateSortValues = function(invoice){
    if((invoice.userCanPayInvoice || (invoice.statusdescription === 'Payment Pending' || invoice.statusdescription === 'Past Due')) && invoice.date){
      return invoice.date.substr(0,10);
    }
    if(invoice.userCanPayInvoice && invoice.statusdescription === 'Past Due' && !invoice.date){
      return $scope.tomorrow;
    }
    if(invoice.statusdescription === 'Payment Pending' && !invoice.date && invoice.pendingtransaction){
      return invoice.pendingtransaction.date.substr(0,10);
    }
  };

  $scope.sortByScheduleDate = function(ascendingDate) {
        $scope.sort = {
      field: 'scheduledate'
    };

   $scope.invoices = $scope.invoices.sort(function(obj1, obj2){
        var sorterval1 = DateService.momentObject($scope.setDateSortValues(obj1));
        var sorterval2 = DateService.momentObject($scope.setDateSortValues(obj2));

        $scope.ascendingDate = !ascendingDate;    
        if(!sorterval1){
          sorterval1 = 0;
        }
        if(!sorterval2){
          sorterval2 = 0;
        }
        if(ascendingDate){      
          return sorterval1 - sorterval2;
        }
        else{
          return sorterval2 - sorterval1;
        }   
   });
  };


  /************
  VIEWING INVOICES FOR ALL CUSTOMERS
  ************/

  function setTempContextForViewingAllCustomers() {
    var tempContext;
    //store current user context temporarily
    currentUserSelectedContext = $scope.selectedUserContext;
    //wipe user context and replace text with all customers
    if(!$scope.isInternalUser){
      tempContext = {
        text: 'All Customers'
      };
      $scope.setSelectedUserContext(tempContext);
    } else {
      $scope.setSelectedUserContext(currentUserSelectedContext);
    }
  }

  //toggles state between all customer invoices and single customer invoices
  $scope.setViewingAllCustomers = function (invoiceContext) {
    $scope.viewingAllCustomers = $scope.selectedInvoiceContext.isViewingAllCustomers;
    // $scope.selectedInvoiceContext = invoiceContext;
     $scope.errorMessage = '';
    
    // clear values to reset page
    $scope.invoices = {};
    $scope.totalInvoices = 0;
    $scope.filterRowFields = {};

    // if ($scope.viewingAllCustomers) {

      $scope.selectedFilterView = $scope.filterViews[0]; // default to Open Invoices filter view
      setTempContextForViewingAllCustomers();
      blockUI.start('Loading Customers...').then(function(){
        invoicePagingModel.getData = InvoiceService.getAllCustomersWithInvoices;
      });


    getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);
    invoicePagingModel.loadData();
  };
  $scope.setViewingAllCustomers();

  function changeUserContext(stateName, stateParams, customerNumber, customerBranch) {
    //generate and set customer context to customerNumber that user selected
    CustomerService.getCustomerDetailsKey(customerNumber, customerBranch).then(function (customer) {
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

  $scope.selectAccount = function (invoice, account) {
    if(invoice.account !== account.accountNumber){
      invoice.account = account.accountNumber;
      invoice.accountName = account.name;
      if(!invoice.isSelected){
        invoice.isSelected = true;
        $scope.selectInvoice(invoice, true);
      }
      $scope.validateBatch();
    } 
  };

 $scope.toggleSelect = function(invoice,type){
    switch(type) {
      case 'amount':
        if (invoice.paymentAmount && invoice.paymentAmount.toString() !== '0'){ // jshint ignore:line
          invoice.isSelected = true;
        } else {
          if(invoice.statusdescription === 'Payment Pending' && invoice.paymentAmount && invoice.paymentAmount.toString() !== '0'){
          invoice.isSelected = true;
          $scope.validateBatch();
          }
          else{
            invoice.isSelected = false;
          }          
        }
        if(invoice.pendingtransaction && invoice.pendingtransaction.amount == invoice.paymentAmount){ // jshint ignore:line
          invoice.isSelected = false;
        }
        break;
      case 'account':   
      case 'date':
        invoice.isSelected = true;
        break;
    }
    
    $scope.selectInvoice(invoice , true);
  };

  $scope.selectInvoice = function(invoice){
    if (invoice.isSelected) {
      if (!invoice.pendingtransaction) {
        invoice.amount = invoice.amountdue;
        if(!invoice.paymentAmount){
          invoice.paymentAmount = invoice.amount.toString();
        }
      } else if (invoice.pendingtransaction && (invoice.paymentAmount == '0.00' || invoice.paymentAmount == '0' || invoice.paymentAmount == undefined)) {
        invoice.paymentAmount = invoice.pendingtransaction.amount;
      }
    } else {
      invoice.failedBatchValidation = false;
      if(invoice.pendingtransaction){
        invoice.paymentAmount = invoice.pendingtransaction.amount;
      } else {
        invoice.paymentAmount = '';
      }
      
    }
    
    calculateTotalForCustomer(invoice);
    $scope.invoiceForm.$setDirty();
  };
  
  function calculateTotalForCustomer(invoice) {
      var customerInvoices = $scope.invoices[getCustomerDetailsKey(invoice.customernumber, invoice.branchid)] || [],
          total = 0;
          
      customerInvoices.forEach(function(invoice) {
          if(invoice.isSelected) {
              total += parseFloat(invoice.paymentAmount || 0);
          }
      });
      
      $scope.customerTotalToPay[getCustomerDetailsKey(invoice.customernumber, invoice.branchid)] = total;
  }

  $scope.selectAll = function (customer, $event) {    
    $event.stopPropagation();
    var customerInvoices = $scope.invoices[getCustomerDetailsKey(customer.customerNumber, customer.customerBranch)];
    customerInvoices.forEach(function(invoice){
    if(invoice.userCanPayInvoice && !($scope.selectedFilterViewName != 'Invoices Pending Payment' && invoice.statusdescription == 'Payment Pending')){
      invoice.isSelected = customer.selected;
      if (invoice.amountdue != 0) {
        $scope.selectInvoice(invoice, invoice.isSelected);
      }
    }
    });
  };

  $scope.totalPaymentAmount = function () {    
    var total = 0;
    $scope.customerTotal = [];
    
    $scope.customers.forEach(function(customer) {
        if($scope.customerTotalToPay[getCustomerDetailsKey(customer.customerNumber, customer.customerBranch)]) {
            total += parseFloat($scope.customerTotalToPay[getCustomerDetailsKey(customer.customerNumber, customer.customerBranch)] || 0);
        }
    });
    
    if($scope.total !== total && (total !==0 || $scope.total)){
      $scope.validateBatch();
    }
    
    $scope.total = total;
    return total;
  };

  $scope.getSelectedInvoices = function(invoices, callback) {
    var selectedInvoices = [];
    var deferredPromises = [];
    if(Object.keys($scope.invoices).length > 0){
        Object.keys($scope.invoices).forEach(function(key) {
            $scope.invoices[key].forEach(function(record){
              if(record.isSelected){
                  var deferred = $q.defer();
                  selectedInvoices.push(record);
                  
                  deferred.resolve(record);
                  deferredPromises.push(deferred.promise);
              }
            });
      });
    } else {
        Object.keys($scope.invoices).forEach(function(key) {
            $scope.invoices[key].forEach(function(record){
                  if(record.isSelected){
                    selectedInvoices.push(record);
                  }
            });
      });
      return selectedInvoices;
    }
    if(callback){
      $q.all(deferredPromises).then(function(){
        return callback(selectedInvoices);
      });
    }
  };

  $scope.defaultDates = function(payments){
    if(payments && payments.length){
      payments.forEach(function(payment){
        if((payment.statusdescription === 'Payment Pending') || (payment.statusdescription === 'Past Due' && payment.amount < 0)){
          
        if(payment.statusdescription === 'Payment Pending' && !payment.date && payment.pendingtransaction && payment.pendingtransaction.date){
             payment.date = DateService.momentObject(payment.pendingtransaction.date,Constants.dateFormat.yearMonthDayHourMinuteSecondDashes).format(Constants.dateFormat.yearMonthDayDashes);
            }                       
        }
        
        if(!payment.date){
          payment.date = $scope.tomorrow;          
        }

        if(payment.date.length !== 10){
          payment.date = DateService.momentObject(payment.date,Constants.dateFormat.yearMonthDayDashes)._i;
        }

      });
    }
      return payments;
  };

  function defaultAccount(invoice){
      if(invoice.statusdescription !== 'Paid' && !invoice.account && invoice.banks.length){
        if(invoice.pendingtransaction){
          invoice.account = invoice.pendingtransaction.account;
        }
        else{
        invoice.account = invoice.banks[0].accountNumber;
        invoice.accountName = invoice.banks[0].name;
        }
      } else {
        return;
      }
  }

  //Opens modal with payments object passed from payInvoices function
  //payments object uses getSelectedInvoices function
  $scope.openInvoiceConfirmation = function(payments) {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/invoiceconfirmationmodal.html',
      controller: 'InvoiceConfirmationModalController',
      backdrop: 'static',
      scope: $scope,
        resolve: {
          payments: function () {
            return payments;
          }
        }
    });
    //Closes modal window and redirects to Invoices Pending Payment view if submit payments button is used
    modalInstance.result.then(function(){
        if(modalInstance.result.$$state.value){
          $scope.invoiceForm.$setPristine();
          $scope.selectFilterView($scope.filterViews[0]);
        }else{
          return;
        }
    });
  };

  $scope.processingPayInvoices = false;
  $scope.payInvoices = function () {
    if (!$scope.processingPayInvoices) {
      $scope.errorMessage = '';
      $scope.processingPayInvoices = true;
      var payments = $scope.getSelectedInvoices($scope.invoices, function(payments){
        payments = $scope.defaultDates(payments);
        $scope.processingPayInvoices = false;
        InvoiceService.checkTotals(payments).then(function(resp) {
          if(resp.successResponse.isvalid){  
            $scope.errorMessage = '';
            Object.keys($scope.invoices).forEach(function(key) {
                    $scope.invoices[key].forEach(function(record){
                            record.failedBatchValidation = false;
                    })
                });
            payments.forEach(function(payment){  
              if(payment.date.length !== 10){
              payment.date = DateService.momentObject(payment.date.substr(0,10)).subtract(1, 'd').format(Constants.dateFormat.yearMonthDayDashes);
              }
            });
          } else{
            $scope.displayValidationError(resp);          
            $scope.processingPayInvoices = false;
          }    
        });
        $scope.openInvoiceConfirmation(payments);
      });
    }
  };

  var validationCalls = [];
  $scope.validateBatch = function(){
    $scope.validating = true;
      $scope.getSelectedInvoices($scope.invoices, function(payments){
        var availablePayments = payments;
        availablePayments = $scope.defaultDates(availablePayments);
        if(availablePayments && availablePayments.length){
          validationCalls.push('validation');
          InvoiceService.checkTotals(availablePayments).then(function(resp) {
            validationCalls.splice(0,1);

            if(validationCalls.length == 0){
              if(resp.successResponse.isvalid){
                $scope.validating = false;
                $scope.clearValidationErrors();
              } else {
                $scope.displayValidationError(resp);
              }
            }

          });
        } else {
          $scope.clearValidationErrors();
          $scope.validating = false;
        }
      });
  };

  $scope.clearValidationErrors = function(){
    $scope.errorMessage = '';
    Object.keys($scope.invoices).forEach(function(key) {
        $scope.invoices[key].forEach(function(record){
            record.failedBatchValidation = false;
        })
    });
  };
  
  $scope.displayValidationError = function(resp){
    $scope.errorMessage = resp.errorMessage || 'There was an issue processing your payment. Please contact your DSR or Ben E. Keith representative.';
    Object.keys($scope.invoices).forEach(function(key) {
        $scope.invoices[key].forEach(function(record){
            record.failedBatchValidation = false;
        })
     });
    resp.successResponse.transactions.forEach(function(transaction){
        Object.keys($scope.invoices).forEach(function(key) {
            $scope.invoices[key].forEach(function(record){
                var invoiceDate = record.date || $scope.tomorrow;
                if(record.pendingtransaction || record.date){
                  invoiceDate = record.date || record.pendingtransaction.date;
                }
                
                if(transaction.account === record.account && 
                  transaction.customernumber === record.customernumber && 
                  transaction.branchid === record.branchid && 
                  DateService.momentObject(resp.successResponse.transactions[0].date,Constants.dateFormat.yearMonthDayHourMinuteSecondDashes).format(Constants.dateFormat.yearMonthDay) === DateService.momentObject(invoiceDate.substr(0,10)).format(Constants.dateFormat.yearMonthDay) && 
                  (record.isSelected || record.statusdescription === 'Payment Pending')){
                  record.failedBatchValidation = true;
                }
            })
        })
      });
    Object.keys($scope.invoices).forEach(function(key) {
        $scope.invoices[key].forEach(function(record){
            if(!record.failedBatchValidation){
              record.failedBatchValidation = false;
            }
        });
    });
  };

  $scope.openExportModal = function () {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        location: function() {
          return {category:'Invoices', action:'Export Invoices'};
        },
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
            params: {
                size: Constants.infiniteScrollPageSize,
                from: 0,
                sort: ($scope.sort!=null) 
                      ? [{field: $scope.sort.field, 
                          order: ($scope.sort.sortDescending) ? 'desc' : 'asc'}] 
                      : null,
                filter: $scope.selectedFilterView
            }
            
          };
        },
        exportType: function() {
           return Constants.exportType.invoiceExport;
        }
      }
    });
  };
}]);
