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
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['identify', emailAddress ]);
                }
            }catch(ex){}
        },
        
        tagEmail: function(emailAddress){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', {email: emailAddress }]);
                    __insp.push(['tagSession', 'user=' + emailAddress ]);
                }
            }catch(ex){}
        },
        
        tagCustomer: function(customernumber){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'customer=' + customernumber ]);
                }
            }catch(ex){}
        },
        
        tagBranch: function(branchId){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'branch=' + branchId ]);
                }
            }catch(ex){}
        },
        
        tagEnvironment: function(){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'environment=' + ENV.name ]);
                }
            }catch(ex){}
        },
        
        tagOrder: function(orderNumber){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'order' ]);
                    __insp.push(['tagSession', 'order=' + orderNumber ]);
                }
            }catch(ex){}
        },
        
        tagChangeOrder: function(orderNumber){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'changeorder' ]);
                    __insp.push(['tagSession', 'order=' + orderNumber ]);
                    __insp.push(['tagSession', 'changeorder=' + orderNumber ]);
                }
            }catch(ex){}
        },
        
        tagCampaignView: function(campaignName){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'campaign' ]);
                    __insp.push(['tagSession', 'campaign=' + campaignName ]);
                }
            }catch(ex){}
        },
        
        tagSearch: function(campaignName){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'search=' + campaignName ]);
                }
            }catch(ex){}
        },
        
        tagAddRecommendedItem: function(item){
            try{ // do not let this be critical
                if(meetsRequirements()){
                    __insp.push(['tagSession', 'addRecommendedItem']);
                    __insp.push(['tagSession', 'addRecommendedItem=' + item ]);
                }
            }catch(ex){}
        }
        
    };
    function meetsRequirements() {
        var isMobileApp = ENV.mobileApp;

//        return ((ENV.name == 'test' || ENV.name == 'prod') && isMobileApp == false);
        return (isMobileApp == false);
      }
      return Service;

}]);