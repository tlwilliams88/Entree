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

    // go to active cart

    // go to first cart in list
    if (CartService.carts && CartService.carts.length > 0) {
      $state.go('menu.cart.items', { cartId: CartService.carts[0].id });
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