'use strict';

angular.module('bekApp')
    .factory('PhonegapPushService', ['$window',
        function($window){
            var Service = {};
            console.log('SERVICE CREATED');
            Service.register = function () {
                console.log('register call');
                if ( device.platform == 'android' || device.platform == 'Android' || device.platform == "amazon-fireos" ){
                    console.log('android register');
                    window.plugins.pushNotification.register(
                    successHandler,
                    errorHandler,
                    {
                        "senderID":"replace_with_sender_id",
                        "ecb":"onNotification"
                    });
                } else if ( device.platform == 'blackberry10'){
                    console.log('blackberry register');
                    window.plugins.pushNotification.register(
                    successHandler,
                    errorHandler,
                    {
                        invokeTargetId : "replace_with_invoke_target_id",
                        appId: "replace_with_app_id",
                        ppgUrl:"replace_with_ppg_url", //remove for BES pushes
                        ecb: "pushNotificationHandler",
                        simChangeCallback: replace_with_simChange_callback,
                        pushTransportReadyCallback: replace_with_pushTransportReady_callback,
                        launchApplicationOnPush: true
                    });
                } else {
                    console.log('iOS register');
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
            $window.tokenHandler = function(success) {
                console.log('tokenHandler: ' + success);
            };

            //Android/Windows/FireOS callback
            $window.successHandler = function (success) {
                console.log('successHandler: ' + success);
            };

            //error for both android and iOS
            $window.errorHandler = function (error) {
                console.log('errorHandler: ' + error);
            };

            return Service;
        }
    ]);

