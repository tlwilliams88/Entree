'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ReportsController
 * @description
 * # ReportsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ReportsController', ['$scope', 'ENV',
    function ($scope, ENV) {
    
    var usernameToken = $scope.userProfile.usernametoken;
    $scope.cognosUrl = ENV.cognosUrl + '?username=' + usernameToken;
}]);
