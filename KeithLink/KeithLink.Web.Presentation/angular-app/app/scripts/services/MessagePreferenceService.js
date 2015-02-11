'use strict';

angular.module('bekApp')
  .factory('MessagePreferenceService', [ '$http', '$q', 'toaster', 'UtilityService', function ($http, $q, toaster, UtilityService) {
    var Service = {
      
      getPreferencesForCustomer: function(customerNumber) {
        var promise = $http.get('/messaging/preferences');
        return UtilityService.resolvePromise(promise).then(function (userPreferences) {
          
          var customerPreferencesFound;

          userPreferences.forEach(function(customerPreferences) {
            
            if (customerNumber === customerPreferences.customerNumber) {
              var customerPreferencesTransformed = [];

              customerPreferences.preferences.forEach(function(preference) {
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

              customerPreferencesFound = customerPreferencesTransformed;
            }
          });

          return customerPreferencesFound;;

        }, function (error) {
          toaster.pop('error', null, 'An error occurred while reading user preferences' + error);
          return $q.reject(error);
        });
      }, 

      updatePreferences: function(preferences, customerNumber, branchId) {

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
          }

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
      }
    };

    return Service;
  }]);


