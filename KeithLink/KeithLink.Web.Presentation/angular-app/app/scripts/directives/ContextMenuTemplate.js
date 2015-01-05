'use strict';

/*
to be used in conjunction with contextMenu directive
*/

angular.module('bekApp')
.factory('ContextMenuService', function() {
      return {
        element: null,
        menuElement: null
      };
    })
.directive('contextMenuTemplate', [ '$modal', '$document', 'ContextMenuService',
  function($modal, $document, ContextMenuService){
  
  return {
    replace: true,
    transclude: true,
    // link: function(scope, elem, attr) {

    //   var modalInstance;
    //   scope.openContextMenu = function (e, item) {
    //     console.log('OPEN CONTEXT MENU');
    //     if (isTouchDevice()) {
    //       modalInstance = $modal.open({
    //         templateUrl: 'views/modals/contextmenumodal.html',
    //         controller: 'ContextMenuModalController',
    //         // inserts modal on the scope where the context menu click handlers are so the modal has access to those methods,
    //         // otherwise it would be inserted on the rootscope
    //         scope: scope, //angular.element('[context-menu]').scope(),
    //         resolve: {
    //           item: function() {
    //             return item;
    //           }
    //         }
    //       });
    //     } else {
    //       scope.isContextMenuDisplayed = true;
    //     }
    //   };

    //   scope.closeContextMenu = function() {
    //     scope.isContextMenuDisplayed = false;
    //   };

    //   scope.closeContextMenuModal = function() {
    //     if (modalInstance) {
    //       modalInstance.close();
    //     }
    //   };

    //   function isTouchDevice() {
    //     // return ('ontouchstart' in window) || navigator.maxTouchPoints > 0 || navigator.msMaxTouchPoints > 0 || window.innerWidth <= 991;
    //     return window.innerWidth <= 991;
    //   }

    //   scope.$on('closeContextMenu', function(event) {
    //     scope.closeContextMenu();
    //     scope.closeContextMenuModal();
    //   });
    // },
    link: function($scope, $element, $attrs) {
      var opened = false;

      function openOnDesktop(event, menuElement) {
        menuElement.addClass('open');

        var doc = $document[0].documentElement;
        
        // doc offset on screen
        var docLeft = (window.pageXOffset || doc.scrollLeft) -
            (doc.clientLeft || 0), 
          docTop = (window.pageYOffset || doc.scrollTop) -
            (doc.clientTop || 0),
          // context menu width and height
          elementWidth = menuElement[0].scrollWidth,
          elementHeight = menuElement[0].scrollHeight;

        // document width and height including offset
        var docWidth = doc.clientWidth + docLeft,
          docHeight = doc.clientHeight + docTop,
          // location of click event + element size (where the edge of the element will be)
          totalWidth = elementWidth + event.pageX,
          totalHeight = elementHeight + event.pageY,
          // location of click event on screen including doc offset, min 0
          left = Math.max(event.pageX - docLeft, 0),
          top = Math.max(event.pageY - docTop, 0);

        // if location of edge of context menu greater than total doc width (context menu will be off screen)
        if (totalWidth > docWidth) {
          // subtract excess pixels to get the max left value
          left = left - (totalWidth - docWidth);
        }

        if (totalHeight > docHeight) {
          top = top - (totalHeight - docHeight);
        }
        // subtract scroll offset
        top = top + $document.scrollTop();

        menuElement.css('top', top + 'px');
        menuElement.css('left', left + 'px');
        opened = true;
      }

      function close(menuElement) {
        menuElement.removeClass('open');

        opened = false;
      }

      function closeContextMenuEvent(event) {
        $scope.$apply(function() {
          close(ContextMenuService.menuElement);
        });
      }

      function openContextMenuEvent(event) {
        if (!opened) {
          if (ContextMenuService.menuElement !== null) {
            close(ContextMenuService.menuElement);
          }
          ContextMenuService.menuElement = angular.element(event.target).closest('.context-menu-template').find('.context-menu');
          ContextMenuService.element = event.target;

          event.preventDefault();
          event.stopPropagation();
          $scope.$apply(function() {
            openOnDesktop(event, ContextMenuService.menuElement);
          });  
        }
      }

      $element.bind('mouseenter', openContextMenuEvent);
      $element.bind('mouseleave', closeContextMenuEvent);
      
      $scope.$on('$destroy', function() {
        $element.unbind('mouseenter', openContextMenuEvent);
        $element.unbind('mouseleave', closeContextMenuEvent);
      });
    },
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);