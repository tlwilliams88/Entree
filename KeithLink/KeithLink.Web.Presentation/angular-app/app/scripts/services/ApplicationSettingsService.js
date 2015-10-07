'use strict';

angular.module('bekApp')
  .factory('ApplicationSettingsService', [ '$http', '$q', 'toaster', 'UtilityService', function ($http, $q, toaster, UtilityService) {
    

        /****************************
          Notification Preferences
        *****************************/

    function transformNotificationPreferences(preferences) {
      var customerPreferencesTransformed = [];
      preferences.forEach(function(preference) {
        var preferenceTransformed = {
          description: preference.description,
          notificationType: preference.notificationType,
          channels: [false, false, false]
        };

        preference.selectedChannels.forEach(function (selectedChannel) {
          switch (selectedChannel.channel) {
            case 1: // Email
              preferenceTransformed.channels[0] = true;
              break;
            case 2: // Mobile Push
              preferenceTransformed.channels[1] = true;
              break;
            case 4: // Web
              preferenceTransformed.channels[2] = true;
              break;
          }
        });

        customerPreferencesTransformed.push(preferenceTransformed);
      });
      return customerPreferencesTransformed;
    }

    var Service = {

      
      getNotificationPreferencesForCustomer: function(customerNumber, customerBranch) {
        var promise = $http.get('/messaging/preferences/' + customerNumber + '/' + customerBranch);
        return UtilityService.resolvePromise(promise).then(function(customerPreferences) {
          // returns user default preferences as the first element and customer preferences as second element
          var customerPreferencesFound = customerPreferences[1];
          return transformNotificationPreferences(customerPreferencesFound.preferences);
        });
      },

      // get preferences and filter by given customerNumber
      getNotificationPreferencesAndFilterByCustomerNumber: function(customerNumber) {
        var promise = $http.get('/messaging/preferences');
        return UtilityService.resolvePromise(promise).then(function (userPreferences) {
          
          var customerPreferencesFound;

          userPreferences.forEach(function(customerPreferences) {
            if (customerNumber === customerPreferences.customerNumber) {
              customerPreferencesFound = transformNotificationPreferences(customerPreferences.preferences);
            }
          });

          return customerPreferencesFound;

        }, function (error) {
          toaster.pop('error', null, 'An error occurred while reading user preferences' + error);
          return $q.reject(error);
        });
      }, 

      updateNotificationPreferences: function(preferences, customerNumber, branchId) {

        var preferencePayload = {
          preferences: [],
          customerNumber: customerNumber,
          branchId: branchId
        };

        // for each topic format object accordingly by pushing only trues to the array
        preferences.forEach(function (preference) {
          
          var preferenceTransformed = {
            description: preference.description,
            notificationType: preference.notificationType,
            selectedChannels: []
          };

          if (preference.channels[0] === true) {
            preferenceTransformed.selectedChannels.push({'Channel': 1}); // Email
          }
          if (preference.channels[1] === true) {
            preferenceTransformed.selectedChannels.push({'Channel': 2}); // Mobile Push
          }
          if (preference.channels[2] === true) {
            preferenceTransformed.selectedChannels.push({'Channel': 4}); // Web
          }

          preferencePayload.preferences.push(preferenceTransformed);
        });

        var promise = $http.put('/messaging/preferences', preferencePayload);
        return UtilityService.resolvePromise(promise).then(function (response) {
          toaster.pop('success', null, 'Successfully saved messaging preferences.');
          return response;
        }, function (error) {
          toaster.pop('error', null, 'An error occurred while updating user preferences' + error);
          return $q.reject(error);
        });
      },

        /****************************
              Page Size Preferences
        *****************************/
    
      saveApplicationSettings: function(payload) {
        return $http.post('/profile/settings', payload).then(function(response) {           
          if (!response) {
            return $q.reject('An error occurred while saving your preferences.');
          }
          return response;
        });
      },

      resetApplicationSettings: function(payload) {
        return $http.delete('/profile/settings', {
            headers: {'Content-Type': 'application/json'},
            data: payload
          }).then(function(response) {
          if (!response) {
            return $q.reject('An error occurred while resetting your preferences.');
          }
          return response;
        });
      },

      getApplicationSettings: function(userId) {
        var promise = $http.get('/profile/settings' + userId);
        return UtilityService.resolvePromise(promise);
      }
};

    return Service;
  }]);


