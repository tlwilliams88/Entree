'use strict';

angular.module('bekApp')
  .controller('CustomerGroupDetailsController', ['$scope', '$state', 'CustomerGroupService',
    function($scope, $state, CustomerGroupService) {
  
  /***********
  FORM EVENTS
  ***********/

  var processingCreateCustomerGroup = false;
  $scope.createNewCustomerGroup = function(group) {
    if (!processingCreateCustomerGroup) {
      processingCreateCustomerGroup = true;
      CustomerGroupService.createGroup(group).then(function(newGroup) {
        $scope.displayMessage('success', 'Successfully created a new customer group.');
        $state.go('menu.admin.customergroupdashboard', {
          customerGroupId: newGroup.id
        });
      }, function(error) {
        $scope.displayMessage('error', 'Error creating new customer group.');
      }).finally(function() {
        processingCreateCustomerGroup = false;
      });
    }
  }


}]);
