'use strict';

angular.module('bekApp')
  .controller('CustomerDetailsController', ['$scope', '$stateParams', 'MessagingService', 'CustomerService',
    function ($scope, $stateParams, MessagingService, CustomerService) {


  $scope.preferencesFound = false;
  $scope.loadingCustomer = true;
  $scope.errorMessage = '';

  // set correct user details link based on role
  $scope.userDetailState = 'menu.admin.user.view';
  if ($scope.canEditUsers) { // inherited from MenuController
    $scope.userDetailState = 'menu.admin.user.edit';
  }

  CustomerService.getCustomerDetails($stateParams.customerNumber, $stateParams.branchNumber).then(function(customer) {
    $scope.showPrices = customer.canViewPricing;
    $scope.customer = customer;

    MessagingService.getPreferencesForCustomer(customer.customerNumber, customer.customerBranch).then(function (customerPreferences) {
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
      MessagingService.updatePreferences(preferences, customerNumber, customerBranch).then(function (success) {      
        $scope.preferencesForm.$setPristine();
        $scope.canEditNotifications = false;
      });
    })
  };

  $scope.restoreDefaults = function (customerNumber, branchId) {
    MessagingService.getPreferencesForCustomer(null).then(function(defaultPreferences) {
      $scope.defaultPreferences = defaultPreferences;
      $scope.savePreferences(defaultPreferences, customerNumber, branchId);
    });
  };

}]);
