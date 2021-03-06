'use strict';

angular.module('bekApp')
  .controller('TransactionController', ['$scope', '$state', '$modal', 'TransactionService', 'LocalStorage', 'CustomerService', 'PagingModel', 'SessionRecordingService',
    function ($scope, $state, $modal, TransactionService, LocalStorage, CustomerService, PagingModel, SessionRecordingService) {

  // set context to all customers
  var tempContext = {
    text: 'All Customers'
  };
  $scope.setSelectedUserContext(tempContext);

  var transactionPagingModel = new PagingModel( 
    TransactionService.getPendingTransactions, 
    setTransactions,
    appendTransactions,
    startLoading,
    stopLoading
  );

  transactionPagingModel.loadData();

  function setTransactions(data) {
    $scope.transactions = data.results;
    $scope.totalTransactions = data.totalResults;
  }
  function appendTransactions(data) {
    $scope.transactions = $scope.transactions.concat(data.results);
  }
  function startLoading() {
    $scope.loadingResults = true;
  }
  function stopLoading() {
    $scope.loadingResults = false;
  }

  /***
  NAVIGATION LINKS 
  ***/

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

      SessionRecordingService.tagCustomer(LocalStorage.getCustomerNumber());

      //refresh the page
      $state.transitionTo(stateName, stateParams, {
        reload: true,
        inherit: false,
        notify: true
      });
    });
  }

  //change the selected user context to the one the user clicked and refresh the page
  $scope.goToInvoicesForCustomer = function(customerNumber, branch) {
    changeUserContext('menu.invoice', {}, customerNumber, branch);
  };

  $scope.goToInvoiceDetails = function(customerNumber, branch, invoiceNumber){
    changeUserContext('menu.invoiceitems', { invoiceNumber: invoiceNumber }, customerNumber, branch);
  };

  /**********
  PAGING, SORTING, FILTERING
  **********/

  $scope.filterTransactions = function(filterFields) {
    transactionPagingModel.filterData(filterFields);
  };
  $scope.clearFilters = function() {
    $scope.filterFields = {};
    transactionPagingModel.clearFilters();
  };
  $scope.infiniteScrollLoadMore = function() {
    transactionPagingModel.loadMoreData($scope.transactions, $scope.totalTransactions, $scope.loadingResults);
  };
  $scope.sortTransactions = function(field, sortDescending) {
    $scope.sort = {
      field: field,
      sortDescending: sortDescending
    };
    transactionPagingModel.sortData($scope.sort);
  };

  $scope.openExportModal = function () {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {        
        location: function() {
          return {category:'Transactions', action:'Export Pending Transactions'};
        },
        headerText: function () {
          return 'Transactions';
        },
        exportMethod: function () {
          return TransactionService.exportTransactions;
        },
        exportConfig: function () {
          return TransactionService.getTransactionExportConfig();
        },
        exportParams: function () {
          return {
            paging: {
              // filter: getInvoicesFilterObject($scope.filterRowFields, $scope.selectedFilterView)
            }
          };
        },
        exportType: function() {
           return Constants.exportType.standardExport;
        }
      }
    });
  };

}]);
