'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:InventoryReportController
 * @description
 * # InventoryReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('InventoryReportController', ['$scope', '$q', '$modal', 'reports', 'ProductService', 'PricingService', 'ListService', 'List',
    function($scope, $q, $modal, reports, ProductService, PricingService, ListService, List) {

      $scope.subtotal = 0;
      $scope.sortField = 'position';
      $scope.sortDescending = false;

      $scope.listsLoading = true;
      ListService.getListHeaders().then(function(listHeaders) {
        $scope.lists = listHeaders;
      }).finally(function() {
        $scope.listsLoading = false;
      });

      function refreshSubtotal() {
        $scope.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.report.items, 'extprice');
        return $scope.subtotal;
      }

      function onItemQuantityChanged(newVal, oldVal) {
        var changedExpression = this.exp; // jshint ignore:line
        var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
        var item = $scope.report.items[idx];

        item.price = PricingService.getUnitPriceForItem(item);
        item.extprice = PricingService.getPriceForItem(item);
        if (item.quantity) {
          item.quantity = parseFloat(item.quantity);
        }

        refreshSubtotal();
      }

      var watchersQuantity = [];
      var watchersEach = [];

      var deletedItems = [];
      $scope.removeRow = function(item) {
        $scope.inventoryForm.$setDirty();
        
        var idx = $scope.report.items.indexOf(item);
        var quantityWatch = watchersQuantity.splice(watchersQuantity.length - 1, 1);
        quantityWatch[0]();
        var eachWatch = watchersEach.splice(watchersEach.length - 1, 1);
        eachWatch[0]();

        var deletedItem = $scope.report.items.splice(idx, 1);
        if (item.listitemid) {
          item.isdeleted = true;
          deletedItems.push(item);
        }
      };

      $scope.addRow = function(item, useListItemId) {
        var reportItem = {
          itemid: item.itemnumber,
          name: item.name,
          packsize: item.packsize,
          label: item.label,
          quantity: item.quantity,
          each: false,
          packageprice: item.packageprice,
          caseprice: item.caseprice,
          catchweight: item.catchweight,
          hasPackagePrice: PricingService.hasPackagePrice(item)
        };
        if (useListItemId === true) {
          reportItem.listitemid = item.listitemid;
          reportItem.position = item.position;
        }

        $scope.report.items.push(reportItem);

        var lastIndex = $scope.report.items.length - 1;
        watchersQuantity.push($scope.$watch('report.items[' + lastIndex + '].quantity', onItemQuantityChanged));
        watchersEach.push($scope.$watch('report.items[' + lastIndex + '].each', onItemQuantityChanged));
      };

      $scope.addItemByItemNumber = function(itemNumber) {
        $scope.successMessage = '';
        $scope.errorMessage = '';

        ProductService.getProductDetails(itemNumber)
          .then(function(item) {
            $scope.successMessage = 'Added Item # ' + itemNumber + ' to the report.';
            $scope.inventoryForm.$setDirty();
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
          $scope.inventoryForm.$setDirty();
          listFound.items.forEach($scope.addRow);
        });
      };

      $scope.sortTable = function(field, sortDescending) {
        $scope.sortDescending = $scope.sortField === field ? !sortDescending : false;
        $scope.sortField = field;
      };

      $scope.saveReport = function(scopeReport) {
        var report = angular.copy(scopeReport);
        report.name = moment().format('YYYY-MM-DD');
        report.message = 'Saving report...';

        report.items.forEach(function(item) {
          item.itemnumber = item.itemid;
          delete item.itemid;
        });

        report.items = report.items.concat(deletedItems);

        var promise;
        if (report.listid) {
          promise = List.update({}, report).$promise;
        } else {
          promise = List.save({ type: 'InventoryValuation' }, report).$promise;
        }

        promise.then(function(response) {
          $scope.inventoryForm.$setPristine();
          deletedItems = [];
          $scope.displayMessage('success', 'Successfully saved report.');
        }, function() {
          $scope.displayMessage('error', 'Error saving report.');
        });
      };

      $scope.clearReport = function(listId) {
        // clear item watches
        watchersQuantity.forEach(function(watch) {
          watch();
        });
        watchersEach.forEach(function(watch) {
          watch();
        });

        List.delete({
            listId: listId
          }).$promise.then(function() {
            $scope.report = {};
            $scope.report.items = [];
          });
      };

      $scope.openExportModal = function() {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/inventoryreportexportmodal.html',
          controller: 'InventoryReportExportModalController',
          resolve: {
            items: function() {
              return $scope.report.items;
            }
          }
        });
      };

      $scope.report = {
        items: []
      };
      if (reports && reports.length > 0) {
        $scope.report.listid = reports[0].listid;
        reports[0].items.forEach(function(item) {
          $scope.addRow(item, true);
        });
      }

    }
  ]);
