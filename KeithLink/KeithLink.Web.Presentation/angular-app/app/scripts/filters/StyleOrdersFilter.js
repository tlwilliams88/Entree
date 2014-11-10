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

      switch(order.invoicestatus) {
        case 'Late':
          order.highlight = 'red';
          break;
        case 'Paid':
          order.highlight = 'green';
          break;
      }

    });

    return orders;
  };
}]);