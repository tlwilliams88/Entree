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

    var Service = {
      lists: [],
      favoritesList: {},
      labels: [],

      getAllLists: function(requestParams) {
        return $http.get('/list/' + getBranch(), {
          params: requestParams
        }).then(function(response) {
          angular.copy(response.data, Service.lists);
          setFavoritesList();
          return response.data;
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

        var date = new Date(),
        dateString = date.getTime(),
        newList = {
          name: 'New List ' + dateString,
          items: items
        };

        return $http.post('/list/' + getBranch(), newList).then(function(response) {
          newList.listid = response.data.listitemid;
          Service.lists.push(newList);
          return response.data; // return listId
        });
      },

      createListWithItem: function(item) {
        var items = [item];

        return $q.all([
          this.createList(items),
          this.addItemToFavorites(item)
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
          item.isEditing = false;

          // add label to list of labels if it is new
          if (Service.labels.indexOf(item.label) === -1) {
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
        
        var newFavoritesListItemId;

        if (!existingItem) {
          newFavoritesListItemId = addItemToList(Service.favoritesList.listid, item).then(function(response) {
            var newListItemId = response.listitemid;

            newItem.listitemid = newListItemId;
            newItem.favorite = true;
            if (Service.favoritesList.items) {
              Service.favoritesList.items.push(newItem);
            }

            return newListItemId;
          });
        } else {
          newFavoritesListItemId = existingItem.listitemid;
        }
        return newFavoritesListItemId;
      },

      removeItemFromFavorites: function(itemNumber) {
        var removedItem, removedIndex;
        angular.forEach(Service.favoritesList.items, function(item, index) {
          if (item.itemnumber === itemNumber) {
            removedItem = item;
            removedIndex = index;
          }
        });

        return this.deleteItem(Service.favoritesList.listid, removedItem.listitemid).then(function(response) {
          Service.favoritesList.items.splice(removedIndex, 1);

          return response.data;
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