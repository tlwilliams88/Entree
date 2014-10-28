'use strict';

angular.module('bekApp')
.controller('ListImportModalController', ['$scope', '$modalInstance', '$upload', '$state', 'ListService',
  function ($scope, $modalInstance, $upload, $state, ListService) {
  
  $scope.upload = [];
  $scope.onFileSelect = function($files) {
    //$files: an array of files selected, each file has name, size, and type.
    for (var i = 0; i < $files.length; i++) {
      var file = $files[i];
      (function(index) {
          $scope.upload[index] = ListService.importList(file).then(function(data) {
            $modalInstance.close(data);
            $state.go('menu.lists.items', { listId: data.listid });
          });
      })(i);
    }
  };
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);