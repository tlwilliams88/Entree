'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$analytics', '$modalInstance', 'PrintService', 'ListService', 'CartService', 'list', 'cart', 'pagingModelOptions', 'contractFilter',
  function ($scope, $analytics, $modalInstance, PrintService, ListService, CartService, list, cart, pagingModelOptions, contractFilter) {
 
  $scope.printingOrder = false;
  
  if(list) {
      $scope.list = list;
  }
  
  if(cart) {
    $scope.cart = cart;
    $scope.printingOrder = true;
  }
  $scope.originalPagingModel = pagingModelOptions;
  
  $scope.contractFilter = contractFilter.filter;

  if($scope.list.isfavorite || $scope.list.read_only || $scope.list.ismandatory || $scope.list.isreminder){
    $scope.defaultList = true;
  }

  $scope.labelOptions = [{
    type: '5160',
    numberOnPage: 30,
    columns: 3
  }];
  $scope.selectedLabelOption = $scope.labelOptions[0]; // pre-select first option
  $scope.groupLabels = $scope.showparvalues = $scope.shownotes = $scope.landscape = false;

  // $scope.printLabels = function(items, labelOption) {
  //   var data = {
  //     labelOption: labelOption,
  //     items: items
  //   };
  //   PrintService.print('views/printTemplates/productlabels.html', data);
  // };

  $scope.printLabels = function(list, labelOption) {
    $analytics.eventTrack('Print List', {  category: 'Lists', label: 'Print Labels' });
    ListService.printBarcodes(list);
  };

  $scope.printList = function(list, landscape, showparvalues, groupLabels, shownotes, prices) {
    if(groupLabels){
      pagingModelOptions= {
              sort: [{
                field: 'label',
                order:  'desc'
              }],
              terms: undefined
            };
    } else {
     pagingModelOptions = $scope.originalPagingModel;
    }
    
    var contractFilter;
    if(list.is_contract_list == true) {
      contractFilter = $scope.contractFilter;
    }
    
    if(!$scope.printingOrder){
      $analytics.eventTrack('Print List', {  category: 'Lists', label: 'Print Page' });
      ListService.printList(list, landscape, showparvalues, pagingModelOptions, shownotes, prices, contractFilter);
    }
    else{
      $analytics.eventTrack('Print Order', {  category: 'Add To Order'});
      CartService.printOrder(list, $scope.cart.id, landscape, showparvalues, pagingModelOptions, shownotes);
    }
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);
