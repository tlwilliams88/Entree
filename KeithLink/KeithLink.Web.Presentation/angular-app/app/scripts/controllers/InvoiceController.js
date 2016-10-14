  'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$q', '$scope', '$filter', '$modal', 'Constants', 'InvoiceService', '$rootScope', 'DateService', 'LocalStorage', 'CartService', 'CustomerService', '$state', 'PagingModel', 'BankAccountService', 'blockUI',
    function ($q, $scope, $filter, $modal, Constants, InvoiceService, $rootScope, DateService, LocalStorage, CartService, CustomerService, $state, PagingModel, BankAccountService, blockUI) {

  CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
  });

  $scope.loadingResults = true;

  var currentUserSelectedContext = {};
  var customers = [];
  $scope.selectedFilterViewName;
  $scope.errorMessage = '';
  $scope.invoiceCustomers = {};
  $scope.areAllSelected = false;
  $scope.selectedSortParameter = 'Invoice Date';
  $scope.sortParametervalue = 'invoicedate';
  $scope.sortDirection = 'Asc';

  $scope.invoiceCustomerContexts = [{
    text: 'All Customers',
    isViewingAllCustomers: true
  }, {
    text: $scope.selectedUserContext.customer.customerNumber + ' - ' + $scope.selectedUserContext.customer.customerName,
    isViewingAllCustomers: false
  }];
  $scope.selectedInvoiceContext = $scope.invoiceCustomerContexts[1];
 
  $scope.currDate = DateService.momentObject().format(Constants.dateFormat.yearMonthDayDashes);

  if(DateService.momentObject().utc().format(Constants.dateFormat.hourMinuteSecond) < 190000){
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
    InvoiceService.getAllOpenInvoices,
    setInvoices,
    stopLoading,
    startLoading
  );

  if(!InvoiceService.selectedFilterView){
    $scope.selectedFilterView = InvoiceService.selectedFilterView = $scope.filterViews[0];
  }
  retrieveFilter();

  // Fixes dropdown touch issue on mobile
  $('body').on('click', '.dateRangeDropdown', function (e) { 
    e.stopPropagation(); 
  });

  $('body').on('click', '.dropdown-dateRange', function() {
    $('.dropdown').removeClass('open');
  });

  function calculateInvoiceFields(customers) {
    customers.forEach(function(customer) {
      customer.haspayableinvoices = false;
      customer.invoices.results.forEach(function(invoice){
      // determine which invoices are payable
      if (invoice.pendingtransaction && invoice.pendingtransaction.editable) {
          $scope.canpayinvoice = true;  
          invoice.userCanPayInvoice = true;
          customer.haspayableinvoices = true;
          invoice.paymentAmount = invoice.pendingtransaction.amount;
          invoice.date = invoice.pendingtransaction.date.substr(0,10); // get format '2014-01-31'
      } else if (invoice.ispayable) {
        $scope.canpayinvoice = true;
        invoice.userCanPayInvoice = true;
        customer.haspayableinvoices = true;
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
    });
  }

  function setInvoices(data) {
    $scope.totalInvoices = data.customerswithinvoices.totalinvoices;
    $scope.hasPayableInvoices = true;
    $scope.totalAmountDue = data.totaldue;

    data.customerswithinvoices.results.forEach(function(customer){
      customer.invoices.results.forEach(function(invoice){
        defaultAccount(invoice);
      });
    });

    calculateInvoiceFields(data.customerswithinvoices.results);
    $scope.loadingResults = false;
    $scope.invoices = data.customerswithinvoices.results;
    $scope.invoices.forEach(function(customer){
      customer.invoices.results.forEach(function(invoice){
        invoice.amountdue = invoice.amount;
      });
    });
    if($scope.invoices.length){
      $scope.invoices.forEach(function(invoice){
        invoice.failedBatchValidation = false;       
      });
    }
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
    blockUI.stop();
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
    if(!input && filter) {
      $scope.invoices.forEach(function(customer){
        customer.invoices.results = $filter('filter')(customer.invoices.results, {hascreditmemos: true});
        customer.invoices.totalResults = customer.invoices.results.length;
      });
    } else if(input && filter == "invoicenumber") {
      invoicesFilter = [{
        filter: {
          field: filter,
          value: input
        }
      }];
      loadFilteredInvoices(invoicesFilter[0]);
    } else if(input && filter != "invoicenumber") {
      invoicesFilter = [{
        search: {
          field: filter,
          value: input
        }
      }];
      loadFilteredInvoices(invoicesFilter[0]);
    } else {
      blockUI.start('Loading Invoices...').then(function(){
        invoicePagingModel.loadData();
      });
    }
  };

  function loadFilteredInvoices(filter){
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
    $scope.filterRowFields = InvoiceService.filterRowFields = {};
    getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterViewName);    
    invoicePagingModel.loadData();
    $scope.showFilter = false;
    });
  };

  $scope.selectFilterView = function (filterView, rangeYear, rangeMonth) {
    invoicePagingModel.clearFiltersWithoutReload();
    if($scope.selectedFilterViewName === filterView && !rangeYear){
      return;
    } else if(rangeYear) {
      var dateFilterView = [{
        name: 'Invoices By Month',
        daterange: {
          field: 'yearmonth',
          value: rangeYear + ',' + ($scope.dateRangeMonths.indexOf(rangeMonth) + 1)
        }
      }];

      if($scope.selectedFilterViewName == dateFilterView){
        return;
      } else {
        blockUI.start('Loading Invoices...').then(function(){
          $scope.errorMessage = '';
          $scope.selectedFilterViewName = rangeMonth + ', ' + rangeYear;
          invoicePagingModel.setAdditionalParams(dateFilterView[0]);
          loadFilteredInvoices(dateFilterView[0]);
        });
      }
    } else {
      blockUI.start('Loading Invoices...').then(function(){
      $scope.errorMessage = '';
      $scope.selectedFilterViewName = filterView.name;
      loadFilteredInvoices(filterView);
      });
    }

  };

  $scope.sortInvoices = function(sortDirection, sortField) {
    var sortDescending;

    if(sortDirection === 'Asc'){
      sortDescending = true;
      $scope.sortDirection = 'Desc';
    } else {
      sortDescending = false;
      $scope.sortDirection = 'Asc';
    }

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
    $scope.viewingAllCustomers = true;
    $scope.selectedInvoiceContext = invoiceContext;
     $scope.errorMessage = '';
    
    // clear values to reset page
    $scope.invoices = [];
    $scope.totalInvoices = 0;
    $scope.filterRowFields = {};

    if ($scope.viewingAllCustomers) {

      $scope.selectedFilterView = $scope.filterViews[0]; // default to Open Invoices filter view
      setTempContextForViewingAllCustomers();
      blockUI.start('Loading Invoices...').then(function(){
        invoicePagingModel.getData = InvoiceService.getAllOpenInvoices;
      });
    } else {
  
      //restore previously selected user context
      $scope.setSelectedUserContext(currentUserSelectedContext);
      invoicePagingModel.getData = InvoiceService.getInvoices;
    }

    getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView);
    invoicePagingModel.loadData();
  };
  $scope.setViewingAllCustomers();

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
        $scope.selectInvoice(invoice , true);
        break;
    }
  };

  $scope.selectInvoice = function(invoice, isSelected){
    if (isSelected) {
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
    $scope.invoiceForm.$setDirty();
  };

  $scope.selectAll = function (fromLocation, customer, $event) {
    if(!$scope.areAllSelected && $scope.errorMessage){
      $scope.errorMessage = '';
    }
    if(fromLocation === 'selectAllInvoices'){
      $scope.collapsed = !$scope.collapsed;
      $scope.expandcollapseAll($scope.collapsed);
      angular.forEach($scope.invoices, function (customer, index) {
        customer.selected = !customer.selected;
        angular.forEach(customer.invoices.results, function(invoice, index){
          if(invoice.userCanPayInvoice){
            invoice.isSelected = customer.selected;
            if (invoice.amountdue != 0) {
              $scope.selectInvoice(invoice, invoice.isSelected);
            }
          }
        });
      });
    }else{
      $event.stopPropagation();
      customer.invoices.results.forEach(function(invoice){
        if(invoice.userCanPayInvoice){
          invoice.isSelected = customer.selected;
          if (invoice.amountdue != 0) {
            $scope.selectInvoice(invoice, invoice.isSelected);
          }
        }
      });
    }
  };

  $scope.totalPaymentAmount = function () {    
    var total = 0;
    if($scope.invoices.length ){
      $scope.invoices.forEach(function (customer) {
        if(customer.invoices.results && customer.invoices.results.length){
          customer.invoices.results.forEach(function (invoice){
            if (invoice.isSelected) {
              total += parseFloat(invoice.paymentAmount || 0);
            }
          });
        }
      });
    }
    if($scope.total !== total && (total !==0 || $scope.total)){
      $scope.validateBatch();
    }
    $scope.total = total;
    return total;
  };

  $scope.expandcollapseAll = function(state){
    for (var i=0; i<$scope.invoices.length; i++) {
      $scope.invoices[i].isOpen=state;
    }
  };

  $scope.getSelectedInvoices = function(customers, callback) {
    var invoices = [];
    var deferredPromises = [];
    if(customers){
      customers.forEach(function(customer){
        customer.invoices.results.forEach(function(invoice){
          var deferred = $q.defer();
          if(invoice.isSelected){
            invoices.push(invoice);
          }
          deferred.resolve(invoice);
          deferredPromises.push(deferred.promise);
        });
      });
    } else {
      $scope.invoices.forEach(function(customer){
        customer.invoices.results.forEach(function(invoice){
          if(invoice.isSelected){
            invoices.push(invoice);
          }
        });
      });
      return invoices;
    }
    if(callback){
      $q.all(deferredPromises).then(function(){
        return callback(invoices);
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
          $scope.selectFilterView($scope.filterViews[1]);
        }else{
          return;
        }
    });
  };

  var processingPayInvoices = false;
  $scope.payInvoices = function () {
    if (!processingPayInvoices) {
      $scope.errorMessage = '';
      processingPayInvoices = true;
      var payments = $scope.getSelectedInvoices($scope.invoices, function(payments){
        payments = $scope.defaultDates(payments);
        processingPayInvoices = false;
        InvoiceService.checkTotals(payments).then(function(resp) {
          if(resp.successResponse.isvalid){  
            $scope.errorMessage = '';
            $scope.invoices.forEach(function(invoice){
              invoice.failedBatchValidation = false;
            });
            payments.forEach(function(payment){  
              if(payment.date.length !== 10){
              payment.date = DateService.momentObject(payment.date.substr(0,10)).subtract(1, 'd').format(Constants.dateFormat.yearMonthDayDashes);
              }
            });
          } else{
            $scope.displayValidationError(resp);          
            processingPayInvoices = false;
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
    $scope.invoices.forEach(function(invoice){
      invoice.failedBatchValidation = false;
    });
  };
  
  $scope.displayValidationError = function(resp){
    $scope.errorMessage = resp.errorMessage || 'There was an issue processing your payment. Please contact your DSR or Ben E. Keith representative.';
    $scope.invoices.forEach(function(invoice){
       invoice.failedBatchValidation = false;
     });
    resp.successResponse.transactions.forEach(function(transaction){
      $scope.invoices.forEach(function(invoice){       
        var invoiceDate = invoice.date || $scope.tomorrow;
        if(invoice.pendingtransaction || invoice.date){
          invoiceDate = invoice.date || invoice.pendingtransaction.date;
        }
        if(transaction.account === invoice.account && 
          transaction.customernumber === invoice.customernumber && 
          transaction.branchid === invoice.branchid && 
          DateService.momentObject(resp.successResponse.transactions[0].date,Constants.dateFormat.yearMonthDayHourMinuteSecondDashes).format(Constants.dateFormat.yearMonthDay) === DateService.momentObject(invoiceDate.substr(0,10)).format(Constants.dateFormat.yearMonthDay) && 
          (invoice.isSelected || invoice.statusdescription === 'Payment Pending')){
          invoice.failedBatchValidation = true;
        }
      }); 
    });
    $scope.invoices.forEach(function(invoice){
      if(!invoice.failedBatchValidation){
        invoice.failedBatchValidation = false;
      }
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
            paging: {
              filter: getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView)
            }
          };
        }
      }
    });
  };
}]);
