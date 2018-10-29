'use strict';

angular.module('bekApp')
  .factory('TutorialService', ['$rootScope', 'LocalStorage',
    function ($rootScope, LocalStorage) {

    var Service = {
        setTutorial: function(id, title, description, buttons, overlay, attachTo, position, offset, width, highlight) {
            guiders.createGuider({
                id: id,
                title: title,
                description: description,
                buttons: buttons,
                overlay: overlay,
                attachTo: attachTo,
                position: position,
                offset: offset,
                width: width,
                highlight: highlight
            })
            Service.setDisplayTutorial('run', id);
        },
        
        setDisplayTutorial: function(display, location){
            
            switch(display) {
                case "run":
                    $rootScope.tutorialRunning = true;
                    guiders.show(location);
                break;

                case "hide":
                    location(true)
                    $rootScope.tutorialRunning = false;
                    guiders.hideAll();
                break;
            }
            
        }
    };
 
    return Service;

  }]);