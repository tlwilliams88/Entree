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
      'imageUrl': null,
      'role': 'Kitchen Manager',
      'phone': 9348234934,
      'location': 'Dallas',
      'stores': [{
        'name': 'Jimmy\'s Chicken Shack',
        'customerNumber': 453234
      }, {
        'name': 'Saltgrass',
        'customerNumber': 534939
      }],
      'accountNumber': 9783459,
      'salesRep': {
        'id': 34234,
        'name': 'Heather Hill',
        'phone': 8889122342,
        'email': 'heather.hill@ben.e.keith.com',
        'imageUrl': null
      }
    };


  }]);
