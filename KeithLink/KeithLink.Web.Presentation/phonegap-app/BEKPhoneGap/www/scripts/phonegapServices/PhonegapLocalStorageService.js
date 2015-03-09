'use strict';

angular.module('bekApp')
.factory('PhonegapLocalStorageService', ['localStorageService', 'Constants',
  function(localStorageService, Constants) {

    var Service = {
      setLabels: function(labels) {
        localStorageService.set(Constants.offlineLocalStorage.labels, labels);
      },
      getLabels: function()  {
        return localStorageService.get(Constants.offlineLocalStorage.labels);
      },
      setDeletedListGuids: function(guids)  {
        localStorageService.set(Constants.offlineLocalStorage.deletedListGuids, guids);
      },
      getDeletedListGuids: function()  {
        return localStorageService.get(Constants.offlineLocalStorage.deletedListGuids);
      },
      removeDeletedListGuids: function() {
        localStorageService.remove(Constants.offlineLocalStorage.deletedListGuids);
      }
    };

    return Service;
  }
  ]);