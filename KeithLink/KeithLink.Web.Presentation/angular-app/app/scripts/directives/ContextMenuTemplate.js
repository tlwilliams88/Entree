'use strict';

/*
to be used in conjunction with contextMenu directive
*/

angular.module('bekApp')
.factory('ContextMenuService', function() {
      return {
        menuElement: null,
        modalElement: null
      };
    })
.directive('contextMenuTemplate', [ '$modal', '$document', '$timeout', 'ContextMenuService', 'UtilityService',
  function($modal, $document, $timeout, ContextMenuService, UtilityService){
  
  return {
    replace: true,
    transclude: true,
    link: function($scope, $element, $attrs) {

      $scope.mouseenterEvent = function(e) {
        var doc = $document[0].documentElement;
        var element = angular.element(e.target).siblings('.dropdown-menu');
        var elementHeight = element.height();

        var totalHeight = event.clientY + elementHeight;

        // check that the bottom of the element is not off the screen and that the element is smaller than the height of the screen 
        if (totalHeight > doc.clientHeight && elementHeight < doc.clientHeight) {
          element.find('div.btn-group-vertical').css('top', -1 * elementHeight + 37);
        } else {
          element.find('div.btn-group-vertical').removeAttr('style');
        }

      };

      var opened = false,
        mouseenter = false;

      function openOnDesktop(event, menuElement) {
        menuElement.addClass('open');

        var doc = $document[0].documentElement;

        var elementHeight = menuElement[0].scrollHeight;
        var totalHeight = event.clientY + elementHeight;

        if (totalHeight > doc.clientHeight) {
          // add bottom 0
          menuElement.css('bottom', '0');
        } else {
          menuElement.removeAttr('style');
        }
        
        opened = true;
      }

 
      function openContextMenuMouseoverEvent(event) {
        mouseenter = true;
        // desktop
        if (!isMobileDevice() && !opened) {
          if (ContextMenuService.menuElement !== null) {
            closeDesktop(ContextMenuService.menuElement);
          }
          ContextMenuService.menuElement = angular.element(event.target).closest('.context-menu-template').find('.context-menu');
          
          event.preventDefault();
          event.stopPropagation();
          $scope.$apply(function() {
            openOnDesktop(event, ContextMenuService.menuElement);
          });  
        }
      }

      function openContextMenuClickEvent(event) {
        if (isMobileDevice()) {
          // open modal
          ContextMenuService.modalElement = $modal.open({
            templateUrl: 'views/modals/contextmenumodal.html',
            controller: 'ContextMenuModalController',
            // inserts modal on the scope where the context menu click handlers are so the modal has access to those methods,
            // otherwise it would be inserted on the rootscope
            scope: $scope,
            resolve: {
              item: function() {
                return $scope.item; // directive inherits $scope.item from html where the directive is located
              }
            }
          });
        } 
      }

      var timer;
      function closeTimeout(menuElement) {
        mouseenter = false;
        if (opened) {
          timer = $timeout(function() {
            if (!mouseenter) {
              closeDesktop(menuElement);
            }
          }, 250);
        }
      }

      function closeDesktop(menuElement) {
        menuElement.removeClass('open');
        opened = false;
      }

      function closeContextMenuEvent(event) {
        if (ContextMenuService.menuElement) {
          $scope.$apply(function() {
            closeTimeout(ContextMenuService.menuElement);
          });
        }
      }

      $scope.$on('closeContextMenu', function(event) {
        if (ContextMenuService.menuElement) {
          closeDesktop(ContextMenuService.menuElement);
        }
      });

      $element.bind('mouseenter', openContextMenuMouseoverEvent);
      $element.bind('click', openContextMenuClickEvent);
      $element.bind('mouseleave', closeContextMenuEvent);
      
      $scope.$on('$destroy', function() {
        $element.unbind('mouseenter', openContextMenuMouseoverEvent);
        $element.unbind('click', openContextMenuClickEvent);
        $element.unbind('mouseleave', closeContextMenuEvent);
        $timeout.cancel( timer );
      });
    },
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);