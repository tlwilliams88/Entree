'use strict';

angular.module('bekApp')
.controller('ListImportModalController', ['$scope', '$modalInstance', '$state', 'ListService',
  function ($scope, $modalInstance, $state, ListService) {
  
  $scope.upload = [];
  var files = [];

  $scope.onFileSelect = function($files) {
    files = [];
    for (var i = 0; i < $files.length; i++) {
      files.push($files[i]);
      // var file = $files[i];
      // /* jshint ignore:start */
      // (function(index) {
      //     $scope.upload[index] = ListService.importList(file).then(function(data) {
      //       $state.go('menu.lists.items', { listId: data.listid }).then(function() {
      //         $modalInstance.close(data);
      //       });
      //     });
      // })(i);
      // /* jshint ignore:end */
    }
  };

  $scope.startUpload = function() {
    var file = files[0];
    ListService.importList(file).then(function(data) {
      $state.go('menu.lists.items', { listId: data.listid }).then(function() {
        $modalInstance.close(data);
      });
    });
  };
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);