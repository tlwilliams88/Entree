angular.module('bekApp')
.filter('sortLists', [ '$filter', 'ListService', function($filter, ListService) {
  return function(items) {

    angular.forEach(items, function(item) {
      
      // add display priority list of lists
      if (item.isFavoritesList) {
        item.sortPriority = 1;
      } else if (item.is_contract_list) {
        item.sortPriority = 2;
      } else {
        item.sortPriority = 10;
      }
    });

    // sort lists by priority then name
    return $filter('orderBy')(items, ['sortPriority', 'name'], false);
  }
}]);