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
    var currentCart = CartService.getSelectedCart($state.params.cartId);
    if (currentCart && currentCart.id) {
      $state.go('menu.cart.items', { cartId: currentCart.id });
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