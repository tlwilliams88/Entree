'use strict';

angular.module('bekApp')
  .factory('MessagingService', [ '$http', '$q', 'toaster', 'UtilityService', function ($http, $q, toaster, UtilityService) {
    
    function transformPreferences(preferences) {
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


      //  BROADCAST MESSAGE CREATION


      getBroadcastOptions: function() {
        //return list of available branches
        return $http.get('/messaging/createalert');
      },

      broadcastMandatoryMessage: function(payload) {
        return $http.post('/messaging/createalert', payload).then(function(response) {           
          if (!response) {
            return $q.reject('An error occurred while sending this message.');
          }
          return response;
        });
      },

      broadcastMessage: function(messagePayload){
        var promise = $http.post('/messaging/mail', messagePayload);
        return UtilityService.resolvePromise(promise);
      },


      //  MESSAGE PREFERENCES


      getPreferencesForCustomer: function(customerNumber, customerBranch) {
        var promise = $http.get('/messaging/preferences/' + customerNumber + '/' + customerBranch);
        return UtilityService.resolvePromise(promise).then(function(customerPreferences) {
          // returns user default preferences as the first element and customer preferences as second element
          var customerPreferencesFound = customerPreferences[1];
          return transformPreferences(customerPreferencesFound.preferences);
        });
      },

      // get preferences and filter by given customerNumber
      getPreferencesAndFilterByCustomerNumber: function(customerNumber) {
        var promise = $http.get('/messaging/preferences');
        return UtilityService.resolvePromise(promise).then(function (userPreferences) {
          
          var customerPreferencesFound;

          userPreferences.forEach(function(customerPreferences) {
            if (customerNumber === customerPreferences.customerNumber) {
              customerPreferencesFound = transformPreferences(customerPreferences.preferences);
            }
          });

          return customerPreferencesFound;

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
      }
    };

    return Service;
  }]);


