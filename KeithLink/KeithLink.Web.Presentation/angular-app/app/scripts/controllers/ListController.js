'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', 'ListService', function($scope, ListService) {

    $scope.alerts = [];
    $scope.loadingResults = true;

    ListService.getAllLists().then(function(data) {
      $scope.lists = data;
      $scope.selectedList = $scope.lists[0];
      $scope.loadingResults = false;
    });

    ListService.getAllLabels().then(function(data) {
      $scope.labels = data;
    });

    $scope.goToList = function(list) {
      $scope.selectedList = list;
    };

    $scope.createList = function() {

      // generate new list name
      var date = new Date(),
        dateString = date.getMonth() + 1 + '-' + date.getDate(),
        newList = {
          name: 'New List ' + dateString
        };

      ListService.createList(newList).then(function(data) {
        newList.listid = data;
        $scope.lists.push(newList);
        $scope.selectedList = newList; 
        addSuccessAlert('Successfully created a new list.');
      }, function(error) {
        addErrorAlert('Error creating list.');
      });
    };

    $scope.updateItem = function(listId, item) {
      ListService.updateItem(listId, item).then(function(data) {
        addSuccessAlert('Successfully added label ' + item.label + ' to item ' + item.itemnumber + '.');
      });
    };

    $scope.addNewLabel = function(listId, newLabel, item) {
      item.label = newLabel;
      ListService.updateItem(listId, item).then(function(data) {
        item.isEditing = false;
        $scope.labels.push(newLabel);
        addSuccessAlert('Successfully created new label ' + newLabel + ' and added it to item ' + item.itemnumber + '.');
      });
    };

    $scope.addSavedItem = function(item) {
      console.log('add item # ' + item.itemnumber + ' to saved items');
    };

    $scope.removeSavedItem = function(item) {
      console.log('remove item # ' + item.itemnumber + ' from saved items');
    };

    $scope.renameList = function (listId, listName) {
      var list = angular.copy($scope.selectedList);
      list.name = listName;

      ListService.updateList(list).then(function(data) {
        $scope.selectedList.name = listName;
        $scope.displayEditingListName = false;
        addSuccessAlert('Successfully renamed list.');
      });
    };

    $scope.cancelEditListName = function() {
      $scope.displayEditingListName = false;
    };

    $scope.startEditListName = function(listName) {
      $scope.editList = {};
      $scope.editList.name = angular.copy(listName);
      $scope.displayEditingListName = true;
    };

    $scope.addItemToList = function (event, helper, listId) {
      var selectedItem = helper.draggable.data('product');

      ListService.addItem(listId, selectedItem).then(function(data) {
        debugger;
      },function(error) {
        addErrorAlert('Error adding item ' + selectedItem.itemnumber + ' to list.');
      });
    };

    $scope.deleteItem = function(event, helper, list) {
      var selectedItem = helper.draggable.data('product');

      console.log('delete item ' + selectedItem.itemnumber + ' from list ' + list.name);
    };

    $scope.deleteList = function(listId) {
      var idx = $scope.lists.indexOf($scope.selectedList);

      // TODO: ask for confirmation, should we allow users to delete lists with items?
      ListService.deleteList(listId).then(function(data) {
        $scope.lists.splice(idx, 1);
        $scope.selectedList = $scope.lists[0];
        addSuccessAlert('Successfully deleted list.');
      });
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

    $scope.setIsDragging = function(event, helper, isDragging) {
      $scope.isDragging = isDragging;
    };


    // ALERTS
    function addSuccessAlert(message) {
      addAlert('success', message);
    };
    function addErrorAlert(message) {
      addAlert('danger', message);
    };
    function addAlert(alertType, message) {
      $scope.alerts[0] = { type: alertType, msg: message };
    };
    $scope.closeAlert = function(index) {
      $scope.alerts.splice(index, 1);
    };

    // function errorHandler(message, error) {

    // };

  }]);