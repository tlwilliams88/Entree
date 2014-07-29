'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('MenuController', ['$scope', 'UserProfileService', function ($scope, UserProfileService) {
    
    $scope.isAdmin = false;
    $scope.isBuyer = false;
    $scope.isPayer = false;


    $scope.currentUser = {
      'name': 'Steven',
      'customerNumber': 12345,
      'imageUrl': '../images/placeholder-user.png',
      'role': 'Owner',
      'phone': 9348234934,
      'location': 'Dallas',
      'stores': [{
        'name': 'Dallas Ft Worth',
        'customerNumber': 453234,
        'branchId': 'fdf'
      }, {
        'name': 'San Antonio',
        'customerNumber': 534939,
        'branchId': 'fsa'
      }, {
        'name': 'Amarillo',
        'customerNumber': 534939,
        'branchId': 'fam'
      }],
      'accountNumber': 9783459,
      'salesRep': {
        'id': 34234,
        'name': 'Heather Hill',
        'phone': '(888) 912-2342',
        'email': 'heather.hill@ben.e.keith.com',
        'imageUrl': '../images/placeholder-dsr.jpg'
      }
    };

    $scope.test = function(e) {
      $scope.testMessage = new Date();
    };

    $scope.mouseupEvent = function(e) {
      $scope.testMessage = 'move';
    };

    $scope.currentUser.currentLocation = $scope.currentUser.stores[0];
  }]);
