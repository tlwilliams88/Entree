'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:InventoryReportController
 * @description
 * # InventoryReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('InventoryReportController', ['$scope', '$modal', 'ProductService', 'PricingService', 'ListService',
    function($scope, $modal, ProductService, PricingService, ListService) {

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
        item.extprice = PricingService.getPriceForItem(item);

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
          itemnumber: item.itemnumber,
          name: item.name,
          packsize: item.packsize,
          label: item.label,
          quantity: 0,
          each: false,
          packageprice: item.packageprice,
          caseprice: item.caseprice,
          hasPackagePrice: PricingService.hasPackagePrice(item)
        });

        var lastIndex = $scope.items.length - 1;
        watchersQuantity.push($scope.$watch('items[' + lastIndex + '].quantity', onItemQuantityChanged));
        watchersEach.push($scope.$watch('items[' + lastIndex + '].each', onItemQuantityChanged));
      };

      $scope.addItemByItemNumber = function(itemNumber) {
        ProductService.getProductDetails(itemNumber)
          .then(function(item) {
            $scope.displayMessage('success', 'Adding ' + item.name + ' to report.');  
            return item;
          })
          .then($scope.addRow);
      };

      $scope.addItemsFromList = function(listId) {
        ListService.getListWithItems(listId).then(function(listFound) {
          $scope.displayMessage('success', 'Adding ' + listFound.items.length + ' items to report.');
          listFound.items.forEach($scope.addRow);
        });
      };

      //     $scope.openExportModal = function() {
      //     var modalInstance = $modal.open({
      //     templateUrl: 'views/modals/exportmodal.html',
      //     controller: 'ExportModalController',
      //     resolve: {
      //       headerText: function () {
      //         return 'Item Usage';
      //       },
      //       exportMethod: function() {
      //         return ReportService.exportItem;
      //       },
      //       exportConfig: function() {
      //         return ReportService.getExportConfig();
      //       },
      //       exportParams: function() {
      //         var params = {                
      //             sortfield: $scope.sortField,
      //             sortdir: $scope.sortReverse === true ? 'desc' : 'asc'
      //            };
      //         return '/report/itemusage/export?' + jQuery.param(params);
      //       }
      //     }
      //   });
      // };

    }
  ]);
