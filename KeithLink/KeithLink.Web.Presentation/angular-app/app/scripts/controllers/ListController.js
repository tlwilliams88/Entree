'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', '$modal', 'originalList', 'Constants', 'ListService', 'PricingService', 'ListPagingModel',
    function($scope, $filter, $timeout, $state, $stateParams, $modal, originalList, Constants, ListService, PricingService, ListPagingModel) {

    if ($stateParams.listId !== originalList.listid.toString()) {
      $state.go('menu.lists.items', {listId: originalList.listid, renameList: null}, {location:'replace', inherit:false, notify: false});
    }

    var orderBy = $filter('orderBy');

    var deletedItems = []; // keep track of deleted items

    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;

    // used for the 'Show More' button
    $scope.showMoreListNames = true;
    $scope.numberListNamesToShow = 10;

    if (ListService.findMandatoryList()) {
      $scope.hideMandatoryListCreateButton = true;
    }
    if (ListService.findRecommendedList()) {
      $scope.hideRecommendedListCreateButton = true;
    }

    function resetPage(list) {
      $scope.selectedList = angular.copy(list);
      originalList = list;
      $scope.selectedList.items.unshift({}); // adds empty item that allows ui sortable work with a header row
      $scope.selectedList.isRenaming = false;
      $scope.selectedList.allSelected = false;

      if ($scope.listForm) {
        $scope.listForm.$setPristine();
      }

      $scope.selectedList.items.forEach(function(item) {
        item.editPosition = item.position;
      });
    }
    function appendListItems(list) {
      list.items.forEach(function(item) {
        item.editPosition = item.position;
      });
      $scope.selectedList.items = $scope.selectedList.items.concat(list.items);
    }
    function startLoading() {
      $scope.loadingResults = true;
    }
    function stopLoading() {
      $scope.loadingResults = false;
    }

    $scope.sort = {
      field: 'position',
      sortDescending: false
    };
    var listPagingModel = new ListPagingModel( 
      originalList.listid,
      resetPage,
      appendListItems,
      startLoading,
      stopLoading,
      $scope.sort
    );

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
    PAGING
    **********/

    $scope.filterItems = function(searchTerm) {
      $scope.hideDragToReorder = !!searchTerm.length;
      listPagingModel.filterListItems(searchTerm);
    };
    $scope.sortList = function(sortBy, sortOrder) {
      if (sortBy === $scope.sort.field) {
        sortOrder = !sortOrder;
      } else {
        sortOrder = false;
      }
      $scope.sort = {
        field: sortBy,
        sortDescending: sortOrder
      };
      listPagingModel.sortListItems($scope.sort);
    };
    $scope.infiniteScrollLoadMore = function() {
      listPagingModel.loadMoreData($scope.selectedList.items, $scope.selectedList.itemCount, $scope.loadingResults, deletedItems);
    };

    /**********
    CREATE LIST
    **********/

    var processingCreateList = false;
    $scope.createList = function(items) {
      if (!processingCreateList) {
        processingCreateList = true;
        ListService.createList(items)
          .then(goToNewList)
          .finally(function() {
            processingCreateList = false;
          });
      }
    };

    $scope.createListFromMultiSelect = function() {
      var items = angular.copy(getMultipleSelectedItems());
      $scope.createList(items);
    };

    $scope.createListFromDrag = function(event, helper) {
      var dragSelection = getSelectedItemsFromDrag(helper);
      $scope.createList(dragSelection);
    };

    /**********
    CREATE MANDATORY LIST
    **********/

    $scope.createMandatoryList = function(items) {
      ListService.createMandatoryList(items).then(function(list) {
        $scope.hideMandatoryListCreateButton = true;
        return list;
      }).then(goToNewList);
    };

    $scope.createMandatoryListFromDrag = function(event, helper) {
      var dragSelection = getSelectedItemsFromDrag(helper);
      $scope.createMandatoryList(dragSelection);
    };

    /**********
    CREATE RECOMMENDED LIST
    **********/

    $scope.createRecommendedList = function(items) {
      ListService.createRecommendedList(items).then(function(list) {
        $scope.hideRecommendedListCreateButton = true;
        return list;
      }).then(goToNewList);
    };

    $scope.createRecommendedListFromDrag = function(event, helper) {
      var dragSelection = getSelectedItemsFromDrag(helper);
      $scope.createRecommendedList(dragSelection);
    };

    /**********
    DELETE LIST
    **********/

    $scope.deleteList = function(listId) {
      ListService.deleteList(listId).then(function(list) {
        if (ListService.findMandatoryList() && ListService.findMandatoryList().listid === listId) {
          $scope.hideMandatoryListCreateButton = false;
        }
        return list;
      }).then($scope.goToList);
    };

    /**********
    SAVE LIST
    **********/

    var processingSaveList = false;
    $scope.saveList = function(list) {

      if (!processingSaveList) {
        processingSaveList = true;
        var updatedList = angular.copy(list);

        // remove empty item that is used for ui sortable
        if (updatedList.items.length && !updatedList.items[0].listitemid) {
          updatedList.items.splice(0, 1);
        }

        angular.forEach(updatedList.items, function(item, itemIndex) {
          if (item.listitemid) {
            if (item.editLabel && item.isEditing) {
              item.label = item.editLabel;
            }
            item.position = item.editPosition;
            item.isEditing = false;
          }
        });

        // mark deleted items
        deletedItems.forEach(function(item) {
          item.isdeleted = true;
        });
        updatedList.items = updatedList.items.concat(deletedItems);
        
        // reset paging model 
        listPagingModel.resetPaging();

        return ListService.updateList(updatedList)
          .then(resetPage)
          .finally(function() {
            processingSaveList = false;
          });
      }
    };

    $scope.renameList = function (listId, listName) {
      var list = angular.copy($scope.selectedList);
      list.name = listName;


      $scope.saveList(list).then(function() {
        // update cached list name
        $scope.lists.forEach(function(list) {
          if (list.listid === listId) {
            list.name = listName;
          }
        });
      });
    };

    $scope.cancelRenameList = function() {
      $scope.selectedList.name = originalList.name;
      $scope.selectedList.isRenaming = false;
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

      deletedItems = deletedItems.concat($scope.selectedList.items.splice(deletedIndex, 1));
      updateItemPositions();

      // load more items if number of items fell below page size
      if ($scope.selectedList.items.length < 30) {
        $scope.infiniteScrollLoadMore();
      }

      $scope.listForm.$setDirty();
    };

    $scope.deleteMultipleItems = function() {
      deletedItems = deletedItems.concat($filter('filter')($scope.selectedList.items, {isSelected: 'true'}));

      $scope.selectedList.items = $filter('filter')($scope.selectedList.items, {isSelected: '!true'});
      $scope.selectedList.allSelected = false;

      // load more items if number of items fell below page size
      if ($scope.selectedList.items.length < 30) {
        $scope.infiniteScrollLoadMore();
      }

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
      $scope.multiSelect.showLabels = false;
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
      
      ListService.addMultipleItems(list.listid, items).then(function(updatedList) {
        if ($scope.selectedList.listid === updatedList.listid) {
          $scope.selectedList = list;
        }

        $scope.multiSelect.showLists = false;
        unselectAllDraggedItems();
      });
    };


    /********************
    DRAG HELPERS
    ********************/

    function getMultipleSelectedItems() {
      return $filter('filter')($scope.selectedList.items, {isSelected: 'true'});
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
      angular.forEach($scope.selectedList.items, function(item, index) {
        if (item.itemnumber) {
          item.isSelected = $scope.selectedList.allSelected;
        }
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
      var draggedRow = angular.element(event.target).closest('tbody'),
        multipleSelectedItems = getMultipleSelectedItems();

      var helperElement;
      if (multipleSelectedItems.length > 0 && draggedRow.hasClass('item-selected')) {
        helperElement = angular.element('<div style="padding:10px;background-color:white;border:1px solid #c3bc9c;">' + multipleSelectedItems.length + ' Items</div>');
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
    DRAG TO REORDER
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
      if ($scope.listForm && $scope.selectedList.permissions.canReorderItems) {
        $scope.listForm.$setDirty();
      }
    }

    // reorder list by drag and drop
    $scope.stopReorder = function (e, ui) {
      ui.item.addClass('bek-reordered-item');

      var colorRowTimer = $timeout(function() {
        ui.item.removeClass('bek-reordered-item');
        $timeout.cancel(colorRowTimer);
      }, 500);


      updateItemPositions();
    };

    // // sort list by column
    // $scope.sortList = function(sortBy, sortOrder) {
    //   var sortField = sortBy;

    //   if ($scope.selectedList.items.length > 0 && !$scope.selectedList.items[0].hasOwnProperty('position')) {
    //     $scope.selectedList.items.splice(0,1);
    //   }

    //   $scope.selectedList.items = orderBy($scope.selectedList.items, function(item) {
    //     // move items with position 0 to bottom of list
    //     if ((sortField === 'editPosition' || sortField === 'position') && item[sortField] === 0) {
    //       return 1000;
    //     }

    //     return item[sortField];
    //   }, sortOrder);

    //   $scope.selectedList.items.unshift({});

    //   $scope.sortBy = sortBy;
    //   updateItemPositions();
    // };

    // // FILTER LIST
    // $scope.listSearchTerm = '';
    // $scope.search = function (row) {
    //   var term = $scope.listSearchTerm.toLowerCase(),
    //     itemnumberMatch,
    //     nameMatch,
    //     labelMatch;

    //   if (row.itemnumber) {
    //     itemnumberMatch = row.itemnumber.toLowerCase().indexOf(term || '') !== -1;
    //     nameMatch = row.name && (row.name.toLowerCase().indexOf(term || '') !== -1);
    //     labelMatch = row.label && (row.label.toLowerCase().indexOf(term || '') !== -1);  
    //   }

    //   return !!(itemnumberMatch || nameMatch || labelMatch);
    // };

    // // INFINITE SCROLL
    // var itemsPerPage = Constants.infiniteScrollPageSize;
    // $scope.itemsToDisplay = itemsPerPage;
    // $scope.infiniteScrollLoadMore = function() {
    //   if ($scope.itemsToDisplay < $scope.selectedList.items.length) {
    //     $scope.itemsToDisplay += itemsPerPage;
    //   }
    // };

    /******
    MODALS
    ******/

    $scope.openListImportModal = function () {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/listimportmodal.html',
        controller: 'ImportModalController'
      });
    };

    $scope.openExportModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/exportmodal.html',
        controller: 'ExportModalController',
        resolve: {
          headerText: function () {
            return 'List ' + $scope.selectedList.name;
          },
          exportMethod: function() {
            return ListService.exportList;
          },
          exportConfig: function() {
            return ListService.getExportConfig($scope.selectedList.listid);
          },
          exportParams: function() {
            return $scope.selectedList.listid;
          }
        }
      });
    };

    $scope.openReplicateListModal = function (list) {

      var modalInstance = $modal.open({
        templateUrl: 'views/modals/replicatelistmodal.html',
        controller: 'ReplicateListModalController',
        windowClass: 'no-padding-modal',
        scope: $scope,
        resolve: {
          list: function() {
            return list;
          }
        }
      });

      modalInstance.result.then(function(sharedWith) {
        $scope.selectedList.sharedwith = sharedWith;
        if (sharedWith.length === 0) {
          $scope.selectedList.issharing = false;
        } else if (sharedWith.length > 0) {
          $scope.selectedList.issharing = true;
        }
      });
    };

    $scope.clearFilter = function(){   
      $scope.listSearchTerm = '';
      $scope.hideDragToReorder = false;
      $scope.filterItems( $scope.listSearchTerm );     
    };

    $scope.openPrintOptionsModal = function(list) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/printoptionsmodal.html',
        controller: 'PrintOptionsModalController',
        scope: $scope,
        resolve: {
          list: function() {
            return list;
          },
          pagingModelOptions: function() {
            return { 
              sort: [{
                field: $scope.sort.field,
                order: $scope.sort.sortDescending ? 'desc' : 'asc'
              }],
              terms: $scope.listSearchTerm
            };
          }
        }
      });
    };

    resetPage(angular.copy(originalList));
    // $scope.selectedList.isRenaming = ($stateParams.renameList === 'true' && $scope.selectedList.permissions.canRenameList) ? true : false;

    if (ListService.renameList === true) {
      console.log('rename list');
      ListService.renameList = false;
      if ($scope.selectedList.permissions.canRenameList) {
        $scope.selectedList.isRenaming = true;
      }
    }

  }]);