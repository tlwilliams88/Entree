'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', 'toaster', 'Constants', 'ListService', 
    function($scope, $filter, $timeout, $state, $stateParams, toaster, Constants, ListService) {
    
    var orderBy = $filter('orderBy');

    $scope.alerts = [];
    
    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;

    function goToNewList(newList) {
      $scope.listForm.$setPristine();
      $state.go('menu.lists.items', {listId: newList.listid, renameList: true});
    }

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      if ($scope.itemsToDisplay < $scope.selectedList.items.length) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    // LIST INTERACTIONS
    $scope.goToList = function(list) {
      return $state.go('menu.lists.items', {listId: list.listid, renameList: false});
    };

    $scope.createList = function() {
      ListService.createList().then(function(data) {
        addSuccessAlert('Successfully created a new list.');
        goToNewList(data);
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
        $scope.listForm.$setPristine();
      });

      ListService.updateList(updatedList).then(function(data) {
        // hide renameListForm form
        $scope.selectedList.isRenaming = false;

        // reset column sort 
        $scope.sortBy = 'editPosition';
        $scope.sortOrder = false;

        $scope.selectedList = updatedList;
        $scope.listForm.$setPristine();
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
    };

    /********************
    DRAG EVENTS
    ********************/

    // determines if user is dragging one or multiple items and returns the selected item(s)
    // helper object is passed in from the drag event
    function getSelectedItemsFromDrag(helper) {

      var draggedItems = {};

      var multipleItemsSelectedList = [];
      angular.forEach($scope.selectedList.items, function(item, index) {
        if (item.isSelected) {
          multipleItemsSelectedList.push(item);
        }
      });

      // multiple items selected
      if (multipleItemsSelectedList.length > 0) {
        draggedItems.isSingle = false;
        draggedItems.items = multipleItemsSelectedList;
      
      // single item selected
      } else {
        draggedItems.isSingle = true;
        draggedItems.items = helper.draggable.data('product');
      }

      return draggedItems;
    }

    $scope.deleteItemFromDrag = function(event, helper) {
      var dragSelection = getSelectedItemsFromDrag(helper);

      if (dragSelection.isSingle) {
        $scope.deleteItem(dragSelection.items);
      } else {
        angular.forEach(dragSelection.items, function(item, index) {
          $scope.deleteItem(item);
        });
      }
    };

    $scope.createListWithItem = function(event, helper) {
      
      var dragSelection = getSelectedItemsFromDrag(helper);

      if (dragSelection.isSingle) {
        ListService.createListWithItem(dragSelection.items).then(function(data) {
          addSuccessAlert('Successfully created a new list with item ' + dragSelection.items.itemnumber + '.');
          goToNewList(data[0]);
        }, function(error) {
          addErrorAlert('Error creating list.');
        });
      
      } else {
        ListService.createList(dragSelection.items).then(function(data) {
          addSuccessAlert('Successfully created list with ' + dragSelection.items.length + ' items.');
          goToNewList(data);
        }, function() {
          addErrorAlert('Error creating list with ' + dragSelection.items.length + ' items.');
        });
      }
    };

    $scope.addItemToList = function (event, helper, list) {
      var selectedItem = helper.draggable.data('product');

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

    // disable drag on mobile
    $scope.isDragEnabled = function() {
      return window.innerWidth > 991;
    };

    // Check if element is being dragged, used to enable DOM elements
    $scope.setIsDragging = function(event, helper, isDragging) {
      $scope.isDragging = isDragging;
    };

    /********************
    ORDERING/SORTING LIST
    ********************/

    // saves new item indexes in cached editPosition field after sorting or ordering the list items
    function updateItemPositions() {
      angular.forEach($scope.selectedList.items, function(item, index) {
        item.editPosition = index + 1;
      });
      if ($scope.listForm) {
        $scope.listForm.$setDirty();
      }
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
      }, 500);

      updateItemPositions();
    };
    
    $scope.chromeFix = function(event, ui) {  // fix for chrome position:relative issue
      // ui.helper.css({'top' : ui.position.top + angular.element(window).scrollTop() + 'px'});
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
      addAlert('error', message);
    }
    function addAlert(alertType, message) {
      toaster.pop(alertType, null, message);
    }

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
        // delete $stateParams.renameList;
        if (!$scope.selectedList.isFavoritesList) {
          $scope.startEditListName($scope.selectedList.name);
        }
        $state.go('menu.lists.items', {listId: $scope.selectedList.listid});
      }

      $scope.itemsToDisplay = itemsPerPage;
      $scope.loadingResults = false;
    }
    initPage();

  }]);