'use strict';

angular.module('bekApp')
  .controller('CustomerDetailsController', ['$scope', '$stateParams', 'MessagePreferenceService', 'CustomerService',
    function ($scope, $stateParams, MessagePreferenceService, CustomerService) {


  $scope.preferencesFound = false;
  $scope.loadingCustomer = true;
  $scope.errorMessage = '';

  CustomerService.getCustomerDetails($stateParams.customerNumber, $stateParams.branchNumber).then(function(customer) {
    
    $scope.customer = customer;

    MessagePreferenceService.getPreferencesForCustomer(customer.customerNumber, customer.customerBranch).then(function (customerPreferences) {
      if (customerPreferences) {
        $scope.preferencesFound = true;
        $scope.defaultPreferences = customerPreferences;
      }
    });
  
  }).finally(function() {
    $scope.loadingCustomer = false;
    if (!$scope.customer) {
      $scope.errorMessage = 'No customer found.';
    }
  });

  $scope.savePreferences = function (preferences, customerNumber, customerBranch) {
    MessagePreferenceService.updatePreferences(preferences, customerNumber, customerBranch).then(function (success) {
      
      $scope.preferencesForm.$setPristine();
      $scope.canEditNotifications = false;

    });
  };

  $scope.restoreDefaults = function (customerNumber, branchId) {
    MessagePreferenceService.getPreferencesForCustomer(null).then(function(defaultPreferences) {
      $scope.defaultPreferences = defaultPreferences;
      $scope.savePreferences(defaultPreferences, customerNumber, branchId);
    });
  };

}]);
