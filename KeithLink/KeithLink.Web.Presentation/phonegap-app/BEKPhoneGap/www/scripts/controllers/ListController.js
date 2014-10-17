'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', 'Constants', 'ListService', 'UtilityService', 'CartService', 
    function($scope, $filter, $timeout, $state, $stateParams, Constants, ListService, UtilityService, CartService) {
    
    var orderBy = $filter('orderBy');

    $scope.alerts = [];
    
    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;
    $scope.carts = CartService.carts;

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

    $scope.revertChanges = function(selectedList) {
      $scope.selectedList = angular.copy(ListService.findListById(selectedList.listid));
      $scope.selectedList.isRenaming = false;
      $scope.allSelected = false;
      $scope.sortList('position', false);
      $scope.listForm.$setPristine();
    };

    $scope.createList = function() {
      ListService.createList().then(function(data) {
        $scope.displayMessage('success', 'Successfully created a new list.');
        goToNewList(data);
      }, function(error) {
        $scope.displayMessage('error', 'Error creating list.');
      });
    };

    $scope.deleteList = function(listId) {
      ListService.deleteList(listId).then(function(data) {
        $scope.selectedList = angular.copy(ListService.getFavoritesList());
        $scope.displayMessage('success', 'Successfully deleted list.');
      },function(error) {
        $scope.displayMessage('error', 'Error deleting list.');
      });
    };

    $scope.saveList = function(list) {

      var updatedList = angular.copy(list);

      angular.forEach(updatedList.items, function(item, itemIndex) {
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
        $scope.displayMessage('success', 'Successfully saved list ' + list.name + '.');
      }, function(error) {
        $scope.displayMessage('error', 'Error saving list ' + list.name + '.');
      });
    };

    $scope.renameList = function (listId, listName) {
      var list = angular.copy($scope.selectedList);
      list.name = listName;

      ListService.updateList(list).then(function(data) {
        $scope.selectedList.isRenaming = false;
        $scope.selectedList.name = listName;
        $scope.displayMessage('success', 'Successfully renamed list to ' + listName + '.');
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
    Multi-Select events
    ********************/

    $scope.deleteMultipleItems = function() {
      $scope.selectedList.items = $filter('filter')($scope.filteredItems, {isSelected: '!true'});
      $scope.allSelected = false;
      $scope.listForm.$setDirty();
    };

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

    $scope.createListWithItems = function() {
      var items = angular.copy(getMultipleSelectedItems());
      ListService.createList(items).then(function(list) {
        $scope.displayMessage('success', 'Successfully created list with ' + items.length + ' items.');
        goToNewList(list);
      }, function() {
        $scope.displayMessage('error', 'Error creating new list with ' + items.length + ' items.');
      });
    };

    $scope.addItemsToList = function(list) {
      var items = angular.copy(getMultipleSelectedItems());
      UtilityService.deleteFieldFromObjects(items, ['listitemid', 'position', 'label', 'parlevel']);
      list.items = list.items.concat(items);
      ListService.updateList(list).then(function(data) {
        $scope.showLists = false;
        unselectAllDraggedItems();
        $scope.displayMessage('success', 'Successfully added ' + items.length + ' items to list ' + list.name + '.');
      },function(error) {
        $scope.displayMessage('error', 'Error adding ' + items.length + ' items to list ' + list.name + '.');
      });
      // ListService.addMultipleItems(list.listid, items).then(function(data) {
      //   $scope.showLists = false;
      //   unselectAllDraggedItems();
      //   $scope.displayMessage('success', 'Successfully added ' + items.length + ' items to list ' + list.name + '.');
      // },function(error) {
      //   $scope.displayMessage('error', 'Error adding ' + items.length + ' items to list ' + list.name + '.');
      // });
    };

    $scope.favoriteAll = function() {
      var items = getMultipleSelectedItems();

      ListService.addMultipleItemsToFavorites(angular.copy(items)).then(function() {
        angular.forEach(items, function(item) {
          item.favorite = true;
        });
        unselectAllDraggedItems();
        $scope.displayMessage('success', 'Successfully added ' + items.length + ' items to Favorites.');
      }, function(error) {
        $scope.displayMessage('error', 'Error adding ' + items.length + ' items to Favorites.');
      });
    };

    $scope.unfavoriteAll = function() {
      var items = getMultipleSelectedItems();

      ListService.removeMultipleItemsFromFavorites(items).then(function() {
        angular.forEach(items, function(item) {
          item.favorite = false;
        });
        unselectAllDraggedItems();
        $scope.displayMessage('success', 'Successfully removed ' + items.length + ' items from Favorites.');
      }, function(error) {
        $scope.displayMessage('error', 'Error removing ' + items.length + ' items from Favorites.');
      });
    };

    /********************
    DRAG EVENTS
    ********************/

    function getMultipleSelectedItems() {
      return $filter('filter')($scope.filteredItems, {isSelected: 'true'});
    }

    // determines if user is dragging one or multiple items and returns the selected item(s)
    // helper object is passed in from the drag event
    function getSelectedItemsFromDrag(helper) {

      var draggedItems = {},
        selectedItem = helper.draggable,
        multipleItemsSelectedList = getMultipleSelectedItems();

      // multiple items selected
      if (multipleItemsSelectedList.length > 0 && selectedItem.hasClass('item-selected')) {
        draggedItems.isSingle = false;
        draggedItems.items = multipleItemsSelectedList;
      
      // single item selected
      } else {
        draggedItems.isSingle = true;
        draggedItems.items = selectedItem.data('product');
      }

      return draggedItems;
    }

    function unselectAllDraggedItems() {
      angular.forEach($scope.filteredItems, function(item, index) {
        item.isSelected = false;
      });
      $scope.allSelected = false;
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
      
      ListService.createList(dragSelection.items).then(function(data) {
        $scope.displayMessage('success', 'Successfully created list with ' + dragSelection.items.length + ' items.');
        goToNewList(data);
      }, function() {
        $scope.displayMessage('error', 'Error creating new list with ' + dragSelection.items.length + ' items.');
      });
    };

    $scope.addItemToList = function (event, helper, list) {
      var dragSelection = getSelectedItemsFromDrag(helper);

      if (dragSelection.isSingle) {
        var promise;
        if (list.listid === ListService.getFavoritesList().listid) {
          promise = ListService.addItemToFavorites(dragSelection.items);
        } else {
          promise = ListService.addItem(list.listid, dragSelection.items);
        }

        promise.then(function(data) {
          $scope.displayMessage('success', 'Successfully added item ' + dragSelection.items.itemnumber + ' to list ' + list.name + '.');
        },function(error) {
          $scope.displayMessage('error', 'Error adding item ' + dragSelection.items.itemnumber + ' to list ' + list.name + '.');
        });

      } else {
        var newItems = angular.copy(dragSelection.items);
        
        UtilityService.deleteFieldFromObjects(newItems, ['listitemid', 'position', 'label', 'parlevel']);
        list.items = list.items.concat(newItems);
        ListService.updateList(list).then(function(data) {
          $scope.displayMessage('success', 'Successfully added ' + newItems.length + ' items to list ' + list.name + '.');
        },function(error) {
          $scope.displayMessage('error', 'Error adding ' + newItems.length + ' items to list ' + list.name + '.');
        });
        // ListService.addMultipleItems(list.listid, newItems).then(function(data) {
        //   unselectAllDraggedItems();
        //   $scope.displayMessage('success', 'Successfully added ' + dragSelection.items.length + ' items to list ' + list.name + '.');
        // },function(error) {
        //   $scope.displayMessage('error', 'Error adding ' + dragSelection.items.length + ' items to list ' + list.name + '.');
        // });
      }

    };

    $scope.changeAll = function() {
      angular.forEach($scope.filteredItems, function(item, index) {
        item.isSelected = $scope.allSelected;
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

    $scope.generateDragHelper = function(event) {
      var draggedRow = angular.element(event.target).closest('tbody'),
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
        // move items with position 0 to bottom of list
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
        $scope.selectedList = angular.copy(ListService.findListById($state.params.listId));
      } 
      if (!$scope.selectedList) {
        $scope.selectedList = angular.copy(ListService.getFavoritesList());
        $state.go('menu.lists.items', { listId: ListService.getFavoritesList().listid });
      }

      // set placeholders for editable item fields
      angular.forEach(ListService.lists, function(list, listIndex) {
        angular.forEach(list.items, function(item, itemIndex) {
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