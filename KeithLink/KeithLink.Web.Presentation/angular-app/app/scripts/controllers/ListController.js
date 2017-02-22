'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$filter', '$timeout', '$state', '$stateParams', '$modal', 'blockUI', 'originalList',
   'Constants', 'ListService', 'CartService', 'PricingService', 'ListPagingModel', 'LocalStorage', 'UtilityService', 'DateService', 'ProductService',
    function($scope, $filter, $timeout, $state, $stateParams, $modal, blockUI, originalList, Constants, ListService, CartService,
     PricingService, ListPagingModel, LocalStorage, UtilityService, DateService, ProductService) {

    if(originalList.name == 'Non BEK Items'){
      originalList.listid = 'nonbeklist';
      $scope.selectedList = originalList;
    }
    if ($stateParams.listId !== originalList.listid.toString()) {
      $state.go('menu.lists.items', {listId: originalList.listid, renameList: null}, {location:'replace', inherit:false, notify: false});
    }

    var orderBy = $filter('orderBy');

    CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });

    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;

    // used for the 'Show More' button
    $scope.showMoreListNames = true;
    $scope.allSelected = false;
    $scope.rangeSelected = false;
    $scope.numberListNamesToShow = 10;
    $scope.indexOfSDestroyedRow = '';
    $scope.isMobileDevice = UtilityService.isMobileDevice();
    $scope.showRowOptionsDropdown = false;
    $scope.forms = {};
    $scope.isCustomInventoryList = originalList.iscustominventory ? true : false;
    $scope.selectedFilterParameter = 'Filter By...';

    $scope.availableFilterParameters = [
      {
        name: 'Recently Added Items',
        filter: {
          field: 'delta',
          value: 'newly added'
        }
      },
      {
        name: 'Recently Deleted Items',
        filter: {
          field: 'delta',
          value: 'newly deleted'
        }
      },
      {
        name: 'Recently Added/Deleted Items',
        filter: {
          condition: 'or',
          filter:[{
            field: 'delta',
            value: 'newly added'
          },
          {
            field: 'delta',
            value: 'newly deleted'
          }]
        }
      }
    ];

    $scope.selectFilterParameter = function(filterparameter) {
      $scope.selectedFilterParameter = filterparameter.name;
      $scope.selectedFilter = filterparameter.filter;

      $scope.filterItems();
    };

    // detect IE
    // returns $scope.isIE is true if IE or false, if browser is not IE
    function detectIE() {
        var ua = window.navigator.userAgent;

        var msie = ua.indexOf('MSIE ');//IE <11
        var trident = ua.indexOf('Trident/');//IE 11
        if (msie > 0) {
          $scope.isIE = true;
        } else if( trident > 0) {
          $scope.isIE = true;
        } else {
          $scope.isIE = false;
        }
    }
    detectIE();
   

    if (ListService.findMandatoryList()) {
      $scope.hideMandatoryListCreateButton = true;
    }
    if (ListService.findRecommendedList()) {
      $scope.hideRecommendedListCreateButton = true;
    }

    //Toggle scope variable to render Reports side panel when screen is resized
    $(window).resize(function(){ 
      $scope.$apply(function(){ 
        $scope.renderSidePanel();
      });
    });
  
    $scope.renderSidePanel = function(){
      $scope.resized = window.innerWidth > 991;
    };
    $scope.renderSidePanel();

    $scope.rowChanged = function(index){
      $scope.indexOfSDestroyedRow = index;
    };

    $scope.initPagingValues = function(resetPage){
      $scope.visitedPages = [];
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.itemCountOffset = 0;
      if(resetPage){        
        $scope.currentPage = 1;
      }
    };

    $scope.initPagingValues();

    var selectionStartIndex,
        selectionStopIndex,
        lastSingleClick,
        selectionList;

    $scope.shiftSelectOtherRows = function(evt, item, index){
      selectionList = $scope.selectedList.items.slice(0, $scope.selectedList.items.length);
      if (evt.shiftKey) {
        $scope.rangeSelected = true;
        if (lastSingleClick != undefined) {
          selectionStartIndex = lastSingleClick;
        }
        selectionStopIndex = selectionList.indexOf(item);
        selectRowsBetweenIndexes([selectionStopIndex, selectionStartIndex], selectionList);
      } else {
        lastSingleClick = selectionList.indexOf(item);
        item.isSelected = !item.isSelected;
      }
    };

    function selectRowsBetweenIndexes(indexes, selectionList) {
      indexes.sort(function(a, b) {
          return a - b;
      });

      for (var i = indexes[0]; i <= indexes[1]; i++) {
        if(selectionList[i].itemnumber){
          selectionList[i].isSelected = true;
        }
      }
    }

    $scope.blockUIAndChangePage = function(page){
      $scope.startingPoint = 0;
      $scope.endPoint = 0;       
      var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});
      blockUI.start('Loading List...').then(function(){
        if(visited.length > 0){
          $timeout(function() {
            $scope.isChangingPage = true;
            $scope.pageChanged(page, visited);
          }, 100);
        }
        else{
          $scope.pageChanged(page, visited);
        }
      });
    };

    $scope.pageChanged = function(page) {      
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.loadingPage = true;    
      $scope.currentPage = page.currentPage;
      $scope.startingPoint = ((page.currentPage - 1)*$scope.pagingPageSize) + 1;
      $scope.endPoint = angular.copy($scope.startingPoint + $scope.pagingPageSize);
      $scope.setRange();
      $scope.selectedList.allSelected = false;
      $scope.allSelected = false;
      $scope.rangeSelected = false;
      var deletedItems = [];
      $scope.selectedList.items.forEach(function(item){
        if(item.deleted){
          deletedItems.push(item);
        }
      });
      var visited = $filter('filter')($scope.visitedPages, {page: $scope.currentPage});
      if(!visited.length){             
        listPagingModel.loadMoreData($scope.startingPoint - 1, $scope.endPoint - 1, $scope.loadingResults, deletedItems);       
      } else {
        $scope.setStartAndEndPoints(visited[0]);
        if($filter('filter')($scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint), {isSelected: true, isdeleted: false}).length === ($scope.endPoint - $scope.startingPoint)){
          $scope.selectedList.allSelected = true;
        }
        updateItemPositions();
      }
    };

     $scope.setStartAndEndPoints = function(page){
      var foundStartPoint = false;
        $scope.selectedList.items.forEach(function(item, index){
          if(page.items[0] && item.listitemid === page.items[0].listitemid){
            $scope.startingPoint = index;
            $scope.endPoint = angular.copy($scope.startingPoint + $scope.pagingPageSize);
            foundStartPoint = true;
          }
        });

        if(!foundStartPoint){
          appendListItems(page);
        }
        //We need two calls for stop here because we have two paging directives on the view. If the page change is triggered 
        //automatically (deleting all items on page/saving) the event will fire twice and two loading overlays will be generated.
        blockUI.stop();
        blockUI.stop();
     };

    $scope.setRange = function(){    
      $scope.rangeStart = (($scope.currentPage - 1)*$scope.pagingPageSize) + 1;
      $scope.rangeEnd = $scope.rangeStart + $scope.pagingPageSize;
      $scope.rangeEnd = ($scope.rangeEnd > $scope.selectedList.itemCount) ? $scope.selectedList.itemCount : $scope.rangeEnd -1;
      if($scope.rangeStart === 0){
        $scope.rangeStart++;
        if($scope.rangeEnd === $scope.pagingPageSize - 1){
          $scope.rangeEnd ++;
        }
      }
    };

    $scope.pagingPageSize = parseInt(LocalStorage.getPageSize());

    function resetPage(list, initialPageLoad) {
      $scope.initPagingValues();
      $scope.activeElement = true;
      $scope.selectedList = angular.copy(list);
      $scope.totalItems = $scope.selectedList.itemCount;
      originalList = list;
      $scope.selectedList.isRenaming = false;
      $scope.selectedList.allSelected = false;
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.setStartAndEndPoints(list);

      if(initialPageLoad){      
        $scope.currentPage = 1;
        $scope.visitedPages.push({page: 1, items: $scope.selectedList.items, deletedCount: 0});
        updateItemPositions();
      }
      $scope.setRange();

      if ($scope.forms.listForm) {
        $scope.forms.listForm.$setPristine();
      }


      if($scope.selectedList.name == 'Non BEK Items' && $scope.selectedList.items.length == 0){
        $scope.addNewItemToList();
      }

      $scope.selectedList.items.forEach(function(item) {
        item.editPosition = item.position;
        if(item.custominventoryitemid > -1){
          $scope.listHasCustomItems = true;
        }
      });
    }

    function appendListItems(list) {
      list.items.forEach(function(item) {
        item.editPosition = item.position;
      });

      $scope.visitedPages.push({page: $scope.currentPage, items: list.items, deletedCount: 0});
      //Since pages can be visited out of order, sort visited pages into numeric order.
      $scope.visitedPages = $scope.visitedPages.sort(function(obj1, obj2){   
        var sorterval1 = obj1.page;      
        var sorterval2 = obj2.page;       
        return sorterval1 - sorterval2;         
      });
      //Rebuild selectedList with all items currently in the controller, in the correct order.
      $scope.selectedList.items = [];
      $scope.visitedPages.forEach(function(page){
        $scope.selectedList.items = $scope.selectedList.items.concat(page.items);
      });

      if(list.items.length === 0){
        $scope.startingPoint = 0;
        $scope.endPoint = 0;
      }
      else{
       $scope.setStartAndEndPoints(list);
      }
       updateItemPositions();
      if($filter('filter')($scope.selectedList.items.slice($scope.startingPoint, $scope.rangeEnd), {isSelected: true}).length === ($scope.rangeEnd - $scope.startingPoint)){
        $scope.selectedList.allSelected = true;
      }
      
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
    $scope.goToList = function(listid) {
      if($scope.selectedList.iscustominventory){
        $scope.isCustomInventoryList = false;
      }

      var timeset =  DateService.momentObject().format(Constants.dateFormat.yearMonthDayHourMinute);
    
      var lastlist ={
          listId: listid,          
          timeset: timeset
      };
     
      LocalStorage.setLastList(lastlist);
      if($scope.unsavedChangesConfirmation()){
        if($scope.forms.listForm) {
          $scope.forms.listForm.$setPristine();
        }
        blockUI.start('Loading List...').then(function(){
          return $state.go('menu.lists.items', {listId: listid, renameList: false});
        });
      }
    };
    
    function goToNewList(newList) {
      // user loses changes if they go to a new list
      $scope.forms.listForm.$setPristine();
     var timeset =  DateService.momentObject().format(Constants.dateFormat.yearMonthDayHourMinute);
     var lastlist ={
          listId: newList.listid,          
          timeset: timeset
         };
    
      LocalStorage.setLastList(lastlist);
      $state.go('menu.lists.items', {listId: newList.listid, renameList: true});
    }

    $scope.undoChanges = function() {
      resetPage(angular.copy(originalList));
    };

    $scope.unsavedChangesConfirmation = function(){
      if($scope.forms.listForm && $scope.forms.listForm.$dirty){
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
      if($scope.unsavedChangesConfirmation() && searchTerm){
        $scope.initPagingValues(true);
        listPagingModel.filterListItems(searchTerm);
      } else if($scope.unsavedChangesConfirmation() && $scope.selectedFilter) {
        $scope.initPagingValues(true);
        listPagingModel.filterListItemsByMultipleFields($scope.selectedFilter);
      } else {
        listPagingModel.filterListItems();
      }
    };
    
    $scope.sortList = function(sortBy, sortOrder) {
      if($scope.unsavedChangesConfirmation()){         
        $scope.initPagingValues(true);        
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
    $scope.saveList = function(list, addingItem) {
      var params = {
          from: $scope.rangeStart - 1,
          size: $scope.pagingPageSize,
          sort: $scope.sort
        };

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

        return ListService.updateList(updatedList, false, params, addingItem)
          .then(resetPage).finally(function() {
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

      // saves new item indexes in cached editPosition field after sorting or ordering the list items
     function updateItemPositions() {

      if($scope.selectedList.read_only){
        return;
      }
      var deletedItemCount = 0;
      var currentPageDeletes = 0;
      var currentPageDeletedCount = 0;
      $scope.itemCountOffset = 0;
      $scope.visitedPages.forEach(function(page){
      if($scope.currentPage > page.page){
        deletedItemCount += page.deletedCount;
      }
      if($scope.currentPage === page.page){
        currentPageDeletedCount = page.deletedCount;
      }
      $scope.itemCountOffset += page.deletedCount;
      });

      $scope.rangeStartOffset = ($scope.currentPage === 1) ? 0 : deletedItemCount;
      $scope.rangeEndOffset = deletedItemCount + currentPageDeletedCount;
      var newPosition = (($scope.pagingPageSize*($scope.currentPage - 1)) + 1) - deletedItemCount;
      angular.forEach($scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint), function(item, index) {
        if(!item.isdeleted){
            item.position = newPosition;
            item.editPosition = newPosition;
            newPosition += 1;
        }
      });
    }

    /**********
    DELETE ITEMS
    **********/
     function updateDeletedCount() {
      $scope.visitedPages.forEach(function(page){
        if($scope.currentPage === page.page){
          page.deletedCount++;
        }
      });
    }

    $scope.deleteItem = function(item) {    
      $scope.forms.listForm.$setDirty();
      item.isdeleted = true;
      updateDeletedCount();
      updateItemPositions();
    };

    $scope.deleteMultipleItems = function() {
      $scope.isDeletingItem = true;

      if($scope.isCustomInventoryList){
        var itemsToDelete = $filter('filter')($scope.selectedList.items, {isSelected: true, id: ''});
        ListService.deleteCustomInventoryItems(itemsToDelete).then(function(response) {
          $scope.forms.listForm.$setPristine();
          $scope.selectedList.items = response.items;
        });
      } else {
        $scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint).forEach(function(item){
          if(item.isSelected){
            item.isdeleted = true;
            updateDeletedCount();
          }
        });
      }

      $scope.selectedList.allSelected = false;
      updateItemPositions();
      $scope.forms.listForm.$setDirty();
      $scope.isDeletingItem = false;
    };

    /**********
    LABELS
    **********/

    $scope.addLabels = function(label) {
      var items = getMultipleSelectedItems();
      angular.forEach(items, function(item, index) {
        item.label = label;
      });
      $scope.forms.listForm.$setDirty();
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
      $scope.forms.listForm.$setDirty();
    };

    /**********
    ADD ITEMS
    **********/

    $scope.addItemsToList = function(list, items) {
      if (!items) {
        items = angular.copy(getMultipleSelectedItems());
      }

      if($scope.isCustomInventoryList){
        ListService.addNewItemsFromCustomInventoryList(list.listid, items);
      } else {
        ListService.addMultipleItems(list.listid, items).then(function(updatedList) {
          if ($scope.selectedList.listid === updatedList.listid) {
            $scope.selectedList = list;
          }
          if($scope.multiSelect){
            $scope.multiSelect.showLists = false;
          }
        });
      }
      unselectAllDraggedItems();
    };

    $scope.addNewItemToList = function(){
      var newItem = [],
          item = {
            brand:null,
            caseprice:0,
            each:false,
            itemnumber:null,
            pack:null,
            packageprice:0,
            parlevel:0,
            id:null,
            quantity:0,
            size:null,
            vendor:null
          };
      $scope.selectedList.items.push(item);
    };

    $scope.addItemByItemNumber = function(itemNumber) {
      $scope.successMessage = '';
      $scope.errorMessage = '';

      blockUI.start().then(function(){
        ProductService.getProductDetails(itemNumber).then(function(item) {
          ListService.addItem($scope.selectedList.listid, item).then(function(data){
            $scope.saveList($scope.selectedList, true);
            $scope.listItemNumber = '';
            $scope.displayMessage('success', 'Successfully added item to list.');
          }, function() {
            $scope.displayMessage('error', 'Error adding item to list.');
          });
        });
      });
    };



    /********************
    PARLEVEL
    ********************/

    $scope.parlevelChanged = function(evt) {
      var keycode=evt.keyCode ? evt.keyCode : evt.charCode;
      if (keycode >= Constants.jskeycodes.int0 && keycode <= Constants.jskeycodes.int9 && $scope.forms.listForm.$pristine) {
        $scope.forms.listForm.$setDirty();
      }else{
        return;
      }
    };


    /********************
    DRAG HELPERS
    ********************/

    function getMultipleSelectedItems() {
      if($scope.isCustomInventoryList){
        return $filter('filter')($scope.selectedList.items, {isSelected: 'true'});
      } else {
        return $filter('filter')($scope.selectedList.items, {isSelected: 'true', isdeleted:'!true', catalog_id:'!CUSTOM'});
      }
      
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
      if($scope.selectedList.iscustominventory){
        $scope.startingPoint = 0;
      } 
      angular.forEach($scope.selectedList.items.slice($scope.startingPoint, $scope.selectedList.items.length) , function(item, index) {
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
    $scope.setIsDragging = function(event, helper, isDragging, itemId ) {
      $scope.selectedList.items.forEach(function(item){
        if(itemId === item.listitemid){
          item.isSelected = true;
        }
      });
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
      $scope.isDeletingItem = true;

      angular.forEach(dragSelection, function(item, index) {
        $scope.deleteItem(item);
      });
    };

    $scope.addItemFromDrag = function (event, helper, list) {
      var dragSelection = getSelectedItemsFromDrag(helper);
      $scope.addItemsToList(list, dragSelection);
    };

    /*********************
    CUSTOM INVENTORY LIST
    *********************/

    $scope.getCustomInventoryList = function() {
      if($scope.selectedList.name != 'Non BEK Items') {
        $scope.previousList = originalList;
      }

      ListService.getCustomInventoryList().then(function(resp){
        originalList = resp;
        $scope.selectedList = originalList;
        $scope.isCustomInventoryList = true;
        if($scope.selectedList.items.length == 0){
          $scope.addNewItemToList();
        }
      });
    };

    $scope.saveCustomInventoryList = function(list) {
      var itemsToSave = $filter('filter')(list, {itemnumber: ''});
        itemsToSave.forEach(function(item){
          if(item.editLabel && item.isEditing){
            item.label = item.editLabel;
          }
        });
      ListService.saveCustomInventoryList(itemsToSave).then(function(resp){
        $scope.selectedList = resp;
        $scope.isCustomInventoryList = true;
        $scope.forms.listForm.$setPristine();
      });
    };

    $scope.deleteCustomInventoryItem = function(listitem, index) {
      if(!listitem.id){
        $scope.selectedList.items.splice(index, 1);
      } else {
        ListService.deleteCustomInventoryItem(listitem.id).then(function(){
          $scope.getCustomInventoryList();
        });
      }

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
          },
          listType: function() {
            if($scope.isCustomInventoryList){
              return 'CustomInventory';
            } else {
              return 'StandardList';
            }
          }
        }
      });
    };

    $scope.openExportModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/exportmodal.html',
        controller: 'ExportModalController',
        resolve: {
          location: function() {
            return {category:'Lists', action:'Export List'};
          },
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

    $scope.clearFilter = function(clearSearch){
      if($scope.unsavedChangesConfirmation()){
        if(clearSearch){
          $scope.listSearchTerm = '';
        }
        
        $scope.selectedFilterParameter = 'Filter By...';
        $scope.selectedFilter = '';
        $scope.filterItems( $scope.listSearchTerm );       
      }    
    };

    $scope.initParLvl = function(item) {  
      if(!item.parlevel){   
        item.parlevel='0';
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
          cart: function() {
            return false;
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

    if((($scope.canManageLists || $scope.canCreateOrders || $scope.canSubmitOrders))){
      $scope.showRowOptionsDropdown = true;
    }

    resetPage(angular.copy(originalList), true);
    // $scope.selectedList.isRenaming = ($stateParams.renameList === 'true' && $scope.selectedList.permissions.canRenameList) ? true : false;

    if (ListService.renameList === true) {
      ListService.renameList = false;
      if ($scope.selectedList.permissions.canRenameList) {
        $scope.selectedList.isRenaming = true;
      }
    }

  $scope.scrollToTop = function($var) {
    $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').css({'display': 'inline'});
    var duration = 300;
    event.preventDefault();
    jQuery('html, body').animate({scrollTop: 0}, duration);
    return false;
  };

  $(window).scroll(function() {
    if($(this).scrollTop() > 190){
      $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').fadeIn('fast');
      $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').css('visibility', 'visible');
    } else {
      $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').fadeOut('fast');
    }
  });

}]);
