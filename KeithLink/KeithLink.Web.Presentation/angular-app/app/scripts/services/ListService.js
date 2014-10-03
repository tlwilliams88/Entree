'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', ['$http', '$filter', 'UserProfileService', 'UtilityService', 'List',
    function($http, $filter, UserProfileService, UtilityService, List) {

      var filter = $filter('filter');

      function updateItemPositions(list) {
        angular.forEach(list.items, function(item, index) {
          item.position = index+1;
        });
      }

      function isFavoritesList(listName) {
        return listName === 'Favorites';
      }

      function doFlagFavoritesList(list) {
        if (isFavoritesList(list.name)) {
          list.isFavoritesList = true;
        }
      }

      function flagFavoritesList() {
        angular.forEach(Service.lists, function(list, index) {
          doFlagFavoritesList(list);
        });
      }

      // updates favorite status of given itemNumber in all lists
      function updateListFavorites(itemNumber, isFavorite) {
        angular.forEach(Service.lists, function(list, listIndex) {
          angular.forEach(list.items, function(item, itemIndex) {
            if (item.itemnumber === itemNumber) {
              item.favorite = isFavorite;
            }
          });
        });
      }

      var Service = {

        lists: [],
        labels: [],

        // accepts "header: true" params to get only list names
        // return array of list objects
        getAllLists: function(params) {
          return List.query({}, params).$promise.then(function(lists) {
            angular.copy(lists, Service.lists);
            flagFavoritesList();

            // TODO: get favorites list items if header param is true
            return lists;
          });
        },

        // accepts listId (guid)
        // returns list object
        getList: function(listId) {
          return List.get({
            listId: listId,
          }).$promise.then(function(list) {
            
            // update new list in cache object
            var existingList = UtilityService.findObjectByField(Service.lists, 'listid', list.listid);

            // flag list if it is the Favorites List, used for display purposes            
            doFlagFavoritesList(list);
            
            if (existingList) {
              var idx = Service.lists.indexOf(existingList);
              angular.copy(list, Service.lists[idx]);
            } else {
              Service.lists.push(list);
            }
  
            return list;
          });
        },

        findListById: function(listId) {
          var itemsFound = filter(Service.lists, {listid: listId});
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
            return Service.getList(response.listitemid);
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

            return Service.getList(response.listid);
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
            return;
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
            
            var updatedList = Service.findListById(listId);
            if (updatedList && updatedList.items) {
              updatedList.items.push(item);
            }

            return response.listitemid;
          });
        },

        // accepts listId (guid) and item object
        updateItem: function(listId, item) {
          return List.updateItem({
            listId: listId
          }, item).$promise.then(function(response) {
            // TODO: add label to Service.labels if it does not exist
            // TODO: replace item in Service.lists
            return response.data;
          });
        },

        // accepts listId and listItemId for item to be deleted
        deleteItem: function(listId, listItemId) {
          return List.deleteItem({
            listId: listId,
            listItemId: listItemId
          }).$promise.then(function(response) {
            var updatedList = Service.findListById(listId);
            // TODO: clean this up
            angular.forEach(updatedList.items, function(item, index) {
              if (item.listitemid === listItemId) {
                updatedList.items.splice(index, 1);
              }
            });

            updateItemPositions(updatedList);
            return;
          });
        },

        /********************
        EDIT MULTIPLE ITEMS
        ********************/

        // accepts listId (guid) and an array of items to add
        // ** Note this does not add duplicate item numbers to a list (10/3/14)
        addMultipleItems: function(listId, items) {
          
          UtilityService.deleteFieldFromObjects(items, ['listitemid', 'position', 'label', 'parlevel']);

          return List.addMultipleItems({
            listId: listId
          }, items).$promise.then(function() {
            return Service.getList(listId);
          });
        },

        // accepts listId (guid) and an array of items
        deleteMultipleItems: function(listId, items) {

          // create array of list item ids
          var listItemIds = [];
          angular.forEach(items, function(item, index) {
            listItemIds.push(item.listitemid);
          });

          console.log(listItemIds);

          return $http.delete('/list/' + listId + '/item', { 
            headers: {'Content-Type': 'application/json'},
            data: listItemIds 
          });
          // return List.deleteMultipleItems({
          //   listId: listId
          // }, listItemIds).$promise;
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

        // accepts listId (guid)
        // returns array of labels as strings that are found in the given list
        getLabelsForList: function(listId) {
          return $http.get('/list/' + listId + '/labels').then(function(response) {
            // TODO: add new labels to Service.labels
            return response.data;
          });
        },

        /********************
        FAVORITES LIST
        ********************/

        getFavoritesList: function() {
          return filter(Service.lists, {isFavoritesList: true})[0];
        },

        // accepts item object
        // returns new item list id
        addItemToFavorites: function(item) {
          var newItem = item,
            favoritesList = Service.getFavoritesList();
          
          // check if item number already exists in favorites list
          var existingItem = filter(favoritesList.items, {itemnumber: item.itemnumber});
          
          // return existing item or add new item to favorites list
          var newListItemId;
          if (existingItem.length === 0) {
            newListItemId = Service.addItem(favoritesList.listid, item).then(function(listitemid) {
              newItem.favorite = true;
              
              // favorite the item in all other lists
              updateListFavorites(newItem.itemnumber, true);

              return listitemid;
            });
          } else {
            newListItemId = existingItem.listitemid;
          }
          return newListItemId;
        },

        // accepts item number to remove from favorites list
        removeItemFromFavorites: function(itemNumber) {
          var favoritesList = Service.getFavoritesList();
          var itemToDelete = filter(favoritesList.items, {itemnumber: itemNumber})[0];

          return Service.deleteItem(favoritesList.listid, itemToDelete.listitemid).then(function() {
            updateListFavorites(itemToDelete.itemnumber, false);
            return;
          });
        },

        addMultipleItemsToFavorites: function(items) {
          var favoritesList = Service.getFavoritesList();
          return Service.addMultipleItems(favoritesList.listid, items).then(function() {
            angular.forEach(items, function(item, index) {
              updateListFavorites(item.itemnumber, true);
            });
          });
        },

        removeMultipleItemsFromFavorites: function(items) {
          var favoritesList = Service.getFavoritesList();

          // find listitemids of items in the Favorites List
          var itemsToRemove = [];
          angular.forEach(items, function(item, index) {
            var itemsFound = $filter('filter')(favoritesList.items, {itemnumber: item.itemnumber});
            if (itemsFound.length > 0) {
              itemsToRemove = itemsToRemove.concat(itemsFound);
            }
          });

          return Service.deleteMultipleItems(favoritesList.listid, itemsToRemove).then(function() {
            angular.forEach(items, function(item, index) {
              updateListFavorites(item.itemnumber, false);
            });
          });
        }


      };

      return Service;

    }
  ]);