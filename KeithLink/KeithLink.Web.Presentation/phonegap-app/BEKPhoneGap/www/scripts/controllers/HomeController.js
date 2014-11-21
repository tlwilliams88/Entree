'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$state', '$modal', 'CartService', 'OrderService', 'MarketingService',
    function($scope, $state, $modal, CartService, OrderService, MarketingService) {
    
    $scope.myInterval = -1;

    $scope.loadingOrders = true;
    OrderService.getAllOrders().then(function(orders) {
      $scope.orders = orders;
      $scope.loadingOrders = false;
    });

    $scope.loadingPromoItems = true;

    MarketingService.getPromoItems().then(function(items) {
      console.log(items);
      $scope.promoItems = items;
      $scope.loadingPromoItems = false;
    });
 
    $scope.createNewCart = function() {
      return CartService.createCart().then(function(cartId) {
        CartService.setActiveCart(cartId);
        $state.go('menu.cart.items', {cartId: cartId, renameCart: true});
      });
    };

    $scope.showPromoItemContent = function(promoItem) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/promoitemcontentmodal.html',
        controller: 'PromoItemContentModalController',
        resolve: {
          promoItem: function() {
            return promoItem;
          }
        }
      });
    };

  }]);