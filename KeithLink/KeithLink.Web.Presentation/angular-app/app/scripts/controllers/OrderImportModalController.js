'use strict';

angular.module('bekApp')
.controller('OrderImportModalController', ['$scope', '$modalInstance', '$state', 'CartService',
  function ($scope, $modalInstance, $state, CartService) {
  
  $scope.upload = [];
  $scope.onFileSelect = function($files) {
    //$files: an array of files selected, each file has name, size, and type.
    for (var i = 0; i < $files.length; i++) {
      var file = $files[i];
      /* jshint ignore:start */
      (function(index) {
          $scope.upload[index] = CartService.importCart(file).then(function(data) {
            $state.go('menu.cart.items', { cartId: data.id }).then(function() {
              $modalInstance.close(data);
            });
          });
      })(i);
      /* jshint ignore:end */
    }
  };
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);