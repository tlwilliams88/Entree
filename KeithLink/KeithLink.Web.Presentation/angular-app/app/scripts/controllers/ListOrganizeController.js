'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListOrganizeController
 * @description
 * # ListOrganizeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListOrganizeController', ['$scope', '$filter', '$timeout', 'list', 'ListService', 'UtilityService', 'blockUI',
    function($scope, $filter, $timeout, list, ListService, UtilityService, blockUI) {

  var orderBy = $filter('orderBy');
  $scope.isMobileDevice = UtilityService.isMobileDevice();

  function initItemPositions() {
    $scope.list.items.forEach(function(item, index){
      //index 0 is assigned to the dummy row used for drag-to-reorder functionality
      item.editPosition = index;
      item.position = index;
    });
  }

  function setList(list) {
    $scope.list = list ? list : $scope.list;
    $scope.list.items.forEach(function(item) {
      item.editPosition = item.position;
      item.positionStarting = item.editPosition;
    });
    $scope.list.items.unshift({ position: 0 });

    $scope.sortField = 'position';
    $scope.sortDescending = false;
    $scope.list.items = orderBy($scope.list.items, $scope.sortField, $scope.sortDescending);
    blockUI.stop();
  }

  setList(list);
  initItemPositions();
  function updateItemPositions(listItemId, oldPosition, newPosition) {
    var isMovingUp = oldPosition > newPosition;
    var duplicatePositionItems = $filter('filter')($scope.list.items, function(item){
      return item.position === newPosition;
    });

    if(duplicatePositionItems.length > 1){
          $scope.list.items.forEach(function(item, index) {
      if (item.listitemid !== listItemId) {
        if (isMovingUp) {

          // items above the new position move down
          if (item.editPosition < oldPosition && item.editPosition >= newPosition) {
            item.position = item.editPosition + 1;
            item.editPosition = item.position;
          }
         } else { // moving down
          // item below the new position move up
          if (item.editPosition > oldPosition && item.editPosition <= newPosition) {
            item.position = item.editPosition - 1;
            item.editPosition = item.position;
          }
        }
      }
    });
    }

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

  $scope.deleteItem = function(deletedItem){
    deletedItem.isdeleted = true;
    $scope.list.items.forEach(function(item){
      if(item.position && item.position > deletedItem.position){
        item.position--;
        item.editPosition = item.position;
      }
    });
  };

  $scope.changePosition = function(items, movedItem, removeNullValue) {
    if(movedItem.position === '0' || !movedItem.position){
      if(removeNullValue || movedItem.position === '0'){
        movedItem.position = movedItem.editPosition;
      }
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
      item.position = index;
      item.editPosition = item.position;
    });

    $scope.organizeListForm.$setDirty();
  };

  var processingSaveList = false;
  $scope.saveList = function(list) {
    if (!processingSaveList) {
      processingSaveList = true;

      // remove empty item that is used for ui sortable
      if (list.items.length && !list.items[0].listitemid) {
        list.items.splice(0, 1);
      }

      blockUI.start('Saving List...').then(function(){
        ListService.updateList(list, true).then(function(updatedList) {
          $scope.organizeListForm.$setPristine();
          setList();
        }).finally(function() {
          processingSaveList = false;
        });
      })
    }
  };
  
  $scope.findElementByIndex = function(query) {
      $('tbody > tr').removeClass('foundItem');
      
      if(query.length > 0) {
          var foundItemsByName = $filter('filter')($scope.list.items, {name: query}),
              foundItemsById = $filter('filter')($scope.list.items, {itemnumber: query}),
              foundItemsByBrand = $filter('filter')($scope.list.items, {brand: query}),
              foundItemsByClass = $filter('filter')($scope.list.items, {class: query});
          
          $scope.foundItems = [];
          
          foundItemsByName.forEach(function(item) {
              $scope.foundItems.push(item);
          })
          
          foundItemsById.forEach(function(item) {
              $scope.foundItems.push(item);
          })
          
          foundItemsByBrand.forEach(function(item) {
              $scope.foundItems.push(item)
          })
          
          foundItemsByClass.forEach(function(item) {
              $scope.foundItems.push(item);
          })
          
          $scope.foundItems.sort(function(a, b) {
              return a.position - b.position;
          })
          
          $scope.foundItemIdx = $scope.list.items.indexOf($scope.foundItems[0]);
          $scope.initialIndex = 0;
          $('tbody > tr').get(($scope.foundItemIdx-1)).classList += ' foundItem';
          
          var w = $(window);
          
          $('html,body').animate({scrollTop: $('.foundItem').offset().top - (w.height()/7)}, 50 );
      }
  }
        
  $scope.goToNextFoundElement = function() {
    
    if($scope.initialIndex == ($scope.foundItems.length - 1)) {
        return false;
    }
    
    if($scope.initialIndex < ($scope.foundItems.length - 1)) {
        $('tbody > tr').removeClass('foundItem');
        $scope.initialIndex = $scope.initialIndex+1;
        $scope.foundItemIdx = $scope.list.items.indexOf($scope.foundItems[$scope.initialIndex]);
        $('tbody > tr').get(($scope.foundItemIdx-1)).classList += ' foundItem';
        
        var w = $(window);
        $('html,body').animate({scrollTop: $('.foundItem').offset().top - (w.height()/7)}, 50 );
    }
  }

  $scope.goToPreviousFoundElement = function() {
    
    if($scope.initialIndex == 0) {
        return false;
    } else {
        $('tbody > tr').removeClass('foundItem');
        
        $scope.initialIndex = $scope.initialIndex-1;
        $scope.foundItemIdx = $scope.list.items.indexOf($scope.foundItems[$scope.initialIndex]);
        $('tbody > tr').get(($scope.foundItemIdx-1)).classList += ' foundItem';
        
        var w = $(window);
        $('html,body').animate({scrollTop: $('.foundItem').offset().top - (w.height()/7)}, 50 );
    }
  }

  $timeout(function() {
    $('#findInput').on("input", function() { 
        $scope.findElementByIndex(this.value);
    })
  }, 100);

}]);
