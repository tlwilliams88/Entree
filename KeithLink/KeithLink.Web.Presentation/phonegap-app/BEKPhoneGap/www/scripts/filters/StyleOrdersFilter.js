'use strict';

angular.module('bekApp')
.filter('styleOrders', [ function() {
  return function(orders) {

    angular.forEach(orders, function(order) {

      // add css class to orders based on their invoice status
      switch(order.invoicestatus) {
        case 'Past Due':
          order.highlightClass = 'order-summary__order--late';
          break;
        case 'Paid':
          order.highlightClass = 'order-summary__order--paid';
          break;
      }

    });

    return orders;
  };
}]);