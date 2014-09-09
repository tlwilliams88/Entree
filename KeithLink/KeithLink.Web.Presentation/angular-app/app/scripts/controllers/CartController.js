'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartController', ['$scope', '$state', 'toaster', 'CartService', 
    function($scope, $state, toaster, CartService) {
    
    $scope.$state = $state;

    // navigate to appropriate cart if no cart is selected
    var currentCartId;
    if (!$state.params.cartId) {
      // go to active cart
      angular.forEach(CartService.carts, function(cart, index) {
        if (cart.active) {
          currentCartId = cart.id;
        }
      });

      // go to first cart in list
      if (!currentCartId && CartService.carts && CartService.carts.length > 0) {
        currentCartId = CartService.carts[0].id;
      }

      if (currentCartId) {
        $state.go('menu.cart.items', { cartId: currentCartId });
      }
    }
    
    $scope.createNewCart = function() {
      CartService.createCart().then(function(response) {
        $state.go('menu.cart.items', {cartId: response.listitemid, renameCart: true});
        toaster.pop('success', null, 'Successfully created new cart.');
      }, function() {
        toaster.pop('error', null, 'Error creating new cart.');
      });
    };

  }]);