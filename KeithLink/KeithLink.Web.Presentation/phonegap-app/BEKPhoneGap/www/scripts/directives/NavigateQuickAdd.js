'use strict';

angular.module('bekApp')
  .directive('navigateQuickAdd', [ function(){
    return function(scope, element, attr, ctrl) {

    // inherit addRow method from CartQuickAddModalController
    var addRow = scope.addRow;

    // select all on focus
    element.keydown(function (e) {
        
        var key = { enter: 13 };

        // shortcut for key other than arrow keys
        if (angular.element.inArray(e.which, [key.enter]) < 0) { return; }

        var input = e.target;
        var td = angular.element(e.target).closest('td');
        var moveTo = td.next('td');

        // go to next input if available
        if (moveTo && moveTo.length) {
          e.preventDefault();

          moveTo.find('input,textarea').each(function (i, input) {
            input.focus();
            input.select();
          });

        } else {
          // go to next row
          e.preventDefault();
          addRow();
          scope.$digest();
          var newRow = td.next('tr');
        }
    });
      
    scope.$on('$destroy',function() {
      // console.log('done!');
      angular.element( window ).off( 'resize.Viewport' );
    });
  };
  }]);