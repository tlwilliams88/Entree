'use strict';

angular.module('bekApp')
.factory('PhonegapListService', ['$http', '$q', '$log', 'toaster', 'ListService', 'PhonegapDbService', 'PhonegapLocalStorageService', 'PricingService', 'List',
  function($http, $q, $log, toaster, ListService, PhonegapDbService, PhonegapLocalStorageService, PricingService, List) {

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

    function getAllListDetails(listHeaders) {
      listHeaders.forEach(function(list) {
        originalListService.getListWithItems(list.listid).then(function(listWithItems) {
          PhonegapDbService.setItem(db_table_name_lists, listWithItems.listid, listWithItems);
        });
      });
    }

    Service.getAllListsForOffline = function() {
      var listPromise = PhonegapDbService.dropTable(db_table_name_lists)
        .then(originalListService.getListHeaders)
        .then(getAllListDetails);
      var labelsPromise = originalListService.getAllLabels()
        .then(PhonegapLocalStorageService.setLabels);

      return $q.all([
        listPromise,
        labelsPromise
      ]);
    };

    Service.getListHeaders = function() {
      if (navigator.connection.type === 'none') {
        $log.debug('getting lists from DB');
        return PhonegapDbService.getAllItems(db_table_name_lists).then(function(listHeaders) {
          angular.copy(listHeaders, Service.lists);
          return listHeaders;
        });
      } else {
        $log.debug('getting all lists from server');
        return originalListService.getListHeaders();
      }
    };

    Service.getList = function(listId, params) {
      if (navigator.connection.type === 'none') {
        return PhonegapDbService.getItem(db_table_name_lists, listId).then(function(list) {
          PricingService.updateCaculatedFields(list.items);
          Service.updateListPermissions(list);
          
          // calculate item count
          var notDeletedItemCount = 0;
          angular.forEach(list.items, function(item, index) {
            if (item.name && !item.isdeleted) {
              notDeletedItemCount +=1;
            }
          });
          list.itemCount = notDeletedItemCount;

          return list;
        });
      } else {
        return originalListService.getList(listId, params);
      }
    };

    Service.remapItems = function(item) {
      delete item.parlevel;
      delete item.label;
      delete item.listitemid;
      item.position = 0;
      return item;
    };

    Service.createList = function(items, params) {
      if (navigator.connection.type === 'none') {
        
        // create new list object
        var newList = originalListService.beforeCreateList(items, params);
        newList.isNew = true;
        newList.listid = generateId();

        newList.items.forEach(function(item) {
          item.listitemid = generateId();
          item.isNew = true;
          item.editPosition = 0;
        });
        
        // persist new list
        PhonegapDbService.setItem(db_table_name_lists, newList.listid, newList);
        Service.lists.push(newList);  

        Service.renameList = true;      

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
          list.isChanged = true;
        }
        
        // flag new items and give them a temp id 
        list.items.forEach(function(item) {
          if (!item.listitemid && item.name) {
            item.listitemid = generateId();
            item.isNew = true;
          }
          if(item.isdeleted){
            list.itemCount -= 1;
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

        toaster.pop('success', null, 'Successfully saved list ' + list.name);
        var deferred = $q.defer();
        deferred.resolve(list);
        return deferred.promise;
      } else {
        return originalListService.updateList(list);

      }
    };

    Service.deleteList = function(listId) {
      if (navigator.connection.type === 'none') {
        $log.debug('deleting list offline');          

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
      $log.debug('getting labels');
        
      if (navigator.connection.type === 'none') {
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

    Service.addItem = function(listId, item) {
      if (navigator.connection.type === 'none') {
        var deferred = $q.defer();

        delete item.listitemid;
        item.position = 0;
        item.label = null;
        item.parlevel = null;
        item.editPosition = 0;
        item.isNew = true;
        item.listitemid = generateId();
        
        Service.getList(listId).then(function(updatedList) {
          if (updatedList) {
            updatedList.items.push(item);
            updatedList.isChanged = true;
          }

          deferred.resolve(item);
          
          PhonegapDbService.setItem(db_table_name_lists, listId, updatedList);
        });

        return deferred.promise;
      } else {
        return originalListService.addItem(listId, item);
      }
    };

    Service.deleteItem = function(listItemId) {
      if (navigator.connection.type === 'none') {
        var deferred = $q.defer();

        // loop through all lists and items and find the item to delete
        Service.lists.forEach(function(listHeader) {
          PhonegapDbService.getItem(db_table_name_lists, listHeader.listid).then(function(dbList) {
            var indexToDelete;
            dbList.items.forEach(function(listItem, index) {
              if (listItem.listitemid === listItemId) {
                indexToDelete = index;
              }
            });

            if (indexToDelete !== undefined) {
              deferred.resolve();
              $log.debug('deleting item');
              dbList.isChanged = true;
              dbList.items[indexToDelete].isdeleted = true;
              PhonegapDbService.setItem(db_table_name_lists, dbList.listid, dbList);
            }
          });
        });

        return deferred.promise;
      } else {
        return originalListService.deleteItem(listItemId);
      }
    };

    // update DB list items favorite flag
    function updateListItemFavorites(itemNumber, isFavorite) {
      Service.lists.forEach(function(listHeader) {
        PhonegapDbService.getItem(db_table_name_lists, listHeader.listid).then(function(dbList) {
          dbList.items.forEach(function(listItem) {
            if (listItem.itemnumber === itemNumber) {
              listItem.favorite = isFavorite;
            }
          });
          PhonegapDbService.setItem(db_table_name_lists, dbList.listid, dbList);
        });
      });
    }

    Service.addItemToFavorites = function(item) {
      if (navigator.connection.type === 'none') {
        return Service.addItem(Service.getFavoritesList().listid, item, true).then(function() {
          updateListItemFavorites(item.itemnumber, true);
        });
      } else {
        return originalListService.addItemToFavorites(item);
      }
    };

    Service.removeItemFromFavorites = function(itemNumber) {
      if (navigator.connection.type === 'none') {

        var deferred = $q.defer();
        var favoritesList = Service.getFavoritesList();
        PhonegapDbService.getItem(db_table_name_lists, favoritesList.listid).then(function(list) {
          var listItemIdToDelete;
          list.items.forEach(function(item) {
            if (item.itemnumber === itemNumber) {
              listItemIdToDelete = item.listitemid;
            }
          });

          if (listItemIdToDelete) {
            Service.deleteItem(listItemIdToDelete).then(function() {
              deferred.resolve();
              updateListItemFavorites(itemNumber, false);
            });
          } else {
            $log.debug('No item found with item number.');
            deferred.resolve();
          }
        });

        return deferred.promise;
      } else {
        return originalListService.removeItemFromFavorites(itemNumber);
      }
    };

    Service.addMultipleItems = function(listId, items) {
      if (navigator.connection.type === 'none') {
        var deferred = $q.defer();

        var newItems = [];
        items.forEach(function(item) {
          delete item.isSelected;
          newItems.push(item);
        });

        var updatedList = Service.findListById(listId);
        if (updatedList) {
          updatedList.items = updatedList.items.concat(newItems);
          updatedList.isChanged = true;
        }
        // TEST: does this update Service.lists?

        PhonegapDbService.setItem(db_table_name_lists, listId, updatedList);
        deferred.resolve(updatedList);
        return deferred.promise;
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

    Service.getCriticalItemsLists = function() {
      
      if (navigator.connection.type === 'none') {
        var deferred = $q.defer();
        PhonegapDbService.getAllItems(db_table_name_lists).then(function(lists) {
          
          // find mandatory and reminder lists
          var criticalItems = [];
          lists.forEach(function(list) {
            if (list.ismandatory) {
              criticalItems.push(list);
            } else if (list.isreminder) {
              criticalItems.push(list);
            }
          });

          deferred.resolve(criticalItems);
        }, function(err) {
          deferred.reject(err);
        });
        return deferred.promise;
      } else {
        return originalListService.getCriticalItemsLists();
      }
    };

    Service.updateListsFromLocal = function() {
      $log.debug('updating lists after back online');
      
      PhonegapDbService.getAllItems(db_table_name_lists).then(function(storedLists) {
       
        var promises = [];
        angular.forEach(storedLists, function(list, index) {
          
          if (list.isNew) { // create lists
            
            var newItems = [];
            list.items.forEach(function(item) {
              if (item.itemnumber) {
                newItems.push({
                  itemnumber: item.itemnumber,
                  position: item.position ? item.position : 0,
                  label: item.label ? item.label : null,
                  parlevel: item.parlevel ? item.parlevel : null
                });
              }
            });
            var newList = {
              name: list.name,
              items: newItems
            };
            promises.push(Service.createListFromLocal(newList));
          }
          else if (list.isChanged) { // update lists
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

        
        $q.all(promises).then(function() {
          $log.debug('lists updated!');
          
          //update from server and remove deleted array
          Service.getAllListsForOffline();
          
          PhonegapLocalStorageService.removeDeletedListGuids();
        }, function() {
          $log.debug('error updating lists');
        });
      });
    };

    Service.createListFromLocal = function(newList) {
      return List.save({}, newList).$promise;
    };

    return Service;
  }
]);