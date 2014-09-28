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
          if ($.inArray(e.which, [key.left, key.up, key.right, key.down, key.enter]) < 0) { return; }

          var input = e.target;
          var td = $(e.target).closest('td');
          var moveTo = null;

          switch (e.which) {

              case key.left: {
                  if (input.selectionStart == 0) {
                      moveTo = td.prev('td:has(input,textarea)');
                  }
                  break;
              }
              case key.right: {
                  if (input.selectionEnd == input.value.length) {
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
                  if (e.which == key.down || e.which === key.enter) {
                      moveToRow = tr.next('tr').next('tr');;

                      if (!moveToRow.length) { // go to first row
                        moveToRow = angular.element('.results-table > tbody > tr:not(.filter-row, .mobile-details-row)').first();
                      }
                  }
                  else if (e.which == key.up) {
                      moveToRow = tr.prev('tr').prev('tr');;

                      if (!moveToRow.length) { // go to last row
                        moveToRow = angular.element('.results-table > tbody > tr:not(.filter-row, .mobile-details-row)').last();
                      }
                  }

                  if (moveToRow.length) {
                      moveTo = $(moveToRow[0].cells[pos]);
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
        var focusedElement = $(e.target);
        var nextElement = focusedElement.parent().next();
        if (nextElement.find('input').length>0){
          nextElement.find('input').focus();
        } else {
          nextElement = nextElement.parent().next().find('input').first();
          nextElement.focus();
        }
      }
    }
  }
    //   element.on('keyup', 'input[type="text"]', handleNavigation);


    //   function handleNavigation(e){

    //     var keys = { left: 37, up: 38, right: 39, down: 40, enter: 13 };

    //     // select all on focus
    //     element.find('input').keydown(function (e) {

    //       // shortcut for key other than arrow keys and enter
    //       if (angular.element.inArray(e.which, [keys.left, keys.up, keys.right, keys.down, keys.enter]) < 0) { return; }

    //       var input = e.target;
    //       var inputName = input.name;
    //       var td = angular.element(e.target).closest('.table-cell');
    //       var moveTo = null;

    //       switch (e.which) {

    //         case keys.left: {
    //           if (input.selectionStart === 0) {
    //             moveTo = td.prev('.table-cell:has(input,textarea)').find('input');
    //           }
    //           break;
    //         }
    //         case keys.right: {
    //           if (input.selectionEnd === input.value.length) {
    //             moveTo = td.next('.table-cell:has(input,textarea)').find('input');
    //           }
    //           break;
    //         }

    //         case keys.up:
    //         case keys.down:
    //         case keys.enter: {

    //           var tr = td.closest('.table-row');
    //           // var pos = td[0].cellIndex;

    //           var moveToRow = null;
    //           if (e.which === keys.down || e.which === keys.enter) {
    //             moveToRow = tr.next('.table-row');

    //             if (!moveToRow.length) { // move to top of list
    //               moveToRow = angular.element('.table-body .table-row:not(.filter-row)').first();
    //             }
    //           } else if (e.which === keys.up) {
    //             moveToRow = tr.prev('.table-row:not(.filter-row)');

    //             if (!moveToRow.length) { // move to bottom of list
    //               moveToRow = angular.element('.table-body .table-row').last();
    //             }
    //           }

    //           moveToRow = moveToRow.find('.table-row-data');

    //           if (moveToRow.length) {
    //             moveTo = angular.element(moveToRow.find('input[name=' + inputName + ']'));
    //           }
    //           break;
    //         }

    //       }

    //       if (moveTo && moveTo.length) {

    //         e.preventDefault();

    //         // moveTo.find('input,textarea').each(function (i, input) {
    //         //   input.focus();
    //         //   input.select();
    //         // });
    //         moveTo[0].select();

    //       }

    //     });


    //     var key = e.keyCode ? e.keyCode : e.which;
    //     if(key === 13) {
    //       var focusedElement = angular.element(e.target);
    //       var nextElement = focusedElement.parent().next();
    //       if (nextElement.find('input').length>0){
    //         nextElement.find('input').focus();
    //       } else {
    //         nextElement = nextElement.parent().next().find('input').first();
    //         nextElement.focus();
    //       }
    //     }
    //   }
    // };
  }]);