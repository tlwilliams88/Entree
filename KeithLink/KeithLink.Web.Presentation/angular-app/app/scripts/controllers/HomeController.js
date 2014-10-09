'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', 'OrderService', function($scope, OrderService) {
    
    $scope.loadingOrders = true;
    OrderService.getAllOrders().then(function(orders) {
      $scope.orders = orders;
      $scope.mostRecentOrder = orders[0];
      $scope.loadingOrders = false;
    });

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

  }]);