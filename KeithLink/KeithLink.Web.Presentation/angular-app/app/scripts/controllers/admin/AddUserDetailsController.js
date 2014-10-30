'use strict';

angular.module('bekApp')
  .controller('AddUserDetailsController', ['$scope', 'UserProfileService', 'LocalStorage',
    function ($scope, UserProfileService, LocalStorage) {

      /*Create vs Find User*/
      $scope.isExistingUser = false;

      /*Roles*/
      //temporary hardcoded roles in use until a role endpoint is created
      $scope.userRoles = ["Owner", "Accounting", "Approver", "Buyer", "Guest"];

      /*Customers*/
      $scope.customers = LocalStorage.getProfile().user_customers;

      /*NA/RA grouping logic*/
      var indexedGroups = [];

      //rechecks if group numbers have changed each time the page is refreshed
      $scope.customersToFilter = function() {
        indexedGroups = [];
        return $scope.customers;
      };

      //filters group numbers from customers ensuring they are unique and not null
      $scope.filterCustomers = function(customer) {
          var groupNum = customer.nationalOrRegionalAccountNumber;
          var groupIsNew = indexedGroups.indexOf(groupNum) == -1;
          if (groupIsNew && !$scope.isNullEmptyUndefined(groupNum)) {
              indexedGroups.push(groupNum);
              return groupIsNew;
          } else {
            return false;
          }
      };

      //filters out all customers with an assigned group to populate the unassigned customer list
      $scope.filterOutGroups = function(customer) {
        return !!$scope.isNullEmptyUndefined(customer.nationalOrRegionalAccountNumber);
      };

      /*Selected customers logic*/
      $scope.selectedCustomers = [];

      //updates the selected list each time a selection has changed
      $scope.updateSelectedList = function(){
        $scope.selectedCustomers = [];
        $scope.customers.forEach(function(customer){
          if(customer.selected) {
            $scope.selectedCustomers.push(customer);
          }
        });
      };

      //updates all customers in a group to the same selection status
      $scope.selectAllCustomersInGroup = function(groupNumber, groupSelectionStatus){
        $scope.customers.forEach(function(customer){
          if(customer.nationalOrRegionalAccountNumber == groupNumber){
            customer.selected = groupSelectionStatus;
          }
        });
        $scope.updateSelectedList();
      };

      /*Convenience Methods*/

      //allows for proper checking of empty group numbers
      $scope.isNullEmptyUndefined = function(val){
        return !!(angular.isUndefined(val) || val === null || val.length == 0);
      };
    }]
  );
