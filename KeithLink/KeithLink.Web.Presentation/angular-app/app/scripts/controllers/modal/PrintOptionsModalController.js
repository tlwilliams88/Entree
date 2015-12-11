'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'ListService', 'CartService', 'list', 'cart', 'pagingModelOptions',
  function ($scope, $modalInstance, PrintService, ListService, CartService, list, cart, pagingModelOptions) {

  $scope.list = list;
  if(cart){
    $scope.cart = cart;
    $scope.printingOrder = true;
  }
  $scope.originalPagingModel = pagingModelOptions;

  if($scope.list.isfavorite || $scope.list.read_only || $scope.list.ismandatory || $scope.list.isreminder){
    $scope.defaultList = true;
  }

  $scope.labelOptions = [{
    type: '5160',
    numberOnPage: 30,
    columns: 3
  }];
  $scope.selectedLabelOption = $scope.labelOptions[0]; // pre-select first option
  $scope.groupLabels = $scope.showparvalues = $scope.shownotes= $scope.landscape = false;

  // $scope.printLabels = function(items, labelOption) {
  //   var data = {
  //     labelOption: labelOption,
  //     items: items
  //   };
  //   PrintService.print('views/printTemplates/productlabels.html', data);
  // };

  $scope.printLabels = function(list, labelOption) {
    ListService.printBarcodes(list.listid);
  };

  $scope.printList = function(list, landscape, showparvalues, groupLabels, shownotes) {
    if(groupLabels){
      pagingModelOptions= { 
              sort: [{
                field: 'label',
                order:  'desc' 
              }],
              terms: undefined
            };
    }
    else{
     pagingModelOptions = $scope.originalPagingModel;
    }
    if(!$scope.printingOrder){
      ListService.printList(list.listid, landscape, showparvalues, pagingModelOptions, shownotes);
    }
    else{
    CartService.printOrder(list.listid, $scope.cart.id, landscape, showparvalues, pagingModelOptions, shownotes);
    }
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);