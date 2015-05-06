'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:InventoryReportController
 * @description
 * # InventoryReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('InventoryReportController', ['$scope', '$q', '$modal', 'ProductService', 'PricingService', 'ListService',
    function($scope, $q, $modal, ProductService, PricingService, ListService) {

      $scope.items = [];
      $scope.subtotal = 0;

      $scope.listsLoading = true;
      ListService.getListHeaders().then(function(listHeaders) {
        $scope.lists = listHeaders;
      }).finally(function() {
        $scope.listsLoading = false;
      });

      function refreshSubtotal() {
        $scope.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.items, 'extprice');
        return $scope.subtotal;
      }

      function onItemQuantityChanged(newVal, oldVal) {
        var changedExpression = this.exp; // jshint ignore:line
        var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
        var item = $scope.items[idx];

        item.price = PricingService.getUnitPriceForItem(item);
        item.extprice = PricingService.getPriceForItem(item);
        item.quantity = parseInt(item.quantity, 10);

        refreshSubtotal();
      }

      var watchersQuantity = [];
      var watchersEach = [];

      $scope.removeRow = function(item) {
        var idx = $scope.items.indexOf(item);

        // cancel item watches for last item in list
        var quantityWatch = watchersQuantity.splice(watchersQuantity.length - 1, 1);
        quantityWatch[0]();
        var eachWatch = watchersEach.splice(watchersEach.length - 1, 1);
        eachWatch[0]();

        $scope.items.splice(idx, 1);
      };

      $scope.addRow = function(item) {
        $scope.items.push({
          itemid: item.itemnumber,
          name: item.name,
          packsize: item.packsize,
          label: item.label,
          quantity: 0,
          each: false,
          packageprice: item.packageprice,
          caseprice: item.caseprice,
          catchweight: item.catchweight,
          hasPackagePrice: PricingService.hasPackagePrice(item)
        });

        var lastIndex = $scope.items.length - 1;
        watchersQuantity.push($scope.$watch('items[' + lastIndex + '].quantity', onItemQuantityChanged));
        watchersEach.push($scope.$watch('items[' + lastIndex + '].each', onItemQuantityChanged));
      };

      $scope.addItemByItemNumber = function(itemNumber) {
        $scope.successMessage = '';
        $scope.errorMessage = '';

        ProductService.getProductDetails(itemNumber)
          .then(function(item) {
            $scope.successMessage = 'Added Item # ' + itemNumber + ' to the report.';
            return item;
          }, function() {
            $scope.errorMessage = 'Item # ' + itemNumber + ' not found.';
            return $q.reject();
          })
          .then($scope.addRow);
      };

      $scope.addItemsFromList = function(listId) {
        $scope.successMessage = '';

        ListService.getListWithItems(listId).then(function(listFound) {
          $scope.successMessage = 'Added ' + listFound.items.length + ' items from ' + listFound.name + ' to report.'
          listFound.items.forEach($scope.addRow);
        });
      };

      $scope.sortTable = function(field, sortDescending) {
        $scope.sortDescending = $scope.sortField === field ? true : false;
        $scope.sortField = field;
      };

      $scope.openExportModal = function() {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/inventoryreportexportmodal.html',
          controller: 'InventoryReportExportModalController',
          resolve: {
            items: function() {
              return $scope.items;
            }
          }
        });
      };

    }
  ]);
