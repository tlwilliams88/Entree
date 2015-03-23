'use strict';

angular.module('bekApp')
.factory('PhonegapListService', ['$http', '$q', 'ListService', 'PhonegapDbService', 'PhonegapLocalStorageService', 'PricingService', 'List',
  function($http, $q, ListService, PhonegapDbService, PhonegapLocalStorageService, PricingService, List) {

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
        return PhonegapDbService.getAllItems(db_table_name_lists).then(function(data) {
          angular.copy(data, Service.lists);
          return data;
        });
      } else {
        console.log('getting all lists from server');
        return originalListService.getAllLists();
      }
    };

    Service.getList = function(listId) {
      if (navigator.connection.type === 'none') {
        return PhonegapDbService.getItem(db_table_name_lists, listId).then(function(list) {
          PricingService.updateCaculatedFields(list.items);
          Service.updateListPermissions(list);
          var notDeletedItemCount = 0;
            angular.forEach(list.items, function(item, index) {
          if (item.name||!item.isdeleted) {
           notDeletedItemCount +=1;
          }
        });
          list.itemCount = notDeletedItemCount;          
          return list;
        });
      } else {
        return originalListService.getList(listId);
      }
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
        var deletedCount =0;
        // flag new items and give them a temp id 
        list.items.forEach(function(item) {
          if (!item.listitemid && item.name) {
            item.listitemid = generateId();
            item.isNew = true;
          }
          if(item.isdeleted){
            list.itemCount -=1;
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
        console.log('deleting list offline');          

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
      console.log('getting labels');
        
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

    // NOT USED? 3/10/15
    Service.addMultipleItems = function(listId, items) {
      if (navigator.connection.type === 'none') {

        var newItems = [];
        items.forEach(function(item) {
          newItems.push({
            itemnumber: item.itemnumber
          });
        });

        var updatedList = Service.findListById(listId);
        if (updatedList) {
          updatedList.concat(newItems);
          updatedList.isChanged = true;
        }
        // TEST: does this update Service.lists?
       

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
      console.log('updating lists after back online');
      
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
          console.log('lists updated!');
          
          //update from server and remove deleted array
          PhonegapDbService.dropTable(db_table_name_lists)
            .then(Service.getAllLists);
          
          PhonegapLocalStorageService.removeDeletedListGuids();
        }, function() {
          console.log('error updating lists');
        });
      });
    };

    Service.createListFromLocal = function(newList) {
      return List.save({}, newList).$promise;
    };

    return Service;
  }
]);