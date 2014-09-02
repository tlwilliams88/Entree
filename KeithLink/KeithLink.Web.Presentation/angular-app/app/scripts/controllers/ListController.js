'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', 'ListService', function($scope, $filter, $timeout, $state, $stateParams, ListService) {
    var orderBy = $filter('orderBy');

    $scope.alerts = [];
    
    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;

    function goToListUrl(listId) {
      return $state.go('menu.lists.items', {listId: listId});
    }

    // INFINITE SCROLL
    var itemsPerPage = 30;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      $scope.itemsToDisplay += itemsPerPage;
    };

    // LIST INTERACTIONS
    $scope.goToList = function(list) {
      goToListUrl(list.listid);
    };

    $scope.createList = function() {
      ListService.createList().then(function(data) {
        goToListUrl(data.listid);
        $scope.selectedList = data;
        $scope.startEditListName($scope.selectedList.name);
        addSuccessAlert('Successfully created a new list.');
      }, function(error) {
        addErrorAlert('Error creating list.');
      });
    };

    $scope.createListWithItem = function(event, helper) {
      var selectedItem = helper.draggable.data('product');
      
      ListService.createListWithItem(selectedItem).then(function(data) {
        var newList = data[0];
        goToListUrl(newList.listid);
        $scope.selectedList = newList;
        $scope.startEditListName($scope.selectedList.name);
        addSuccessAlert('Successfully created a new list with item ' + selectedItem.itemnumber + '.');
      }, function(error) {
        addErrorAlert('Error creating list.');
      });
    };

    $scope.deleteList = function(listId) {
      ListService.deleteList(listId).then(function(data) {
        $scope.selectedList = ListService.favoritesList;
        addSuccessAlert('Successfully deleted list.');
      },function(error) {
        addErrorAlert('Error deleting list.');
      });
    };

    $scope.saveList = function(list) {

      var updatedList = angular.copy(list);

      angular.forEach(updatedList.items, function(item, itemIndex) {
        item.label = item.editLabel;
        item.parlevel = item.editParlevel;
        item.position = item.editPosition;
        item.isEditing = false;
      });

      ListService.updateList(updatedList).then(function(data) {
        // hide renameListForm form
        $scope.selectedList.isRenaming = false;

        // reset column sort 
        $scope.sortBy = 'editPosition';
        $scope.sortOrder = false;

        $scope.unsavedChanges = false;

        $scope.selectedList = updatedList;
        addSuccessAlert('Successfully saved list ' + list.name + '.');
      }, function(error) {
        addErrorAlert('Error saving list ' + list.name + '.');
      });
    };

    $scope.renameList = function (listId, listName) {
      var list = angular.copy($scope.selectedList);
      list.name = listName;

      ListService.updateList(list).then(function(data) {
        $scope.selectedList.isRenaming = false;
        $scope.selectedList.name = listName;
        addSuccessAlert('Successfully renamed list to ' + listName + '.');
      });
    };

    $scope.startEditListName = function(listName) {
      $scope.editList = {};
      $scope.editList.name = angular.copy(listName);
      $scope.selectedList.isRenaming = true;
    };

    $scope.addItemToList = function (event, helper, list) {
      var selectedItem = angular.copy(helper.draggable.data('product'));

      var promise;
      if (list.listid === ListService.favoritesList.listid) {
        promise = ListService.addItemToFavorites(selectedItem);
      } else {
        promise = ListService.addItemToListAndFavorites(list.listid, selectedItem);
      }

      promise.then(function(data) {
        addSuccessAlert('Successfully added item ' + selectedItem.itemnumber + ' to list ' + list.name + '.');
      },function(error) {
        addErrorAlert('Error adding item ' + selectedItem.itemnumber + ' to list ' + list.name + '.');
      });
    };

    $scope.deleteItem = function(item) {

      var deletedIndex = $scope.selectedList.items.indexOf(item);

      if (deletedIndex < 0) {
        var deletedItem = angular.copy(item);
        angular.forEach($scope.selectedList.items, function(listItem, index) {
          if (listItem.listitemid === deletedItem.listitemid) {
            deletedIndex = index;
          }
        });
      }

      $scope.selectedList.items.splice(deletedIndex, 1);
      updateItemPositions();
      $scope.unsavedChanges = true;
    };

    $scope.deleteItemFromDrag = function(event, helper) {
      var selectedItem = angular.copy(helper.draggable.data('product'));
      $scope.deleteItem(selectedItem);
    };

    // warn user when exiting page without saved changes
    // window.onbeforeunload = function(){
    //   if (unsavedChanges) {
    //     debugger;
    //   }
    // };

    // ORDERING/SORTING LIST

    // saves new item indexes in cached editPosition field after sorting or ordering the list items
    function updateItemPositions() {
      angular.forEach($scope.selectedList.items, function(item, index) {
        item.editPosition = index + 1;
      });
    }

    // sort list by column
    $scope.sortList = function(sortBy, sortOrder) {
      var sortField = sortBy;
      $scope.selectedList.items = orderBy($scope.selectedList.items, function(item) {
        if ((sortField === 'editPosition' || sortField === 'position') && item[sortField] === 0) {
          return 1000;
        }
        return item[sortField];
      }, sortOrder);

      $scope.sortBy = sortBy;
      updateItemPositions();
    };

    // reorder list by drag and drop
    $scope.stopReorder = function (e, ui) {
      ui.item.addClass('bek-reordered-item');

      $timeout(function() {
        ui.item.removeClass('bek-reordered-item');
        console.log('remove class');
      }, 500);

      updateItemPositions();

    };
    
    $scope.work = function(event, ui) {  // fix for chrome position:relative issue
        // ui.helper.css({'top' : ui.position.top + angular.element(window).scrollTop() + 'px'});
    };

    // Dragging, used to enable DOM elements
    $scope.setIsDragging = function(event, helper, isDragging) {
      $scope.isDragging = isDragging;
    };

    // CONTEXT MENU
    $scope.showContextMenu = function(e) {
      var openMenuLink = angular.element(e.target.parentElement),
        contextMenu = angular.element('.context-menu');
      openMenuLink.append(contextMenu);
      $scope.isContextMenuDisplayed = true;
    };

    $scope.hideContextMenu = function() {
      $scope.isContextMenuDisplayed = false;
    };

    // SHOW MORE
    // limit number of list names displayed in the sidebar
    var showMoreLimit = 5;
    $scope.itemsLimit = function() {
      return showMoreLimit;
    };
    $scope.showMore = function() {
      showMoreLimit = $scope.lists.length;
    };
    $scope.hasMoreItemsToShow = function() {
      if ($scope.lists) {
        return showMoreLimit < $scope.lists.length;
      }
    };

    // ALERTS
    function addSuccessAlert(message) {
      addAlert('success', message);
    }
    function addErrorAlert(message) {
      addAlert('danger', message);
    }
    function addAlert(alertType, message) {
      $scope.alerts[0] = { type: alertType, msg: message };
    }
    $scope.closeAlert = function(index) {
      $scope.alerts.splice(index, 1);
    };

    // FILTER LIST
    $scope.listSearchTerm = '';
    $scope.search = function (row) {
      var term = $scope.listSearchTerm.toLowerCase();

      var itemnumberMatch = row.itemnumber.toLowerCase().indexOf(term || '') !== -1,
        nameMatch = row.name===null || row.name.toLowerCase().indexOf(term || '') !== -1,
        labelMatch =  row.label && (row.label.toLowerCase().indexOf(term || '') !== -1);

      return !!(itemnumberMatch || nameMatch || labelMatch);
    };

    function initPage() {  
      // switch to specific list or default to Favorites list
      if ($state.params.listId) {
        $scope.selectedList = ListService.findListById($state.params.listId, $scope.lists);
      } 
      if (!$scope.selectedList) {
        $scope.selectedList = ListService.favoritesList;
        $state.go('menu.lists.items', { listId: ListService.favoritesList.listid });
      }

      // set placeholders for editable item fields
      angular.forEach(ListService.lists, function(list, listIndex) {
        angular.forEach(list.items, function(item, itemIndex) {
          item.editLabel = item.label === '' ? null : item.label;
          item.editParlevel = item.parlevel;
          item.editPosition = item.position;
        });
      });

      // set initial sort fields
      $scope.sortList('position', false);

      // set focus to rename list if it is a new list
      if ($stateParams.renameList === 'true') {
        if (!$scope.selectedList.isFavoritesList) {
          $scope.startEditListName($scope.selectedList.name);
        }
        goToListUrl($scope.selectedList.listid);
      }

      $scope.itemsToDisplay = itemsPerPage;
      $scope.loadingResults = false;
    }
    initPage();

  }]);