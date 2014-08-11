'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$state', '$stateParams', 'ListService', function($scope, $state, $stateParams, ListService) {

    $scope.alerts = [];
    $scope.loadingResults = true;

    $scope.sortBy = 'itemnumber';
    $scope.sortOrder = false;

    $scope.lists = ListService.lists;
    $scope.labels = ListService.labels;

    ListService.getAllLists().then(function(data) {
      
      if ($stateParams.listId) {
        $scope.selectedList = ListService.findListById($stateParams.listId, data);
      }
      if (!$scope.selectedList) {
        $scope.selectedList = $scope.lists[0];
        $state.transitionTo('menu.lists', {}, {notify: false});
      }

      $scope.loadingResults = false;
    });

    ListService.getAllLabels();

    $scope.goToList = function(list) {
      $state.transitionTo('menu.listitems', {listId: list.listid}, {notify: false});
      $scope.selectedList = list;
    };

    $scope.createList = function(items) { //DONE
      ListService.createList().then(function(data) {
        addSuccessAlert('Successfully created a new list.');
      }, function(error) {
        addErrorAlert('Error creating list.');
      });
    };

    $scope.createListWithItem = function(event, helper) { //DONE
      var selectedItem = helper.draggable.data('product');
      ListService.createListWithItem(selectedItem).then(function(data) {
        addSuccessAlert('Successfully created a new list with item ' + selectedItem.itemnumber + '.');
      }, function(error) {
        addErrorAlert('Error creating list.');
      });
    };

    $scope.deleteList = function(listId) { //DONE
      ListService.deleteList(listId).then(function(data) {
        $scope.selectedList = $scope.lists[0];
        addSuccessAlert('Successfully deleted list.');
      },function(error) {
        addErrorAlert('Error deleting list.');
      });
    };

    $scope.updateLabel = function(listId, item) { //DONE
      ListService.updateItem(listId, item).then(function(data) {
        addSuccessAlert('Successfully added label ' + item.label + ' to item ' + item.itemnumber + '.');
      },function(error) {
        addErrorAlert('Error deleting list.');
      });
    };

    $scope.addNewLabel = function(listId, newLabel, item) { //DONE
      item.label = newLabel;
      $scope.updateLabel(listId, item);
    };

    $scope.renameList = function (listId, listName) { //DONE
      var list = angular.copy($scope.selectedList);
      list.name = listName;

      ListService.updateList(list).then(function(data) {
        $scope.displayEditingListName = false;
        $scope.selectedList.name = listName;
        addSuccessAlert('Successfully renamed list to ' + listName + '.');
      });
    };

    $scope.cancelEditListName = function() { //DONE
      $scope.displayEditingListName = false;
    };

    $scope.startEditListName = function(listName) { //DONE
      $scope.editList = {};
      $scope.editList.name = angular.copy(listName);
      $scope.displayEditingListName = true;
    };

    $scope.addItemToList = function (event, helper, listId) {
      var selectedItem = helper.draggable.data('product');

      ListService.addItemToListAndFavorites(listId, selectedItem).then(function(data) {
        addSuccessAlert('Successfully added item ' + selectedItem.itemnumber + ' to list.');
      },function(error) {
        addErrorAlert('Error adding item ' + selectedItem.itemnumber + ' to list.');
      });
    };

    $scope.editParLevel = function(listId, item) {
      ListService.updateItem(listId, item).then(function(data) {
        addSuccessAlert('Successfully update PAR Level for item ' + item.itemnumber + '.');
      },function(error) {
        addErrorAlert('Error updating PAR level.');
      });
    };

    $scope.deleteItem = function(event, helper, list) { //DONE
      var selectedItem = helper.draggable.data('product');
      
      ListService.deleteItem(list.listid, selectedItem.listitemid).then(function(data) {
        addSuccessAlert('Successfully removed item ' + selectedItem.itemnumber + '.');
      },function(error) {
        addErrorAlert('Error removing item ' + selectedItem.itemnumber + ' from list.');
      });
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

    $scope.listSearchTerm = '';
    $scope.search = function (row) {
      var term = $scope.listSearchTerm.toLowerCase();

      var itemnumberMatch = row.itemnumber.toLowerCase().indexOf(term || '') !== -1,
        nameMatch = row.name.toLowerCase().indexOf(term || '') !== -1,
        labelMatch =  row.label && (row.label.toLowerCase().indexOf(term || '') !== -1);

      return !!(itemnumberMatch || nameMatch || labelMatch);
    };

  }]);