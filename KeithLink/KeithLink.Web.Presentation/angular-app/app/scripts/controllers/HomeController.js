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

    var chart = c3.generate({
      bindto: '#chart_div',
      data: {
        x: 'x',
        columns: [
          ['x', 'J', 'F', 'M', 'A', 'M', 'J'],
          ['data1', 30, 200, 100, 400, 150, 250], // bar
          ['data2', 130, 340, 200, 500, 250, 350] // line
        ],
        types: {
            data1: 'bar',
            data2: 'line'
        }
      },
      axis: {
        x: {
          type: 'category'
        },
        y: {
          tick: {
            values: [100, 1000, 10000]
          }
        }
      },
      legend: {
        hide: true
      },
      color: {
        pattern: ['#f3f1eb', '#489ecf'] // background, blue
      },
      padding: {
        bottom: -15
      },
      interaction: {
        enabled: false
      },
      grid: {
        y: {
          show: true
        }
      }
    });

  }]);