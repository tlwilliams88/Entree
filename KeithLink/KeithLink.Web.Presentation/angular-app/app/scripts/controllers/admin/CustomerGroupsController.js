'use strict';

angular.module('bekApp')
  .controller('CustomerGroupsController', ['$scope', 'CustomerGroupService',
    function ($scope, CustomerGroupService) {
    
    $scope.searchCustomerGroups = function(searchTerm) {
      CustomerGroupService.searchGroups(searchTerm).then(function(groups) {
        $scope.customerGroups = groups;
      });
    }

  }]);
