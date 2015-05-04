'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:InventoryReportController
 * @description
 * # InventoryReportController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('InventoryReportController', ['$scope', '$state', 'ReportService', 'ProductService', '$modal', 'ListService',
    function ($scope, $state, ReportService, ProductService, $modal, ListService ) {
       
$scope.listsLoading = true;
 ListService.getAllLists().then(function(data){
  $scope.listsLoading = false;
$scope.lists = data;
 });


 $scope.removeRow = function(item) {
    var idx = $scope.items.indexOf(item);
    $scope.items.splice(idx, 1);
  };

    $scope.addRow = function() {
    $scope.items.push({
      itemnumber: '',
      itemname: '',
      quantity: '',
      each: false
    });
  };

// $scope.searchItemNum = function(itemNum){
// if(itemNum.length === 6)
// ListService.getItem(itemNum);
// };


$scope.selectList = function(list){
$scope.items.forEach(function(item){
if(!item.itemname && !item.itemnumber)
$scope.removeRow(item);
});

list.items.forEach(function(item){
$scope.items.push({
      itemnumber: item.itemnumber,
      itemname: item.itemname,
      quantity: '',
      each: item.each
    });

});

};

    // $scope.goToItemDetails = function (item) {
    //     ProductService.selectedProduct = item;
    //     $state.go('menu.catalog.products.details', {
    //         itemNumber: item.itemnumber
    //     });
    // };


   
        // $scope.sortTable = function (field) {
        //     $scope.itemsPerPage = 50;
        //     $scope.itemIndex = 0;

        //     if (field !== 'caseprice' || $scope.totalItems <= $scope.maxSortCount) {
        //         if ($scope.sortField !== field) { // different field
        //             $scope.sortField = field;
        //             $scope.sortReverse = true;
        //         } else { // same field
        //             $scope.sortReverse = !$scope.sortReverse;
        //         }
        //         loadItemUsage();
        //     }
        // };

       
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

  $scope.items = [];
  $scope.addRow();

  }]);
