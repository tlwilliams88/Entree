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
    from.setMonth(from.getMonth()-5);
    from.setDate(1);
    to.setMonth(to.getMonth()+1);
    to.setDate(1);

    var fromString = $filter('date')(from, 'yyyy-MM-dd');
    var toString = $filter('date')(to, 'yyyy-MM-dd');

    // get order summary data
    OrderService.getOrdersByDate(fromString, toString).then(function(orders) {

      // get order totals for each month in form...
      /*
      {
        10: 2354,
        11: 5432,
        ...
      }
      */
      var months = {};
      orders.forEach(function(order) {
        var date = new Date(order.createddate);
        var month = date.getMonth() + 1;
        var total = months[month];
        if (total) { // check if month already exists in object
          months[month] += order.ordertotal;
        } else {
          months[month] = order.ordertotal;
        }
      });

      // add 0 amount for months with no data
      var startDate = from;
      for (var i = 0; i < 6; i ++) {
        var monthNum = startDate.getMonth()+1;
        if (!months.hasOwnProperty(monthNum)) {
          months[monthNum] = 0;
        }
        startDate.setMonth(startDate.getMonth()+1)  
      }
      
      
      // format data to match graph 
      // [0, 2354, 5432, 0, ...]
      var monthData = [],
        barData = [],
        lineData = [];

      for (var month in months) {
        var total = months[month];
        monthData.push(month); 
        barData.push(total);
        lineData.push(total);
      }

      // determine y axis values
      var maxAmount = Math.max.apply(null, barData);
      var yAxisValues = [];

      for (var i = 0; i < 5; i++) {
        var yValue = Math.ceil(maxAmount * i / 4 / 100) * 100; // round up to nearest hundred
        yAxisValues.push(yValue);
      }

      monthData.unshift('x');
      barData.unshift('bar');
      lineData.unshift('line');

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
          },
          y: {
            tick: {
              values: yAxisValues
            },
            max: yAxisValues[yAxisValues.length - 1]
          }
        },
        legend: {
          hide: true
        },
        color: {
          pattern: ['#f3f1eb', '#489ecf'] // background, blue
        },
        padding: {
          bottom: -20,
          left: 40
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