'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:PagingModel
 * @description
 * # PagingModel
 * Service of the bekApp
 */
angular.module('bekApp').factory('PagingModel', ['Constants', function (Constants) {

  // $scope.params = {
  //   size: Constants.infiniteScrollPageSize,
  //   from: 0,
  //   sort: [{
  //     field: 'messagecreatedutc',
  //     order: 'desc'
  //   }],
  //   filter: {
  //     field: 'subject',
  //     value: 'value',
  //     filter: [
  //        {
  //          field: 'name',
  //          value: 'value'
  //        }
  //     ]
  //   }
  // };

  // Define the constructor function.
  function PagingModel( getData, setData, appendData, startLoading, stopLoading, sort, secondarySort, filter  ) {
    this.pageSize = Constants.infiniteScrollPageSize;
    this.pageIndex = 0;
    this.sort = sort;
    this.secondarySort = secondarySort;
    this.filter = filter;
    
    this.getData = getData;
    this.setData = setData;
    this.appendData = appendData;
    this.startLoading = startLoading;
    this.stopLoading = stopLoading;
  }

  // Define the "instance" methods using the prototype
  // and standard prototypal inheritance.
  PagingModel.prototype = {

    getSortObject: function(sort) {
      return {
        order: sort.sortDescending ? 'desc' : 'asc',
        field: sort.field
      };
    },
    
    getSortArray: function(sort, secondarySort) {
      var sortObjects = [];

      if (sort) {
        sortObjects.push(this.getSortObject(sort));
      }
      if (secondarySort) {
        sortObjects.push(this.getSortObject(secondarySort));
      }

      return sortObjects;
    },

    loadData: function(appendData) {
      var setData = this.setData;
      if (appendData) {
        setData = this.appendData;
      }

      this.startLoading();

      var params = {
        size: this.pageSize,
        from: this.pageIndex,
        terms: this.searchTerm,
        sort: this.getSortArray(this.sort, this.secondarySort),
        filter: this.filter
      };
      return this.getData(params)
        .then(setData)
        .finally(this.stopLoading);
    },

    filterData: function(filterFields, baseFilterList) {
      var filterList = [];

      if (baseFilterList) {
        filterList = angular.copy(baseFilterList);
      }

      for(var propertyName in filterFields) {
        if (filterFields[propertyName] && filterFields[propertyName] !== '') {
          filterList.push({
            field: propertyName,
            value: filterFields[propertyName] 
          });
        }
      }

      if (filterList.length > 0) {
        var firstFilter = filterList[0];
        filterList.splice(0,1);

        var filterParamObject = {
          field: firstFilter.field,
          value: firstFilter.value,
          filter: filterList
        };
        this.filter = filterParamObject;
      } else {
        this.filter = null;
      }      

      this.pageIndex = 0;
      this.loadData();
    },

    clearFilters: function() {
      this.pageIndex = 0;
      this.filter = [];
      this.loadData();
    },

    sortData: function(sort) {
      this.pageIndex = 0;
      this.sort = sort;
      this.loadData();
    },

    loadMoreData: function(results, total, loading) {
      if ( (!results || results.length < total) && !loading ) {
        this.pageIndex += this.pageSize;
        this.loadData(true);
      }
    }

  };


  // Define the "class" / "static" methods. These are
  // utility methods on the class itself; they do not
  // have access to the "this" reference.
  // PagingModel.fromFullName = function( fullName ) {

  //     var parts = trim( fullName || '' ).split( /\s+/gi );

  //     return(
  //         new PagingModel(
  //             parts[ 0 ],
  //             parts.splice( 0, 1 ) && parts.join( " " )
  //         )
  //     );

  // };


  // Return constructor - this is what defines the actual
  // injectable in the DI framework.
  return( PagingModel );

}]);