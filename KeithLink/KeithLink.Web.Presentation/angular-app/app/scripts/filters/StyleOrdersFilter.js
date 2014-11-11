'use strict';

angular.module('bekApp')
.filter('styleOrders', [ function() {
  return function(orders) {

    angular.forEach(orders, function(order) {

      // switch(order.status) {
      //   case 'Confirmed With Exceptions':
      //   case 'Confirmed With Changes And Exceptions':
      //     order.highlight = 'red';
      //     break;
      //   case 'Delivered':
      //     order.highlight = 'green';
      //     break;
      // }

      // add css class to orders based on their invoice status
      switch(order.invoicestatus) {
        case 'Late':
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