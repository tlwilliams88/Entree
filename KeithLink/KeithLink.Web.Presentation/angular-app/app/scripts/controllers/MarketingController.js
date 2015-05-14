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



      }

      $scope.loadingResults = false;
      $scope.registeredUsersQuery = {};

      // determine starting date of 1 month in the past
      var initialFromDate = new Date();
      initialFromDate = initialFromDate.setMonth(initialFromDate.getMonth() - 1);
      initialFromDate = new Date(initialFromDate);

      $scope.registeredUsersQuery.fromDate = initialFromDate;
      $scope.registeredUsersQuery.toDate = new Date();

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



      var itemsPerPage = 50,
        itemIndex = 0;
      $scope.sortTable = function(field, sortDescending) {
        $scope.sortDescending = field === $scope.sortField ? !sortDescending : false;
        $scope.sortField = field;

        getRegisteredUsers();
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
