'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', ['$http', '$q', '$filter', '$upload', '$analytics', 'toaster', 'UtilityService', 'ExportService', 'PricingService', 'List', 'LocalStorage', 'UserProfileService', 'DateService', 'Constants', 'SessionService', '$rootScope', 'blockUI',
    function($http, $q, $filter, $upload, $analytics, toaster, UtilityService, ExportService, PricingService, List, LocalStorage, UserProfileService, DateService, Constants, SessionService, $rootScope, blockUI) {

      function updateItemPositions(list) {
        angular.forEach(list.items, function(item, index) {
          item.position = index+1;
        });
      }

      function getCurrentUserProfile() {

        Service.isInternalUser = SessionService.userProfile.internal ? SessionService.userProfile.internal : false;

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

        if(Service.isInternalUser == undefined) {
          getCurrentUserProfile();
        }

        // FAVORITES - RECOMMENDED - MANDATORY FOR INTERNAL USE - REMINDER
        if (list.isfavorite || list.isrecommended || (list.ismandatory && Service.isInternalUser) || list.isreminder) {
          permissions.canEditList = true;
          permissions.canDeleteItems = true;
          permissions.canAddItems = true;
          permissions.canReorderItems = true;
          permissions.canAddNonBEKItems = false;
          if(list.isrecommended && list.ismandatory) {
            permissions.canDeleteList = true;
          } else if(list.isfavorite) {
            permissions.specialDisplay = true;
          } else if(list.ismandatory) {
            permissions.canEditParlevel = true;
            permissions.canSeeParlevel = true;
            permissions.alternativeParHeader = 'Required Qty';
          }
        // CONTRACT, WORKSHEET / HISTORY
        } else if (list.is_contract_list || list.isworksheet) {
          if(list.is_contract_list){
            //Set Header and fields for wildcard columns on lists page.
            //Contract items have two: Contract Category and read-only Each.
            //History has one: read-only Each.
            permissions.alternativeFieldName = 'category';
            permissions.alternativeFieldHeader = 'Contract Category';
          }

          if (list.items) {
            list.items.forEach(function(item) {
              item.eachString = item.each ? 'Y' : 'N';
            });
          }

        // MANDATORY NON-INTERNAL USE -- only editable by internal users
        } else if (list.ismandatory && !Service.isInternalUser) {
            permissions.canDeleteList = false;
            permissions.canAddItems = false;
            permissions.canEditList = false;
            permissions.canDeleteItems = false;
            permissions.canEditParlevel = false;
            permissions.canSeeParlevel = true;
            permissions.alternativeParHeader = 'Required Qty';

        // REMINDER
        } else if (list.isreminder) {
          permissions.canEditList = true;
          permissions.canAddItems = true;
          permissions.canDeleteItems = true;
          permissions.canReorderItems = true;
          permissions.canAddNonBEKItems = false;

        // CUSTOM LISTS (only these can be shared/copied)
        } else {

          // SHARED WITH ME
          if (list.isshared) {
            permissions.canSeeLabels = true;
            permissions.canSeeParlevel = true;
            // permissions.canEditParlevel = true;
            permissions.canAddNonBEKItems = false;

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
            permissions.canAddNonBEKItems = true;
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
          permissions.canAddNonBEKItems = false;
        }

        list.permissions = permissions;
      }

      var Service = {

        renameList: false,

        lists: [],
        labels: [],
        userProfile: {},
        listHeaders: [],

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
          return List.get(params).$promise.then(function(lists) {
            var listHeaders = lists.successResponse;
            listHeaders.forEach(function(list) {
              updateListPermissions(list);
            });
            angular.copy(listHeaders, Service.lists);
            Service.listHeaders = listHeaders;
            return listHeaders;
          });
        },

        getListHeaders: function() {
          return Service.getAllLists({ header: true });
        },

        getParamsObject: function(params, page) {
          var deferred = $q.defer();
            //if no stored page size, use default 30
             var filterObject = LocalStorage.getDefaultSort();
             var  fields =  [
             { 'field': 'position', 'order': ''},
             { 'field': 'itemnumber', 'order': ''},
             { 'field': 'name', 'order': ''},
             { 'field': 'brandextendeddescription', 'order': ''},
             { 'field': 'itemclass', 'order': ''},
             { 'field': 'notes', 'order': ''},
             { 'field': 'label', 'order': ''},
             { 'field': 'parlevel', 'order': ''}];

             //Decode stored sort preferences and buils params sort object with it.
             //A description of how this works exists on the BEK ecommerce wiki under the title: Default Sort String: Explaination

             if(filterObject && filterObject.length > 6){
              var settings = [];
              if(page === 'addToOrder'){
                settings = filterObject.slice(filterObject.indexOf('a') + 3, filterObject.length);
              }

              if(page === 'lists'){
                settings = filterObject.slice(3, filterObject.indexOf('a'));
              }

                for (var i = 0;  i < settings.length; i++) {
                  if(settings[i] !== 'y' && settings[i] !== 'n'){
                    fields[settings[i]].order = (settings[i + 1] === 'n') ? 'asc':'desc';
                    params.sort.push(fields[settings[i]]);
                  }
                }
             }
             deferred.resolve(params);
             return deferred.promise;
        },


        getCustomListHeaders: function() {
          return $http.get('/list/type/custom', { params: { headerOnly: true } }).then(function(response) {
            return response.data.successResponse;
          });
        },

        getListsByType: function(type, params) {
          return List.getByType({ type: type }, { params: params }).$promise.then(function(resp){
            return resp.successResponse;
          });
        },

        // accepts listId (guid)
        // returns list object
        getListWithItems: function(list, params, displayMessage) {
          var savingOrLoadingItems = displayMessage ? displayMessage : 'Loading Items...';
          Service.userProfile = UserProfileService.getCurrentUserProfile(savingOrLoadingItems);
          if (!params) {
            params = {
              includePrice: true
            };
          }
          var data = {
            params: params
          };
          return $http.get('/list/' + list.listType + '/' + list.listId, data).then(function(response) {
            var list = response.data.successResponse;
            if (!list) {
              return $q.reject('No list found.');
            }
            if(params.includePrice){
              PricingService.updateCaculatedFields(list.items);
            }

            updateListPermissions(list);

            Service.updateCache(list);

            return list;
          });
        },

        // accepts listId (guid), paging params
        // returns paged list object
        getList: function(list, params) {

            getCurrentUserProfile();

            if (!params) {
              var pageSize = LocalStorage.getPageSize();
              params = {
                size: pageSize,
                from: 0
              };
            }

            var listType = list.type != null ? list.type : list.listType,
                listId = list.listid != null ? list.listid : list.listId;

            Service.sortObject = params.sort;
            return $http.post('/list/' + listType + '/' + listId, params).then(function(response) {
              var list = response.data.successResponse;
              if (!list) {
                return $q.reject('No list found.');
              }

              // transform paged data
              list.itemCount = list.items.totalResults;
              list.items = list.items.results;
              list.items.forEach(function(item){
                if(item.onhand < 0.01){
                  item.onhand = '';
                } else if(item.quantity < 1){
                    item.quantity = '';
                }
              });

              // get calculated fields
              PricingService.updateCaculatedFields(list.items);
              updateListPermissions(list, Service.isInternalUser);

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
        CUSTOM INVENTORY LIST
        ********************/

        getCustomInventoryList: function() {
          return $http.get('/custominventory').then(function(response){
            var customInventory = response.data.successResponse;

            customInventory.iscustominventory = true;
            customInventory.name = 'Non BEK Items';

            updateListPermissions(customInventory);

            return customInventory;
          });
        },

        addNewItemFromCustomInventoryList: function(listitem) {
          return $http.post('/list/' +  list.type + '/' + list.listid + '/custominventoryitem', listitem).then(function(response){
            var customInventoryItems = response.data.successResponse.items;

            return customInventoryItems;
          });
        },

        addNewItemsFromCustomInventoryList: function(list, listitems) {
          var itemsToAdd = [];

          listitems.forEach(function(item){
            itemsToAdd.push(item.id);
          });

          return $http.post('/list/' +  list.type + '/' + list.listid + '/custominventoryitem', itemsToAdd).then(function() {
            toaster.pop('success', null, 'Successfully added items to list.');
          }, function(error) {
            toaster.pop('error', null, 'Error adding items to list.');
          });
        },

        saveCustomInventoryList: function(listitems) {
          return $http.post('/custominventory', listitems).then(function(response){
            var customInventory = response.data.successResponse;
            toaster.pop('success', null, 'Successfully saved Non BEK Items list.');

            customInventory.iscustominventory = true;
            customInventory.name = 'Non BEK Items';

            updateListPermissions(customInventory);

            return customInventory;

          }, function(error) {
            toaster.pop('error', null, 'Error saving Non BEK Items list.');
          });
        },

        deleteCustomInventoryItem: function(listitem) {
          return $http.delete('/custominventory/' + listitem).then(function(response){
            toaster.pop('success', null, 'Successfully deleted item from list.');
            return response.data.successResponse;
          }, function(error) {
            toaster.pop('error', null, 'Error deleting item to list.');
          });
        },

        deleteCustomInventoryItems: function(listitems) {
          return $http.post('/custominventory/delete', listitems).then(function(response){
            toaster.pop('success', null, 'Successfully deleted items from list.');
            return response.data.successResponse;
          }, function(error) {
            toaster.pop('error', null, 'Error deleting items from list');
          });
        },

        importNonBEKListItems: function(file, options) {
          var deferred = $q.defer();

          $upload.upload({
            url: '/import/custominventory',
            method: 'POST',
            data: { options: options },
            file: file,
          }).then(function(response) {
            var data = response.data.successResponse;

            if (response.data.isSuccess && data.success) {

              // display messages
              if (data.warningmsg) {
                toaster.pop('warning', null, data.warningmsg);
              } else {
                toaster.pop('success', null, 'Successfully imported items to Non-BEK Items list.');
              }

              deferred.resolve(data);
            } else {
              var errorMessage = response.data.errorMessage;
              if(data && data.errormsg){
                toaster.pop('error', null, data.errormsg);
                errorMessage = data.errormsg;
              }
              deferred.reject(errorMessage);
            }
          });
          return deferred.promise;
        },

        /********************
        EXPORT
        ********************/

        getExportConfig: function(list) {
          return List.exportConfig({
            listId: list.listid,
            listType: list.type
          }).$promise.then(function(resp){
            return resp.successResponse;
          });
        },

        exportList: function(config, list) {
          ExportService.export('/list/export/' + list.type + '/' + list.listid, config);
        },

        printBarcodes: function(list) {
          var promise = $http.get('/list/barcode/' + list.type + '/' + list.listid, {
            responseType: 'arraybuffer'
          });
          return ExportService.print(promise);
        },

        printList: function(list, landscape, showparvalues, options, shownotes, prices) {

            var printparams = {
              landscape: landscape,
              showparvalues: showparvalues,
              shownotes: shownotes,
              paging: options,
              showprices: prices
            };


          var promise = $http.post('/list/print/' + list.type + '/' + list.listid, printparams, {
            responseType: 'arraybuffer'
          });
          return ExportService.print(promise);
        },

        /********************
        EDIT LIST
        ********************/

        confirmQuantity: function(type, item, value) {
          var pattern = /^([0-9])\1+$/; // repeating digits pattern

          if (value > 50 || pattern.test(value)) {
            var isConfirmed = window.confirm('Do you want to continue with entered quatity of ' + value + '?');
            if (!isConfirmed) {
              // clear input
            if(type==='quantity'){
              item.quantity = null;
            }
            else{
              item.onhand=null;
            }
            }
          }
        },

        remapItems: function(item) {
          return {
            itemnumber: item.itemnumber,
            catalog_id: item.catalog_id,
            each: item.each,
            category: item.category
          };
        },

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

          newList.items = newItems.map(Service.remapItems);

          if (params.type === 9) {
            newList.name = 'Mandatory';
          } else if (params.type === 10) {
            newList.name = 'Recommended';
          } else if (params.name && params.name !== null) {
            newList.name = params.name;
          }
          else{
            newList.name = UtilityService.generateName('New List', Service.lists);
          }

          return newList;
        },

        // items: accepts null, item object, or array of item objects
        // params: type (recommendedItems, Mandatory, InventoryValuation)
        // returns promise and new item listitemid
        createList: function(items, params) {
          var newList = Service.beforeCreateList(items, params);
          if (!params) {
            params = {};
          }
          $analytics.eventTrack('Create List', {  category: 'Lists'});
          newList.message = 'Creating list...';
          return List.save(params, newList).$promise.then(function(response) {
            Service.renameList = true;
            Service.getListHeaders();
            toaster.pop('success', null, 'Successfully created list.');
            return Service.getList(response.successResponse.listitemid);
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
            var data = response.data.successResponse;

            if (response.data.isSuccess && data.success) {
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
              var errorMessage = response.data.errorMessage;
              if(data && data.errormsg){
                toaster.pop('error', null, data.errormsg);
                errorMessage = data.errormsg;
              }
              deferred.reject(errorMessage);
            }
          });

          return deferred.promise;
        },

        // accepts list object
        // returns promise and updated list object
        updateList: function(list, getEntireList, params, addingItem) {

          return List.update(null, list).$promise.then(function(response) {
            var list = response.successResponse;

            // update labels
            angular.forEach(list.items, function(item, index) {
              if (item.label && Service.labels.indexOf(item.label) === -1) {
                Service.labels.push(item.label);
              }
            });

            if(response.isSuccess == true) {
                toaster.pop('success', null, 'Successfully updated list.');
            } else {
                toaster.pop('error', null, 'Error updating list.' + response.errorMessage);
            }

            blockUI.stop();
            return;
          })
        },

        // accepts listId (guid)
        deleteList: function(list) {
          return List.delete({
            listId: list.listid,
            listType: list.type
        }).$promise.then(function(response) {
            // TODO: can I clean this up?
            var deletedList = Service.findListById(list.listid);
            var idx = Service.lists.indexOf(deletedList);
            if (idx > -1) {
              Service.lists.splice(idx, 1);
            }

            if(response.isSuccess == true) {
                toaster.pop('success', null, 'Successfully deleted list.');
                return Service.getFavoritesList();
            } else {
                toaster.pop('error', null, 'Error deleting list.');
                return $q.reject(error);
            }

          })
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
        // Only context menu uses this function
        addItem: function (list, item) {
          delete item.listitemid;
          item.position = 0;
          item.label = null;
          item.parlevel = null;

          if(list) {
              return List.addItem({
                listId: list.listid,
                listType: list.type
              }, item).$promise.then(function(response) {
                Service.getListHeaders().then(function(listheaders){
                  $rootScope.listHeaders = listheaders;
                })
                item.listitemid = response.successResponse.listitemid;
                item.editPosition = 0;
                return item;
              }, function(error) {
                return $q.reject(error);
              });
            }

        },

        // accepts listId (guid) and item object
        updateItem: function(listId, item) {
          return List.updateItem({}, item).$promise.then(function(response) {
            // TODO: add label to Service.labels if it does not exist
            // TODO: replace item in Service.lists
            return response.data.successResponse;
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

        deleteItemByItemNumber: function(list, itemNumber) {
          return List.deleteItemByItemNumber({
            listId: list.listid,
            listType: list.type,
            itemNumber: itemNumber
          }).$promise.then(function(response) {
            toaster.pop('success', null, 'Successfully removed item from list.');
            return;
          }, function(error) {
            toaster.pop('error', null, 'Error removing item from list.');
            return $q.reject(error);
          });
        },

        /********************
        EDIT MULTIPLE ITEMS
        ********************/

        // accepts listId (guid) and an array of items to add
        // params: allowDuplicates
        addMultipleItems: function(list, items) {

          var newItems = [];
          items.forEach(function(item) {
            newItems.push({
              itemnumber: item.itemnumber,
              each: item.each,
              catalog_id: item.catalog_id,
              category: item.category,
              label: item.label
            });
          });

          return List.addMultipleItems({
            listId: list.listid,
            listType: list.type
          }, newItems).$promise.then(function() {
            // TODO: favorite all items if favorites list
            toaster.pop('success', null, 'Successfully added ' + items.length + ' items to list.');
            return Service.getList(list);
          }, function(error) {
            toaster.pop('error', null, 'Error adding ' + items.length + ' items to list.');
            return $q.reject(error);
          });
        },

        // accepts listId (guid) and an array of items
        // NOTE $resource does not accept deletes with payloads
        deleteMultipleItems: function(list, items) {

          // create array of list item ids
          var listItemIds = [];
          angular.forEach(items, function(item, index) {
            listItemIds.push(item.listitemid);
          });

          return $http.delete('/list/' + list.type + '/' + list.listid + '/item', {
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
            angular.copy(response.data.successResponse, Service.labels);
            return response.data.successResponse;
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
          return Service.deleteItemByItemNumber(favoritesList, itemNumber);
        },

        addItemToFavorites: function(item) {
          $analytics.eventTrack('Add Favorite', {  category: 'Lists'});
          return Service.addItem(Service.getFavoritesList(), item, true);
        },

        /*****************************
        REMINDER/MANDATORY ITEMS LISTS
        *****************************/

        createMandatoryList: function(items) {
          // Type 9 == Mandatory
          var params = { type: 9 };
          return Service.createList(items, params);
        },

        getCriticalItemsLists: function() {
          return List.getCriticalItems().$promise.then(function(resp) {
            if(resp.successResponse){
              resp.successResponse.forEach(function(list) {
                PricingService.updateCaculatedFields(list.items);
              });
            }

            return resp.successResponse;
          });
        },

        findMandatoryList: function() {
          return UtilityService.findObjectByField(Service.lists, 'ismandatory', true);
        },

        /**********************
        RECOMMENDED ITEMS LISTS
        ***********************/

        createRecommendedList: function(items) {
          // Type = 10 - Recommended list type that needs to be passed in
          var params = { type: 10 };
          return Service.createList(items, params);
        },

        getRecommendedItems: function() {
          return List.getRecommendedItems().$promise.then(function(response){
            return response.successResponse;
          });
        },

        findRecommendedList: function() {
          return UtilityService.findObjectByField(Service.lists, 'isrecommended', true);
        },

        /**********************
        OTHER SPECIAL LISTS
        ***********************/

        findList: function(field, value) {
          return UtilityService.findObjectByField(Service.lists, field, value);
        },

        /***************
        SHARING/COPYING
        ***************/

        shareList: function(list, customers) {
          var copyListData = {
            listid: list.listid,
            listtype: list.type,
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
            listtype: list.type,
            customers: customers
          };

          return List.copyList(copyListData).$promise.then(function(newLists) {
            if(newLists.isSuccess == true) {
                toaster.pop('success', null, 'Successfully copied list ' + list.name + ' to ' + customers.length + ' customers.');
                return newLists.successResponse;
            } else {
                toaster.pop('error', null, 'Error copying list.');
                return $q.reject(newLists.errorMessage);
            }
          })
        },

        // copy list to current customer and redirect to that list
        duplicateList: function(list, customers) {
          return Service.copyList(list, customers).then(function(lists) {
            var newList = lists[0];
            Service.lists.push(newList);
            return newList;
          });
        },

        /*******************
        SET LAST ORDER LIST
        *******************/
        setLastOrderList: function(listId, listType){
          var orderList = {
                listId: listId,
                listType: listType,
            };

          LocalStorage.setLastOrderList(orderList);
          }
        };


      return Service;

    }
  ]);
