'use strict';

/*
to be used in conjunction with contextMenu directive
*/

angular.module('bekApp')
.directive('contextMenuTemplate', [ '$modal',
  function($modal){
  
  return {
    replace: true,
    transclude: true,
    link: function(scope, elem, attr) {

      var modalInstance;
      scope.openContextMenu = function (e, item) {
        if (isTouchDevice()) {
          modalInstance = $modal.open({
            templateUrl: 'views/contextmenumodal.html',
            controller: 'ContextMenuModalController',
            // inserts modal on the scope where the context menu click handlers are so the modal has access to those methods,
            // otherwise it would be inserted on the rootscope
            scope: angular.element('[context-menu]').scope(),
            resolve: {
              item: function() {
                return item;
              }
            }
          });
        } else {
          scope.isContextMenuDisplayed = true;
        }
      };

      scope.closeContextMenu = function() {
        scope.isContextMenuDisplayed = false;
      };

      scope.closeContextMenuModal = function() {
        if (modalInstance) {
          modalInstance.close();
        }
      };

      function isTouchDevice() {
        // return ('ontouchstart' in window) || navigator.maxTouchPoints > 0 || navigator.msMaxTouchPoints > 0 || window.innerWidth <= 991;
        return window.innerWidth <= 991;
      }

      scope.$on('closeContextMenu', function(event) {
        scope.closeContextMenu();
        scope.closeContextMenuModal();
      });
    },
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);