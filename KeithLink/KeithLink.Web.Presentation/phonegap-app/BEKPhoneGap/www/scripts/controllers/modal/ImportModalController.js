'use strict';

angular.module('bekApp')
.controller('ImportModalController', ['$scope', '$modalInstance', '$state', 'ListService', 'CartService',
  function ($scope, $modalInstance, $state, ListService, CartService) {
  
  $scope.upload = [];
  var files = [];

  function goToImportedPage(routeName, routeParams) {
    $state.go(routeName, routeParams).then(function() {
      $modalInstance.close();
    });
  }

  $scope.onFileSelect = function($files) {
    files = [];
    for (var i = 0; i < $files.length; i++) {
      files.push($files[i]);
    }
  };

  $scope.startListUpload = function(options) {
    var file = files[0];

    ListService.importList(file, options).then(function(data) {
      goToImportedPage('menu.lists.items', { listId: data.listid });
    });
  };

  $scope.startOrderUpload = function(options) {
    var file = files[0];

    CartService.importCart(file, options).then(function(data) {
      $state.go('menu.cart.items', { cartId: data.listid }).then(function() {
        $modalInstance.close(data);
      });
    });
  };
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);
