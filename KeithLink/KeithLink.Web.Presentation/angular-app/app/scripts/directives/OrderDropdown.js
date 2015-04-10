'use strict';

angular.module('bekApp')
.directive('orderDropdown', [function(){
  return {
    restrict: 'E',
    // replace : true,
    // scope: true,
    templateUrl: 'views/directives/orderdropdown.html',
    controller: ['$scope', '$modal', '$state', function($scope, $modal, $state){

      $scope.openQuickAddModal = function() {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/cartquickaddmodal.html',
          controller: 'CartQuickAddModalController'
        });
   
        modalInstance.result.then(function(cartId) {
          $state.go('authorize.menu.cart.items', {cartId: cartId});
        });
      };
   
      $scope.openOrderImportModal = function () {
        var modalInstance = $modal.open({
          templateUrl: 'views/modals/orderimportmodal.html',
          controller: 'ImportModalController'
        });
      };
    }]
  };
}]);