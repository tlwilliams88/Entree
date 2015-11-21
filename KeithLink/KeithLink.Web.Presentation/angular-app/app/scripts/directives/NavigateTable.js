'use strict';

/*
 * Navigates between items with the class of 'tabstop' in a table with repeating tbody
 *
 * takes one optional attr
 * default -- behaves like a regular table with no extra rows
 * "lists" attr (navigate-table="lists") -- tbody tags can be used as rows, see lists.html page
 * "mobile" attr -- allows for .mobile-details-row, see searchresults.html page
 * 
 */

angular.module('bekApp')
.directive('navigateTable', [ function(){
  return function(scope, element, attr) {

    element.on('keyup', 'input[type="text"]', handleNavigation);

    function getPreviousTabstop(row) {
      return row.prevUntil('tr td .tabstop').find('.tabstop').last()[0];
    }

    function getNextTabstop(row) {
      return row.nextUntil('tr td .tabstop').find('.tabstop')[0];
    }

    function moveLeft(td, row) {
      var moveTo = td.prev('td:has(input,textarea)').find('input,textarea')[0];
      if (!moveTo) {
        moveTo = getPreviousTabstop(row);
      }
      return moveTo;
    }

    function moveRight(td, row) {
      var moveTo = td.next('td:has(input,textarea)').find('input,textarea')[0];
      if (!moveTo) {
        moveTo = getNextTabstop(row);
      }
      return moveTo;
    }

    function handleNavigation(e) {
      // select all on focus
      element.find('input').keydown(function (e) {
        var key = { left: 37, up: 38, right: 39, down: 40, enter: 13, tab: 9 };

        // shortcut for key other than arrow keys
        if (angular.element.inArray(e.which, [key.left, key.up, key.right, key.down, key.enter, key.tab]) < 0) { return; }

        var input = e.target;
        var td = angular.element(e.target).closest('td');
        var row = td.closest('tr');
        if (attr.navigateTable === 'lists') {
          row = td.closest('tbody');
        }
        var moveTo = null;

        switch (e.which) {

          case key.left: {
            if (input.type === 'checkbox' || input.selectionStart === 0) {
              moveTo = moveLeft(td, row); 
            }
            break;
          }
          case key.right:
          case key.tab: {
            if (input.type == 'checkbox' || input.selectionEnd === input.value.length) {
              moveTo = moveRight(td, row);
            }
            break;
          }

          case key.up: {
            moveTo = getPreviousTabstop(row);
            break;
          }
          case key.down:
            case key.enter: {
            moveTo = getNextTabstop(row);
            break;
          }
        }

        if (moveTo) {
          e.preventDefault();

          moveTo.focus();
          if (moveTo.type != 'checkbox') {
            moveTo.select();
          }
        }

      });


      var key = e.keyCode ? e.keyCode : e.which;
      // // enter key
      // if(key === 13) {
      //   var focusedElement = angular.element(e.target);
      //   var nextElement = focusedElement.parent().next();
      //   if (nextElement.find('input').length>0){
      //     nextElement.find('input').focus();
      //   } else {
      //     nextElement = nextElement.parent().next().find('input').first();
      //     nextElement.focus();
      //   }
      // }
    }

    scope.$on('$destroy',function() {
      console.log('done!');
      angular.element( window ).off( 'resize.Viewport' );
    });
  };
}]);
