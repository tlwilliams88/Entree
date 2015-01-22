'use strict';

angular.module('bekApp')
  .controller('UserProfileController', ['$scope', 'UserProfileService', 'branches', 'LocalStorage', '$state', 'MessagePreferenceService', 'CustomerService',
    function ($scope, UserProfileService, branches, LocalStorage, $state, MessagePreferenceService, CustomerService) {

  var init = function(){
    /*---init---*/
    $scope.userProfile = angular.copy(LocalStorage.getProfile());
    $scope.branches = branches;
    
    loadCustomers(customersConfig).then(setCustomers);

    /*---process user preferences---*/
    var prefArray = [];
    //for every topic of notification build a preference object
    $scope.userProfile.messagingpreferences[0].preferences.forEach(function(preference){
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
  };

  /**********
  CUSTOMERS
  **********/
  $scope.customersSortAsc = true;
  $scope.customersSortField = 'customerName';
  var customersConfig = {
    term: '',
    size: 30,
    from: 0,
    sortField: $scope.customersSortField,
    sortOrder: 'asc'
  };

  function loadCustomers(customersConfig) {
    $scope.loadingCustomers = true;
    return CustomerService.getCustomers(
      customersConfig.term,
       customersConfig.size,
       customersConfig.from,
       customersConfig.sortField,
       customersConfig.sortOrder
    ).then(function(data) {
      $scope.loadingCustomers = false;
      $scope.totalCustomers = data.totalResults;
      return data.results;
    });
  }

  function setCustomers(customers) {
    $scope.customers = customers;
  }
  function appendCustomers(customers) {
    $scope.customers = $scope.customers.concat(customers);
  }

  $scope.searchCustomers = function (searchTerm) {
    customersConfig.from = 0;
    customersConfig.term = searchTerm;
    loadCustomers(customersConfig).then(setCustomers);
  };

  $scope.sortCustomers = function(field, order) {
    customersConfig.from = 0;
    customersConfig.size = 30;
    customersConfig.sortField = field;
    $scope.customersSortField = field;

    $scope.customersSortAsc = order;
    if (order) {
      customersConfig.sortOrder = 'asc';
    } else {
      customersConfig.sortOrder = 'desc';
    }
    
    loadCustomers(customersConfig).then(setCustomers);
  };

  $scope.infiniteScrollLoadMore = function() {
    if (($scope.customers && $scope.customers.length >= $scope.totalCustomers) || $scope.loadingCustomers) {
      return;
    }
    customersConfig.from += customersConfig.size;
    loadCustomers(customersConfig).then(appendCustomers);
  };

  /*********
  AVATAR
  *********/
  $scope.files = [];
  $scope.onFileSelect = function($files) {
    $scope.files = [];
    for (var i = 0; i < $files.length; i++) {
      $scope.files.push($files[i]);
    }
  };

  function refreshAvatarUrl() {
    var now = new Date();
    var newUrl = $scope.userProfile.imageurl + '?d=' + now.toString();
    $scope.userProfile.imageurl = newUrl;
    $scope.$parent.userProfile.imageurl = newUrl;
  }

  $scope.uploadAvatar = function() {
    // console.log(avatarFile);
    // var file = {
    //   name: avatarFile.filename,
    //   file: avatarFile.base64
    // };
    var file = {
      name: $scope.files[0].name, 
      file: $scope.files[0]
    };
    UserProfileService.uploadAvatar(file).then(refreshAvatarUrl);
  };

  $scope.removeAvatar = function() {
    UserProfileService.removeAvatar().then(refreshAvatarUrl);
  };

  $scope.cancelChanges = function () {
    $scope.userProfile = angular.copy(LocalStorage.getProfile());
    $scope.updateProfileForm.$setPristine();
  };

  $scope.updateUserProfile = function(userProfile) {
    userProfile.email = userProfile.emailaddress;
    $scope.updateProfileErrorMessage = null;
    
    UserProfileService.updateUserProfile(userProfile).then(function(profile) {
      $scope.$parent.userProfile = profile;
      $scope.displayMessage('success', 'Successfully updated profile.');
    }, function(errorMessage) {
      $scope.updateProfileErrorMessage = errorMessage;
    });
  };

  $scope.changePassword = function (changePasswordData) {
    $scope.changePasswordErrorMessage = null;
    changePasswordData.email = $scope.userProfile.emailaddress;

    UserProfileService.changePassword(changePasswordData).then(function (success) {
      $scope.changePasswordData = {};
      $scope.changePasswordForm.$setPristine();
      $scope.displayMessage('success', 'Successfully changed password.');
    }, function (errorMessage) {
      $scope.changePasswordErrorMessage = errorMessage;
      $scope.displayMessage('error', 'Error updating profile.');
    });
  };

  $scope.savePreferences = function () {
    //creates new payload object
    var preferencePayload = {};

    //initializes properties
    preferencePayload.customerNumber = null;
    preferencePayload.preferences = [];

    //for each topic format object accordingly by pushing only trues to the array
    $scope.defaultPreferences.forEach(function (preference) {
      var newTopic = {};
      newTopic.description = preference.description;
      newTopic.notificationType = preference.notificationType;
      newTopic.selectedChannels = [];
      if(preference.channels[0] === true){
        newTopic.selectedChannels.push({'Channel': 1});
      }
      if(preference.channels[1] === true){
        newTopic.selectedChannels.push({'Channel': 2});
      }
      if(preference.channels[2] === true){
        newTopic.selectedChannels.push({'Channel': 4});
      }
      preferencePayload.preferences.push(newTopic);
    });

    //make ajax call
    MessagePreferenceService.updatePreferences(preferencePayload).then(function (success) {
      //refresh user profile locally

    }, function (error) {
      $scope.displayMessage('error', 'An error occurred while updating user preferences' + error);
    });
  };

  init();

}]);
