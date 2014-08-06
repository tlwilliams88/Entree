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

    $scope.loadingResults = true;

    $scope.lists = [
    {
        'listid': '710ab77a-a97b-4898-a140-dbc27f853abc',
        'name': 'Sample List A',
        'items': [
            {
                'listitemid': 'ed699437-57ad-4f51-b15f-d04042cc589c',
                'itemnumber': '284569',
                'label': 'This is a label',
                'parlevel': 0,
                'position': 0
            },
            {
                'listitemid': 'ed859f7f-5e40-4c3b-9d6e-7b05d81cbd14',
                'itemnumber': '287100',
                'label': null,
                'parlevel': 0,
                'position': 1
            }
        ]
    },
    {
        'listid': '58d88c28-87c1-4476-a166-53df19367e05',
        'name': 'Sample List B',
        'items': [
            {
                'listitemid': 'a9e1546f-674d-46a9-b527-fa47f5498f53',
                'itemnumber': '287302',
                'label': null,
                'parlevel': 0,
                'position': 0
            },
            {
                'listitemid': '11240cb1-b83e-4b65-af9c-700d49f6710d',
                'itemnumber': '287770',
                'label': 'Test Label',
                'parlevel': 0,
                'position': 1
            },
            {
                'listitemid': '9f9629e0-22b6-48db-b3be-85a5e72c0454',
                'itemnumber': '287402',
                'label': null,
                'parlevel': 0,
                'position': 2
            }
        ]
    }
];
$scope.selectedList = $scope.lists[0];

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
      // TODO: determine new list default name
      $scope.selectedList = {
        name: 'New List'
      };
      // TODO: post list to server, append new list to list of lists
    };

    $scope.updateItemLabel = function(product) {
      console.log('update label to ' + product.label + ' for listItemId ' + product.itemnumber);
    };

    $scope.addSavedItem = function(item) {
      console.log('add item # ' + item.itemnumber + ' to saved items');
    };

    $scope.removeSavedItem = function(item) {
      console.log('remove item # ' + item.itemnumber + ' from saved items');
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
      console.log('add item ' + selectedProduct.itemnumber + ' to list ' + list.name);
    };

    $scope.deleteItem = function(event, helper, list) {
      console.log('delete item ' + selectedProduct.itemnumber + ' from list ' + list.name);
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
    $scope.alerts = [
      // { type: 'danger', msg: 'Oh snap! Change a few things up and try submitting again.' },
      // { type: 'success', msg: 'Well done! You successfully read this important alert message.' }
    ];

    $scope.closeAlert = function(index) {
      $scope.alerts.splice(index, 1);
    };

  }]);