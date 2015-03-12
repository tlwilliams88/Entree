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
      },

      setShipDates: function(shipDates) {
        localStorageService.set(Constants.offlineLocalStorage.shipDates, shipDates);
      },
      getShipDates: function()  {
        return localStorageService.get(Constants.offlineLocalStorage.shipDates);
      }, 
      setDeletedCartGuids: function(guids)  {
        localStorageService.set(Constants.offlineLocalStorage.deletedCartGuids, guids);
      },
      getDeletedCartGuids: function()  {
        return localStorageService.get(Constants.offlineLocalStorage.deletedCartGuids);
      },
      removeDeletedCartGuids: function() {
        localStorageService.remove(Constants.offlineLocalStorage.deletedCartGuids);
      },
    };

    return Service;
  }
  ]);