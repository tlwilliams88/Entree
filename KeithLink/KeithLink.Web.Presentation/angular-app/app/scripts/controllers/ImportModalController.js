'use strict';

angular.module('bekApp')
.controller('ImportModalController', ['$scope', '$modalInstance', '$state', 'ListService', 'OrderService',
  function ($scope, $modalInstance, $state, ListService, OrderService) {
  
  $scope.upload = [];
  var files = [];

  $scope.onFileSelect = function($files) {
    files = [];
    for (var i = 0; i < $files.length; i++) {
      files.push($files[i]);
    }
  };

  $scope.startListUpload = function() {
    var file = files[0];
    ListService.importList(file).then(function(data) {
      $state.go('menu.lists.items', { listId: data.listid }).then(function() {
        $modalInstance.close(data);
      });
    });
  };

  $scope.startOrderUpload = function() {
    var file = files[0];
    CartService.importCart(file).then(function(data) {
      $state.go('menu.cart.items', { cartId: data.id }).then(function() {
        $modalInstance.close(data);
      });
    });
  }
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);