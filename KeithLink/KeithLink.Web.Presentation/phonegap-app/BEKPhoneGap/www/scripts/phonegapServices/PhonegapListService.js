'use strict';

angular.module('bekApp')
.factory('PhonegapListService', ['$http', '$q', 'ListService', 'PhonegapDbService', 'PhonegapLocalStorageService', 'UserProfileService', 'PricingService', 'List', 'UtilityService',
  function($http, $q, ListService, PhonegapDbService, PhonegapLocalStorageService, UserProfileService, PricingService, List, UtilityService) {

    var originalListService = angular.copy(ListService);

    var Service = angular.extend(ListService, {});

    var db_table_name_lists = 'lists';

    function generateId() {
      return 'a' + Math.floor((1 + Math.random()) * 0x10000).toString(16); // generate 5 digit id
    }

    function updateCachedLists(updatedList) {
      Service.lists.forEach(function(list, index) {
        if (list.listid === updatedList.listid) {
          Service.lists[index] = updatedList;
        }
      });
    }

    Service.getAllLists = function() {
      if (navigator.connection.type === 'none') {
        console.log('getting lists from DB');
        // TEST: keep db lists up to date while online
        // if (Service.lists) {
        //   localStorageService.set('labels', Service.labels);
        // }
        return PhonegapDbService.getAllItems(db_table_name_lists).then(function(data) {
          angular.copy(data, Service.lists);
          debugger;
          return data;
        });
      } else {
        console.log('getting all lists from server');
        return originalListService.getAllLists();
      }
    };

    Service.getList = function(listId) {
      if (navigator.connection.type === 'none') {
        var list = originalListService.findListById(listId);
        debugger;

        PricingService.updateCaculatedFields(list.items);
        Service.updateListPermissions(list);
        return list;
      } else {
        console.log('getting list');
        return originalListService.getList(listId);
      }
    };

    Service.createList = function(items, params) {
      if (navigator.connection.type === 'none') {
        
        // create new list object
        var newList = originalListService.beforeCreateList(items, params);
        newList.isNew = true;
        newList.listid = generateId();
        
        // persist new list
        PhonegapDbService.setItem(db_table_name_lists, newList.listid, newList);
        Service.lists.push(newList);        

        //return a promise
        var deferred = $q.defer();
        deferred.resolve(newList);
        return deferred.promise;
      } else {
        return originalListService.createList(items, params);
      }
    };

    Service.updateList = function(list) {
      if (navigator.connection.type === 'none') {
        if (!list.isNew) {
          list.isUpdated = true;
        }

        list.items.forEach(function(item) {
          if (!item.listitemid) {
            item.listitemid = generateId();
            item.isNew = true;
          }
        });

        PhonegapDbService.setItem(db_table_name_lists, list.listid, list);
        updateCachedLists(list); // update Service.lists

        // update labels in local storage
        angular.forEach(list.items, function(item, index) {
          if (item.label && Service.labels.indexOf(item.label) === -1) {
            Service.labels.push(item.label);
          }
        });
        PhonegapLocalStorageService.setLabels(Service.labels);

        var deferred = $q.defer();
        deferred.resolve(list);
        return deferred.promise;
      } else {
        return originalListService.updateList(list);

      }
    };

    Service.deleteList = function(listId) {
      if (navigator.connection.type === 'none') {
        debugger;

        Service.lists.forEach(function(list, index) {
          if (list.listid === listId) {
            Service.lists.splice(index, 1);
            PhonegapDbService.removeItem(db_table_name_lists, listId);

            // add listId to list of deleted lists for deleting on the server when back online
            var deletedListGuids = PhonegapLocalStorageService.getDeletedListGuids();
            if (!deletedListGuids) {
              deletedListGuids = [];
            }
            if (!list.isNew) { // lists created while offline don't need to be sent to the server
              deletedListGuids.push(listId);
              PhonegapLocalStorageService.setDeletedListGuids(deletedListGuids);
            }
          }
        });

        var deferred = $q.defer();
        deferred.resolve(originalListService.getFavoritesList());
        return deferred.promise;

      } else {
        return originalListService.deleteList(listId);
      }
    };

    Service.getAllLabels = function() {
      if (navigator.connection.type === 'none') {
        debugger;
        if (Service.labels) {
          PhonegapLocalStorageService.setLabels(Service.labels);
        }
        var localLabels = PhonegapLocalStorageService.getLabels();
        angular.copy(localLabels, Service.labels);
        return localLabels;
      } else {
        return originalListService.getAllLabels();
      }

    };

    // NOT USED 3/2/15
    // Service.getLabelsForList = function(listId) {
    //   if (navigator.connection.type === 'none') {
    //     var localLabels = localStorageService.get("labels");
    //   } else {
    //     console.log('getting labels for lists');
    //     return originalListService.getLabelsForList(listId);
    //   }

    // };

    Service.addItem = function(listId, item) {
      if (navigator.connection.type === 'none') {
        
        delete item.listitemid;
        item.position = 0;
        item.label = null;
        item.parlevel = null;
        item.editPosition = 0;
        item.isNew = true;
        item.listitemid = generateId();

        
        var updatedList = Service.findListById(listId);
        if (updatedList) {
          updatedList.items.push(item);
          updatedList.isChanged = true;
        }

        PhonegapDbService.setItem(db_table_name_lists, listId, updatedList);

        return item;
      } else {
        return originalListService.addItem(listId, item);
      }
    };

    Service.addMultipleItems = function(listId, items) {
      if (navigator.connection.type === 'none') {

        UtilityService.deleteFieldFromObjects(items, ['listitemid', 'position', 'label', 'parlevel']);

        var updatedList = Service.findListById(listId);
        if (updatedList) {
          updatedList.concat(items);
          updatedList.isChanged = true;
        }
        // TEST: does this update Service.lists?
        debugger;

        PhonegapDbService.setItem(db_table_name_lists, listId, updatedList);

      } else {
        originalListService.addMultipleItems(listId, items);
      }
    };

    // NOT USED and outdated 3/2/15
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
      debugger;
      
      PhonegapDbService.getAllItems(db_table_name_lists).then(function(storedLists) {
        debugger;
        var promises = [];
        angular.forEach(storedLists, function(list, index) {
          // create lists
          if (list.isNew) {
            delete list.listid;
            delete list.isNew;
            promises.push(originalListService.createList(list));
          }

          // update lists
          if (list.isChanged) {
            delete list.isChanged;
            list.items.forEach(function(item) {
              if (item.isNew) {
                delete item.listitemid;
              }
            });
            promises.push(originalListService.updateList(list));
          }
        });

        // delete lists
        var deletedListGuids = PhonegapLocalStorageService.getDeletedListGuids();
        if (deletedListGuids) {
          promises.push(Service.deleteMultipleLists(deletedListGuids));
        }

        debugger;
        $q.all(promises).then(function() {
          //update from server and remove deleted array
          Service.getAllLists();
          console.log('lists updated!');
          PhonegapLocalStorageService.removeDeletedListGuids();
        }, function() {
          console.log('error updating lists');
        });
      });
    };

    Service.createListFromLocal = function(newList) {
      return List.save({}, newList).$promise.then(function(response) {
        return Service.getList(response.listitemid);
      });
    };

    return Service;
  }
]);