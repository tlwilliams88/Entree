'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', ['$http', '$q', 'UserProfileService', function ($http, $q, UserProfileService) {

    function getBranch() {
      return UserProfileService.getCurrentLocation().branchId;
    }

    function setFavoritesList() {

      var listFound;
      angular.forEach(Service.lists, function(list, index) {
        if (list.name === 'Favorites') {
          listFound = list;
        }
      });

      listFound.isFavoritesList = true;
      angular.copy(listFound, Service.favoritesList);

      if (!Service.favoritesList.items) {
        Service.getList(Service.favoritesList.listid).then(function (response) {
          Service.favoritesList.items = response.items;
        });
      }
    }

    function addItemToList(listId, item) {
      return $http.post('/list/' + listId + '/item', item).then(function(response) {
        item.listitemid = response.data.listitemid;
        var updatedList = Service.findListById(listId);
        if (updatedList && updatedList.items){
          updatedList.items.push(item);
        }
        return response.data;
      });
    }

    function isUsedName(listNames, name, number) {
      return listNames.indexOf(name + ' ' + number) > -1;
    }

    function generateNewListName() {
      var name = 'New List',
        number = 0;

      var listNames = [];

      angular.forEach(Service.lists, function(list, index) {
        listNames.push(list.name);
      });

      var isNameUsed = isUsedName(listNames, name, number);
      while (isNameUsed) {
        number++;
        isNameUsed = isUsedName(listNames, name, number);
      }

      return name + ' ' + number;
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
      favoritesList: {},
      labels: [],

      getAllLists: function(requestParams) {
        return $http.get('/list/' + getBranch(), {
          params: requestParams
        }).then(function(response) {

          var returnedLists = response.data;

          angular.copy(returnedLists, Service.lists);
          setFavoritesList();
          return returnedLists;
        });
      },

      getList: function(listId) {
        return $http.get('/list/' + getBranch() + '/' + listId).then(function(response) {
          return response.data;
        });
      },

      getAllLabels: function() {
        return $http.get('/list/' + getBranch() + '/labels').then(function(response) {
          angular.copy(response.data, Service.labels);
          return response.data;
        });
      },

      getLabelsForList: function(listId) {
        return $http.get('/list/' + getBranch() + '/' + listId + '/labels').then(function(response) {
          return response.data;
        });
      },

      createList: function(items) {
        if (!items) {
          items = [];
        }

        var newList = {
          name: generateNewListName(),
          items: items
        };

        return $http.post('/list/' + getBranch(), newList).then(function(response) {
          newList.listid = response.data.listitemid;
          Service.lists.push(newList);
          return newList; // return listId
        });
      },

      createListWithItem: function(item) {
        var items = [item];

        return $q.all([
          Service.createList(items),
          Service.addItemToFavorites(item)
        ]);
      },

      deleteList: function(listId) {
        return $http.delete('/list/' + listId).then(function(response) {
          var deletedList = Service.findListById(listId);
          var idx = Service.lists.indexOf(deletedList);
          if (idx > -1) {
            Service.lists.splice(idx, 1);
          }
          return response.data;
        });
      },

      addItemToListAndFavorites: function(listId, item) {
        return $q.all([
          this.addItemToFavorites(item),
          addItemToList(listId, item)
        ]);
      },

      updateItem: function(listId, item) {
        return $http.put('/list/' + listId + '/item', item).then(function(response) {

          // add label to list of labels if it is new
          if (item.label && Service.labels.indexOf(item.label) === -1) {
            Service.labels.push(item.label);
          }

          return response.data;
        });
      },

      deleteItem: function(listId, listItemId) {
        // TODO: sometimes reloads all listitemids but this is inconsistent
        return $http.delete('/list/' + listId + '/item/' + listItemId).then(function(response) {
          var updatedList = Service.findListById(listId);
          angular.forEach(updatedList.items, function(item, index) {
            if (item.listitemid === listItemId) {
              updatedList.items.splice(index, 1);
            }
          });
          return response.data;
        });
      },

      updateList: function(list) {
        return $http.put('/list', list).then(function(response) {
          var updatedList = Service.findListById(list.listid);
          var idx = Service.lists.indexOf(updatedList);
          Service.lists[idx] = list;
        });
      },

      addItemToFavorites: function(item) {
        var newItem = item;

        // check if item number already exists in favorites list
        var existingItem;
        angular.forEach(Service.favoritesList.items, function(item, index) {
          if (item.itemnumber === newItem.itemnumber) {
            existingItem = item;
          }
        });
        
        // return existing item or add new item to favorites list
        var newFavoritesListItemId;
        if (!existingItem) {
          newFavoritesListItemId = addItemToList(Service.favoritesList.listid, item).then(function(response) {
            var newListItemId = response.listitemid;

            newItem.listitemid = newListItemId;
            newItem.favorite = true;
            if (Service.favoritesList.items) {
              Service.favoritesList.items.push(newItem);
            }

            // favorite the item in all other lists
            updateListFavorites(newItem.itemnumber, true);

            return newListItemId;
          });
        } else {
          newFavoritesListItemId = existingItem.listitemid;
        }
        return newFavoritesListItemId;
      },

      removeItemFromFavorites: function(itemNumber) {

        var removedItem, removedIndex;
        
        var updatedFavoritesList = angular.copy(Service.favoritesList);

        var newPosition = 1;
        angular.forEach(updatedFavoritesList.items, function(item, index) {
          if (item.itemnumber === itemNumber) {
            // find deleted item in the list
            removedItem = item;
            removedIndex = index;
          } else {
            // update positions of remaining items
            item.position = newPosition;
            newPosition++;
          }
        });        
        updatedFavoritesList.items.splice(removedIndex, 1);

        return this.updateList(updatedFavoritesList).then(function(response) {
          angular.copy(updatedFavoritesList, Service.favoritesList);

          // unfavorite the item in all other lists
          updateListFavorites(removedItem.itemnumber, false);
        });
      },

      findListById: function(listId) {
        var listFound;
        angular.forEach(Service.lists, function(list, index) {
          if (list.listid === listId) {
            listFound = list;
          }
        });

        return listFound;
      },

      findItemInList: function(listItemId, items) {
        var itemFound;
        angular.forEach(items, function(item, index) {
          if (item.listitemid === listItemId) {
            itemFound = item;
          }
        });
        return itemFound;
      }

    };

    return Service;
 
  }]);