'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListOrganizeController
 * @description
 * # ListOrganizeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListOrganizeController', ['$scope', '$location', '$timeout', 'list',
    function($scope, $location, $timeout, list) {

  $scope.list = list;

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

    var anchor = 'item_' + item.position;
    afterReorder(anchor);
  };


  }]);