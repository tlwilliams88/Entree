'use strict';

angular.module('bekApp')
.factory('PhonegapPushService', ['$window', '$http', '$q', '$log',
  function($window, $http, $q, $log){
    var Service = {};

    function registerDevice(object) {
      var deferred = $q.defer();
      $http.put('/messaging/registerpushdevice', object).then(function(response) {
        var data = response.data;
        if (data.successResponse) {
          deferred.resolve(data.successResponse);
        } else {
          deferred.reject('error registering device through api');
        }
      });
      return deferred.promise;
    }

    Service.register = function () {
      if ( device.platform == 'android' || device.platform == 'Android' || device.platform == "amazon-fireos" ){
        window.plugins.pushNotification.register(
          successHandler,
          errorHandler,
          {
            "senderID":"1013977805145",
            "ecb":"onNotification"
          });
      } else {                    
        window.plugins.pushNotification.register(
          tokenHandler,
          errorHandler,
          {
            "badge":"true",
            "sound":"true",
            "alert":"true",
            "ecb":"onNotificationAPN"
          });
      }
    };

    //iOS token handler callback
    $window.tokenHandler = function(token) {
      var object = {
        deviceos: 1,
        deviceid: device.uuid,
        providertoken: token
      };
      registerDevice(object);
    };

    //Android/Windows/FireOS callback
    $window.successHandler = function (success) {
      $log.debug('successHandler: ' + success);
    };

    //error for both android and iOS
    $window.errorHandler = function (error) {
      $log.debug('errorHandler: ' + error);
    };

    $window.onNotificationAPN = function (event) {
      $log.debug('APN notification received');
      
      if ( event.alert ) {
        $log.debug('alert received');
        navigator.notification.alert(event.alert);
      }

      if ( event.badge ) {
        $log.debug('badge set');
        window.plugins.pushNotification.setApplicationIconBadgeNumber(successHandler, errorHandler, event.badge);
      }
    };

    $window.onNotification = function(e) {
      $log.debug('event received');

      switch( e.event ) {
        case 'registered':
          if ( e.regid.length > 0 ) {
            $log.debug('token registered');
            // Your GCM push server needs to know the regID before it can push to this device
            // here is where you might want to send it the regID for later use.
            $log.debug("regID = " + e.regid);
            var object = {
              deviceos: 2,
              deviceid: device.uuid,
              providertoken: e.regid
            };
            registerDevice(object);
          }
          break;

        case 'message':
          // if this flag is set, this notification happened while we were in the foreground.
          // you might want to play a sound to get the user's attention, throw up a dialog, etc.
          if ( e.foreground ) {
            $log.debug('inline notification fired');

            // on Android soundname is outside the payload.
            // On Amazon FireOS all custom attributes are contained within payload
            var soundfile = e.soundname || e.payload.sound;
            // if the notification contains a soundname, play it.
            var my_media = new Media("/android_asset/www/"+ soundfile);
            my_media.play();
          } else {  // otherwise we were launched because the user touched a notification in the notification tray.
            if ( e.coldstart ) {
              $log.debug('coldstart notification');
            } else {
              $log.debug('background notification');
            }
          }

          $log.debug('message: ' + e.payload.message);
          //Only works for GCM
          $log.debug('message count: ' + e.payload.msgcnt);
          break;

        case 'error':
          $log.debug('error: '+ e.msg);
          break;

        default:
          $log.debug('unknown event received');
          break;
        }
      };

      return Service;
    }
 ]);

