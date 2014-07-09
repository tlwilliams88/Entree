'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:MainCtrl
 * @description
 * # MainCtrl
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeCtrl', function ($scope) {
    
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
