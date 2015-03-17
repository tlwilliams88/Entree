'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', ['$http', '$q', '$filter', '$upload', 'toaster', 'UtilityService', 'ExportService', 'PricingService', 'List',
    function($http, $q, $filter, $upload, toaster, UtilityService, ExportService, PricingService, List) {

      function updateItemPositions(list) {
        angular.forEach(list.items, function(item, index) {
          item.position = index+1;
        });
      }

      /*
      VALID PERMISSIONS
      canEditList             -- save changes, cancel changes
      specialDisplay          -- in a list of lists this will be hidden (context menu, multi select menu on list page)
      canReorderItems         -- can use the drag to rearrage or reorder list items
      canDeleteList
      ??canCreateList
      canDeleteItems          -- can remove items from list
      canAddItems             -- can add items via drag/drop and context menu
      canRenameList           -- 
      canSeeLabels            -- show label column
      canEditLabels           -- read only label
      canSeeParlevel          -- show parlevel column
      canEditParlevel         -- read only parlevel
      alternativeParHeader    -- different label for parlevel field (ex. Required Qty for Mandatory list)
      alternativeFieldName    -- an additional JSON field that is only used for certain types of lists (ex. category for contract list)
      alternativeFieldHeader  -- header display text for the additinal field
      canShareList
      canCopyList
      */

      function updateListPermissions(list) {
        var permissions = {};

        // FAVORITES
        if (list.isfavorite) {
          permissions.canEditList = true;
          permissions.canDeleteItems = true;
          permissions.canAddItems = true;
          permissions.specialDisplay = true;
          permissions.canReorderItems = true;

        // CONTRACT
        } else if (list.is_contract_list) {
          permissions.alternativeFieldName = 'category';
          permissions.alternativeFieldHeader = 'Category';

        // WORKSHEET
        } else if (list.isworksheet) {

        // RECOMMENDED
        } else if (list.isrecommended) {
          permissions.canEditList = true;
          permissions.canDeleteList = true;
          permissions.canAddItems = true;
          permissions.canDeleteItems = true;
          permissions.canDeleteList = true;
          permissions.canReorderItems = true;

        // MANDATORY -- only editable by internal users
        } else if (list.ismandatory) {
          permissions.canSeeParlevel = true;
          permissions.alternativeParHeader = 'Required Qty';
          permissions.canDeleteList = true;
          permissions.canAddItems = true;
          permissions.canEditList = true;
          permissions.canDeleteItems = true;
          permissions.canEditParlevel = true;
          permissions.canDeleteList = true;

        // REMINDER
        } else if (list.isreminder) {
          permissions.canEditList = true;
          permissions.canAddItems = true;
          permissions.canDeleteItems = true;
          permissions.canReorderItems = true;
        
        // CUSTOM LISTS (only these can be shared/copied)
        } else {

          // SHARED WITH ME
          if (list.isshared) {
            permissions.canSeeLabels = true;
            permissions.canSeeParlevel = true;            

          // OWNER OF LIST
          } else {
            permissions.canEditList = true;
            permissions.canDeleteList = true;
            permissions.canDeleteItems = true;
            permissions.canAddItems = true;
            permissions.canRenameList = true;
            permissions.canSeeLabels = true;
            permissions.canEditLabels = true;
            permissions.canSeeParlevel = true;
            permissions.canEditParlevel = true;
            permissions.canShareList = true;
            permissions.canCopyList = true;
            permissions.canReorderItems = true;
          }
        }

        // overwrite read only lists
        if (list.read_only) {
          permissions.canEditList = false;
          permissions.canReorderItems = false;
          permissions.canDeleteList = false;
          permissions.canDeleteItems = false;
          permissions.canAddItems = false;
          permissions.canRenameList = false;
          permissions.canEditLabels = false;
          permissions.canEditParlevel = false;
          permissions.canShareList = false;
          permissions.canCopyList = false;
        }

        list.permissions = permissions;        
      }

      var Service = {

        lists: [],
        labels: [],

        updateListPermissions: updateListPermissions,

        eraseCachedLists: function() {
          Service.lists = [];
          Service.labels = [];
        },

        updateCache: function(list) {
          // update new list in cache object
          var cacheList = angular.copy(list);
          cacheList.items = null;

          var existingList = UtilityService.findObjectByField(Service.lists, 'listid', cacheList.listid);
          if (existingList) {
            var idx = Service.lists.indexOf(existingList);
            angular.copy(cacheList, Service.lists[idx]);
          } else {
            Service.lists.push(cacheList);
          }
        },

        // accepts "header: true" params to get only list names
        // return array of list objects
        getAllLists: function(params) {
          if (!params) {
            params = {};
          }
          return List.query(params).$promise.then(function(lists) {
            lists.forEach(function(list) {
              updateListPermissions(list);
            });
            angular.copy(lists, Service.lists);
            return lists;
          });
        },

        getListHeaders: function() {
          return Service.getAllLists({ header: true });
        },

        // accepts listId (guid)
        // returns list object
        getListWithItems: function(listId) {
          return $http.get('/list/' + listId).then(function(response) {
            var list = response.data;
            if (!list) {
              return $q.reject('No list found.');
            }
            PricingService.updateCaculatedFields(list.items);
            updateListPermissions(list);

            Service.updateCache(list);

            return list;
          });
        },

        // accepts listId (guid), paging params
        // returns paged list object
        getList: function(listId, params) {
          if (!params) {
            params = {
              size: 30,
              from: 0
            };
          }
          return $http.post('/list/' + listId, params).then(function(response) {
            var list = response.data;
            if (!list) {
              return $q.reject('No list found.');
            }
            
            // transform paged data
            list.itemCount = list.items.totalResults;
            list.items = list.items.results;

            // get calculated fields
            PricingService.updateCaculatedFields(list.items);
            updateListPermissions(list);

            Service.updateCache(list);

            return list;
          });
        },

        findListById: function(listId) {
          if (!isNaN(parseInt(listId))) {
            listId = parseInt(listId);
          }
          return UtilityService.findObjectByField(Service.lists, 'listid', listId);
        },

        /********************
        EXPORT
        ********************/

        getExportConfig: function(listId) {
          return List.exportConfig({
            listId: listId
          }).$promise;
        },

        exportList: function(config, listId) {
          ExportService.export('/list/export/' + listId, config);
        },

        printBarcodes: function(listId) {
          var promise = $http.get('/list/barcode/' + listId, {
            responseType: 'arraybuffer'
          });
          return ExportService.print(promise);
        },

        /********************
        EDIT LIST
        ********************/
        
        beforeCreateList: function(items, params) {
          if (!params) {
            params = {};
          }

          var newList = {};

          var newItems = [];
          if (Array.isArray(items)) { // if multiple items
            newItems = items;
          } else if (typeof items === 'object') { // if one item
            newItems = [items];
          }

          newList.items = newItems.map(function(item) {
            return {
              itemnumber: item.itemnumber
            };
          });

          if (params.isMandatory === true) {
            newList.name = 'Mandatory';
          } else if (params.isRecommended === true) {
            newList.name = 'Recommended';
          } else {
            newList.name = UtilityService.generateName('List', Service.lists);
          }
          
          return newList;
        },

        // items: accepts null, item object, or array of item objects
        // params: isMandatory param for creating mandatory list
        // returns promise and new list object
        createList: function(items, params) {
          var newList = Service.beforeCreateList(items, params);
          if (!params) {
            params = {};
          }
          
          return List.save(params, newList).$promise.then(function(response) {
            toaster.pop('success', null, 'Successfully created list.');
            return Service.getList(response.listitemid);
          }, function(error) {
            toaster.pop('error', null, 'Error creating list.');
            return $q.reject(error);
          });
        },

        importList: function(file, options) {
          var deferred = $q.defer();

          $upload.upload({
            url: '/import/list',
            method: 'POST',
            data: { options: options },
            file: file, // or list of files ($files) for html5 only
          }).then(function(response) {
            var data = response.data;

            if (data.success) {
              // add new list to cache
              var list = {
                listid: data.listid,
                name: 'Imported List'
              };
              Service.lists.push(list);

              // display messages
              if (data.warningmsg) {
                toaster.pop('warning', null, data.warningmsg);
              } else {
                toaster.pop('success', null, 'Successfully imported a new list.');
              }

              deferred.resolve(data);
            } else {
              toaster.pop('error', null, data.errormsg);
              deferred.reject(data.errormsg);
            }
          });

          return deferred.promise;
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
          }, function(error) {
            toaster.pop('error', null, 'Error saving list ' + list.name + '.');
            return $q.reject(error);
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
          }, function(error) {
            toaster.pop('error', null, 'Error deleting list.');
            return $q.reject(error);
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
        addItem: function (listId, item, doNotDisplayMessage) {
          delete item.listitemid;
          item.position = 0;
          item.label = null;
          item.parlevel = null;

          return List.addItem({
            listId: listId
          }, item).$promise.then(function(response) {
            item.listitemid = response.listitemid;
            item.editPosition = 0;

            if (!doNotDisplayMessage) {
              toaster.pop('success', null, 'Successfully added item to list.');
            }
            return item;
          }, function(error) {
            toaster.pop('error', null, 'Error adding item to list.');
            return $q.reject(error);
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
          }, function(error) {
            toaster.pop('error', null, 'Error deleting item from list.');
            return $q.reject(error);
          });
        },

        /********************
        EDIT MULTIPLE ITEMS
        ********************/

        // accepts listId (guid) and an array of items to add
        // params: allowDuplicates
        addMultipleItems: function(listId, items) {
          
          var newItems = [];
          items.forEach(function(item) {
            newItems.push({
              itemnumber: item.itemnumber
            });
          });

          return List.addMultipleItems({
            listId: listId
          }, newItems).$promise.then(function() {
            // TODO: favorite all items if favorites list
            toaster.pop('success', null, 'Successfully added ' + items.length + ' items to list.');
            return Service.getList(listId);
          }, function(error) {
            toaster.pop('error', null, 'Error adding ' + items.length + ' items to list.');
            return $q.reject(error);
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
          }, function(error) {
            toaster.pop('error', null, 'Error deleting ' + items.length + ' from list.');
            return $q.reject(error);
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
          return UtilityService.findObjectByField(Service.lists, 'isfavorite', true);
        },

        // accepts item number to remove from favorites list
        removeItemFromFavorites: function(itemNumber) {
          var favoritesList = Service.getFavoritesList();
          return Service.getListWithItems(favoritesList.listid).then(function(list) {
            var itemToDelete = $filter('filter')(list.items, {itemnumber: itemNumber})[0];

            return Service.deleteItem(itemToDelete.listitemid);
          });
        },

        addItemToFavorites: function(item) {
          return Service.addItem(Service.getFavoritesList().listid, item, true);
        },

        /*****************************
        REMINDER/MANDATORY ITEMS LISTS
        *****************************/

        createMandatoryList: function(items) {
          var params = { isMandatory: true };
          return Service.createList(items, params);
        },

        getCriticalItemsLists: function() {
          return List.getCriticalItems().$promise.then(function(criticalLists) {
            criticalLists.forEach(function(list) {
              PricingService.updateCaculatedFields(list.items);
            });
            return criticalLists;
          });
        },

        findMandatoryList: function() {
          return UtilityService.findObjectByField(Service.lists, 'ismandatory', true);
        },

        /**********************
        RECOMMENDED ITEMS LISTS
        ***********************/

        createRecommendedList: function(items) {
          var params = { isRecommended: true };
          return Service.createList(items, params);
        },

        getRecommendedItems: function() {
          return List.getRecommendedItems().$promise;
        },

        findRecommendedList: function() {
          return UtilityService.findObjectByField(Service.lists, 'isrecommended', true);
        },

        /***************
        SHARING/COPYING
        ***************/

        shareList: function(list, customers) {
          var copyListData = {
            listid: list.listid,
            customers: customers
          };

          return List.shareList(copyListData).$promise.then(function() {
            list.issharing = true;
            toaster.pop('success', null, 'Successfully shared list ' + list.name + ' with ' + customers.length + ' customers.');
          }, function(error) {
            toaster.pop('error', null, 'Error sharing list.');
            return $q.reject(error);
          });
        },

        copyList: function(list, customers) {
          var copyListData = {
            listid: list.listid,
            customers: customers
          };

          return List.copyList(copyListData).$promise.then(function() {
            toaster.pop('success', null, 'Successfully copied list ' + list.name + ' to ' + customers.length + ' customers.');
          }, function(error) {
            toaster.pop('error', null, 'Error copying list.');
            return $q.reject(error);
          });
        },
      };

      return Service;

    }
  ]);
