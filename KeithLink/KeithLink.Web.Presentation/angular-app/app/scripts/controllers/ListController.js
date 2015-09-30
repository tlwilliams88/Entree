'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', '$modal', 'blockUI', 'originalList', 'Constants', 'ListService', 'PricingService', 'ListPagingModel', 'LocalStorage', 'UtilityService',
    function($scope, $filter, $timeout, $state, $stateParams, $modal, blockUI, originalList, Constants, ListService, PricingService, ListPagingModel, LocalStorage, UtilityService) {
    if ($stateParams.listId !== originalList.listid.toString()) {
      $state.go('menu.lists.items', {listId: originalList.listid, renameList: null}, {location:'replace', inherit:false, notify: false});
    }

    var orderBy = $filter('orderBy');


    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;  

    // used for the 'Show More' button
    $scope.showMoreListNames = true;
    $scope.numberListNamesToShow = 10;
    $scope.indexOfSDestroyedRow = '';
    $scope.isMobileDevice = UtilityService.isMobileDevice();
    $scope.visitedPages = [];

    if (ListService.findMandatoryList()) {
      $scope.hideMandatoryListCreateButton = true;
    }
    if (ListService.findRecommendedList()) {
      $scope.hideRecommendedListCreateButton = true;
    }


    $scope.rowChanged = function(index){
      $scope.indexOfSDestroyedRow = index + 1;
    }

    $scope.blockUIAndChangePage = function(page){
      $scope.startingPoint = 0;
       $scope.endPoint = 0;
        var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});
        blockUI.start("Loading List...").then(function(){
          if(visited.length > 0){
            $timeout(function() {
              $scope.pageChanged(page, visited);
            }, 100);
          }
          else{
            $scope.pageChanged(page, visited);
          }
        })     
    }

     $scope.pageChanged = function(page) {       
      $scope.currentPage = page.currentPage;
      $scope.startingPoint = ((page.currentPage - 1)*parseInt($scope.pagingPageSize)) + 1;
      $scope.endPoint = angular.copy($scope.startingPoint + parseInt($scope.pagingPageSize));
      $scope.setRange();
      $scope.selectedList.allSelected = false;
      var deletedItems = [];
      $scope.selectedList.items.forEach(function(item){
        if(item.deleted){
          deletedItems.push(item);
        }

      })

      var visited = $filter('filter')($scope.visitedPages, {page: $scope.currentPage});
      if(!visited.length){             
        listPagingModel.loadMoreData($scope.startingPoint - 1, $scope.endPoint - 1, $scope.loadingResults, deletedItems);       
      }
      else{
          $scope.setStartAndEndPoints(visited[0]);
          if($filter('filter')($scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint), {isSelected: true, isdeleted: false}).length === ($scope.endPoint - $scope.startingPoint)){
            $scope.selectedList.allSelected = true;
          };
      }
     };

     $scope.setStartAndEndPoints = function(page){
      var foundStartPoint = false;
        $scope.selectedList.items.forEach(function(item, index){
          if(page.items[0] && item.listitemid === page.items[0].listitemid){
            $scope.startingPoint = index;
            $scope.endPoint = angular.copy($scope.startingPoint + parseInt($scope.pagingPageSize));
            foundStartPoint = true;
          }
        })

        if(!foundStartPoint){
          appendListItems(page);
        }
        blockUI.stop();
     }

    $scope.setRange = function(){
      $scope.endPoint = $scope.endPoint;
      $scope.rangeStart = $scope.startingPoint;
      $scope.rangeEnd = ($scope.endPoint > $scope.selectedList.itemCount) ? $scope.selectedList.itemCount : $scope.endPoint - 1;
      if($scope.rangeStart === 0){
        $scope.rangeStart++;
        if($scope.rangeEnd === parseInt($scope.pagingPageSize) - 1){
          $scope.rangeEnd ++;
        }
      }
    }

    $scope.pagingPageSize = LocalStorage.getPageSize();
    
    function resetPage(list, initialPageLoad) {
      $scope.selectedList = angular.copy(list);
      $scope.totalItems = $scope.selectedList.itemCount;
      originalList = list;
      $scope.selectedList.isRenaming = false;
      $scope.selectedList.allSelected = false;
      $scope.setStartAndEndPoints(list);

      if(initialPageLoad){      
        $scope.currentPage = 1;
        $scope.visitedPages.push({page: 1, items: $scope.selectedList.items});
      }
      $scope.setRange();

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

      $scope.visitedPages.push({page: $scope.currentPage, items: list.items});
      //Since pages can be visited out of order, sort visited pages into numeric order.
      $scope.visitedPages = $scope.visitedPages.sort(function(obj1, obj2){   
        var sorterval1 = obj1.page;      
        var sorterval2 = obj2.page;       
        return sorterval1 - sorterval2;         
      })
      //Rebuild selectedList with all items currently in the controller, in the correct order.
      $scope.selectedList.items = [];
      $scope.visitedPages.forEach(function(page){
        $scope.selectedList.items = $scope.selectedList.items.concat(page.items);
      })

      if(list.items.length === 0){
        $scope.startingPoint = 0;
        $scope.endPoint = 0;
      }
      else{
       $scope.setStartAndEndPoints(list);
      }

      if($filter('filter')($scope.selectedList.items.slice($scope.startingPoint, $scope.rangeEnd), {isSelected: true}).length === ($scope.rangeEnd - $scope.startingPoint)){
        $scope.selectedList.allSelected = true;
      };
      
    }

    function startLoading() {
      $scope.loadingResults = true;
    }

    function stopLoading() {
      $scope.loadingResults = false;
    }

    $scope.sort = [{
      field: 'position',
      order: 'asc'
    }];

    if($stateParams.sortingParams && $stateParams.sortingParams.sort.length){
      $scope.sort = $stateParams.sortingParams.sort;
    }

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

      var timeset =  moment().format('YYYYMMDDHHmm');
    
      var lastlist ={
          listId: list.listid,          
          timeset: timeset
      }
     
      LocalStorage.setLastList(lastlist);
      if(list.listid !== $scope.selectedList.listid && $scope.unsavedChangesConfirmation()){
        $scope.listForm.$setPristine();
        blockUI.start("Loading List...").then(function(){
          return $state.go('menu.lists.items', {listId: list.listid, renameList: false});      
        });
      }
    };
    
    function goToNewList(newList) {
      // user loses changes if they go to a new list
      $scope.listForm.$setPristine();
     var timeset =  moment().format('YYYYMMDDHHmm');
     var lastlist ={
          listId: newList.listid,          
          timeset: timeset
         }  
    
      LocalStorage.setLastList(lastlist);
      $state.go('menu.lists.items', {listId: newList.listid, renameList: true});
    }

    $scope.undoChanges = function() {
      resetPage(angular.copy(originalList));
    };

    $scope.unsavedChangesConfirmation = function(){
      if($scope.listForm.$dirty){
          var r = confirm('Unsaved data will be lost. Do you wish to continue?');
          return r;   
      }
      else{
        return true;
      }  
    };


    /**********
    PAGING
    **********/

    $scope.filterItems = function(searchTerm) {
      if($scope.unsavedChangesConfirmation()){
        $scope.visitedPages = [];
        listPagingModel.filterListItems(searchTerm);
      }
    };
    
    $scope.sortList = function(sortBy, sortOrder) {
      if($scope.unsavedChangesConfirmation()){         
        $scope.visitedPages = [];
        $scope.currentPage = 1;
        if (sortBy === $scope.sort[0].field) {
         sortOrder = (sortOrder === 'asc') ? 'desc' : 'asc';
        } else {
          sortOrder = 'asc';
        }
        $scope.sort = [{
          field: sortBy,
          order: sortOrder
        }];
        listPagingModel.sortListItems($scope.sort); 
      } 
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

    $scope.copyList = function(list) {
      var customers = [{
        customerNumber: $scope.selectedUserContext.customer.customerNumber,
        customerBranch: $scope.selectedUserContext.customer.customerBranch
      }];
      ListService.duplicateList(list, customers).then(function(newListId) {
        $state.go('menu.lists.items', { listId: newListId});
      });
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
      var params = {
          from: $scope.rangeStart - 1,
          size: $scope.pagingPageSize,
          sort: []    
        }

      if($stateParams.sortingParams){
        params.size = $stateParams.pageSize;
        params.sort = $stateParams.sortingParams;
      }

      if (!processingSaveList) {
        processingSaveList = true;
        var updatedList = angular.copy(list);

        angular.forEach(updatedList.items, function(item, itemIndex) {
          if (item.listitemid) {
            if (item.editLabel && item.isEditing) {
              item.label = item.editLabel;
            }
            item.position = item.editPosition;
            item.isEditing = false;
          }
        });

       listPagingModel.resetPaging();

        return ListService.updateList(updatedList, false, params)
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
      $scope.selectedList.items[deletedIndex].isdeleted = true;
      $scope.listForm.$setDirty();
    };

    $scope.deleteMultipleItems = function() {
      var selectedItemsForDelete = $filter('filter')($scope.selectedList.items, {isSelected: 'true'});
      $scope.selectedList.items.forEach(function(item){
        if(item.isSelected){
          item.isdeleted = true;
        }
      })

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
      $scope.multiSelect.showLabels = false;
    };

    $scope.applyNewLabel = function(label) {
      var items = getMultipleSelectedItems();
      angular.forEach(items, function(item, index) {
        item.isEditing = true;
        item.editLabel = label;
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
        if($scope.multiSelect){
          $scope.multiSelect.showLists = false;
        }
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

    $scope.changeAllSelectedItems = function(allSelected) {
      angular.forEach($scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint) , function(item, index) {
        if (item.itemnumber) {      
            item.isSelected = !allSelected; 
        }
      });
    };

    function unselectAllDraggedItems() {
      $scope.selectedList.allSelected = false;
      $scope.changeAllSelectedItems(true);
    }
    // disable drag on mobile
    $scope.isDragEnabled = function() {
      $scope.dragEnabled = window.innerWidth > 991 && !$scope.isMobileDevice;
    };

    $scope.isDragEnabled();

    $(window).resize(function(){ 
      $scope.$apply(function(){ 
      $scope.isDragEnabled();
      });
    });

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

   
    /******
    MODALS
    ******/

    $scope.openListImportModal = function () {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/listimportmodal.html',
        controller: 'ImportModalController',
        resolve: {
          customListHeaders: function() {
            return [];
          }
        }
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
      if($scope.unsavedChangesConfirmation()){
        $scope.listSearchTerm = '';
        $scope.filterItems( $scope.listSearchTerm );       
      }    
    };

    $scope.initParLvl = function(item) {  
      if(!item.parlevel){   
        item.parlevel=0;
      }   
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
              sort: $scope.sort,
              terms: $scope.listSearchTerm
            };
          }
        }
      });
    };

    resetPage(angular.copy(originalList), true);
    // $scope.selectedList.isRenaming = ($stateParams.renameList === 'true' && $scope.selectedList.permissions.canRenameList) ? true : false;

    if (ListService.renameList === true) {
      ListService.renameList = false;
      if ($scope.selectedList.permissions.canRenameList) {
        $scope.selectedList.isRenaming = true;
      }
    }

  }]);
