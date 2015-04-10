'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListOrganizeController
 * @description
 * # ListOrganizeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListOrganizeController', ['$scope', '$filter', '$timeout', 'list', 'ListService',
    function($scope, $filter, $timeout, list, ListService) {

  var orderBy = $filter('orderBy');
  $scope.list = list;

  $scope.sortField = 'position';
  $scope.sortDescending = false;

  $scope.list.items = orderBy($scope.list.items, $scope.sortField, $scope.sortDescending);

  function updateItemPostions(items) {
    items.forEach(function(item, index) {
      item.position = index + 1;
    });
  }

  function afterReorder(anchor) {
    updateItemPostions($scope.list.items);
    // $location.hash(anchor);
  }

  $scope.stopReorder = function (e, ui) {
    $scope.organizeListForm.$setDirty();
    
    ui.item.addClass('bek-reordered-item');
    var colorRowTimer = $timeout(function() {
      ui.item.removeClass('bek-reordered-item');
      $timeout.cancel(colorRowTimer);
    }, 500);

    var anchor = ui.item.attr('id');
    afterReorder(anchor);
  };

  function move(arr, from, to) {
    var array = angular.copy(arr);
    array.splice(to, 0, array.splice(from, 1)[0]);
    return array;
  }

  $scope.reorderItem = function(items, item) {
    if (!item.position) { return; }
    
    var oldIndex = items.indexOf(item);
    $scope.list.items = move(items, oldIndex, item.position - 1);

    // set focus onto correct text box
    // set class to highlight row

    var anchor = 'item_' + item.position;
    afterReorder(anchor);
  };

  $scope.sort = function (field, oldSortDescending) {
    var sortDescending = !oldSortDescending;
    if (oldSortDescending) {
      sortDescending = false;
    }
    $scope.sortField = field;
    $scope.sortDescending = sortDescending;

    $scope.list.items = orderBy($scope.list.items, field, sortDescending);
    updateItemPostions($scope.list.items);
    $scope.organizeListForm.$setDirty();
  };

  var processingSaveList = false;
  $scope.saveList = function(list) {
    if (!processingSaveList) {
      processingSaveList = true;
      ListService.updateList(list).then(function(updatedList) {
        $scope.organizeListForm.$setPristine();
        $scope.list = updatedList;
      }).finally(function() {
        processingSaveList = false;
      });
    }
  };

}]);