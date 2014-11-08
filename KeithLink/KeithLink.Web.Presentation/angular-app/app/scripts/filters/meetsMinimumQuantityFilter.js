'use strict';

angular.module('bekApp')
.filter('meetsMinimumQuantity', [ '$filter', function($filter) {
  return function(listItems, cartItems) {
    var filteredList = [];

    angular.forEach(listItems, function(listItem) {
      
      // check if item is in the cart
      var matchingCartItems = $filter('filter')(cartItems, { itemnumber: listItem.itemnumber });

      // check if item in cart has a zero quantity
      var itemHasQuantitty = false;
      // listItem.qtyInCart = 0;
      angular.forEach(matchingCartItems, function(cartItem) {
        if (cartItem.quantity > 0) {
          itemHasQuantitty = true;
          // listItem.qtyInCart += cartItem.quantity;
        }
      });


      if (!itemHasQuantitty) {
        filteredList.push(listItem);
      }

    });
    
    return filteredList;
  };
}]);