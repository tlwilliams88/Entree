'use strict';

angular.module('bekApp')
.controller('ItemUsageSummaryModalController', ['$scope', '$modalInstance', 'item', 'ProductService',
  function ($scope, $modalInstance, item, ProductService) {

 $scope.item = item;
 
    ProductService.getProductDetails(item.itemnumber).then(function(item) {
      $scope.item = item;
       // used to determine if the item has order history in the view
      $scope.item.orderHistoryKeys = Object.keys(item.orderhistory);
    });



  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);