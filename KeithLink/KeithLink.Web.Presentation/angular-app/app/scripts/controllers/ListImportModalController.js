'use strict';

angular.module('bekApp')
.controller('ListImportModalController', ['$scope', '$modalInstance', '$upload',
  function ($scope, $modalInstance, $upload) {
  
  $scope.upload = [];
  $scope.onFileSelect = function($files) {
    //$files: an array of files selected, each file has name, size, and type.
    for (var i = 0; i < $files.length; i++) {
      var file = $files[i];
      (function(index) {
        $scope.upload[index] = $upload.upload({
        url: '/import/list', //upload.php script, node.js route, or servlet url
        method: 'POST',
        file: file, // or list of files ($files) for html5 only
      }).progress(function(evt) {
        console.log('percent: ' + parseInt(100.0 * evt.loaded / evt.total));
      }).success(function(data, status, headers, config) {
        // file is uploaded successfully
        console.log(data);
      })
      .error(function(data) {
        console.log(data);
      });
      })(i);
    }
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);