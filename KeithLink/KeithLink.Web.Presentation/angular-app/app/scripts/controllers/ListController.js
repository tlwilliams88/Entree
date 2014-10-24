'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', 'originalList', 'Constants', 'ListService', 'UtilityService', 
    function($scope, $filter, $timeout, $state, $stateParams, originalList, Constants, ListService, UtilityService) {
    
    var orderBy = $filter('orderBy');

    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;

    function resetPage(list) {
      $scope.selectedList = list;
      $scope.selectedList.items.unshift({}); // allows ui sortable work with a header row
      $scope.selectedList.isRenaming = false;
      $scope.selectedList.allSelected = false;
      $scope.sortList('position', false);
      if ($scope.listForm) {
        $scope.listForm.$setPristine();
      }
    }

    // LIST INTERACTIONS
    $scope.goToList = function(list) {
      return $state.go('menu.lists.items', {listId: list.listid, renameList: false});
    };
    
    function goToNewList(newList) {
      // user loses changes if they go to a new list
      $scope.listForm.$setPristine();
      $state.go('menu.lists.items', {listId: newList.listid, renameList: true});
    }

    $scope.undoChanges = function() {
      resetPage(angular.copy(originalList));
    };

    /**********
    CREATE LIST
    **********/

    $scope.createList = function() {
      ListService.createList().then(goToNewList);
    };

    $scope.createListWithItems = function() {
      var items = angular.copy(getMultipleSelectedItems());
      $scope.createList(items);
    };

    $scope.createListWithItem = function(event, helper) {
      var dragSelection = getSelectedItemsFromDrag(helper);
      $scope.createList(dragSelection);
    };

    /**********
    DELETE LIST
    **********/

    $scope.deleteList = function(listId) {
      ListService.deleteList(listId).then($scope.goToList);
    };

    /**********
    SAVE LIST
    **********/

    $scope.saveList = function(list) {

      var updatedList = angular.copy(list);

      angular.forEach(updatedList.items, function(item, itemIndex) {
        if (item.listitemid) {
          item.position = item.editPosition;
          item.isEditing = false;
        }
      });
      
      ListService.updateList(updatedList).then(resetPage);
    };

    $scope.renameList = function (listId, listName) {
      var list = angular.copy($scope.selectedList);
      list.name = listName;

      $scope.saveList(list);
    };

    $scope.startEditListName = function(listName) {
      $scope.editList = {};
      $scope.editList.name = angular.copy(listName);
      $scope.selectedList.isRenaming = true;
    };

    /**********
    DELETE ITEMS
    **********/

    $scope.deleteItem = function(item) {

      var deletedIndex = $scope.selectedList.items.indexOf(item);

      if (deletedIndex < 0) {
        angular.forEach($scope.selectedList.items, function(listItem, index) {
          if (listItem.listitemid === item.listitemid) {
            deletedIndex = index;
          }
        });
      }

      $scope.selectedList.items.splice(deletedIndex, 1);
      updateItemPositions();
    };

    $scope.deleteMultipleItems = function() {
      $scope.selectedList.items = $filter('filter')($scope.filteredItems, {isSelected: '!true'});
      $scope.selectedList.allSelected = false;
      $scope.listForm.$setDirty();
    };

    /**********
    LABELS
    **********/

    $scope.addLabels = function(label) {
      var items = getMultipleSelectedItems();
      angular.forEach(items, function(item, index) {
        item.label = label;
      });
      $scope.listForm.$setDirty();
      $scope.showLabels = false;
    };

    $scope.applyNewLabel = function(label) {
      var items = getMultipleSelectedItems();
      angular.forEach(items, function(item, index) {
        item.isEditing = true;
        item.label = label;
      });
      $scope.addingNewLabel = false;
      $scope.newLabel = null;
      unselectAllDraggedItems();
      $scope.listForm.$setDirty();
    };

    /**********
    ADD ITEMS
    **********/

    $scope.addItemsToList = function(list, items) {
      if (!items) {
        items = angular.copy(getMultipleSelectedItems());
      }

      UtilityService.deleteFieldFromObjects(items, ['listitemid', 'position', 'label', 'parlevel']);
      
      ListService.addMultipleItems(list.listid, items).then(function(list) {
        $scope.selectedList = list;
        $scope.showLists = false;
      });
    };

    $scope.favoriteAll = function() {
      $scope.addItemsToList(ListService.getFavoritesList());
    };

    $scope.unfavoriteAll = function() {
      var items = getMultipleSelectedItems(),
        favoritesList = ListService.getFavoritesList();

      ListService.deleteMultipleItems(favoritesList.listid, items);
    };

    /********************
    DRAG HELPERS
    ********************/

    function getMultipleSelectedItems() {
      return $filter('filter')($scope.filteredItems, {isSelected: 'true'});
    }

    // determines if user is dragging one or multiple items and returns the selected item(s)
    // helper object is passed in from the drag event
    function getSelectedItemsFromDrag(helper) {

      var dragItemSelection = [],
        selectedItem = helper.draggable,
        multipleItemsSelectedList = getMultipleSelectedItems();

      // multiple items selected
      if (multipleItemsSelectedList.length > 0 && selectedItem.hasClass('item-selected')) {
        dragItemSelection = multipleItemsSelectedList;
      
      // single item selected
      } else {
        dragItemSelection = selectedItem.data('product');
        dragItemSelection = [dragItemSelection];
      }

      return dragItemSelection;
    }

    $scope.changeAllSelectedItems = function() {
      angular.forEach($scope.filteredItems, function(item, index) {
        item.isSelected = $scope.selectedList.allSelected;
      });
    };

    function unselectAllDraggedItems() {
      $scope.selectedList.allSelected = false;
      $scope.changeAllSelectedItems();
    }

    // disable drag on mobile
    $scope.isDragEnabled = function() {
      return window.innerWidth > 991;
    };

    // Check if element is being dragged, used to enable DOM elements
    $scope.setIsDragging = function(event, helper, isDragging) {
      $scope.isDragging = isDragging;
    };

    $scope.generateDragHelper = function(event) {
      var draggedRow = angular.element(event.target).closest('tr'),
        multipleSelectedItems = getMultipleSelectedItems();

      var helperElement;
      if (multipleSelectedItems.length > 0 && draggedRow.hasClass('item-selected')) {
        helperElement = angular.element('<div style="padding:10px;">' + multipleSelectedItems.length + ' Items</div>');
      } else {
        helperElement = angular.element(draggedRow).clone();
      }
      return helperElement;
    };

    /********************
    DRAG EVENTS
    ********************/

    $scope.deleteItemFromDrag = function(event, helper) {
      var dragSelection = getSelectedItemsFromDrag(helper);

      angular.forEach(dragSelection, function(item, index) {
        $scope.deleteItem(item);
      });
    };

    $scope.addItemFromDrag = function (event, helper, list) {
      var dragSelection = getSelectedItemsFromDrag(helper);
      $scope.addItemsToList(list, dragSelection);
    };

    /********************
    ORDERING/SORTING LIST
    ********************/

    // saves new item indexes in cached editPosition field after sorting or ordering the list items
    function updateItemPositions() {
      var newPostion = 1;
      angular.forEach($scope.selectedList.items, function(item, index) {
        if (item.listitemid) {
          item.editPosition = newPostion;
          newPostion += 1;
        }
      });
      if ($scope.listForm && !$scope.selectedList.read_only) {
        $scope.listForm.$setDirty();
      }
    }

    // reorder list by drag and drop
    $scope.stopReorder = function (e, ui) {
      ui.item.addClass('bek-reordered-item');

      $timeout(function() {
        ui.item.removeClass('bek-reordered-item');
      }, 500);

      updateItemPositions();
    };

    // sort list by column
    $scope.sortList = function(sortBy, sortOrder) {
      var sortField = sortBy;
      $scope.selectedList.items = orderBy($scope.selectedList.items, function(item) {
        // move items with position 0 to bottom of list
        if ((sortField === 'editPosition' || sortField === 'position') && item[sortField] === 0) {
          return 1000;
        }
        return item[sortField];
      }, sortOrder);

      $scope.sortBy = sortBy;
      updateItemPositions();
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

    // FILTER LIST
    $scope.listSearchTerm = '';
    $scope.search = function (row) {
      var term = $scope.listSearchTerm.toLowerCase(),
        itemnumberMatch,
        nameMatch,
        labelMatch;

      if (row.itemnumber) {
        itemnumberMatch = row.itemnumber.toLowerCase().indexOf(term || '') !== -1;
        nameMatch = row.name.toLowerCase().indexOf(term || '') !== -1;
        labelMatch = row.label && (row.label.toLowerCase().indexOf(term || '') !== -1);  
      }

      return !!(itemnumberMatch || nameMatch || labelMatch);
    };

    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      if ($scope.itemsToDisplay < $scope.selectedList.items.length) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };

    resetPage(angular.copy(originalList));
    $scope.selectedList.isRenaming = $stateParams.renameList === 'true' ? true : false;

  }]);