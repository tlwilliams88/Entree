'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:SessionRecordingService
 * @description
 * # SessionRecordingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('SessionRecordingService', [function() {

    var Service = {

        identify: function(emailAddress){
            __insp.push(['identify', emailAddress ]);
        },
        
        tagEmail: function(emailAddress){
            __insp.push(['tagSession', {email: emailAddress }]);
        },
        
        tagCustomer: function(customernumber){
            __insp.push(['tagSession', customernumber ]);
        }
        
    };
    return Service;

}]);