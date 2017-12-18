'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:AnalyticsService
 * @description
 * # AnalyticsService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('AnalyticsService', ['$window', '$state', 'Analytics', 'SessionService', function($window, $state, Analytics, SessionService) {

    var Service = {

        setUserProperties: function(userName, roleName, isInternalUser, isKbitCustomer, isPowerMenuCustomer){
            //Angulartics Google Analytics provider doesn't support custom dimensions
            //so we have to use our other Analytics tracker to set these values
            Analytics.trackEvent('Process', 
                                 'Set User Properties', 
                                 '', 
                                 0, 
                                 true, 
                                 {
                                    userId: roleName + '.' + userName,
                                    dimension2: roleName,
                                    dimension3: isInternalUser,
                                    dimension4: isKbitCustomer,
                                    dimension5: isPowerMenuCustomer
                                 })
        },
        
        recordTransaction: function(customerName, orderNumber, cart, customerNumber, customerBranch){
            cart.items.forEach(function(item){
                item.price = item.caseprice && item.packageprice == '0.00' ? item.caseprice : item.packageprice;

                // Add item to transaction
                Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);
            });

            // Create transaction
            Analytics.trackTransaction(orderNumber, 
                                       customerBranch + '.' + customerNumber + '.' + customerName, 
                                       cart.subtotal, 
                                       '', 
                                       '', 
                                       '', 
                                       'ListId: ' + cart.listid, 
                                       'Cart Submission', 
                                       '');
        },
        
        recordCheckout: function(cart, step, option){
            if (cart != null){
                cart.items.forEach(function(item){
                    item.price = item.caseprice && item.packageprice == '0.00' ? item.caseprice : item.packageprice;

                    // Add item to cart
                    Analytics.addProduct(item.itemnumber, item.name, item.class, item.brand, '', item.price, item.quantity, '', item.position);
                });
            }

            // Create Checkout Record
            Analytics.trackCheckout(step, option);
        },

        recordAddToCart: function(item, customerNumber, branchId){
            var addedFrom = SessionService.sourceProductList.pop();
            SessionService.sourceProductList.push(addedFrom);

            // Add item being added
            Analytics.addProduct(item.itemnumber +
                                 "_" +
                                 addedFrom, 
                                 item.name, 
                                 item.class, 
                                 item.brand, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            $window.ga('set', 'dimension7', customerNumber);
            $window.ga('set', 'dimension6', branchId);

            // Create Cart Record
            Analytics.trackCart('add', addedFrom);
        },
        
        recordRemoveItem: function(item, customerNumber, branchId){
            var removedFrom = SessionService.sourceProductList.pop();
            SessionService.sourceProductList.push(removedFrom);

            // Add item being removed
            Analytics.addProduct(item.itemnumber +
                                 "_" +
                                 removedFrom, 
                                 item.name, 
                                 item.class, 
                                 item.brand, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            $window.ga('set', 'dimension7', customerNumber);
            $window.ga('set', 'dimension6', branchId);

            // Create Cart Record
            Analytics.trackCart('remove', removedFrom);
        },
        
        recordViewDetail: function(customerNumber, branchId, item){
            var whatList = SessionService.sourceProductList.pop();
            SessionService.sourceProductList.push(whatList);

            // Add Item Viewed
            Analytics.addProduct(item.itemnumber +
                                 "_" +
                                 whatList, 
                                 item.name, 
                                 item.class, 
                                 item.brand, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            $window.ga('set', 'dimension7', customerNumber);
            $window.ga('set', 'dimension6', branchId);
            
            Analytics.trackDetail();
        },

        setSelectedCustomer: function(customerNumber, branchId){
            // inject customernumber and branch into detail hit
            $window.ga('set', 'dimension7', customerNumber);
            $window.ga('set', 'dimension6', branchId);
        },

        recordSearchImpressions: function(products, customerNumber, branchId, listName){
          var renderedIndex = 0;
          var pageIndex = 0;
          products.forEach(function(item){
            pageIndex++;
            // Add Viewable Item
            Analytics.addImpression(item.itemnumber, 
                                    item.name, 
                                    listName, 
                                    item.brand, 
                                    item.class, 
                                    item.packsize, 
                                    pageIndex, 
                                    item.caseprice.toString());
            renderedIndex++;
            if(renderedIndex>20){
                  Analytics.trackEvent('Search', 
                                       'Listing', 
                                       '', 
                                       0, 
                                       true, 
                                       {
                                          dimension6: branchId,
                                          dimension7: customerNumber
                                       })
                  renderedIndex=0;
            }
          });
          Analytics.trackEvent('Search', 
                               'Listing', 
                               '', 
                               0, 
                               true, 
                               {
                                  dimension6: branchId,
                                  dimension7: customerNumber
                               })
        },
        
        recordPromotion: function(promoItems, customerNumber, branchId){
          promoItems.forEach(function(promoItem){
            // Add promotion
            Analytics.addPromo(promoItem.uri, 
                               promoItem.name, 
                               promoItem.enddate.toString(), 
                               '');
          });

            Analytics.trackEvent('Internal Promotions', 
                               'impressions', 
                               '', 
                               0, 
                               true, 
                               {
                                  'nonInteraction': 1,
                                  dimension6: branchId,
                                  dimension7: customerNumber
                               });
        },
        
        recordPromotionClick: function(id, name, creative, position, customerNumber, branchId){

            // Add promotion
            Analytics.addPromo(id, name, creative, position);

            // inject customernumber and branch into detail hit
            $window.ga('set', 'dimension7', customerNumber);
            $window.ga('set', 'dimension6', branchId);
            
            Analytics.promoClick(name);
        }
    };
    return Service;

}]);