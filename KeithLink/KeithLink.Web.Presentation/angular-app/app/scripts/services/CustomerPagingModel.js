'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:CustomerPagingModel
 * @description
 * # CustomerPagingModel
 * Service of the bekApp
 */
angular.module('bekApp').factory('CustomerPagingModel', ['Constants', 'CustomerService', function (Constants, CustomerService) {

  // Define the constructor function.
  function CustomerPagingModel( setCustomers, appendCustomers, startLoading, stopLoading, sortField, sortDescending ) {
    
    this.pageSize = 30;
    this.pageIndex = 0;
    this.searchTerm = '';
    this.sortField = sortField;
    this.sortDescending = sortDescending;
    this.accountId = '';

    this.setCustomers = setCustomers;
    this.appendCustomers = appendCustomers;
    this.startLoading = startLoading;
    this.stopLoading = stopLoading;
  }

  CustomerPagingModel.prototype = {

    loadCustomers: function(appendData) {
      var setData = this.setCustomers;
      if (appendData) {
        setData = this.appendCustomers;
      }

      this.startLoading();
      return CustomerService.getCustomers(
        this.searchTerm,
        this.pageSize,
        this.pageIndex,
        this.sortField,
        this.sortDescending ? 'desc' : 'asc',
        this.accountId,
        this.type
      )
        .then(setData)
        .finally(this.stopLoading);
    },

    filterCustomers: function(searchTerm, type) {
      this.searchTerm = searchTerm;
      this.type=type;
      this.pageIndex = 0;
      this.loadCustomers();
    },

    sortCustomers: function(sortField, sortDescending) {
      this.pageIndex = 0;
      this.sortField = sortField;
      this.sortDescending = sortDescending;
      this.loadCustomers();
    },

    loadMoreData: function(results, total, loading) {
      if ( (!results || results.length < total) && !loading ) {
        this.pageIndex += this.pageSize;
        this.loadCustomers(true);
      }
    }

  };
  return( CustomerPagingModel );

}]);