'use strict';

angular.module('bekApp')
.controller('ItemUsageSummaryModalController', ['$scope', '$modalInstance', 'item', 'ProductService',
  function ($scope, $modalInstance, item, ProductService) {

 $scope.item = item;
 
    ProductService.getProductDetails(item.itemnumber, 'BEK').then(function(item) {
      $scope.item = item;
       // used to determine if the item has order history in the view
      $scope.orderHistory =[];
      $scope.month = {date:'',number:''};
      var i = 0;
        for (var key in item.orderhistory) {
	        if (item.orderhistory.hasOwnProperty(key)) {
	           $scope.month.date = key;
	           $scope.month.number = item.orderhistory[key];
	           $scope.orderHistory.unshift(angular.copy($scope.month));
	           i++;
	        }
    	}	
    });



  $scope.closeModal = function() {
    $modalInstance.dismiss('cancel');
  };

}]);