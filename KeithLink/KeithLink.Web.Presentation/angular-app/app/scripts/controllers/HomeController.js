'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$state', '$modal', '$filter', 'CartService', 'OrderService', 'MarketingService', 'NotificationService', 'Constants',
    function($scope, $state, $modal, $filter, CartService, OrderService, MarketingService, NotificationService, Constants) {
    
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

    $scope.openQuickAddModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/cartquickaddmodal.html',
        controller: 'CartQuickAddModalController'
      });

      modalInstance.result.then(function(cartId) {
        $state.go('menu.cart.items', {cartId: cartId});
      });
    };

    $scope.openOrderImportModal = function () {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/orderimportmodal.html',
        controller: 'ImportModalController'
      });
    };

 $scope.showAdditionalInfo = function(notification) {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/notificationdetailsmodal.html',
      controller: 'NotificationDetailsModalController',
      windowClass: 'color-background-modal',
      scope: $scope,
      resolve: {
        notification: function() {
          return notification;
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

    $scope.notificationParams = {
     size: 8,
     from:0,
     sort: [{
      field: 'messagecreatedutc',
      order: 'desc'

    }]
    // filter: {
    //   field: 'subject',
    //   value: 'value',
    //   filter: [
    //      {
    //        field: 'name',
    //        value: 'value'
    //      }
    //   ]
    // }
  };

    $scope.loadingResults = true;
      NotificationService.getMessages($scope.notificationParams).then(function(data) {
        var notifications =data.results,
        notificationDates ={},
        dates = [];
        notifications.forEach(function(notification){
         var date = $filter('date')(notification.messagecreatedutc, 'yyyy-MM-dd');
         if(notificationDates[date]){
          notificationDates[date].push(notification);
          }
          else{
            dates.push(date);
            notificationDates[date] = [notification];
          }          
         
        });
      $scope.notificationDates = notificationDates;
      $scope.dates = dates;      
      $scope.loadingResults = false;
    });

    // get order summary data
    OrderService.getOrdersByDate(fromString, toString).then(function(orders) {

      // get array of month values in form [8, 9, 10, 11, 12, 1]
      var months = [],
        tempDate = new Date();
      for (var i = 0; i < 6; i++) {
        tempDate.setMonth(from.getMonth() + i);
        var month = tempDate.getMonth()+1;
        months.push(month);
      }

      // calculate order totals for each month
      var monthTotals = [0, 0, 0, 0, 0, 0];
      orders.forEach(function(order) {
        var orderDate = new Date(order.createddate);
        var month = orderDate.getMonth() + 1;
        var idx = months.indexOf(month);
        monthTotals[idx] += order.ordertotal ? order.ordertotal : 0;
      });

      var barData = [],
        lineData = [],
        monthData = months;

      monthTotals.forEach(function(total) {
        barData.push(total);
        lineData.push(total);
      });

      // determine y axis values based on largest order total for one month
      var maxAmount = Math.max.apply(null, barData);
      var yAxisValues = [];

      for (var j = 0; j < 5; j++) {
        var yValue = Math.ceil(maxAmount * j / 4 / 100) * 100; // round up to nearest hundred
        yAxisValues.push(yValue);
      }

      monthData.unshift('x');
      barData.unshift('bar');
      lineData.unshift('line');



      // see c3js.org/reference.html website for list of options
      var chart = c3.generate({
        bindto: '#chart_div',
        data: {
          x: 'x',
          columns: [
            // ['x', 'J', 'F', 'M', 'A', 'M', 'J'],
            monthData,
            barData,
            lineData
            // ['bar', 0, 0, 0, 0, 0, 48000], // bar
            // ['line', 0, 0, 0, 0, 0, 48000] // line
          ],
          types: {
              bar: 'bar',
              line: 'line'
          }
        },
        axis: {
          x: {
            type: 'category',
            label: {
              text: 'Month',
              position: 'outer-center'
            }
          },
          y: {
            tick: {
              count: 5,
              values: yAxisValues,
              format: function (amount) { 
                return '$' + $filter('number')(amount); 
              }
            },
            // max: yAxisValues[yAxisValues.length - 1]
            label: {
              text: 'Amount',
              position: 'outer-middle'
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
          // bottom: -20,
          // left: 40
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