'use strict';

angular.module('bekApp')
    .factory('PhonegapListService', ['$http', '$q', 'ListService', 'localStorageService', 'UserProfileService', 'List', 'UtilityService',
        function($http, $q, ListService, localStorageService, UserProfileService, List, UtilityService) {
            var Service = angular.extend(ListService, {});

            function getBranch() {
                //return UserProfileService.getCurrentBranchId();
                return 'fdf';
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
                    return List.query({
                        branchId: getBranch()
                    }).$promise.then(function(lists) {
                        angular.copy(lists, Service.lists);
                        flagFavoritesList();

                        // TODO: get favorites list items if header param is true
                        console.log('returning all lists');
                        return lists;
                    });
                }

            };

            Service.getList = function(listId) {
                if (navigator.connection.type === 'none') {
                    return Service.findListById(listId);
                } else {
                    console.log('getting list');
                    return List.get({
                        listId: listId,
                        branchId: getBranch()
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
                }
            };

            Service.createList = function(items) {
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

                if (navigator.connection.type === 'none') {
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
                    return List.save({
                        branchId: getBranch()
                    }, newList).$promise.then(function(response) {
                        return Service.getList(response.listitemid);
                    });
                }
            };

            // Service.updateList = function(list) {
            //     if (navigator.connection.type === 'none') {
                    
            //     } else {
            //         return List.update(null, list).$promise.then(function(response) {

            //             // update labels
            //             angular.forEach(list.items, function(item, index) {
            //                 if (item.label && Service.labels.indexOf(item.label) === -1) {
            //                     Service.labels.push(item.label);
            //                 }
            //             });

            //             return Service.getList(response.listid);
            //         });
            //     }
            // };

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
                    return $http.get('/list/' + getBranch() + '/labels').then(function(response) {
                        angular.copy(response.data, Service.labels);
                        console.log('returning all labels');
                        return response.data;
                    });
                }

            };

            Service.getLabelsForList = function(listId) {
                if (navigator.connection.type === 'none') {
                    var localLabels = localStorageService.get("labels");
                } else {
                    console.log('getting labels for lists');
                    return $http.get('/list/' + getBranch() + '/' + listId + '/labels').then(function(response) {
                        return response.data;
                    });
                }

            };

            Service.updateListsFromLocal = function() {
                var localLists = localStorageService.get("lists");
                angular.forEach(localLists, function(list, index) {
                    //need to change this logic in the future
                    if (list.listid === list.name) {
                        delete list.listid;
                        Service.createListFromLocal(list);
                    } else {
                        Service.updateList(list);
                    }
                });

            };

            Service.createListFromLocal = function(newList) {
                return List.save({
                    branchId: getBranch()
                }, newList).$promise.then(function(response) {
                    return Service.getList(response.listitemid);
                });
            };

        }
    ]);