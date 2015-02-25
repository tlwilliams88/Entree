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
    link: function($scope, $element, $attrs) {
      var opened = false;

      function openOnDesktop(event, menuElement) {
        menuElement.addClass('open');

        // var doc = $document[0].documentElement;
        
        // // doc offset on screen
        // var docLeft = (window.pageXOffset || doc.scrollLeft) -
        //     (doc.clientLeft || 0), 
        //   docTop = (window.pageYOffset || doc.scrollTop) -
        //     (doc.clientTop || 0),
        //   // context menu width and height
        //   elementWidth = menuElement[0].scrollWidth,
        //   elementHeight = menuElement[0].scrollHeight;

        // // document width and height including offset
        // var docWidth = doc.clientWidth + docLeft,
        //   docHeight = doc.clientHeight + docTop,
        //   // location of click event + element size (where the edge of the element will be)
        //   totalWidth = elementWidth + event.pageX,
        //   totalHeight = elementHeight + event.pageY,
        //   // location of click event on screen including doc offset, min 0
        //   left = Math.max(event.pageX - docLeft, 0),
        //   top = Math.max(event.pageY - docTop, 0);

        // // if location of edge of context menu greater than total doc width (context menu will be off screen)
        // if (totalWidth > docWidth) {
        //   // subtract excess pixels to get the max left value
        //   left = left - (totalWidth - docWidth);
        // }

        // if (totalHeight > docHeight) {
        //   top = top - (totalHeight - docHeight);
        // }
        // // subtract scroll offset
        // top = top + $document.scrollTop();

        // menuElement.css('top', top + 'px');
        // menuElement.css('left', left + 'px');
        opened = true;
      }

      // TODO: move to UtilityService
       function isMobileDevice() {
        var check = false;
        
        // Check for mobile browsers using http://detectmobilebrowsers.com/ script
        (function(a,b){
          if(/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino|android|ipad|playbook|silk/i.test(a)||/1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(a.substr(0,4))) {
            check = true;
          }
        })(navigator.userAgent||navigator.vendor||window.opera);

        // Check for screen width
        if (window.innerWidth < 992) {
          check = true;
        }
        return check;
      }

      function openContextMenuMouseoverEvent(event) {
        // desktop
        if (!isMobileDevice() && !opened) {
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

      var modalInstance;
      function openContextMenuClickEvent(event) {
        if (isMobileDevice()) {
          // open modal
          modalInstance = $modal.open({
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

      function close(menuElement) {
        menuElement.removeClass('open');

        opened = false;
      }

      function closeContextMenuEvent(event) {
        if (ContextMenuService.menuElement) {
          $scope.$apply(function() {
            close(ContextMenuService.menuElement);
          });
        }
      }

      function closeContextMenuModal() {
        if (modalInstance) {
          modalInstance.close();
        }
      }

      $scope.$on('closeContextMenu', function(event) {
        if (ContextMenuService.menuElement) {
          close(ContextMenuService.menuElement);
        }
        closeContextMenuModal();
      });

      $element.bind('mouseenter', openContextMenuMouseoverEvent);
      $element.bind('click', openContextMenuClickEvent);
      $element.bind('mouseleave', closeContextMenuEvent);
      
      $scope.$on('$destroy', function() {
        $element.unbind('mouseenter', openContextMenuMouseoverEvent);
        $element.unbind('click', openContextMenuClickEvent);
        $element.unbind('mouseleave', closeContextMenuEvent);
      });
    },
    templateUrl: 'views/directives/contextmenu.html'
  };
}]);