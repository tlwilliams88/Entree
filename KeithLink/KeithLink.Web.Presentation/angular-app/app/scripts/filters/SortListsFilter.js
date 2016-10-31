'use strict';

/**
 * sortLists filter
 * Sorts list types for when listsare displayed in a list
 */
angular.module('bekApp')
.filter('sortLists', [ '$filter', 'ListService', function($filter, ListService) {
  return function(items) {

    angular.forEach(items, function(item) {
      
      // add display priority list of lists
      if (item.isfavorite) {
        item.sortPriority = 1;
      } else if (item.is_contract_list) {
        item.sortPriority = 2;
      } else if (item.isworksheet) {
        item.sortPriority = 3;
      } else if (item.isreminder) {
        item.sortPriority = 4;
      } else if (item.ismandatory) {
        item.sortPriority = 5;
      } else if (item.isrecommended) {
        item.sortPriority = 6;
      } else if (item.iscustominventory) {
        item.sortPriority = 7;
      } else {
        item.sortPriority = 10;
      }
    });

    // sort lists by priority then name
    var sortedItems = $filter('orderBy')(items, ['sortPriority', 'name'], false);
    return sortedItems;
  };
}]);