'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', function($scope) {
    
    $scope.orders = [{
      orderNum: 212342342,
      deliveryDate: '12/13/2014',
      totalCost: '1234.32',
      status: 'Open',
      paymentStatus: 'N/A'
    }];

    $scope.locations = [
      'Jimmy\'s Chicken Shack',
      'Torchy\'s Tacos'
    ];

    $scope.myInterval = -1;
    var items = $scope.items = [{
      id: 1,
      imageUrl: 'images/demoimage1.jpg',
      name: '50% off of apples!'
    },{
      id: 1,
      imageUrl: 'images/demoimage2.jpg',
      name: '50% off of apples!'
    },{
      id: 1,
      imageUrl: 'images/demoimage3.jpg',
      name: '50% off of apples!'
    },{
      id: 1,
      imageUrl: 'images/demoimage4.jpg',
      name: '50% off of apples!'
    }];

  });