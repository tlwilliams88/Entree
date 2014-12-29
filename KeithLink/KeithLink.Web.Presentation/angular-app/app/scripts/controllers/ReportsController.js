'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CatalogController
 * @description
 * # CatalogController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ReportsController', ['$scope', '$state', 'ReportService',
    function ($scope, $state, ReportService) {
        $scope.itemusagequery = {};
        $scope.itemUsageForm = {};
        $scope.itemusagequery.fromDate = '1/23/2014';
        $scope.itemusagequery.toDate = '1/11/2015';
        $scope.getItemUsage = ReportService.getItemUsageReport;
        $scope.sortField = 'TotalQuantityOrdered';
        $scope.sortReverse = true;

        function loadItemUsage() {
            $scope.loadingResults = true;
            $scope.getItemUsage($scope.itemusagequery.fromDate, $scope.itemusagequery.toDate,
                                                         $scope.sortField, $scope.sortReverse == true ? 'desc' : 'asc')
                .then(function (items) {
                    $scope.itemusages = items;
                    $scope.loadingResults = false;
                });
        }

        $scope.itemUsageForm.updateItems = function () {
            loadItemUsage();
        }

        $scope.goToItemDetails = function (item) {
            ProductService.selectedProduct = item;
            $state.go('menu.catalog.products.details', {
                itemNumber: item.itemnumber
            });
        };

        $scope.sortTable = function (field) {
            $scope.itemsPerPage = 50;
            $scope.itemIndex = 0;

            if (field !== 'caseprice' || $scope.totalItems <= $scope.maxSortCount) {
                if ($scope.sortField !== field) { // different field
                    $scope.sortField = field;
                    $scope.sortReverse = true;
                } else { // same field
                    $scope.sortReverse = !$scope.sortReverse;
                }
                loadItemUsage();
            }
        };

        // INIT
        loadItemUsage();
  }]);