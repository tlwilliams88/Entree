'use strict';

/**
 * meetsMinimumQuantity filter
 * Checks whether the quantity of mandatory cart items matches the required quantity 
 * as determine by the mandatory list. Also determines the qtyInCart for list items for display
 */
angular.module('bekApp')
.filter('meetsMinimumQuantity', [ '$filter', function($filter) {
  
  /**
   * @param  {Array} listItems Array of Mandatory List items
   * @param  {Array} cartItems Array of items in the cart
   * @return {Array}           Array of list items that are still missing from the cart
   */
  return function(listItems, cartItems) {
    var filteredList = [];

    angular.forEach(listItems, function(listItem) {
      
      // check if item is in the cart
      var matchingCartItems = $filter('filter')(cartItems, { itemnumber: listItem.itemnumber });

      // find the total quantity of items in the cart for display
      listItem.qtyInCart = 0;
      angular.forEach(matchingCartItems, function(cartItem) {
        listItem.qtyInCart += cartItem.quantity;
      });

      // check if quantity in cart meets the parlevel if there is a parlevel
      if (listItem.qtyInCart < listItem.parlevel || (listItem.parlevel === 0 && listItem.qtyInCart === 0)) {
        filteredList.push(listItem);
      }
      

    });
    
    return filteredList;
  };
}]);