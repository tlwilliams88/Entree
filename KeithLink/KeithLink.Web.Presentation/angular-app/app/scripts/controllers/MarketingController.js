'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:MarketingController
 * @description
 * # MarketingController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MarketingController', ['$scope', '$modal', 'MarketingService', 'DateService', 'CartService', 'ReportService',
    function($scope, $modal, MarketingService, DateService, CartService, ReportService) {

  CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });

      function getRegisteredUsers() {
        $scope.loadingResults = true;
        MarketingService.getUsersAndMarketingInfo(DateService.formatJavascriptDate($scope.fromDate), DateService.formatJavascriptDate($scope.toDate)).then(function(users) {
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
            location: function() {
              return {category:'Marketing', action:'Export Registered Users'};
            },
            headerText: function() {
              return 'Registered Users';
            },
            exportMethod: function() {
              return ReportService.exportItem;
            },
            exportConfig: function() {
              return MarketingService.getMarketingInfoExportConfig();
            },
            exportParams: function() {
              var params = {
                from: DateService.formatJavascriptDate($scope.fromDate),
                to: DateService.formatJavascriptDate($scope.toDate)
              };
              return '/profile/export/marketinginfo?' + jQuery.param(params);
            }
          }
        });
      };
    }

  ]);
