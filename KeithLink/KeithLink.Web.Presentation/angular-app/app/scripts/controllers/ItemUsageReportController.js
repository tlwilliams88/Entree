'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ItemUsageReportController
 * @description
 * # ItemUsageReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemUsageReportController', ['$scope', '$analytics', '$state', 'Constants', 'ReportService', 'DateService', '$modal', 
    function($scope, $analytics, $state, Constants, ReportService, DateService, $modal) {
  

  $scope.itemusagequery = {};
  $scope.itemUsageForm = {};
  $scope.totalCost = 0;

  // Generates today's date for use in formatting and determining the date range that shows in date picker
  $scope.initialFromDate = new Date();
  $scope.initialToDate = new Date();

  // Dates used for date picker
  $scope.mindate = DateService.momentObject($scope.initialFromDate).subtract(6, 'months');
  $scope.maxdate = DateService.momentObject($scope.initialToDate);

  // Dates used for api calls(export, requesting products)
  $scope.itemusagequery.fromDate = $scope.mindate.format();
  $scope.itemusagequery.toDate = $scope.maxdate.format();

  $scope.sortField = 'TotalQuantityOrdered';
  $scope.sortReverse = true;

  $scope.loadItemUsage = function() {  
    $analytics.eventTrack('Run Item Usage', {  category: 'Reports'});
    $scope.loadingResults = true;
    ReportService.itemUsageParams = {
      from: $scope.itemusagequery.fromDate,
      to: $scope.itemusagequery.toDate
    };

    ReportService.getItemUsageReport($scope.itemusagequery.fromDate, $scope.itemusagequery.toDate,
        $scope.sortField, $scope.sortReverse === true ? 'desc' : 'asc')
      .then(function(items) {
        $scope.itemusages = items;
        $scope.loadingResults = false;
        $scope.totalCost = 0;
        angular.forEach(items, function(item, index) {
          var itemCost = Number(item.totalcost);
          if($.isNumeric(itemCost)){
            $scope.totalCost += itemCost;
          }
        });
        return $scope.totalCost;
      });
  }

  // $scope.goToItemDetails = function (item) {
  //     ProductService.selectedProduct = item;
  //     $state.go('menu.catalog.products.details', {
  //         itemNumber: item.itemnumber
  //     });
  // };

  $scope.openInventoryModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/cartquickaddmodal.html',
      controller: 'CartQuickAddModalController',
      backdrop:'static'
    });

  };

  $scope.sortTable = function(field) {
    $scope.itemsPerPage = 50;
    $scope.itemIndex = 0;

    if (field !== 'caseprice' || $scope.totalItems <= $scope.maxSortCount) {
      if ($scope.sortField !== field) { // different field
        $scope.sortField = field;
        $scope.sortReverse = true;
      } else { // same field
        $scope.sortReverse = !$scope.sortReverse;
      }
      $scope.loadItemUsage();
    }
  };

  $scope.datepickerOptions = {
    minDate: $scope.mindate,
    maxDate: $scope.maxdate,
    options: {
      dateFormat: Constants.dateFormat.yearMonthDayDashes,
      showWeeks: false
    }
  };

  function formatJavascriptDate(date) {
    var day = date.getDate().toString(),
      month = (date.getMonth() + 1).toString(),
      year = date.getFullYear();

    if (day.length < 2) {
      day = '0' + day;
    }
    if (month.length < 2) {
      month = '0' + month;
    }

    return year + '-' + month + '-' + day;
  }

  $scope.openExportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/exportmodal.html',
      controller: 'ExportModalController',
      resolve: {
        location: function() {
          return {category:'Reports', action:'Export Item Usage'}
        },
        headerText: function() {
          return 'Item Usage';
        },
        exportMethod: function() {
          return ReportService.exportItem;
        },
        exportConfig: function() {
          return ReportService.getExportConfig();
        },
        exportParams: function() {
          var params = {
            fromdate: DateService.momentObject($scope.itemusagequery.fromDate,Constants.dateFormat.yearMonthDayDashes)._i,
            todate: DateService.momentObject($scope.itemusagequery.toDate,Constants.dateFormat.yearMonthDayDashes)._i,
            sortfield: $scope.sortField,
            sortdir: $scope.sortReverse === true ? 'desc' : 'asc'
          };
          return '/report/itemusage/export?' + jQuery.param(params);
        }
      }
    });
  };
}]);
