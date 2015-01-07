'use strict';

angular.module('bekApp')
  .controller('InvoiceController', ['$scope', '$filter', '$modal', 'accounts', 'InvoiceService', 'Constants', '$rootScope', 'LocalStorage', 'CustomerService', '$state',
    function ($scope, $filter, $modal, accounts, InvoiceService, Constants, $rootScope, LocalStorage, CustomerService, $state) {

      //handles customer specific invoice loading
      function loadInvoices(params) {
        $scope.loadingResults = true;
        return InvoiceService.getInvoices(params).then(function (data) {
          $scope.loadingResults = false;
          $scope.totalInvoices = data.pagedresults.totalResults;
          $scope.hasPayableInvoices = data.haspayableinvoices;
          $scope.totaldue = '';

          return data.pagedresults.results; // return array of invoices
        });
      }

      //handles loading invoices if the user wishes to view all customers at the same time
      function loadAllOpenInvoices(params) {
        $scope.loadingResults = true;
        return InvoiceService.getAllOpenInvoices(params).then(function (data) {
          $scope.loadingResults = false;
          $scope.totalInvoices = data.pagedresults.totalResults;
          $scope.hasPayableInvoices = data.haspayableinvoices;
          $scope.totaldue = data.totaldue;

          return data.pagedresults.results; // return array of invoices
        });
      }

      function setInvoices(invoices) {
        $scope.invoices = invoices;
      }

      //set defaults and instantiate persistent user context
      $scope.headerText = 'Invoices for: ' + $scope.selectedUserContext.customer.customerName;
      $scope.viewAllButtonText = 'View Open Invoices for All Customers';
      var temporarySelectedUserContext = {};

      //toggles state between all invoices and single customer invoices
      $scope.viewAllOpenInvoices = function () {
        //properly set string values for each state and change the selected user context to a placeholder to prevent confusion
        if (!$scope.viewingAllCustomers) {
          //set button and header text
          $scope.viewAllButtonText = 'Return to Invoices for: ' + $scope.selectedUserContext.customer.customerNumber + ' - ' + $scope.selectedUserContext.customer.customerName;
          $scope.headerText = 'Open Invoices of All Customers';

          //store current user context temporarily
          temporarySelectedUserContext = $scope.$parent.selectedUserContext;

          //wipe user context and replace text with all customers
          $scope.$parent.selectedUserContext = {};
          $scope.$parent.selectedUserContext.text = 'All Customers';

          //wipe current invoices out
          $scope.invoices = [];

          //load all open invoices from all customers and set the customer array to that result
          loadAllOpenInvoices($scope.invoiceParams).then(setInvoices);
        } else {
          //set button text back to View All
          $scope.viewAllButtonText = 'View Open Invoices for All Customers';

          //restore previous selected user context
          $scope.$parent.selectedUserContext = temporarySelectedUserContext;

          //restore previous header text
          $scope.headerText = 'Invoices for: ' + $scope.selectedUserContext.customer.customerName;
        }
        $scope.viewingAllCustomers = !$scope.viewingAllCustomers;
      };

      //listens for state change event to restore selectedUserContext
      $rootScope.$on('$stateChangeStart', function () {
        //change selected user context back to the one stored in LocalStorage here
        $scope.$parent.selectedUserContext = LocalStorage.getCurrentCustomer();
      });

      //change the selected user context to the one the user clicked and refresh the page
      $scope.changePageContext = function (customerNumber) {
        //generate and set customer context to customerNumber that user selected
        CustomerService.getCustomerDetails(customerNumber).then(function (success) {
            //generate new customer context TODO: FIX CODE DUPLICATION HERE
            var generatedUserContext = {};
            generatedUserContext.id = success.customerNumber;
            generatedUserContext.text = success.customerNumber + ' - ' + success.customerName;
            generatedUserContext.customer = success;

            //set the selected context to the generated one
            $scope.$parent.selectedUserContext = generatedUserContext;

            //persist the context change to Local Storage to prevent the stateChangeStart listener from reverting the context change
            LocalStorage.setSelectedCustomerInfo(generatedUserContext);

            //refresh the page
            $state.transitionTo($state.current, $state.params, {
              reload: true,
              inherit: false,
              notify: true
            });
          }
        );
      };

      $scope.linkToReferenceNumber = function(customerNumber, invoiceNumber){
        //generate and set customer context to customerNumber that user selected
        CustomerService.getCustomerDetails(customerNumber).then(function (success) {
            //generate new customer context TODO: FIX CODE DUPLICATION HERE
            var generatedUserContext = {};
            generatedUserContext.id = success.customerNumber;
            generatedUserContext.text = success.customerNumber + ' - ' + success.customerName;
            generatedUserContext.customer = success;

            //set the selected context to the generated one
            $scope.$parent.selectedUserContext = generatedUserContext;

            //persist the context change to Local Storage to prevent the stateChangeStart listener from reverting the context change
            LocalStorage.setSelectedCustomerInfo(generatedUserContext);
            //redirect to specific invoice page
            $state.transitionTo('menu.invoiceitems', {invoiceNumber: invoiceNumber}, {
              reload: true,
              inherit: false,
              notify: true
            })
          }
        );
      };

      /******************************
       PAGING, SORTING, FILTERING
       ******************************/

      $scope.filterInvoices = function (filterFields) {

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
        for (var propertyName in filterFields) {
          if (filterFields[propertyName] && filterFields[propertyName] !== '') {
            var filterObject = {
              field: propertyName,
              value: filterFields[propertyName]
            };
            filterList.push(filterObject);
          }
        }

        var firstFilter = filterList[0];
        filterList.splice(0, 1);

        var filterParamObject = {
          field: firstFilter.field,
          value: firstFilter.value,
          filter: filterList
        };

        $scope.invoiceParams.filter = filterParamObject;

        // reset paging
        $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
        $scope.invoiceParams.from = 0;

        if(!$scope.viewingAllCustomers) {
          loadInvoices($scope.invoiceParams).then(setInvoices);
        } else{
          loadAllOpenInvoices($scope.invoiceParams).then(setInvoices);
        }
      };

      $scope.clearFilters = function () {
        $scope.filterFields = {};
        $scope.invoiceParams.filter = [];

        if(!$scope.viewingAllCustomers) {
          loadInvoices($scope.invoiceParams).then(setInvoices);
        } else {
          loadAllOpenInvoices($scope.invoiceParams).then(setInvoices);
        }
      };

      $scope.sortInvoices = function (field, order) {
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

        if(!$scope.viewingAllCustomers) {
          loadInvoices($scope.invoiceParams).then(setInvoices);
        }else {
          loadAllOpenInvoices($scope.invoiceParams).then(setInvoices);
        }
      };

      $scope.infiniteScrollLoadMore = function () {
        if (($scope.invoices && $scope.invoices.length >= $scope.totalInvoices) || $scope.loadingResults) {
          return;
        }

        $scope.invoiceParams.from += $scope.invoiceParams.size;

        if(!$scope.viewingAllCustomers){
          loadInvoices($scope.invoiceParams).then(function (invoices) {
            $scope.invoices = $scope.invoices.concat(invoices);
          });
        } else {
          loadAllOpenInvoices($scope.invoiceParams).then(function (invoices) {
            $scope.invoices = $scope.invoices.concat(invoices);
          });
        }
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

      $scope.selectFilterView = function (filterView) {
        $scope.selectedFilterView = filterView;

        $scope.invoiceParams.filter = filterView.filterFields;
        $scope.invoiceParams.size = Constants.infiniteScrollPageSize;
        $scope.invoiceParams.from = 0;

        if(!$scope.viewingAllCustomers) {
          loadInvoices($scope.invoiceParams).then(setInvoices);
        } else {
          loadAllOpenInvoices($scope.invoiceParams).then(setInvoices);
        }
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
