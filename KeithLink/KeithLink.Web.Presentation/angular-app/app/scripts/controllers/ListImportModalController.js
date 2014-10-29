'use strict';

angular.module('bekApp')
.controller('ListImportModalController', ['$scope', '$modalInstance', '$state', 'ListService',
  function ($scope, $modalInstance, $state, ListService) {
  
  $scope.upload = [];
  $scope.onFileSelect = function($files) {
    //$files: an array of files selected, each file has name, size, and type.
    for (var i = 0; i < $files.length; i++) {
      var file = $files[i];
      /* jshint ignore:start */
      (function(index) {
          $scope.upload[index] = ListService.importList(file).then(function(data) {
            $state.go('menu.lists.items', { listId: data.listid }).then(function() {
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