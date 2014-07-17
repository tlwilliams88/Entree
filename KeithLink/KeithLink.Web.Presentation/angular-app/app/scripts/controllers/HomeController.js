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
    var items = $scope.items = [];
    $scope.addItem = function() {
      var newWidth = 300 + items.length;
      items.push({
        image: 'http://placebear.com/' + newWidth + '/300',
        description: '50% off of apples!'
      });
    };
    for (var i = 0; i < 4; i++) {
      $scope.addItem();
    }

  });