'use strict';

angular.module('bekApp')
.filter('meetsMinimumQuantity', [ '$filter', function($filter) {
  return function(listItems, cartItems) {
    var filteredList = [];

    angular.forEach(listItems, function(listItem) {
      
      // check if item is in the cart
      var matchingCartItems = $filter('filter')(cartItems, { itemnumber: listItem.itemnumber });

      // find the total quantity of items in the cart
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