'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ReportsController
 * @description
 * # ReportsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ReportsController', ['$scope', '$state', 'ENV',
    function ($scope, $state, ENV) {
    
    // auto-redirect user to item usage report if they don't have access to kbit
    // if (!$scope.userProfile.iskbitcustomer) {
    //   $state.go('menu.itemusagereport');
    // }

    // KBIT ACCESS
    var usernameToken = $scope.userProfile.usernametoken;
    $scope.cognosUrl = ENV.cognosUrl + '?username=' + usernameToken;

}]);
