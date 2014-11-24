'use strict';

angular.module('bekApp')
.controller('ItemNotesModalController', ['$scope', '$modalInstance', '$state', '$q', 'item', 'ProductService',
  function ($scope, $modalInstance, $state, $q, item, ProductService) {

  $scope.item = item;

  $scope.cancel = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.saveNote = function(item) {
    ProductService.updateItemNote(item.itemnumber, item.notes).then(function() {
      $modalInstance.close(item);
      $scope.displayMessage('success', 'Successfully updated note.');
    }, function() {
      $scope.displayMessage('error', 'Error updating note.');
    });
  };

  $scope.deleteNote = function(item) {
    ProductService.deleteItemNote(item.itemnumber).then(function() {
      delete item.notes;
      $modalInstance.close(item);
      $scope.displayMessage('success', 'Successfully deleted note.');
    }, function() {
      $scope.displayMessage('error', 'Error deleting note.');
    });
  };
  
}]);