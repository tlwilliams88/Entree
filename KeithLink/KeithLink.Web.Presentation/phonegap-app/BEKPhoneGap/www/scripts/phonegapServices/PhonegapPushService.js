'use strict';

angular.module('bekApp')
    .factory('PhonegapPushService', ['$window', '$http', '$q',
        function($window, $http, $q){
            var Service = {};
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

            var sendTo = function (token) {
                console.log('sendTo call');
                var deferred = $q.defer();

                var object = {};

                if(device.platform === 'iOS'){
                    object.deviceos = 1;
                }
                if(device.platform === 'Android'){
                    object.deviceos = 2;
                }

                object.deviceid = device.uuid;
                object.providertoken = token;
                console.log(object);
                $http.put('/messaging/registerpushdevice', object).then(function(response) {
                    console.log('got response:' + response);
                  var data = response.data;
                  if (data.successResponse) {
                      console.log('successful response from api');
                    deferred.resolve(data.successResponse);
                  } else {
                      console.log('error response from api');
                    deferred.reject(data.errorMessage);
                  }
                });
                return deferred.promise;
            };

            //iOS token handler callback
            $window.tokenHandler = function(success) {
                console.log('tokenhandler');
                sendTo(success).then(function (success) {
                    console.log('successful push to api');
                }, function (error) {
                    console.log('error pushing to api: ' + error);
                });
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

