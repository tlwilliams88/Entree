'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', ['$scope', '$sce', 'ListService', function($scope, $sce, ListService) {
    
    $scope.selectedList = {
      name: 'My list'
    };

    ListService.getAllLists().then(function(data) {
      $scope.lists = data;
      $scope.selectedList = $scope.lists[0];
      return $scope.selectedList;
    }).then(function(data) {
      
      ListService.getList(data.listid).then(function(data) {
        $scope.selectedList = data;
      });
    });

    ListService.getAllLabels().then(function(data) {
      $scope.labels = data;
    });

    $scope.goToList = function(list) {
      ListService.getList(list.listid).then(function(data) {
        $scope.selectedList = data;
      });
    };

    $scope.createList = function() {
      // TODO: determine new list default name
      $scope.selectedList = {
        name: 'New List'
      };
      // TODO: post list to server, append new list to list of lists
    };

    $scope.updateItemLabel = function(product) {
      console.log('update label to ' + product.label + ' for listItemId ' + product.productid);
    };

    $scope.addSavedItem = function(item) {
      console.log('add item # ' + item.productid + ' to saved items');
    };

    $scope.removeSavedItem = function(item) {
      console.log('remove item # ' + item.productid + ' from saved items');
    };

    // edit list name
    $scope.renameList = function (listId, listName) {
      console.log('rename list ' + listId + ' to ' + listName);
      $scope.editingListName = false;
    };

    $scope.cancelEditListName = function() {
      // $scope.editList = {};
      $scope.editingListName = false;
    };

    $scope.startEditListName = function(listName) {
      $scope.editList = {};
      $scope.editList.name = angular.copy(listName);
      $scope.editingListName = true;
    };

    var selectedProduct;
    $scope.selectItem = function(event, helper, product) {
      selectedProduct = product;
    };

    $scope.addItemToList = function (event, helper, list) {
      console.log('add item ' + selectedProduct.productid + ' to list ' + list.name);
    };

    $scope.deleteItem = function(event, helper, list) {
      console.log('delete item ' + selectedProduct.productid + ' from list ' + list.name);
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
    var limit = 1;
    $scope.itemsLimit = function() {
      return limit;
    };
    $scope.showMore = function() {
      limit = $scope.lists.length;
    };
    $scope.hasMoreItemsToShow = function() {
      if ($scope.lists) {
        return limit < $scope.lists.length;
      }
    };

    // ALERTS
    $scope.alerts = [
      // { type: 'danger', msg: 'Oh snap! Change a few things up and try submitting again.' },
      // { type: 'success', msg: 'Well done! You successfully read this important alert message.' }
    ];

    $scope.closeAlert = function(index) {
      $scope.alerts.splice(index, 1);
    };

  }]);