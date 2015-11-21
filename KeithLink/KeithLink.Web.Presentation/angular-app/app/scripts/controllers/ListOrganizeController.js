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

  function setList(list) {
    $scope.list = list;

    $scope.list.items.forEach(function(item) {
      item.positionStarting = item.editPosition = item.position;      
    });
    $scope.list.items.unshift({ position: 0 });

    $scope.sortField = 'position';
    $scope.sortDescending = false;
    $scope.list.items = orderBy($scope.list.items, $scope.sortField, $scope.sortDescending);
  }

  setList(list);

  function updateItemPositions(listItemId, oldPosition, newPosition) {
    var isMovingUp = oldPosition > newPosition;
    $scope.list.items.forEach(function(item, index) {
      if (item.listitemid !== listItemId) {
        if (isMovingUp) {

          // items above the new position move down
          if (item.editPosition < oldPosition && item.editPosition >= newPosition) {
            item.editPosition = item.position = item.editPosition + 1;
          }
         } else { // moving down
          // item below the new position move up
          if (item.editPosition > oldPosition && item.editPosition <= newPosition) {
            item.editPosition = item.position = item.editPosition - 1;
          }
        }
      }
    });
  }

  $scope.stopReorder = function (e, ui) {
    $scope.organizeListForm.$setDirty();
    
    var classList = ui.item.attr('class');
    var itemIdClass = classList.substr(classList.indexOf('item_'), classList.indexOf(' ') - classList.indexOf('item_'));
    var listItemId = parseInt(itemIdClass.substr(5), 10);

    ui.item.addClass('bek-reordered-item');
    var colorRowTimer = $timeout(function() {
      ui.item.removeClass('bek-reordered-item');
      $timeout.cancel(colorRowTimer);
    }, 500);

    var newPosition, oldPosition;
    $scope.list.items.forEach(function(item, index) {
      if (item.listitemid === listItemId) {
        oldPosition = item.editPosition;
        newPosition = index;
        item.editPosition = newPosition;
        item.position = index;
      }
    });

    updateItemPositions(listItemId, oldPosition, newPosition);
  };

  $scope.changePosition = function(items, movedItem) {
    if(movedItem.position === '0'){
      movedItem.position = movedItem.editPosition;
      return;
    }

    movedItem.position = parseInt(movedItem.position, 10);

    var oldPosition = movedItem.editPosition;
    var newPosition = movedItem.position;
    
    movedItem.editPosition = newPosition;

    updateItemPositions(movedItem.listitemid, oldPosition, newPosition);
  };

  $scope.sort = function (field, oldSortDescending) {
    var sortDescending = !oldSortDescending;
    if (oldSortDescending) {
      sortDescending = false;
    }
    $scope.sortField = field;
    $scope.sortDescending = sortDescending;

    $scope.list.items = orderBy($scope.list.items, field, sortDescending);  
    if($scope.list.items.length && !$scope.list.items[($scope.list.items.length -1)].listitemid){
      var dummy = $scope.list.items.slice($scope.list.items.length -1, $scope.list.items.length );
       $scope.list.items = $scope.list.items.slice(0, $scope.list.items.length -1);
       $scope.list.items.splice(0,0,dummy[0]);  
   }

    $scope.list.items.forEach(function(item, index) {
      item.editPosition = item.position = index ;     
    });

    $scope.organizeListForm.$setDirty();
  };

  var processingSaveList = false;
  $scope.saveList = function(list) {
    if (!processingSaveList) {
      processingSaveList = true;

      // // set item positions
      // var orderedItems = orderBy(list.items, [function(item) {
      //   return item.position;
      // }, function(item) {
      //   if (!item.updatedate) {
      //     return 0;
      //   }
      //   var comparison = item.updatedate.getTime();
      //   if (item.positionStarting > item.position) { // moved item up
      //     return comparison * -1;
      //   } else { // moved item down
      //     return comparison;
      //   }
      // }], false);

      // remove empty item that is used for ui sortable 
      if (list.items.length && !list.items[0].listitemid) {
        list.items.splice(0, 1);
      }
 
    //   orderedItems.forEach(function(item, index) {
    //     item.position = index + 1;
    //   });

    //   $scope.list.items = orderedItems;
    // //   list.items = orderedItems;

      ListService.updateList(list, true).then(function(updatedList) {
        $scope.organizeListForm.$setPristine();
        setList(updatedList);
      }).finally(function() {
        processingSaveList = false;
      });
    }
  };

}]);