'use strict';

angular.module('bekApp')
.controller('OrderController', ['$scope', '$state', '$timeout', '$modal', 'orders', 'LocalStorage', 'OrderService',
  function ($scope, $state, $timeout, $modal, orders, LocalStorage, OrderService) {

  $scope.sortBy = 'createddate';
  $scope.sortOrder = true;

  $scope.orders = orders;

  var profile = LocalStorage.getProfile();
  var customerNumber = LocalStorage.getCustomerNumber();
  var currentCustomer;

  // get last order update time for current customer
  angular.forEach(profile.user_customers, function(customer) {
    if (customer.customerNumber === customerNumber) {
      currentCustomer = customer;
    }
  });

  var data = { response: {}, calls: 0 };
  var poller = function() {
    OrderService.pollOrderHistory().then(function(response) {
      // console.log('poll');
      data.calls++;
      
      if (currentCustomer.lastOrderUpdate === response.lastupdated) {
        // no update made, keep polling

        if (data.calls > 5) {
          // stop polling after x-number calls
          // $timeout.cancel(colorRowTimer);
          $scope.displayMessage('error', 'No updates made. Try again later.');
        } else {
          // keep polling
          $timeout(poller, 2000);
        }

      } else {
        // update order history
        OrderService.getAllOrders().then(function(orders) {
          $scope.displayMessage('success', 'Successfully received lastest order updates.');
          $scope.orders = orders;
        });

      }
    });      
  };

  $scope.refreshOrderHistory = function() {
    OrderService.refreshOrderHistory().then(function(response) {
      poller();  
    });
  };

  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        headerText: function () {
          return 'Orders';
        },
        exportMethod: function() {
          return OrderService.exportOrders;
        },
        exportConfig: function() {
          return OrderService.getExportConfig();
        },
        exportParams: function() {
          return null;
        }
      }
    });
  };
  
}]);