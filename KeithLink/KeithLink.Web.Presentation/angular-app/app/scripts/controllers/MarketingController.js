'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:MarketingController
 * @description
 * # MarketingController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MarketingController', ['$scope', 'MarketingService', 'UtilityService',
    function($scope, MarketingService, UtilityService) {

      function getRegisteredUsers() {
        $scope.loadingResults = true;
        MarketingService.getUsersAndMarketingInfo(UtilityService.formatJavascriptDate($scope.fromDate), UtilityService.formatJavascriptDate($scope.toDate)).then(function(users) {
          $scope.users = users;
        }).finally(function() {
          $scope.loadingResults = false;
        });
      }

      $scope.loadingResults = false;
      
      // determine starting date of 1 month in the past
      var initialFromDate = new Date();
      initialFromDate = initialFromDate.setMonth(initialFromDate.getMonth() - 1);
      initialFromDate = new Date(initialFromDate);

      $scope.fromDate = initialFromDate;
      $scope.toDate = new Date();

      $scope.sortField = 'date';
      $scope.sortDescending = true;

      getRegisteredUsers();

      $scope.datepickerOptions = {
        minDate: initialFromDate,
        maxDate: new Date(),
        options: {
          dateFormat: 'yyyy-MM-dd',
          showWeeks: false
        }
      };

      $scope.sortTable = function(field, sortDescending) {
        $scope.sortDescending = field === $scope.sortField ? !sortDescending : false;
        $scope.sortField = field;
      };

      $scope.refreshSearch = function() {
        getRegisteredUsers();
      };
    
      $scope.openExportModal = function() {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/exportmodal.html',
          controller: 'ExportModalController',
          resolve: {
            headerText: function() {
              return 'Registered Users';
            },
            exportMethod: function() {
              return ReportService.exportItem;
            },
            exportConfig: function() {
              return ReportService.getExportConfig();
            },
            exportParams: function() {
              var params = {
                fromdate: $scope.itemusagequery.fromDate,
                todate: $scope.itemusagequery.toDate,
                sortfield: $scope.sortField,
                sortdir: $scope.sortReverse === true ? 'desc' : 'asc'
              };
              return '/report/itemusage/export?' + jQuery.param(params);
            }
          }
        });
      };
    }

  ]);
