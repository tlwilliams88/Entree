'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', ['$http', '$q', '$filter', 'toaster', 'UserProfileService', 'UtilityService', 'List',
    function($http, $q, $filter, toaster, UserProfileService, UtilityService, List) {

      var filter = $filter('filter');

      function updateItemPositions(list) {
        angular.forEach(list.items, function(item, index) {
          item.position = index+1;
        });
      }

      var Service = {

        lists: [],
        labels: [],
        selectedList: {},

        // accepts "header: true" params to get only list names
        // return array of list objects
        getAllLists: function(params) {
          if (!params) {
            params = {};
          }
          return List.query(params).$promise.then(function(lists) {
            angular.copy(lists, Service.lists);
            return lists;
          });
        },

        getListHeaders: function() {
          return Service.getAllLists({ header: true });
        },

        // accepts listId (guid)
        // returns list object
        getList: function(listId) {
          return List.get({
            listId: listId,
          }).$promise.then(function(list) {

            // update new list in cache object
            var existingList = UtilityService.findObjectByField(Service.lists, 'listid', list.listid);
            if (existingList) {
              var idx = Service.lists.indexOf(existingList);
              angular.copy(list, Service.lists[idx]);
            } else {
              Service.lists.push(list);
            }

            angular.copy(list, Service.selectedList);

            return list;
          });
        },

        findListById: function(listId) {
          var itemsFound = filter(Service.lists, function(list) {
            return list.listid == listId;
          });
          if (itemsFound.length === 1) {
            return itemsFound[0];
          }
        },

        /********************
        EDIT LIST
        ********************/

        // accepts null, item object, or array of item objects
        // returns promise and new list object
        createList: function(items) {

          var newList = {};

          if (!items) { // if null
            newList.items = [];
          } else if (Array.isArray(items)) { // if multiple items
            newList.items = items;
          } else if (typeof items === 'object') { // if one item
            newList.items = [items];
          }

          // remove irrelevant properties from items
          UtilityService.deleteFieldFromObjects(newList.items, ['listitemid', 'position', 'label', 'parlevel']);

          newList.name = UtilityService.generateName('List', Service.lists);

          return List.save({}, newList).$promise.then(function(response) {
            toaster.pop('success', null, 'Successfully created list.');
            return Service.getList(response.listitemid);
          }, function() {
            toaster.pop('error', null, 'Error creating list.');
          });
        },

        // accepts list object
        // returns promise and updated list object
        updateList: function(list) {
          return List.update(null, list).$promise.then(function(response) {
            
            // update labels
            angular.forEach(list.items, function(item, index) {
              if (item.label && Service.labels.indexOf(item.label) === -1) {
                Service.labels.push(item.label);
              }
            });

            return Service.getList(response.listid).then(function(list) {
              toaster.pop('success', null, 'Successfully save list ' + list.name + '.');
              return list;
            });
          }, function() {
            toaster.pop('error', null, 'Error saving list ' + list.name + '.');
          });
        },

        // accepts listId (guid)
        deleteList: function(listId) {
          return List.delete({
            listId: listId
          }).$promise.then(function(response) {
            // TODO: can I clean this up?
            var deletedList = Service.findListById(listId);
            var idx = Service.lists.indexOf(deletedList);
            if (idx > -1) {
              Service.lists.splice(idx, 1);
            }

            toaster.pop('success', null, 'Successfully deleted list ' + deletedList.name + '.');
            return Service.getFavoritesList();
          }, function() {
            toaster.pop('error', null, 'Error deleting list.');
          });
        },

        deleteMultipleLists: function(listGuidArray) {
          return $http.delete('/list', {
            headers: {'Content-Type': 'application/json'},
            data: listGuidArray
          });
        },

        /********************
        EDIT SINGLE ITEM
        ********************/

        // accepts listId (guid) and item object
        // returns promise and listitemid
        addItem: function (listId, item) {
          delete item.listitemid;
          item.position = 0;
          item.label = null;
          item.parlevel = null;

          return List.addItem({
            listId: listId
          }, item).$promise.then(function(response) {
            item.listitemid = response.listitemid;
            item.editPosition = 0;

            toaster.pop('success', null, 'Successfully added item to list.');
            return item;
          }, function() {
            toaster.pop('error', null, 'Error adding item to list.');
          });
        },

        // accepts listId (guid) and item object
        updateItem: function(listId, item) {
          return List.updateItem({}, item).$promise.then(function(response) {
            // TODO: add label to Service.labels if it does not exist
            // TODO: replace item in Service.lists
            return response.data;
          });
        },

        // accepts listId and listItemId for item to be deleted
        deleteItem: function(listItemId) {
          return List.deleteItem({
            listItemId: listItemId
          }).$promise.then(function(response) {
            toaster.pop('success', null, 'Successfully deleted item from list.');
            return;
          }, function() {
            toaster.pop('error', null, 'Error deleting item from list.');
          });
        },

        /********************
        EDIT MULTIPLE ITEMS
        ********************/

        // accepts listId (guid) and an array of items to add
        // params: allowDuplicates
        addMultipleItems: function(listId, items) {
          
          UtilityService.deleteFieldFromObjects(items, ['listitemid', 'position', 'label', 'parlevel']);

          return List.addMultipleItems({
            listId: listId
          }, items).$promise.then(function() {
            // TODO: favorite all items if favorites list
            toaster.pop('success', null, 'Successfully added ' + items.length + ' items to list.');
            return Service.getList(listId);
          }, function() {
            toaster.pop('error', null, 'Error adding ' + items.length + ' items to list.');
          });
        },

        // accepts listId (guid) and an array of items
        // NOTE $resource does not accept deletes with payloads
        deleteMultipleItems: function(listId, items) {

          // create array of list item ids
          var listItemIds = [];
          angular.forEach(items, function(item, index) {
            listItemIds.push(item.listitemid);
          });

          return $http.delete('/list/' + listId + '/item', { 
            headers: {'Content-Type': 'application/json'},
            data: listItemIds 
          }).then(function() {
            // TODO: unfavorite all items if favorites list
            toaster.pop('success', null, 'Successfully deleted ' + items.length + ' from list.');
          }, function() {
            toaster.pop('error', null, 'Error deleting ' + items.length + ' from list.');
          });
        },

        /********************
        LABELS
        ********************/

        // returns array of labels are strings that are found in all lists for the user
        getAllLabels: function() {
          return $http.get('/list/labels').then(function(response) {
            angular.copy(response.data, Service.labels);
            return response.data;
          });
        },

        /********************
        FAVORITES LIST
        ********************/

        getFavoritesList: function() {
          return filter(Service.lists, {isfavorite: true})[0];
        },

        // accepts item number to remove from favorites list
        removeItemFromFavorites: function(itemNumber) {
          var favoritesList = Service.getFavoritesList();
          return Service.getList(favoritesList.listid).then(function() {
            var itemToDelete = filter(favoritesList.items, {itemnumber: itemNumber})[0];

            return Service.deleteItem(itemToDelete.listitemid);
          });
        },

        /********************
        REMINDER/MANDATORY ITEMS LISTS
        ********************/

        getReminderList: function() {
          return Service.getList('84f8a733-fdaf-42b7-9fc1-570aab4e3040');
        }


      };

      return Service;

    }
  ]);