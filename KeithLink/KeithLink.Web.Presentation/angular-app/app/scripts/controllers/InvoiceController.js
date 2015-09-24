  'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$scope', '$filter', '$modal', 'accounts', 'InvoiceService', '$rootScope', 'LocalStorage', 'CustomerService', '$state', 'PagingModel',
    function ($scope, $filter, $modal, accounts, InvoiceService, $rootScope, LocalStorage, CustomerService, $state, PagingModel) {

  var currentUserSelectedContext = {};
  $scope.errorMessage = '';

  $scope.invoiceCustomerContexts = [{
    text: 'All Customers',
    isViewingAllCustomers: true
  }, {
    text: $scope.selectedUserContext.customer.customerNumber + ' - ' + $scope.selectedUserContext.customer.customerName,
    isViewingAllCustomers: false
  }];
  $scope.selectedInvoiceContext = $scope.invoiceCustomerContexts[1];
  $scope.accounts = accounts;
 
  $scope.currDate = new Date();  
  $scope.currDate = moment($scope.currDate).format('YYYY-MM-DD');
  $scope.mindate = moment($scope.currDate).add(1,'d');
  $scope.tomorrow = moment($scope.mindate).format('YYYY-MM-DD');

  $scope.datepickerOptions = {
    minDate: $scope.mindate,
    options: {    
      showWeeks: false,
      defaultDate: false
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
      if(invoice.amount < 0){
        date = moment( date ).add(1, 'year');
      }
       invoice.maxPaymentDate = date.format('YYYY-MM-DD');
    });
  }

  function setInvoices(data) {
    $scope.invoices = data.pagedresults.results
    if($scope.invoices.length){
      $scope.invoices.forEach(function(invoice){
        invoice.failedBatchValidation = false;       
      });
    }
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
  
    $scope.openTransactionSummaryModal = function(invoice) {
        var modalInstance = $modal.open({
        templateUrl: 'views/modals/invoicetransactionsummarymodal.html',
        controller: 'InvoiceTransactionSummaryModalController',
        windowClass: 'color-background-modal',
        scope: $scope,
        resolve: {
          invoice: function() {
            return invoice;
          }
        }
      });
    };

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
    $scope.errorMessage = '';
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

  $scope.setDateSortValues = function(invoice){
    if(invoice.userCanPayInvoice && invoice.statusdescription !== 'Past Due' && invoice.statusdescription !== 'Payment Pending' && invoice.date){
      return invoice.date;
    }
    if(invoice.userCanPayInvoice && invoice.statusdescription === 'Past Due' && !invoice.selectedDate){
      return $scope.tomorrow;
    }
    if(invoice.statusdescription === 'Payment Pending' && !invoice.selectedDate){
      return invoice.date  || invoice.pendingtransaction.date;
    }  
    if((invoice.statusdescription === 'Payment Pending' || invoice.statusdescription === 'Past Due') && invoice.selectedDate){
      return invoice.selectedDate;
    }
  };

  $scope.sortByScheduleDate = function(ascendingDate) {
        $scope.sort = {
      field: 'scheduledate'
    };

   $scope.invoices = $scope.invoices.sort(function(obj1, obj2){
        var sorterval1 = moment($scope.setDateSortValues(obj1));
        var sorterval2 = moment($scope.setDateSortValues(obj2));

        $scope.ascendingDate = !ascendingDate;    
        if(!sorterval1){
          sorterval1 = 0
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
     $scope.errorMessage = '';
    
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
        if (invoice.paymentAmount && invoice.paymentAmount != 0){ // jshint ignore:line
          invoice.isSelected = true;
        } else {
          if(invoice.statusdescription === 'Payment Pending' && invoice.paymentAmount == 0){
          invoice.paymentAmount = '0.00';
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
        invoice.paymentAmount = invoice.amount.toString();
      }
    } else {
      invoice.failedBatchValidation = false;
      if (invoice.pendingtransaction) {
        invoice.paymentAmount = invoice.pendingtransaction.amount; 
      } else {
        invoice.paymentAmount = '0';  
      }
    }   
    $scope.invoiceForm.$setDirty();
  };

  $scope.selectAll = function (areAllSelected) {
    if(!areAllSelected && $scope.errorMessage){
      $scope.errorMessage = '';
    }
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
    if($scope.invoices){
    $scope.invoices.forEach(function (invoice) {
      if (invoice.isSelected) {
        total += parseFloat(invoice.paymentAmount || 0);
      }
    });
  }

  if($scope.total !== total && (total !==0 || $scope.total)){
    $scope.validateBatch();
  }
    $scope.total = total;
    return total;
  };

  $scope.getSelectedInvoices = function() {
    return $filter('filter')($scope.invoices, {isSelected: true});
  };

  $scope.defaultDates = function(payments){
    if(payments.length){
      payments.forEach(function(payment){
        if((payment.statusdescription === 'Payment Pending') || (payment.statusdescription === 'Past Due' && payment.amount < 0)){
          if(payment.date){
            payment.date = payment.date;
          }
          else if(payment.selectedDate){
            payment.date = payment.selectedDate;
          }
          else{
            if(payment.statusdescription === 'Payment Pending'){
             payment.date = moment(payment.pendingtransaction.date,"YYYY-MM-DDTHH:mm:ss").format("YYYY-MM-DD");
            }          
          }             
        }
        
        if(!payment.date){
          payment.date = $scope.tomorrow;          
        }
        if(payment.date.length !== 10){
          payment.date = moment(payment.date).format("YYYY-MM-DD");
        }  
      });
    }
       return payments
  };

  $scope.defaultAccount=function(invoice){
    if(invoice.statusdescription !== 'Paid'){
       if(!invoice.account && accounts.length){ 
          if(invoice.pendingtransaction){
            invoice.account = invoice.pendingtransaction.account;
          }
          else{
          invoice.account = accounts[0].accountNumber;
          invoice.accountName = accounts[0].name;
        }
        }
    }
  }

  var processingPayInvoices = false;
  $scope.payInvoices = function () {
    if (!processingPayInvoices) {
      $scope.errorMessage = '';
      processingPayInvoices = true;
      var payments = $scope.getSelectedInvoices();
      payments = $scope.defaultDates(payments);
      InvoiceService.checkTotals(payments).then(function(resp) {
        if(resp.successResponse.isvalid){  
          $scope.errorMessage = '';
          $scope.invoices.forEach(function(invoice){
            invoice.failedBatchValidation = false;
          });
          payments.forEach(function(payment){
            if(payment.selectedDate){
              delete payment.selectedDate;
            }
            if(payment.date.length !== 10){
            payment.date = moment(payment.date).format("YYYY-MM-DD");
          }
          })
          InvoiceService.payInvoices(payments).then(function() {
            $scope.invoiceForm.$setPristine();
            $state.go('menu.transaction');
          }).finally(function () {
             processingPayInvoices = false;
            });
        }
        else{
          $scope.displayValidationError(resp);          
          processingPayInvoices = false;
        }    
    });
    }
  };

  $scope.validateBatch = function(){
    if(!$scope.validating){
      $scope.validating = true;
      if($scope.selectedFilterView.name === 'Invoices Pending Payment'){
        var payments = $scope.invoices;
      }
      else{
         var payments = $scope.getSelectedInvoices();
       }     
      payments = $scope.defaultDates(payments);
      if(payments.length){
      InvoiceService.checkTotals(payments).then(function(resp) {

           payments.forEach(function(payment){
             if((payment.statusdescription === 'Past Due' || payment.statusdescription === 'Payment Pending') && payment.date){            
                payment.selectedDate = payment.date
               delete payment.date       
             }
           });

        if(resp.successResponse.isvalid){
          $scope.errorMessage = '';
          $scope.invoices.forEach(function(invoice){
            invoice.failedBatchValidation = false;
          });
        }
        else{  
          $scope.displayValidationError(resp);
        }        
     });
    }
    $scope.validating = false;
   }
  };
  
  $scope.displayValidationError = function(resp){
    $scope.errorMessage = resp.errorMessage || "There was an issue processing your payment. Please contact your DSR or Ben E. Keith representative.";
    $scope.invoices.forEach(function(invoice){
       invoice.failedBatchValidation = false;
     });
    resp.successResponse.transactions.forEach(function(transaction){
      $scope.invoices.forEach(function(invoice){       
        var invoiceDate = invoice.date || $scope.tomorrow;
        if(invoice.pendingtransaction || invoice.selectedDate){
          invoiceDate = invoice.selectedDate || invoice.pendingtransaction.date;
        }

        if(transaction.account === invoice.account && transaction.customernumber === invoice.customernumber && transaction.branchid === invoice.branchid && moment(resp.successResponse.transactions[0].date,"YYYY-MM-DDTHH:mm:ss").format("YYYYMMDD") === moment(invoiceDate).format('YYYYMMDD') && (invoice.isSelected || invoice.statusdescription === 'Payment Pending')){
          invoice.failedBatchValidation = true;
        }
      }); 
    })
    $scope.invoices.forEach(function(invoice){
      if(!invoice.failedBatchValidation){
        invoice.failedBatchValidation = false;
      }
    })
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
