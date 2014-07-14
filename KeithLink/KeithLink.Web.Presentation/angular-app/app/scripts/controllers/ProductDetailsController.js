'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ProductDetailsController', function ($scope) {
    
    $scope.orders = [
      {
        orderNum: 212342342,
        deliveryDate: '12/13/2014',
        totalCost: '1234.32',
        status: 'Open',
        paymentStatus: 'N/A'
      }
    ];

    $scope.locations = [
      'Jimmy\'s Chicken Shack',
      'Torchy\'s Tacos'
    ];


  });
