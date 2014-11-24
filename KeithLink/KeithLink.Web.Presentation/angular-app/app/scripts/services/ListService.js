'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:ListService
 * @description
 * # ListService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('ListService', ['$http', '$q', '$filter', '$upload', 'toaster', 'UserProfileService', 'UtilityService', 'List',
    function($http, $q, $filter, $upload, toaster, UserProfileService, UtilityService, List) {

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

        // MANDATORY -- only shown to DSRs
        } else if (list.ismandatory) {
          permissions.canSeeParlevel = true;
          permissions.alternativeParHeader = 'Required Qty';
          permissions.canDeleteList = true;
          permissions.canAddItems = true;
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
          // SHARING WITH OTHERS -- used to show icon on lists page
          // else if (list.issharing) {

          // }

        }

        list.permissions = permissions;        
      }

      var Service = {

        lists: [],
        labels: [],

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
        getList: function(listId) {
          return List.get({
            listId: listId,
          }).$promise.then(function(list) {
            updateListPermissions(list);

            // update new list in cache object
            var existingList = UtilityService.findObjectByField(Service.lists, 'listid', list.listid);
            if (existingList) {
              var idx = Service.lists.indexOf(existingList);
              angular.copy(list, Service.lists[idx]);
            } else {
              Service.lists.push(list);
            }

            return list;
          });
        },

        findListById: function(listId) {
          return UtilityService.findObjectByField(Service.lists, 'listid', parseInt(listId));
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

          /*
          {
            selectedtype: 'csv',

            // only custom exports should have this object
            fields: [{
              label: 'Description',
              field: 'description',
              order: 1
            }]
          }
          */

          $http.post('/list/export/' + listId, config, { responseType: 'arraybuffer' })//.then(function(response) {

            // var hiddenElement = document.createElement('a');
            // hiddenElement.href = 'data:text/csv;charset=utf-8,' + encodeURI(response.data);
            // hiddenElement.target = '_blank';
            // hiddenElement.download = 'myFile.csv';
            // document.body.appendChild(hiddenElement);
            // hiddenElement.click();
            // document.body.removeChild(hiddenElement);

    .success( function(data, status, headers) {

        var octetStreamMime = 'application/octet-stream';
        var success = false;

        // Get the headers
        headers = headers();

        // Get the filename from the x-filename header or default to "download.bin"
        // var filename = headers['x-filename'] || 'download.bin';

        var filename;
        if (config.selectedtype === 'CSV') {
          filename = 'export.csv';
        } else if (config.selectedtype === 'EXCEL') {
          filename = 'export.xlsx';
        } else {
          filename = 'export.txt';
        }

        // Determine the content type from the header or default to "application/octet-stream"
        var contentType = headers['content-type'] || octetStreamMime;

        try
        {
            // Try using msSaveBlob if supported
            console.log("Trying saveBlob method ...");
            var blob = new Blob([data], { type: contentType });
            if(navigator.msSaveBlob)
                navigator.msSaveBlob(blob, filename);
            else {
                // Try using other saveBlob implementations, if available
                var saveBlob = navigator.webkitSaveBlob || navigator.mozSaveBlob || navigator.saveBlob;
                if(saveBlob === undefined) throw "Not supported";
                saveBlob(blob, filename);
            }
            console.log("saveBlob succeeded");
            success = true;
        } catch(ex)
        {
            console.log("saveBlob method failed with the following exception:");
            console.log(ex);
        }

        if(!success)
        {
            // Get the blob url creator
            var urlCreator = window.URL || window.webkitURL || window.mozURL || window.msURL;
            if(urlCreator)
            {
                // Try to use a download link
                var link = document.createElement('a');
                if('download' in link)
                {
                    // Try to simulate a click
                    try
                    {
                        // Prepare a blob URL
                        console.log("Trying download link method with simulated click ...");
                        var blob = new Blob([data], { type: contentType });
                        var url = urlCreator.createObjectURL(blob);
                        link.setAttribute('href', url);

                        // Set the download attribute (Supported in Chrome 14+ / Firefox 20+)
                        link.setAttribute("download", filename);

                        // Simulate clicking the download link
                        var event = document.createEvent('MouseEvents');
                        event.initMouseEvent('click', true, true, window, 1, 0, 0, 0, 0, false, false, false, false, 0, null);
                        link.dispatchEvent(event);
                        console.log("Download link method with simulated click succeeded");
                        success = true;

                    } catch(ex) {
                        console.log("Download link method with simulated click failed with the following exception:");
                        console.log(ex);
                    }
                }

                if(!success)
                {
                    // Fallback to window.location method
                    try
                    {
                        // Prepare a blob URL
                        // Use application/octet-stream when using window.location to force download
                        console.log("Trying download link method with window.location ...");
                        var blob = new Blob([data], { type: octetStreamMime });
                        var url = urlCreator.createObjectURL(blob);
                        window.location = url;
                        console.log("Download link method with window.location succeeded");
                        success = true;
                    } catch(ex) {
                        console.log("Download link method with window.location failed with the following exception:");
                        console.log(ex);
                    }
                }

            }
        }

        if(!success)
        {
            // Fallback to window.open method
            console.log("No methods worked for saving the arraybuffer, using last resort window.open");
            window.open(httpPath, '_blank', '');
        }
    })
    .error(function(data, status) {
        console.log("Request failed with status: " + status);

        // Optionally write the error out to scope
        $scope.errorDetails = "Request failed with status: " + status;
    });

          // });
        },

        /********************
        EDIT LIST
        ********************/

        // items: accepts null, item object, or array of item objects
        // params: isMandatory param for creating mandatory list
        // returns promise and new list object
        createList: function(items, params) {
          if (!params) {
            params = {};
          }

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

          if (params.isMandatory === true) {
            newList.name = 'Mandatory';
          } else {
            newList.name = UtilityService.generateName('List', Service.lists);
          }
          
          return List.save(params, newList).$promise.then(function(response) {
            toaster.pop('success', null, 'Successfully created list.');
            return Service.getList(response.listitemid);
          }, function() {
            toaster.pop('error', null, 'Error creating list.');
          });
        },

        importList: function(file) {
          var deferred = $q.defer();

          $upload.upload({
            url: '/import/list',
            method: 'POST',
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
          }, function() {
            toaster.pop('error', null, 'Error saving list ' + list.name + '.');
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
          }, function() {
            toaster.pop('error', null, 'Error deleting list.');
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
          }, function() {
            toaster.pop('error', null, 'Error adding item to list.');
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
          }, function() {
            toaster.pop('error', null, 'Error deleting item from list.');
          });
        },

        /********************
        EDIT MULTIPLE ITEMS
        ********************/

        // accepts listId (guid) and an array of items to add
        // params: allowDuplicates
        addMultipleItems: function(listId, items) {
          
          UtilityService.deleteFieldFromObjects(items, ['listitemid', 'position', 'label', 'parlevel']);

          return List.addMultipleItems({
            listId: listId
          }, items).$promise.then(function() {
            // TODO: favorite all items if favorites list
            toaster.pop('success', null, 'Successfully added ' + items.length + ' items to list.');
            return Service.getList(listId);
          }, function() {
            toaster.pop('error', null, 'Error adding ' + items.length + ' items to list.');
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
          }, function() {
            toaster.pop('error', null, 'Error deleting ' + items.length + ' from list.');
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
          return Service.getList(favoritesList.listid).then(function() {
            var itemToDelete = $filter('filter')(favoritesList.items, {itemnumber: itemNumber})[0];

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
          return List.getReminderList().$promise;
        },

        findMandatoryList: function() {
          return UtilityService.findObjectByField(Service.lists, 'ismandatory', true);
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
          }, function() {
            toaster.pop('error', null, 'Error sharing list.');
          });
        },

        copyList: function(list, customers) {
          var copyListData = {
            listid: list.listid,
            customers: customers
          };

          return List.copyList(copyListData).$promise.then(function() {
            toaster.pop('success', null, 'Successfully copied list ' + list.name + ' to ' + customers.length + ' customers.');
          }, function() {
            toaster.pop('error', null, 'Error copying list.');
          });
        },
      };

      return Service;

    }
  ]);