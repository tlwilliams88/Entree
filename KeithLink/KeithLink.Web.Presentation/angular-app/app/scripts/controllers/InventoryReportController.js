'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:InventoryReportController
 * @description
 * # InventoryReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('InventoryReportController', ['$scope', '$q', '$modal', '$stateParams', '$state', 'toaster', 'reports', 'ProductService', 'PricingService', 'ListService', 'List',
    function($scope, $q, $modal, $stateParams, $state, toaster, reports, ProductService, PricingService, ListService, List) {
      $scope.reports = reports;
      $scope.subtotal = 0;
      $scope.sortField = 'position';
      $scope.sortDescending = false;
      $scope.confirmQuantity = ListService.confirmQuantity;
      $scope.listsLoading = true;
      $scope.numberReportNamesToShow = 10;
      $scope.today = moment().format('YYYY-MM-DD');
      
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
      }
      $scope.renderSidePanel();

      function init() {
        $scope.report = {
          items: []
        };
        if(reports && reports.length > 0){
          var lastIndex = reports.length - 1;
          if($stateParams.listid){
            if($stateParams.listid === 'newReport'){ 
            //Call save function to create new report             
              $scope.saveReport($scope.report);
              $scope.showMoreReportNames = ((lastIndex + 1) > $scope.numberReportNamesToShow) ? true : false;
            }
            else{
              var found = false;
              //Find the requested report
              reports.forEach(function(report, index){
                if(report.listid == $stateParams.listid || (index === lastIndex) && !found){
                  found = true;
                  $scope.report = report;
                  $scope.showMoreReportNames = ((index + 1) > $scope.numberReportNamesToShow) ? true : false;           
                }
              })
            }
          }
          else{
            //Find last created report if none requested and not creating new report            
            $scope.report = reports[lastIndex];
            $scope.showMoreReportNames = (lastIndex > $scope.numberReportNamesToShow) ? true : false;
          }
        }
        if($scope.report.items.length > 0){
          $scope.report.items.forEach(function(item,index){
            item.position = index + 1;
            watchersQuantity.push($scope.$watch('report.items[' + index + '].quantity', onItemQuantityChanged));
            watchersEach.push($scope.$watch('report.items[' + index + '].each', onItemQuantityChanged));
          })
        }
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
      };

      $scope.addRow = function(item, useListItemId) {
        var reportItem = {
          itemnumber: item.itemnumber,
          position: $scope.report.items.length + 1,
          name: item.name,
          packsize: item.packsize,
          pack: item.pack,
          label: item.label,
          quantity: item.quantity,
          each: item.each,
          packageprice: item.packageprice,
          caseprice: item.caseprice,
          catchweight: item.catchweight,
          hasPackagePrice: PricingService.hasPackagePrice(item),
          hasCasePrice: PricingService.hasCasePrice(item),
          average_weight: item.average_weight,
          class: item.class,
          brand_extended_description: item.brand_extended_description
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
           $scope.sortTable('position', false);
        });
       
      };

      $scope.sortTable = function(field, sortDescending) {
        $scope.sortDescending = $scope.sortField === field ? !sortDescending : false;
        $scope.sortField = field;
      };

      $scope.saveReport = function(scopeReport) {
        var report = angular.copy(scopeReport);
        var sameDayReports = [];
        if(!report.name){         
          report.name = $scope.today;
        }
        
        if($scope.reports.length > 0 && report.name.length === 10){   
          $scope.reports.forEach(function(existingReport){

            if(report.name === existingReport.name.slice(0,10)){             
              sameDayReports.push(existingReport);
            }
          })

          var highestDuplicate = 0;
          if(sameDayReports.length > 0){
            sameDayReports.forEach(function(sameDayReport){
              var openParenthIndex = sameDayReport.name.indexOf('(')
              if(openParenthIndex > -1){
                var duplicateCount = parseInt(sameDayReport.name.slice(openParenthIndex + 1, sameDayReport.name.indexOf(')')));
                if(duplicateCount > highestDuplicate){
                  highestDuplicate = duplicateCount;
                }
              }
            })
            highestDuplicate++;
            report.name = report.name.concat(' ('+(highestDuplicate)+')');
          }
        }

        report.message = 'Saving report...';
        report.items = report.items.concat(deletedItems);

        var promise;
        var creatingList = false;
        if (report.listid) {
          promise = List.update({}, report).$promise;
        } else {
          promise = List.save({ type: 'InventoryValuation' }, report).$promise;
          creatingList = true;
        }

        promise.then(function(response) {
        
          $scope.successMessage = '';
          $scope.errorMessage = '';
          $scope.inventoryForm.$setPristine();

          if(creatingList){
            $scope.goToReport(response.listitemid);
          }
          toaster.pop('success', 'Successfully saved report.');
        }, function() {
          toaster.pop('error', 'Error saving report.');
        });
      };

      $scope.deleteReport = function(listId){
          List.delete({
            listId: listId
          }).$promise.then(function() {
            $scope.reports.forEach(function(report, index){
              if(report.listid === listId){                
                $scope.reports.splice(index,1);
              }
            })
            var rep = ($scope.reports.length > 0) ? $scope.reports[$scope.reports.length - 1].listid : 'newReport';
            $state.go('menu.inventoryreport', {listid: rep});
          });
      }

      $scope.goToReport = function(listId){
        $state.go('menu.inventoryreport', {listid: listId});
      }

      $scope.createReport = function(){        
        $state.go('menu.inventoryreport', {listid: 'newReport'});
      }

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
        })
        deletedItems = $scope.report.items;
        $scope.report.items = [];
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

      init();
    }
  ]);
