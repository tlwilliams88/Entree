'use strict';

angular.module('bekApp')
  .controller('CustomerDetailsController', ['$scope', '$stateParams', 'MessagePreferenceService', 'CustomerService',
    function ($scope, $stateParams, MessagePreferenceService, CustomerService) {


  /*---init---*/
  var init = function(){
    $scope.preferencesFound = false;
    $scope.loadingCustomer = true;
    $scope.errorMessage = '';

    CustomerService.getCustomerDetails($stateParams.customerNumber, $stateParams.branchNumber).then(function(customer) {
      
      $scope.customer = customer;
      var prefArray = [];

    var prefsFromSvc = MessagePreferenceService.loadPreferences();
    prefsFromSvc.then(function (success) {
      success.forEach(function(preference, index){ 
        if(preference.customerNumber === $scope.customer.customerNumber){
          
          if (!$scope.preferencesFound) {
            $scope.preferencesFound = true;
          }

          //for every topic of notification build a preference object
          success[index].preferences.forEach(function(preference){
            var newPreference = {};
            //set description and notification type
            newPreference.description = preference.description;
            newPreference.notificationType = preference.notificationType;
            //set default check to false
            newPreference.channels = [false,false,false];
            //override defaulted false with true if it is sent by the pref object
            preference.selectedChannels.forEach(function (selectedChannel) {
              if(selectedChannel.channel === 1){
                newPreference.channels[0] = true;
              } else if(selectedChannel.channel === 2){
                newPreference.channels[1] = true;
              } else if(selectedChannel.channel === 4){
                newPreference.channels[2] = true;
              }
            });
            //add new pref object with booleans to the temporary array
            prefArray.push(newPreference);
          });

          //persist temp array to scope for use in DOM
          $scope.defaultPreferences = prefArray;
        }
      })
    }, function (error) {
        $scope.displayMessage('error', 'An error occurred while reading user preferences' + error)
      });
    }).finally(function() {
      $scope.loadingCustomer = false;
      if (!$scope.customer) {
        $scope.errorMessage = 'No customer found.';
      }
    });
  };

  //initialize preferences
  init();

  $scope.savePreferences = function () {
    //creates new payload object
    var preferencePayload = {};

    //initializes properties
    preferencePayload.customerNumber = $scope.customer.customerNumber;
    preferencePayload.branchId = $scope.customer.customerBranch;
    preferencePayload.preferences = [];

    //for each topic format object accordingly by pushing only trues to the array
    $scope.defaultPreferences.forEach(function (preference) {
      var newTopic = {};
      newTopic.description = preference.description;
      newTopic.notificationType = preference.notificationType;
      newTopic.selectedChannels = [];
      if(preference.channels[0] === true){
        newTopic.selectedChannels.push({"Channel": 1});
      }
      if(preference.channels[1] === true){
        newTopic.selectedChannels.push({"Channel": 2});
      }
      if(preference.channels[2] === true){
        newTopic.selectedChannels.push({"Channel": 4});
      }
      preferencePayload.preferences.push(newTopic);
    });


    //make ajax call
    MessagePreferenceService.updatePreferences(preferencePayload).then(function (success) {
      //refresh user profile locally

    }, function (error) {
      $scope.displayMessage('error', 'An error occurred while updating user preferences' + error)
    });
  };

  /*---page functions---*/
  $scope.restoreDefaults = function () {
    var prefArray = [];
    //persist temp array to scope for use in DOM
    $scope.defaultPreferences = prefArray;

    var prefsFromSvc = MessagePreferenceService.loadPreferences();
    prefsFromSvc.then(function (success) {
      success[0].preferences.forEach(function(preference){ // first entry is users default preferences...
        var newPreference = {};
        //set description and default to false
        newPreference.description = preference.description;
        newPreference.notificationType = preference.notificationType;
        newPreference.channels = [false,false,false];
        //override defaulted false with true if it is sent by the pref object
        preference.selectedChannels.forEach(function (selectedChannel) {
          if(selectedChannel.channel === 1){
            newPreference.channels[0] = true;
          } else if(selectedChannel.channel === 2){
            newPreference.channels[1] = true;
          } else if(selectedChannel.channel === 4){
            newPreference.channels[2] = true;
          }
        });
        //add new pref object with booleans to the temporary array
        prefArray.push(newPreference);
      });

      //persist temp array to scope for use in DOM
      $scope.defaultPreferences = prefArray;

      $scope.savePreferences();

    }, function (error) {
      $scope.displayMessage('error', 'An error occurred while reading user preferences' + error)
    });
  };

}]);
