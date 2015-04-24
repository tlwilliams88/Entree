'use strict';

angular.module('bekApp')
  .directive('listHeaderStickyScroll', [ function() {
    return {
      restrict: 'EA',
      replace: false,
      scope: { 
          scrollBody: '=',
          scrollStop: '=',
          scrollableContainer: '=',
          scrollOffset: '='            
      },
      link: function(scope, element, attributes, control){
        var header = $(element, this);
        var clonedHeader = null;
        var content = $(scope.scrollBody);
        var scrollableContainer = $(scope.scrollableContainer);
        var w = $(window);

        var scrollOffset = parseInt(scope.scrollOffset, 10) || 0;

        if (scrollableContainer.length == 0){
          scrollableContainer = $(window);
        }

        function createClone(){
          /*
           * switch place with cloned element, to keep binding intact
           */
          clonedHeader = header;
          header = clonedHeader.clone();
          clonedHeader.after(header);
          clonedHeader.addClass('fsm-sticky-header');
          clonedHeader.css({
            position: 'fixed',
            'z-index': 10000,
            visibility: 'hidden'
          });                
          calculateSize();
        }

        function calculateSize() {
          clonedHeader.css({
            top: 245 - w.scrollTop(),
            width: header.outerWidth(),
            left: header.offset().left
          });

          setColumnHeaderSizes();
        };

        function setColumnHeaderSizes() {
          if (clonedHeader.is('tr')) {
            var clonedColumns = clonedHeader.find('th');
            header.find('th').each(function (index, column) {
              var clonedColumn = $(clonedColumns[index]);
              clonedColumn.css( 'width', column.offsetWidth + 'px');
            });
          }
        }            

        function determineVisibility(){
          var scrollTop = scrollableContainer.scrollTop() + scope.scrollStop;
          var contentTop = content.position().top;
          var contentBottom = contentTop + content.outerHeight(false);

          contentTop -= 77;

          if ( scrollTop > contentTop ) {
            if (!clonedHeader){
              createClone();    
              clonedHeader.css({ "visibility": "visible"});
            }
            
            if ( scrollTop < contentBottom && scrollTop > contentBottom - clonedHeader.outerHeight(false) ){
              var top = contentBottom - scrollTop + scope.scrollStop - clonedHeader.outerHeight(false);
              clonedHeader.css('top', top + 'px');
            } else {
              calculateSize();
            }
          } else {
            if (clonedHeader){
              /*
               * remove cloned element (switched places with original on creation)
               */
              header.remove();
              header = clonedHeader;
              clonedHeader = null;

              header.removeClass('fsm-sticky-header');
              header.css({
                position: 'relative',
                left: 0,
                top: 0,
                width: 'auto',
                'z-index': 0,
                visibility: 'visible'
              });
            }
          };
        };

        scrollableContainer.scroll(determineVisibility).trigger( "scroll" );
        scrollableContainer.resize(determineVisibility);
        w.scroll(determineVisibility).trigger( "scroll" );
        w.resize(determineVisibility);
      }
    }
}]);