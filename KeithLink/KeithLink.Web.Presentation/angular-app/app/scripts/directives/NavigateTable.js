'use strict';

/*
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
    
    function rowHasInput(moveToRow, pos) {
      var hasInput = false;
      if (moveToRow) {
        var inputCell = angular.element(moveToRow[0].cells[pos])
        if (inputCell.length) {
          var input = inputCell.find('input,textarea');
          if (input.length) {
            hasInput = true;
          }
        }
      }
      return hasInput;
    }

    function findNextInput(td, navigateTableType, isNavigatingDown) {
      var tr = td.closest('tr');
      if (attr.navigateTable === 'lists') {
        tr = td.closest('tbody');
      }
      var pos = td[0].cellIndex;
      var rowCount = element[0].rows.length;

      var nextRow = tr,
        moveToRow = null,
        count = 0;
      while (!rowHasInput(moveToRow, pos) && count < rowCount) {
        if (isNavigatingDown) {
          if (navigateTableType === 'mobile') {
            moveToRow = nextRow.next('tr').next('tr');
          } else if (navigateTableType === 'lists') {
            moveToRow = nextRow.next('tbody').children('tr:not(.mobile-details-row)');
          } else {
            moveToRow = nextRow.next('tr');
          }

          if (!moveToRow.length) { // go to first row
            moveToRow = element.find('> tbody > tr:not(.filter-row, .mobile-details-row)').first();
          }
        } else {
          if (navigateTableType === 'mobile') {
            moveToRow = nextRow.prev('tr').prev('tr');
          } else if (navigateTableType === 'lists') {
            moveToRow = nextRow.prev('tbody').children('tr:not(.mobile-details-row)');
          } else {
            moveToRow = nextRow.prev('tr');
          }

          if (!moveToRow.length) { // go to last row
            moveToRow = element.find('> tbody > tr:not(.filter-row, .mobile-details-row)').last();
          }
        }

        nextRow = moveToRow;
        count++;
      }

      return angular.element(moveToRow[0].cells[pos]);
    }


    
    function handleNavigation(e) {
      
      // select all on focus
      element.find('input').keydown(function (e) {
          
          var key = { left: 37, up: 38, right: 39, down: 40, enter: 13, tab: 9 };

          // shortcut for key other than arrow keys
          if (angular.element.inArray(e.which, [key.left, key.up, key.right, key.down, key.enter, key.tab]) < 0) { return; }

          var input = e.target;
          var td = angular.element(e.target).closest('td');
          var moveTo = null;

          switch (e.which) {

              case key.left: {
                  if (input.type === 'checkbox' || input.selectionStart === 0) {
                      moveTo = td.prev('td:has(input,textarea)');
                  }
                  break;
              }
              case key.right: {
                  if (input.selectionEnd === input.value.length) {
                      moveTo = td.next('td:has(input,textarea)');
                  }
                  break;
              }

              case key.up:
              case key.down:
              case key.enter:
              case key.tab: {
                var isNavigatingDown = e.which === key.down || e.which === key.enter || e.which === key.tab;
                moveTo = findNextInput(td, attr.navigateTable, isNavigatingDown);
                break;
              }

          }

          if (moveTo && moveTo.length) {

              e.preventDefault();

              moveTo.find('input,textarea').each(function (i, input) {
                  input.focus();
                  input.select();
              });

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
      // console.log('done!');
      angular.element( window ).off( 'resize.Viewport' );
    });
  };
  }]);