'use strict';

angular.module('bekApp')
.controller('ImportModalController', ['$scope', '$analytics', '$modalInstance', '$state', 'ListService', 'CartService', 'customListHeaders', 'listType', 'ExportService', 'LocalStorage',
  function ($scope, $analytics, $modalInstance, $state, ListService, CartService, customListHeaders, listType, ExportService, LocalStorage) {

  $scope.customListHeaders = customListHeaders;

  $scope.nonBEKList = listType == 'CustomInventory' ? true : false;

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
    $analytics.eventTrack('Import List', {category: 'Lists'});
    if($scope.nonBEKList){
      var options = {
        filetype: 'csv'
      };

      ListService.importNonBEKListItems(file, options).then(function(data) {
        goToImportedPage('menu.lists.items', { listId: 'nonBEKList' });
      });
    } else {
      ListService.importList(file, options).then(function(data) {
        var Id = data.list.listid,
            Type = data.list.type,
            list = {
                listId: Id,
                listType: Type
            };

        LocalStorage.setLastList(list);
        
        goToImportedPage('menu.lists.items', { listId: Id, listType: Type });
      });
    }

  };

  $scope.downloadNonBEKTemplate = function() {
    $analytics.eventTrack('Import List', {category: 'Lists'});
    var body = {
      name: 'importcustominventory',
      format: 'csv'
    };

    ExportService.downloadNonBEKTemplate('/template', body);
  };


  $scope.startOrderUpload = function(options) {
    var file = $scope.files[0];
    $analytics.eventTrack('Import Order', {category: 'Orders'});
    CartService.importCart(file, options).then(function(data) {
      $state.go('menu.cart.items', { cartId: data.listid });
      $modalInstance.close(data);
    });
  };

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

}]);
