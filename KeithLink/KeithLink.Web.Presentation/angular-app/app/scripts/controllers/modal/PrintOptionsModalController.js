'use strict';

angular.module('bekApp')
.controller('PrintOptionsModalController', ['$scope', '$modalInstance', 'PrintService', 'ListService', 'CartService', 'list', 'cart', 'pagingModelOptions',
  function ($scope, $modalInstance, PrintService, ListService, CartService, list, cart, pagingModelOptions) {

  $scope.list = list;
  if(cart){
    $scope.cart = cart;
    $scope.printingOrder = true;
  }

  if($scope.list.isfavorite || $scope.list.name ==='History' || $scope.list.ismandatory || $scope.list.isreminder || $scope.list.is_contract_list){
    $scope.defaultList = true;
  }

  $scope.labelOptions = [{
    type: '5160',
    numberOnPage: 30,
    columns: 3
  }];
  $scope.selectedLabelOption = $scope.labelOptions[0]; // pre-select first option
  $scope.groupLabels = $scope.showparvalues = $scope.landscape = false;

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

  $scope.printList = function(list, landscape, showparvalues, groupLabels) {
    if(groupLabels){
      pagingModelOptions= { 
              sort: [{
                field: 'label',
                order:  'desc' 
              }],
              terms: undefined
            };
    }
    if(!$scope.printingOrder){
      ListService.printList(list.listid, landscape, showparvalues, pagingModelOptions);
    }
    else{
    CartService.printOrder(list.listid, $scope.cart.id, landscape, showparvalues, pagingModelOptions);
    }
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);