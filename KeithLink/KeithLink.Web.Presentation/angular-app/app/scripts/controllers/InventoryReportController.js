'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:InventoryReportController
 * @description
 * # InventoryReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('InventoryReportController', ['$scope', '$filter', '$analytics', '$q', '$modal', '$stateParams', '$state', 'toaster', 'reports', 'Constants', 'DateService', 'ProductService', 'PricingService', 'ListService', 'List', 'ListPagingModel',
    function($scope, $filter, $analytics, $q, $modal, $stateParams, $state, toaster, reports, Constants, DateService, ProductService, PricingService, ListService, List, ListPagingModel) {
      $scope.reports = reports;
      $scope.subtotal = 0;
      $scope.sortField = 'position';
      $scope.sortDescending = false;
      $scope.confirmQuantity = ListService.confirmQuantity;
      $scope.listsLoading = true;
      $scope.numberReportNamesToShow = 10;
      $scope.today = DateService.momentObject().format(Constants.dateFormat.yearMonthDayDashes);
      $scope.sortBy = 'position';
      $scope.sortOrder = true;

      var listPagingModel;

      var orderBy = $filter('orderBy');

      ListService.getListHeaders().then(function(listHeaders) {
        $scope.lists = listHeaders;
      }).finally(function() {
        $scope.listsLoading = false;
      });

      function refreshSubtotal() {
        $scope.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.report.items, 'extprice');
        return $scope.subtotal;
      }

      //Toggle scope variable to render Reports side panel when screen is resized
      $(window).resize(function(){
        $scope.$apply(function(){
          $scope.renderSidePanel();
        });
      });

      $scope.renderSidePanel = function(){
        $scope.resized = window.innerWidth > 991;
      };

      $scope.renderSidePanel();

      function init(report) {
        $scope.report = {
          items: []
        };
        if(reports && reports.length > 0){
          var lastIndex = reports.length - 1;
          if($stateParams.listid){
            if($stateParams.listid === 'newReport'){
            //Call save function to create new report
              $scope.saveReport();
              $scope.showMoreReportNames = ((lastIndex + 1) > $scope.numberReportNamesToShow) ? true : false;
            }
            else{
              var found = false;
              //Find the requested report
              reports.forEach(function(report, index){
                if(report.listid == $stateParams.listid || (index == lastIndex) && !found){
                  found = true;
                  $scope.report = report;
                  $scope.selectedReportName = $scope.report.name;
                  $scope.showMoreReportNames = ((index + 1) > $scope.numberReportNamesToShow) ? true : false;
                }
              });
            }
          }
          else{
            //Find last created report if none requested and not creating new report
            $scope.report = reports[lastIndex];
            $scope.selectedReportName = $scope.report.name;
            $scope.showMoreReportNames = (lastIndex > $scope.numberReportNamesToShow) ? true : false;
          }
        }
        if($scope.report.items.length > 0){
          $scope.report.items.forEach(function(item,index){
            item.position = index + 1;
            if(item.custominventoryitemid){
              $scope.hasCustomItems = true;
            }
            watchersQuantity.push($scope.$watch('report.items[' + index + '].quantity', onItemQuantityChanged));
            watchersEach.push($scope.$watch('report.items[' + index + '].each', onItemQuantityChanged));
          });
        }

        if(report) {
          report.items.forEach(function(item) {
            item.quantity = 0;
          })
            $scope.report = report;
            updatePositions();
        }
      }


      function updatePositions() {

        var newPosition = 1;
        $scope.report.items.forEach(function(item, index) {
          if(!item.isdeleted){
              item.position = newPosition;
              item.editPosition = newPosition;
              newPosition += 1;
          }
        });

      }

      function onItemQuantityChanged(newVal, oldVal) {
        var changedExpression = this.exp; // jshint ignore:line
        var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
        var item = $scope.report.items[idx];

        if(item != null && item.each != null){
          item.price = PricingService.getUnitPriceForItem(item);
          item.extprice = PricingService.getPriceForItem(item);
          if (item.quantity) {
            item.quantity = parseFloat(item.quantity);
          }
        }

        refreshSubtotal();
      }

      var watchersQuantity = [];
      var watchersEach = [];

      var deletedItems = [];

      $scope.removeRow = function(item) {
        $scope.inventoryForm.$setDirty();
        item.quantity = 0;
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

        updatePositions();
        refreshSubtotal();
      };

      $scope.addRow = function(item, useListItemId) {
        var nextLineNumber = $scope.report.items.length + 1,
            reportItem = {
              itemnumber: item.itemnumber,
              position: nextLineNumber,
              name: item.name,
              packsize: item.packsize,
              pack: item.pack,
              label: item.label,
              quantity: 0,
              each: item.each,
              packageprice: item.packageprice,
              caseprice: item.caseprice,
              catchweight: item.catchweight,
              hasPackagePrice: PricingService.hasPackagePrice(item),
              hasCasePrice: PricingService.hasCasePrice(item),
              average_weight: item.average_weight,
              class: item.class,
              category: item.category,
              brand_extended_description: item.brand_extended_description || item.brand,
              isCustomInventory: item.isCustomInventory,
              supplier: item.supplier
            };

        if(item.isCustomInventory){
          reportItem.custominventoryitemid = item.id ? item.id : item.custominventoryitemid;
          $scope.hasCustomItems = true;
        }

        if (useListItemId === true) {
          reportItem.listitemid = item.listitemid;
          reportItem.position = item.position;
        }
        $scope.report.items.push(reportItem);

        if(reportItem.category){
          $scope.report.hasContractCategories = true;
        }

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

        if(listId == 'Custom Inventory'){
          ListService.getCustomInventoryList().then(function(list){
            $scope.successMessage = 'Added ' + list.items.length + ' items from ' + list.name + ' to report.';
            $scope.inventoryForm.$setDirty();
            $scope.isCustomInventory = true;
            list.items.forEach(function(item){
              item.isCustomInventory = true;
              $scope.addRow(item);
            });
            $scope.saveReport();
          });
        } else {
            ListService.getListWithItems(listId).then(function(listFound) {
            $scope.successMessage = 'Added ' + listFound.items.length + ' items from ' + listFound.name + ' to report.';
            $scope.inventoryForm.$setDirty();
            listFound.items.forEach(function(item) {
              if(item.custominventoryitemid){
                item.isCustomInventory = true;
              }
              $scope.addRow(item);
            });

            $scope.saveReport();
          });
        }

      };

      $scope.resetItemPositions = function() {

      };

      function startLoading() {
        $scope.loadingResults = true;
      }

      function stopLoading() {
        $scope.loadingResults = false;
      }

      $scope.sortList = function(sortBy, sortOrder) {

        $scope.sortBy = sortBy;

        listPagingModel = new ListPagingModel(
          $scope.report.listid,
          init,
          null,
          startLoading,
          stopLoading,
          $scope.sort,
          $scope.report.items.length
        );

        $scope.sortOrder = sortOrder ? 'asc' : 'desc';
        $scope.sort = [{
          field: $scope.sortBy,
          order: $scope.sortOrder
        }];
        listPagingModel.sortListItems($scope.sort);

      };

      $scope.saveReport = function() {
        // $scope.report.items.reverse();
        var report = angular.copy($scope.report);
        var sameDayReports = [];
        if(!report.name){
          report.name = $scope.today;
        }

        if($scope.reports && $scope.reports.length > 0 && report.name.length === 10){
          $scope.reports.forEach(function(existingReport){

            if(report.name === existingReport.name.slice(0,10)){
              sameDayReports.push(existingReport);
            }
          });

          var highestDuplicate = 0;
          if(sameDayReports.length > 0){
            sameDayReports.forEach(function(sameDayReport){
              var openParenthIndex = sameDayReport.name.indexOf('(');
              if(openParenthIndex > -1){
                var duplicateCount = parseInt(sameDayReport.name.slice(openParenthIndex + 1, sameDayReport.name.indexOf(')')));
                if(duplicateCount > highestDuplicate){
                  highestDuplicate = duplicateCount;
                }
              }
            });
            highestDuplicate++;
            report.name = report.name.concat(' ('+(highestDuplicate)+')');
          }
        }

        report.message = 'Saving report...';
        report.items = report.items.concat(deletedItems);

        var promise;
        var creatingList = false;
        if (report.listid) {
          List.update({}, report).$promise.then(function() {
              $scope.successMessage = '';
              $scope.errorMessage = '';
              $scope.inventoryForm.$setPristine();

              toaster.pop('success', 'Successfully saved report.');
            }, function() {
              toaster.pop('error', 'Error saving report.');
            });
        } else {
          $analytics.eventTrack('Run Inventory Valuation', {  category: 'Reports'});
          creatingList = true;
          List.save({ type: 'InventoryValuation' }, report).$promise.then(function() {
              $scope.successMessage = '';
              $scope.errorMessage = '';
              $scope.inventoryForm.$setPristine();

              toaster.pop('success', 'Successfully saved report.');
            }, function() {
              toaster.pop('error', 'Error saving report.');
            });

          }

        };

      /**************
        Rename Report
      **************/
      $scope.renameReport = function (reportname) {
        $scope.report.isRenaming = false;
        $scope.report.name = reportname;
        $scope.saveReport();
      };

      $scope.cancelRenameReport = function(){
        $scope.report.isRenaming = false;
      };

      $scope.deleteReport = function(listId){
          List.delete({
            listId: listId
          }).$promise.then(function() {
            $scope.reports.forEach(function(report, index){
              if(report.listid === listId){
                $scope.reports.splice(index,1);
              }
            });
            var rep = ($scope.reports.length > 0) ? $scope.reports[$scope.reports.length - 1].listid : 'newReport';
            $state.go('menu.inventoryreport', {listid: rep});
          });
      };

      $scope.goToReport = function(listId){
        $scope.report.isRenaming = false;
        $state.go('menu.inventoryreport', {listid: listId});
        if($scope.reports && $scope.reports.length){
          $scope.selectedReportName = $filter('filter')($scope.reports, {listid: listId})[0].name;
        } else {
          $scope.selectedReportName = $scope.today;
        }

      };

      $scope.createReport = function(){
        $state.go('menu.inventoryreport', {listid: 'newReport'});
      };

      $scope.clearReport = function(listId) {
        // clear item watches
        watchersQuantity.forEach(function(watch) {
          watch();
        });
        watchersEach.forEach(function(watch) {
          watch();
        });
        $scope.inventoryForm.$setDirty();
        $scope.successMessage = '';
        $scope.errorMessage = '';
        $scope.subtotal = 0;
        //$scope.report = {};
        $scope.report.items.forEach(function(item){
          item.isdeleted = true;
        });
        deletedItems = $scope.report.items;
        $scope.report.items = [];
      };

      $scope.openExportModal = function() {

        if($scope.inventoryForm.$dirty){
          $scope.saveReport($scope.report);
        }

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

      init();
    }
  ]);
