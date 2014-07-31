'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ListController
 * @description
 * # ListController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ListController', function($scope) {
    
    $scope.showContextMenu = function(e, idx) {
      $scope.contextMenuLocation = { 
        'x': (e.x - 175 - 130) + 'px',
        'y': (e.y) + 'px'
      };
      $scope.isContextMenuDisplayed = true;
    };

    $scope.products = [
      {
        name: 'Peanut Butter',
        itemnumber: 12345,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 12345,
        cases: 3,
        size: '15 CT'
      }
    ];

    $scope.lists = [
      {
        name: 'Christmas List'
      },
      {
        name: 'List 7'
      },
      {
        name: 'July 4th'
      }
    ];

    $scope.onDragComplete = function ($data,$event) {
      debugger;
    };

    $scope.onDropComplete = function ($data,$event, listName) {
      debugger;
    };

  });