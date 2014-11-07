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

  $scope.startListUpload = function() {
    var file = files[0];
    ListService.importList(file).then(function(data) {
      goToImportedPage('menu.lists.items', { listId: data.listid });
    });
  };

  $scope.startOrderUpload = function() {
    var file = files[0];
    CartService.importCart(file).then(function(data) {
      goToImportedPage('menu.cart.items', { cartId: data.id });
    });
  };
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);