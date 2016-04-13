'use strict';

angular.module('bekApp')
  .controller('CustomerDetailsController', ['$scope', '$stateParams', 'MessagePreferenceService', 'CustomerService',
    function ($scope, $stateParams, MessagePreferenceService, CustomerService) {


  $scope.preferencesFound = false;
  $scope.loadingCustomer = true;
  $scope.errorMessage = '';
  $scope.customerUsers = [];

  // set correct user details link based on role
  $scope.userDetailState = 'menu.admin.user.view';
  if ($scope.canEditUsers) { // inherited from MenuController
    $scope.userDetailState = 'menu.admin.user.edit';
  }

  CustomerService.getCustomerDetails($stateParams.customerNumber, $stateParams.branchNumber).then(function(customer) {
    $scope.customerUsers = customer.customerusers;
    $scope.showPrices = customer.canViewPricing;
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
    $scope.customer.canViewPricing = $scope.showPrices;
    CustomerService.saveAccountingSettings($scope.customer).then(function(){
      MessagePreferenceService.updatePreferences(preferences, customerNumber, customerBranch).then(function (success) {      
        $scope.preferencesForm.$setPristine();
        $scope.canEditNotifications = false;
      });
    })
  };

  $scope.restoreDefaults = function (customerNumber, branchId) {
    MessagePreferenceService.getPreferencesForCustomer(customerNumber, branchId).then(function(defaultPreferences) {
      $scope.defaultPreferences = defaultPreferences;
      $scope.savePreferences(defaultPreferences, customerNumber, branchId);
    });
  };

}]);
