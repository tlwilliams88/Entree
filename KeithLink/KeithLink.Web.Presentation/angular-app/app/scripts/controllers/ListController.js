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
    
    $scope.display1 = {};

    $scope.showContextMenu = function(e, idx) {
      $scope.contextMenuLocation = { 
        'x': (e.x - 175 - 130) + 'px',
        'y': (e.y) + 'px'
      };
      $scope.isContextMenuDisplayed = true;
    };

    var selectedProduct;
    $scope.addItemToList = function (event, helper, list) {
      $scope.display = 'add item ' + selectedProduct.itemnumber + ' to list ' + list.name;
    };


    $scope.selectItem = function(event, helper, product) {
      selectedProduct = product;
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
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
        cases: 3,
        size: '15 CT'
      },
      {
        name: 'Peanut Butter',
        itemnumber: 54312,
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
      $scope.display = $data.itemnumber;
    };

  });