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
            if(emailAddress != null && meetsRequirements()){
                __insp.push(['identify', emailAddress ]);
            }
        },
        
        tagEmail: function(emailAddress){
            if(emailAddress != null && meetsRequirements()){
                __insp.push(['tagSession', {email: emailAddress }]);
                __insp.push(['tagSession', 'user=' + emailAddress ]);
            }
        },
        
        tagCustomer: function(customernumber){
            if(customernumber != null && meetsRequirements()){
                __insp.push(['tagSession', 'customer=' + customernumber ]);
            }
        },
        
        tagBranch: function(branchId){
            if(branchId != null && meetsRequirements()){
                __insp.push(['tagSession', 'branch=' + branchId ]);
            }
        },
        
        tagEnvironment: function(){
            if(meetsRequirements()){
                __insp.push(['tagSession', 'environment=' + ENV.name ]);
            }
        },
        
        tagOrder: function(orderNumber){
            if(orderNumber != null && meetsRequirements()){
                __insp.push(['tagSession', 'order' ]);
                __insp.push(['tagSession', 'order=' + orderNumber ]);
            }
        },
        
        tagChangeOrder: function(orderNumber){
            if(orderNumber != null && meetsRequirements()){
                __insp.push(['tagSession', 'changeorder' ]);
                __insp.push(['tagSession', 'order=' + orderNumber ]);
                __insp.push(['tagSession', 'changeorder=' + orderNumber ]);
            }
        },
        
        tagCampaignView: function(campaignName){
            if(campaignName != null && meetsRequirements()){
                __insp.push(['tagSession', 'campaign' ]);
                __insp.push(['tagSession', 'campaign=' + campaignName ]);
            }
        },
        
        tagSearch: function(searchName){
            if(searchName != null && meetsRequirements()){
                __insp.push(['tagSession', 'search=' + searchName ]);
            }
        },
        
        tagAddRecommendedItem: function(item){
            if(item != null && meetsRequirements()){
                __insp.push(['tagSession', 'addRecommendedItem']);
                __insp.push(['tagSession', 'addRecommendedItem=' + item ]);
            }
        }
        
    };
    function meetsRequirements() {
        var isMobileApp = ENV.mobileApp;

        return (__insp != null && (ENV.name == 'test' || ENV.name == 'prod') && isMobileApp == false);
      }
      return Service;

}]);