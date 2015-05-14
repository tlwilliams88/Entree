'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:MarketingController
 * @description
 * # MarketingController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MarketingController', ['$scope', 'MarketingService',
    function($scope, MarketingService) {

      function loadRegisteredUsers() {
        $scope.loadingResults = true;



      }

      $scope.loadingResults = false;
      $scope.registeredUsersQuery = {};

      var initialFromDate = moment().subtract(6, 'month'),
        initialToDate = moment();
      $scope.registeredUsersQuery.fromDate = initialFromDate.format('YYYY-MM-DD');
      $scope.registeredUsersQuery.toDate = initialToDate.format('YYYY-MM-DD');

      $scope.sortField = 'date';
      $scope.sortDescending = true;

      loadRegisteredUsers();

      $scope.datepickerOptions = {
        minDate: initialFromDate,
        maxDate: new Date(),
        options: {
          dateFormat: 'yyyy-MM-dd',
          showWeeks: false
        }
      };



      $scope.sortTable = function() {

      };

      $scope.refreshSearch = function() {

      };
    
      $scope.openExportModal = function() {
        // var modalInstance = $modal.open({
        //   templateUrl: 'views/modals/exportmodal.html',
        //   controller: 'ExportModalController',
        //   resolve: {
        //     headerText: function() {
        //       return 'Item Usage';
        //     },
        //     exportMethod: function() {
        //       return ReportService.exportItem;
        //     },
        //     exportConfig: function() {
        //       return ReportService.getExportConfig();
        //     },
        //     exportParams: function() {
        //       var params = {
        //         fromdate: $scope.itemusagequery.fromDate,
        //         todate: $scope.itemusagequery.toDate,
        //         sortfield: $scope.sortField,
        //         sortdir: $scope.sortReverse === true ? 'desc' : 'asc'
        //       };
        //       return '/report/itemusage/export?' + jQuery.param(params);
        //     }
        //   }
        // });
      };
    }

  ]);
