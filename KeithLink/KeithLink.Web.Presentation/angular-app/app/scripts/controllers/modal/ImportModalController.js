'use strict';

angular.module('bekApp')
.controller('ImportModalController', ['$scope', '$modalInstance', '$state', 'ListService', 'CartService', 'customListHeaders',
  function ($scope, $modalInstance, $state, ListService, CartService, customListHeaders) {

  $scope.customListHeaders = customListHeaders;
  
  $scope.upload = [];
  $scope.files = [];
  $scope.invalidType = false;      


  function goToImportedPage(routeName, routeParams) {
    $state.go(routeName, routeParams);
    $modalInstance.close();
  }

  $scope.onFileSelect = function($files) {
     if(!$files.length){
      return;
     }
    $scope.files = [];
    var filetype = $files[0].name.slice($files[0].name.length -5,$files[0].name.length);
    filetype = filetype.slice(filetype.indexOf('.'), filetype.length);
   $scope.invalidType = (filetype !== '.xlsx' && filetype !== '.xls' && filetype !== '.csv' && filetype !== '.txt');             

    for (var i = 0; i < $files.length; i++) {     
      $scope.files.push($files[i]);      
    }
  };

  $scope.startListUpload = function(options) {
    var file = $scope.files[0];

    ListService.importList(file, options).then(function(data) {
      goToImportedPage('menu.lists.items', { listId: data.listid });
    });
  };


  $scope.startOrderUpload = function(options) {
    var file = $scope.files[0];

    CartService.importCart(file, options).then(function(data) {
      $state.go('menu.cart.items', { cartId: data.listid });
      $modalInstance.close(data);
    });
  };
  
  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };
  
}]);
