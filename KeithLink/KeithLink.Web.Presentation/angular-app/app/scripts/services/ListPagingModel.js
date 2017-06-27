'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListPagingModel
 * @description
 * # ListPagingModel
 * Service of the bekApp
 */
angular.module('bekApp').factory('ListPagingModel', ['ListService', 'LocalStorage', function (ListService, LocalStorage) {

  // Define the constructor function.
  function ListPagingModel( listId, setListItems, appendListItems, startLoading, stopLoading, sort, pageSize ) {

    this.listId = listId;

    this.pageSize = pageSize ? pageSize : LocalStorage.getPageSize();
    this.pageIndex = 0;
    this.searchTerm = '';
    this.sort = sort;

    this.setListItems = setListItems;
    this.appendListItems = appendListItems;
    this.startLoading = startLoading;
    this.stopLoading = stopLoading;
  }

  ListPagingModel.prototype = {

    loadList: function(appendData) {
      var setData = this.setListItems;
      if (appendData) {
        setData = this.appendListItems;
      }

      var filterObject = LocalStorage.getDefaultSort();

      var sortArray = ListService.sortObject;

       var params = {
        size: this.pageSize,
        from: this.pageIndex,
        terms: this.searchTerm,
        sort: sortArray,
        filter: this.filter,
        message: 'Loading List...'
       };

      this.startLoading();
      return ListService.getList(
        this.listId,
        params
      ).then(setData).finally(this.stopLoading);
    },

    getFilterObject: function(filterFields) {
      var filterList = filterFields;

      if (filterList.length > 0) {
        filterParamObject.filter = filterList;
      }
      return filterParamObject;
    },

    filterListItems: function(searchTerm) {
      if(searchTerm){
        this.searchTerm = searchTerm;
      } else {
        this.searchTerm = '';
        this.filter = '';
      }

      this.pageIndex = 0;
      this.loadList();
    },

    filterListItemsByMultipleFields: function(filterFields) {
      this.filter = filterFields;
      this.pageIndex = 0;
      this.loadList();
    },

    clearFilters: function() {
      this.pageIndex = 0;
      this.filter = [];
      this.searchTerm = null;
      this.loadList();
    },

    sortListItems: function(sort) {
      this.pageIndex = 0;
      this.sort = sort;
      ListService.sortObject = sort;
      this.loadList();
    },

    resetPaging: function() {
      //this.pageIndex = 0;
    },

    loadMoreData: function(results, total, loading, deletedItems) {
      if ( (!results || (results + deletedItems.length) < total) && !loading ) {

        var pageSize =  parseInt(LocalStorage.getPageSize());
        var sortOrder = LocalStorage.getDefaultSort();

        if(pageSize > 0){
          this.pageSize = pageSize;
        }
        this.pageIndex = results;
        this.loadList(true);
      }
    },

    loadAllData: function(results, total, loading) {
      if ( (!results || (results.length < total)) && !loading ) {
        this.pageIndex = 0;
        this.pageSize = total;
        this.loadList(true);
      }
    },

  };
  return( ListPagingModel );

}]);
