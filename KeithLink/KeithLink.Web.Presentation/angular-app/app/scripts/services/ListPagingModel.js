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
  function ListPagingModel( listId, setListItems, appendListItems, startLoading, stopLoading, sort ) {
    
    this.listId = listId;

    this.pageSize = 30;
    this.pageIndex = 0;
    this.searchTerm = '';
    this.sort = sort;

    this.setListItems = setListItems;
    this.appendListItems = appendListItems;
    this.startLoading = startLoading;
    this.stopLoading = stopLoading;
  }

  ListPagingModel.prototype = {

    loadList: function(appendData, usemessage, page) {
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
        filter: this.filter        
       };

      if(usemessage){
        params.message = 'Loading List...'
      }

      this.startLoading();
      return ListService.getList(
        this.listId,
        params
      ).then(setData).finally(this.stopLoading);
    },

    getFilterObject: function(filterFields) {
      var filterList = [];

      for(var propertyName in filterFields) {
        if (filterFields[propertyName] && filterFields[propertyName] !== '') {
          filterList.push({
            field: propertyName,
            value: filterFields[propertyName] 
          });
        }
      }

      var filterParamObject = null;
      if (filterList.length > 0) {
        var firstFilter = filterList.splice(0,1)[0];
        
        filterParamObject = firstFilter;
        filterParamObject.filter = filterList;
      }
      return filterParamObject;
    },

    filterListItems: function(searchTerm) {
      this.searchTerm = searchTerm;
      this.pageIndex = 0;
      this.loadList();
    },

    filterListItemsByMultipleFields: function(filterFields) {
      this.filter = this.getFilterObject(filterFields);
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
      ListService.sortObject = sort
      this.loadList();
    },

    resetPaging: function() {
      this.pageIndex = 0;
    },

    loadMoreData: function(results, total, loading, deletedItems, page) {
      if ( (!results || (results.length + deletedItems.length) < total) && !loading ) {

        var pageSize = parseInt(LocalStorage.getPageSize());
        var sortOrder = LocalStorage.getDefaultSort();

        if(pageSize > 0){
          this.pageSize = pageSize;
        }
        this.pageIndex += this.pageSize; 
        this.loadList(true, false, page);
      }
    },

    loadAllData: function(results, total, loading, page) {
      if ( (!results || (results.length < total)) && !loading ) {
        this.pageIndex = results.length;
        this.pageSize = total - results.length;
        this.loadList(true,true, page);
      }
    },

  };
  return( ListPagingModel );

}]);