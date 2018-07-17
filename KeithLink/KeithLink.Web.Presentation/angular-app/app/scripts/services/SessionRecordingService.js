'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:SessionRecordingService
 * @description
 * # SessionRecordingService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('SessionRecordingService', ['ENV',
      function(ENV) {

    var Service = {

        identify: function(emailAddress){
            if(meetsRequirements()){
                __insp.push(['identify', emailAddress ]);
            }
        },
        
        tagEmail: function(emailAddress){
            if(meetsRequirements()){
                __insp.push(['tagSession', {email: emailAddress }]);
                __insp.push(['tagSession', 'user=' + emailAddress ]);
            }
        },
        
        tagCustomer: function(customernumber){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'customer=' + customernumber ]);
            }
        },
        
        tagEnvironment: function(){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'environment=' + ENV.name ]);
            }
        },
        
        tagOrder: function(orderNumber){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'order=' + orderNumber ]);
            }
        },
        
        tagChangeOrder: function(orderNumber){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'order=' + orderNumber ]);
                __insp.push(['tagSession', 'changeorder=' + orderNumber ]);
            }
        },
        
        tagCampaignView: function(campaignName){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'campaign=' + campaignName ]);
            }
        },
        
        tagSearch: function(campaignName){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'search=' + campaignName ]);
            }
        },
        
        tagAddRecommendedItem: function(item){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'addRecommendedItem']);
                __insp.push(['tagSession', 'addRecommendedItem=' + item ]);
            }
        }
        
    };
    function meetsRequirements() {
        var isMobileApp = ENV.mobileApp;

//        return ((ENV.name == 'test' || ENV.name == 'prod') && isMobileApp == false);
        return (isMobileApp == false);
      }
      return Service;

}]);