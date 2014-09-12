'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
    .factory('ListService', ['$http', '$q', 'UserProfileService', 'NameGeneratorService', 'List',
        function($http, $q, UserProfileService, NameGeneratorService, List) {

            function getBranch() {
                return UserProfileService.getCurrentBranchId();
            }

            function addItemToList(listId, item) {
                delete item.listitemid;
                item.position = 0;
                item.label = null;
                item.parlevel = 0;

                return List.addItem({
                    listId: listId
                }, item).$promise.then(function(response) {
                    item.listitemid = response.listitemid;
                    item.editPositioin = 0;
                    item.editLabel = null;
                    item.editParlevel = 0;

                    var updatedList = Service.findListById(listId);
                    if (updatedList && updatedList.items) {
                        updatedList.items.push(item);
                    }

                    return response;
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
                favoritesList: {},
                labels: [],

                setFavoritesList: function() {
                    var listFound;
                    angular.forEach(Service.lists, function(list, index) {
                        if (list.name === 'Favorites') {
                            listFound = list;
                        }
                    });

                    if (listFound) {
                        listFound.isFavoritesList = true;
                        angular.copy(listFound, Service.favoritesList);

                        if (!Service.favoritesList.items) {
                            Service.getList(Service.favoritesList.listid).then(function(response) {
                                Service.favoritesList.items = response.items;
                            });
                        }
                    }
                },

                getAllLists: function(requestParams) {
                    return List.query({
                        branchId: getBranch()
                    }).$promise.then(function(lists) {
                        angular.copy(lists, Service.lists);
                        Service.setFavoritesList();
                        console.log(lists);
                        return lists;
                    });
                },

                getList: function(listId) {
                    return List.get({
                        listId: listId,
                        branchId: getBranch()
                    }).$promise.then(function(list) {
                        console.log(list);
                        return list;
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
                    } else {
                        angular.forEach(items, function(item, index) {
                            delete item.listitemid;
                        });
                    }

                    var newList = {
                        name: NameGeneratorService.generateName('List', Service.lists),
                        items: items
                    };

                    return List.save({
                        branchId: getBranch()
                    }, newList).$promise.then(function(response) {
                        
                        return Service.getList(response.listitemid).then(function(list) {
                            Service.lists.push(list); 
                            return list;
                        });
                        // return newList;
                    });
                },

                createListWithItem: function(item) {
                    delete item.listitemid;
                    var items = [item];

                    return $q.all([
                        Service.createList(items),
                        Service.addItemToFavorites(item)
                    ]);
                },

                deleteList: function(listId) {
                    return List.delete({
                        listId: listId
                    }).$promise.then(function(response) {
                        var deletedList = Service.findListById(listId);
                        var idx = Service.lists.indexOf(deletedList);
                        if (idx > -1) {
                            Service.lists.splice(idx, 1);
                        }
                        return response;
                    });
                },

                addItemToListAndFavorites: function(listId, item) {
                    return $q.all([
                        this.addItemToFavorites(item),
                        addItemToList(listId, item)
                    ]);
                },

                /*updateItem: function(listId, item) {
        return $http.put('/list/' + listId + '/item', item).then(function(response) {

          // add label to list of labels if it is new
          if (item.label && Service.labels.indexOf(item.label) === -1) {
            Service.labels.push(item.label);
          }

          return response.data;
        });
      },*/

                deleteItem: function(listId, listItemId) {
                    return List.deleteItem({
                        listId: listId,
                        listItemId: listItemId
                    }).$promise.then(function(response) {
                        var updatedList = Service.findListById(listId);
                        angular.forEach(updatedList.items, function(item, index) {
                            if (item.listitemid === listItemId) {
                                updatedList.items.splice(index, 1);
                            }
                        });
                        return response;
                    });
                },

                updateList: function(list) {
                    return List.update(null, list).$promise.then(function(response) {

                        angular.forEach(list.items, function(item, index) {
                            if (item.label && Service.labels.indexOf(item.label) === -1) {
                                Service.labels.push(item.label);
                            }
                        });

                        var updatedList = Service.findListById(list.listid);
                        var idx = Service.lists.indexOf(updatedList);
                        angular.copy(list, Service.lists[idx]);
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

        }
    ]);