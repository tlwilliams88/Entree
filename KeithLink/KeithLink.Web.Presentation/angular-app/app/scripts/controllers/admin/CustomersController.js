'use strict';

angular.module('bekApp')
  .controller('CustomersController', ['$scope', '$stateParams', 'LocalStorage',
    function ($scope, $stateParams, LocalStorage) {

      /*---init---*/
      LocalStorage.getProfile().user_customers.forEach(function (customer) {
        if($stateParams.customerNumber == customer.customerNumber){
          $scope.customer = customer;
          console.log(customer);
        }
      });

      /*---page functions---*/
      $scope.restoreDefaults = function () {
        console.log('restoreDefaults');
      };

      $scope.saveChanges = function () {
        console.log('saveChanges');
      };
  }]);
