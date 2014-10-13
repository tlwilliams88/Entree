'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartController', ['$scope', '$state', 'CartService', 
    function($scope, $state, CartService) {
    
    $scope.$state = $state;

    // get valid ship dates
    CartService.getShipDates().then(function(data) {
      console.log(data.shipdates);
      $scope.shipDates = data.shipdates;
    });

    // navigate to appropriate cart if no cart is selected
    var currentCart = CartService.getSelectedCart($state.params.cartId);
    if (currentCart && currentCart.id) {
      $state.go('menu.cart.items', { cartId: currentCart.id });
    }
    
    $scope.createNewCart = function() {
      CartService.createCart().then(function(response) {
        $state.go('menu.cart.items', {cartId: response.listitemid, renameCart: true});
        $scope.displayMessage('success', 'Successfully created new cart.');
      }, function() {
        $scope.displayMessage('error', 'Error creating new cart.');
      });
    };

  }]);