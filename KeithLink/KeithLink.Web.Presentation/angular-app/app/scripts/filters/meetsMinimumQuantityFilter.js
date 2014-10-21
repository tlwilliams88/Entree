'use strict';

angular.module('bekApp')
.filter('meetsMinimumQuantity', [ '$filter', function($filter) {
  return function(listItems, cartItems) {
    var filteredList = [],
      itemNumbersChecked = [];

    angular.forEach(listItems, function(listItem) {
      
      // do not recheck duplicate item numbers because I combine them
      if (itemNumbersChecked.indexOf(listItem.itemnumber) < 0) {
        
        var minParlevel = calculateParlevel(listItem.itemnumber, listItems),
          qtyInCart = calculateItemsInCart(listItem.itemnumber, cartItems);

        // add calculated fields to returned items to display
        listItem.qtyInCart = qtyInCart;
        listItem.totalPar = minParlevel;

        // if par level is set, check that it is met
        // if par is not set, check that item is present
        // if par is a decimal, the quantity must exceed par
        if (minParlevel > qtyInCart || (minParlevel === 0 && qtyInCart === 0)) {
          filteredList.push(listItem);
        }

        itemNumbersChecked.push(listItem.itemnumber);
      }

    });
    
    return filteredList;
  };

  // calculate total par level for list items with the same item number
  function calculateParlevel(itemNumber, listItems) {
    var matchingItems = $filter('filter')(listItems, {itemnumber: itemNumber});
    
    var totalParlevel = 0;
    angular.forEach(matchingItems, function(item) {
      totalParlevel += item.parlevel;
    });
    
    return totalParlevel;
  }

  // calculate cart quantity of given item number 
  function calculateItemsInCart(itemNumber, cartItems) {
    var qtyInCart = 0;
    
    var items = $filter('filter')(cartItems, {itemnumber: itemNumber});
    if (items.length > 0) {
      qtyInCart = items[0].quantity; // item should only show up in the cart once
    }
    return qtyInCart;
  }
}]);