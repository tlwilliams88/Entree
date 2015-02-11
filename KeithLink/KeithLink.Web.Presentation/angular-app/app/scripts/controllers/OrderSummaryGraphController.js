'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:OrderSummaryGraphController
 * @description
 * # OrderSummaryGraphController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('OrderSummaryGraphController', [ '$scope', '$filter', 'OrderService',
    function($scope, $filter, OrderService) {

  var from = moment().subtract(5, 'months').startOf('month'),
    to = moment().endOf('month');
  var fromString = from.format('YYYY-MM-DD'),
    toString = to.format('YYYY-MM-DD');

  // get order summary data
  $scope.loadingOrderGraph = true;
  OrderService.getOrdersByDate(fromString, toString).then(function(orders) {
    // get array of month values in form [8, 9, 10, 11, 12, 1]
    var months = [],
      tempDate = from;
    for (var i = 0; i < 6; i++) {
      months.push(tempDate.format('MM'));
      tempDate.add(1, 'month');
    }

    // calculate order totals for each month
    var monthTotals = [0, 0, 0, 0, 0, 0];
    orders.forEach(function(order) {
      var orderDate = moment(order.createddate);
      var month = orderDate.format('MM');
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

  }, function(error) {
    $scope.ordersGraphMessage = 'Error loading orders.';
  }).finally(function() {
    $scope.loadingOrderGraph = false;
  });

}]);