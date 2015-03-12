'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListPagingModel
 * @description
 * # ListPagingModel
 * Service of the bekApp
 */
angular.module('bekApp').factory('ListPagingModel', ['ListService', function (ListService) {

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

    loadList: function(appendData) {
      console.log('loading items')
      var setData = this.setListItems;
      if (appendData) {
        setData = this.appendListItems;
      }

      var sortArray = [{
        field: this.sort.field,
        order: this.sort.sortDescending ? 'desc' : 'asc'
      }];

      var params = {
        size: this.pageSize,
        from: this.pageIndex,
        terms: this.searchTerm,
        sort: sortArray,
        filter: this.filter
      };

      this.startLoading();
      return ListService.getList(
        this.listId,
        params
      )
        .then(setData)
        .finally(this.stopLoading);
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
      console.log('filter items')
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
      console.log('sort items')
      this.pageIndex = 0;
      this.sort = sort;
      this.loadList();
    },

    loadMoreData: function(results, total, loading, deletedItems) {
      console.log('load more items')
      if ( (!results || (results.length + deletedItems.length) < total) && !loading ) {
        this.pageIndex += this.pageSize;
        this.loadList(true);
      }
    }

  };
  return( ListPagingModel );

}]);