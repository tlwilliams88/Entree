'use strict';

angular.module('bekApp')
    .factory('PhonegapListService', ['$http', '$q', '$filter',
        'ListService', 'localStorageService', 'UserProfileService', 'List', 'UtilityService',
        function($http, $q, $filter, ListService, localStorageService, UserProfileService, List, UtilityService) {

            var originalListService = angular.copy(ListService);

            var Service = angular.extend(ListService, {});

            var filter = $filter('filter');

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

            function getListById(listId) {
                var itemsFound = filter(localStorageService.get("lists"), {
                    listid: listId
                });
                if (itemsFound.length === 1) {
                    return itemsFound[0];
                }
            }

            Service.getAllLists = function() {
                if (navigator.connection.type === 'none') {
                    if (Service.lists) {
                        localStorageService.set('lists', Service.lists);
                    }
                    var localLists = localStorageService.get("lists");
                    angular.copy(localLists, Service.lists);
                    flagFavoritesList();
                    return localLists;
                } else {
                    console.log('getting all lists');
                    return originalListService.getAllLists().then(function(allLists) {
                        localStorageService.set('lists', allLists);
                        return allLists;
                    });

                }

            };

            Service.getList = function(listId) {
                if (navigator.connection.type === 'none') {
                    return Service.findListById(listId);
                } else {
                    console.log('getting list');
                    return originalListService.getList(listId);
                }
            };

            Service.createList = function(items) {
                if (navigator.connection.type === 'none') {
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
                    newList.isNew = true;
                    var localLists = localStorageService.get("lists");
                    newList.listid = newList.name;
                    localLists.push(newList);
                    localStorageService.set('lists', localLists);
                    Service.lists.push(newList);
                    //return a promise
                    var deferred = $q.defer();
                    deferred.resolve(newList);
                    return deferred.promise;
                } else {
                    return originalListService.createList(items).then(function(response) {
                        localStorageService.set('lists', Service.lists);
                        return response;
                    });
                }
            };

            Service.updateList = function(list) {
                if (navigator.connection.type === 'none') {
                    var localLists = localStorageService.get("lists");
                    angular.forEach(localLists, function(item, index) {
                        if (item.listid === list.listid) {
                            list.isUpdated = true;
                            localLists[index] = list;
                            angular.forEach(list.items, function(item, index) {
                                if (item.label && Service.labels.indexOf(item.label) === -1) {
                                    Service.labels.push(item.label);
                                }
                            });
                            localStorageService.set('labels', Service.labels);
                        }
                    });
                    localStorageService.set('lists', localLists);
                    angular.copy(localLists, Service.lists);
                    var deferred = $q.defer();
                    deferred.resolve(list.listid);
                    return deferred.promise;

                } else {
                    return originalListService.updateList(list).then(function(response) {
                        localStorageService.set('lists', Service.lists);
                        return response;
                    });

                }
            };

            Service.deleteList = function(listId) {
                if (navigator.connection.type === 'none') {
                    var localLists = localStorageService.get("lists");

                    angular.forEach(localLists, function(item, index) {
                        if (item.listid === listId) {
                            localLists.splice(index, 1);

                            if (item.isNew) {
                                var isNew = true;
                            }

                            localStorageService.set('lists', localLists);
                            angular.copy(localLists, Service.lists);
                            if (!isNew) {
                                var deletedListGuids = localStorageService.get("deletedListGuids");
                                if (deletedListGuids) {
                                    deletedListGuids.push(listId);
                                    localStorageService.set('deletedListGuids', deletedListGuids);
                                } else {
                                    var deletedArray = [];
                                    deletedArray.push(listId);
                                    localStorageService.set('deletedListGuids', deletedArray);
                                }
                            }
                        }
                    });
                    var deferred = $q.defer();
                    deferred.resolve();
                    return deferred.promise;

                } else {
                    return originalListService.deleteList(listId);
                }
            };

            Service.getAllLabels = function() {
                if (navigator.connection.type === 'none') {
                    if (Service.labels) {
                        localStorageService.set('labels', Service.labels);
                    }
                    var localLabels = localStorageService.get("labels");
                    angular.copy(localLabels, Service.labels);
                    return localLabels;
                } else {
                    console.log('getting all labels');
                    originalListService.getAllLabels().then(function(allLabels) {
                        localStorageService.set('labels', allLabels);
                        return allLabels;
                    });
                }

            };

            Service.getLabelsForList = function(listId) {
                if (navigator.connection.type === 'none') {
                    var localLabels = localStorageService.get("labels");
                } else {
                    console.log('getting labels for lists');
                    return originalListService.getLabelsForList(listId);
                }

            };

            Service.addItem = function(listId, item) {
                if (navigator.connection.type === 'none') {
                    delete item.listitemid;
                    item.position = 0;
                    item.label = null;
                    item.parlevel = null;
                    var updatedList = Service.findListById(listId);
                    if (updatedList && updatedList.items) {
                        updatedList.items.push(item);
                        updatedList.isChanged = true;
                    }
                    localStorageService.set('lists', Service.lists);
                } else {
                    return originalListService.addItem(listId, item);
                }
            };

            Service.addMultipleItems = function(listId, items) {
                if (navigator.connection.type === 'none') {
                    var updatedList = Service.findListById(listId);
                    if (updatedList && updatedList.items) {
                        angular.forEach(items, function(currentItem, index) {
                            delete currentItem.listitemid;
                            currentItem.position = 0;
                            currentItem.label = null;
                            currentItem.parlevel = null;
                            updatedList.items.push(currentItem);
                        });
                        updatedList.isChanged = true;
                        localStorageService.set('lists', Service.lists);
                    }
                } else {
                    originalListService.addMultipleItems(listId, items);
                }
            };

            // this is ready, but not currently used since we have the "Save" button
            // Service.updateItem = function(listId, item) {
            //     if (navigator.connection.type === 'none') {
            //         var updatedList = Service.findListById(listId);
            //         if (updatedList && updatedList.items) {
            //             angular.forEach(updatedList.items, function(updatedListItem, index) {
            //                 if(item.listitemid === updatedListItem.listitemid){
            //                     updatedList.items[index] = item;
            //                     updatedList.isChanged = true;
            //                 }
            //             });
            //         }
            //         localStorageService.set('lists', Service.lists);
            //     } else {
            //         return originalListService.updateItem(listId, item);
            //     }
            // }

            Service.updateListsFromLocal = function() {
                var promises = [];
                var localLists = localStorageService.get("lists");
                angular.forEach(localLists, function(list, index) {
                    if (list.isNew) {
                        delete list.listid;
                        delete list.isNew;
                        promises.push(Service.createListFromLocal(list));
                    }
                    if (list.isChanged) {
                        delete list.isChanged;
                        promises.push(Service.updateList(list).then(null, function(rejection) {
                            console.log(rejection);
                        }));
                    }

                });
                var deletedListGuids = localStorageService.get('deletedListGuids');
                if (deletedListGuids) {
                    promises.push(Service.deleteMultipleLists(deletedListGuids));
                }

                $q.all(promises).then(function() {
                    //update from server and remove deleted array
                    Service.getAllLists();
                    console.log('lists updated!');
                    localStorageService.remove('deletedListGuids');
                });



            };

            Service.createListFromLocal = function(newList) {
                return List.save({}, newList).$promise.then(function(response) {
                    return Service.getList(response.listitemid);
                });
            };

        }
    ]);