'use strict';

/*
* Must use bootstrap table
* input elements must have a name attr
*/

angular.module('bekApp')
  .directive('navigateTable', [ function(){
    return function(scope, element, attr) {
      

    element.on('keyup', 'input[type="text"]', handleNavigation);
            
            
    function handleNavigation(e) {
      

      // select all on focus
      element.find('input').keydown(function (e) {
          
          var key = { left: 37, up: 38, right: 39, down: 40, enter: 13 };

          // shortcut for key other than arrow keys
          if (angular.element.inArray(e.which, [key.left, key.up, key.right, key.down, key.enter]) < 0) { return; }

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
              case key.enter: {

                  var tr = td.closest('tr');
                  var pos = td[0].cellIndex;

                  var moveToRow = null;
                  if (e.which === key.down || e.which === key.enter) {
                      moveToRow = tr.next('tr').next('tr');

                      if (!moveToRow.length) { // go to first row
                        moveToRow = angular.element('.table > tbody > tr:not(.filter-row, .mobile-details-row)').first();
                      }
                  }
                  else if (e.which === key.up) {
                      moveToRow = tr.prev('tr').prev('tr');

                      if (!moveToRow.length) { // go to last row
                        moveToRow = angular.element('.table > tbody > tr:not(.filter-row, .mobile-details-row)').last();
                      }
                  }

                  if (moveToRow.length) {
                      moveTo = angular.element(moveToRow[0].cells[pos]);
                  }

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
      if(key === 13) {
        var focusedElement = angular.element(e.target);
        var nextElement = focusedElement.parent().next();
        if (nextElement.find('input').length>0){
          nextElement.find('input').focus();
        } else {
          nextElement = nextElement.parent().next().find('input').first();
          nextElement.focus();
        }
      }
    }
  };
  }]);