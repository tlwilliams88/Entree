'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$state', 'CartService', 'OrderService', 
    function($scope, $state, CartService, OrderService) {
    
    $scope.myInterval = -1;

    $scope.loadingOrders = true;
    OrderService.getAllOrders().then(function(orders) {
      $scope.orders = orders;
      $scope.loadingOrders = false;
    });

    $scope.createNewCart = function() {
      return CartService.createCart().then(function(cart) {
        $state.go('menu.cart.items', {cartId: cart.id, renameCart: true});
      });
    };

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