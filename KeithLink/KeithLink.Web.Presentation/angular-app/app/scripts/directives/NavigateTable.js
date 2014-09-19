'use strict';

/*
* Must use bootstrap table
* input elements must have a name attr
*/

angular.module('bekApp')
  .directive('navigateTable', [ function(){
    return function(scope, element, attr) {
      
      element.on('keyup', 'input[type="text"]', handleNavigation);


      function handleNavigation(e){

        var keys = { left: 37, up: 38, right: 39, down: 40, enter: 13 };

        // select all on focus
        element.find('input').keydown(function (e) {

          // shortcut for key other than arrow keys and enter
          if (angular.element.inArray(e.which, [keys.left, keys.up, keys.right, keys.down, keys.enter]) < 0) { return; }

          var input = e.target;
          var inputName = input.name;
          var td = angular.element(e.target).closest('.table-cell');
          var moveTo = null;

          switch (e.which) {

            case keys.left: {
              if (input.selectionStart === 0) {
                moveTo = td.prev('.table-cell:has(input,textarea)').find('input');
              }
              break;
            }
            case keys.right: {
              if (input.selectionEnd === input.value.length) {
                moveTo = td.next('.table-cell:has(input,textarea)').find('input');
              }
              break;
            }

            case keys.up:
            case keys.down:
            case keys.enter: {

              var tr = td.closest('.table-row');
              // var pos = td[0].cellIndex;

              var moveToRow = null;
              if (e.which === keys.down || e.which === keys.enter) {
                moveToRow = tr.next('.table-row');

                if (!moveToRow.length) { // move to top of list
                  moveToRow = angular.element('.table-body .table-row:not(.filter-row)').first();
                }
              } else if (e.which === keys.up) {
                moveToRow = tr.prev('.table-row:not(.filter-row)');

                if (!moveToRow.length) { // move to bottom of list
                  moveToRow = angular.element('.table-body .table-row').last();
                }
              }

              moveToRow = moveToRow.find('.table-row-data');

              if (moveToRow.length) {
                moveTo = angular.element(moveToRow.find('input[name=' + inputName + ']'));
              }
              break;
            }

          }

          if (moveTo && moveTo.length) {

            e.preventDefault();

            // moveTo.find('input,textarea').each(function (i, input) {
            //   input.focus();
            //   input.select();
            // });
            moveTo[0].select();

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