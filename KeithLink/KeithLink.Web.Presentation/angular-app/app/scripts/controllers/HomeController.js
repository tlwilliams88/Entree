'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$state', '$modal', '$filter', 'CartService', 'OrderService', 'MarketingService',
    function($scope, $state, $modal, $filter, CartService, OrderService, MarketingService) {
    
    $scope.myInterval = -1;

    $scope.loadingOrders = true;
    OrderService.getAllOrders().then(function(orders) {
      $scope.orders = orders;
      delete $scope.ordersMessage;
    }, function(error) {
      $scope.ordersMessage = 'Error loading orders.';
    }).finally(function() {
      $scope.loadingOrders = false;
    });

    $scope.loadingPromoItems = true;
    MarketingService.getPromoItems().then(function(items) {
      $scope.promoItems = items;
      delete $scope.promoMessage;
    }, function(error) {
      $scope.promoMessage = 'Error loading promo items.';
    }).finally(function() {
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

    /**********
    order summary graph data
    **********/

    // get date range
    var from = new Date(),
      to = new Date();
    from.setMonth(from.getMonth()-6);
    from.setDate(1);
    to.setMonth(to.getMonth()+1);
    to.setDate(1);

    from = $filter('date')(from, 'yyyy-MM-dd');
    to = $filter('date')(to, 'yyyy-MM-dd');

    // get order summary data
    OrderService.getOrdersByDate(from, to).then(function(orders) {

      // get order totals for each month
      var months = {};
      orders.forEach(function(order) {
        var date = new Date(order.createddate);
        var month = date.getMonth() + 1;
        var total = months[month];
        if (total) {
          months[month] += order.ordertotal;
        } else {
          months[month] = order.ordertotal;
        }
      });
      
      // format data to match graph 
      var monthData = ['x'],
        barData = ['bar'],
        lineData = ['line'];

      for (var month in months) {
        var total = months[month];
        monthData.push(month); 
        barData.push(total);
        lineData.push(total);
      }

      var chart = c3.generate({
        bindto: '#chart_div',
        data: {
          x: 'x',
          columns: [
            // ['x', 'J', 'F', 'M', 'A', 'M', 'J'],
            // ['data1', 130, 340, 200, 500, 250, 350], // bar
            // ['data2', 130, 340, 200, 500, 250, 350] // line
            monthData,
            barData,
            lineData
          ],
          types: {
              bar: 'bar',
              line: 'line'
          }
        },
        axis: {
          x: {
            type: 'category'
          }
          // ,
          // y: {
          //   tick: {
          //     values: [100, 1000, 10000]
          //   }
          // }
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

    });

  }]);