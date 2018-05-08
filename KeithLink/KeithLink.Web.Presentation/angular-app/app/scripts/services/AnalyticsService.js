'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:AnalyticsService
 * @description
 * # AnalyticsService
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('AnalyticsService', ['Analytics', 'SessionService', 'Constants', function(Analytics, SessionService, Constants) {

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
        
        recordTransaction: function(orderNumber, cart, customerNumber, customerBranch){

            var pieceCount = 0;
            var itemCount = cart.items.length;
            var tranPosition = 0;

            cart.items.forEach(function(item){
              pieceCount += item.quantity;
            });

            for (var i = 0; i < cart.items.length; i++){
              // The transaction in GA is our lineitem.  The reason is that
              // in their basic reports, they give a way to make a relationship
              // between a transaction and the list it came from.  We give a 
              // way to have each lineitem be drawn from a different list.
                var item = cart.items[i];

                item.price = item.caseprice && item.packageprice == '0.00' ? item.caseprice : item.packageprice;

                var cost = item.price;
                if (item.catchweight){
                  cost = item.price * item.average_weight
                }

                tranPosition++;

                // Add item to transaction
                Analytics.addProduct(item.itemnumber, 
                                     item.name, 
                                     item.class, 
                                     item.brand_extended_description, 
                                     '', 
                                     cost.toString(), 
                                     item.quantity, 
                                     '', 
                                     tranPosition);

            }

            Analytics.trackTransaction(orderNumber, 
                                       null, 
                                       null, 
                                       '', 
                                       '', 
                                       '', 
                                       null, 
                                       Constants.checkoutSteps.SubmitCart, 
                                       '');

            Analytics.trackEvent('Process', 
                                 'TH', 
                                 '', 
                                 '0', 
                                 true,
                                 {
                                    userId: SessionService.userProfile.displayRole + 
                                            '.' + 
                                            SessionService.userProfile.userid,
                                    dimension6: customerBranch,
                                    dimension7: customerNumber,
                                    dimension8: itemCount.toString(),
                                    dimension9: pieceCount.toString()
                                 });
        },
        
        recordCheckout: function(cart, step, option){
            var sendIndex = 0;
            if (cart != null){
              for (var i = 0; i < cart.items.length; i++){
                  var item = cart.items[i];

                  var price = 0.00; 
                  if(item.caseprice){
                    price = item.caseprice && item.packageprice == '0.00' 
                              ? item.caseprice 
                              : item.packageprice;
                  }

                  var cost = price;
                  if (item.catchweight){
                    cost = price * item.average_weight
                  }

                  // Add item to cart
                  Analytics.addProduct(item.itemnumber, 
                                       item.name, 
                                       item.class, 
                                       item.brand_extended_description, 
                                       '', 
                                       cost.toString(), 
                                       item.quantity, 
                                       '', 
                                       item.position);
                }
            }
            // Create Checkout Record
            Analytics.trackCheckout(step, option);

            Analytics.trackEvent('Process', 
                                 'TC', 
                                 step, 
                                 0, 
                                 true,
                                 {
                                    userId: SessionService.userProfile.displayRole + 
                                            '.' + 
                                            SessionService.userProfile.userid
                                 });
        },
        
        recordAddToCart: function(item, customerNumber, branchId){
            var addedFrom = '';

            if(SessionService.sourceProductList.length>0){
              addedFrom = SessionService.sourceProductList.pop();
              SessionService.sourceProductList.push(addedFrom);
            }

            // Add item being added
            Analytics.addProduct(item.itemnumber, 
                                 item.name, 
                                 item.class, 
                                 item.brand_extended_description, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            Analytics.set('dimension7', customerNumber);
            Analytics.set('dimension6', branchId);
            Analytics.set('userId', SessionService.userProfile.displayRole + 
                                    '.' + 
                                    SessionService.userProfile.userid);

            // Create Cart Record
            Analytics.trackCart('add', addedFrom);
        },
        
        recordRemoveItem: function(item, customerNumber, branchId){
            var removedFrom = item.sourceProductList;

            // Add item being removed
            Analytics.addProduct(item.itemnumber, 
                                 item.name, 
                                 item.class, 
                                 item.brand_extended_description, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            Analytics.set('dimension7', customerNumber);
            Analytics.set('dimension6', branchId);
            Analytics.set('userId', SessionService.userProfile.displayRole + 
                                    '.' + 
                                    SessionService.userProfile.userid);

            // Create Cart Record
            Analytics.trackCart('remove', removedFrom);
        },
        
        recordProductClick: function(customerNumber, branchId, item){
            var whatList = '';

            if(SessionService.sourceProductList.length>0){
              whatList = SessionService.sourceProductList.pop();
              SessionService.sourceProductList.push(whatList);
            }

            // Add Item Viewed
            Analytics.addProduct(item.itemnumber, 
                                 item.name, 
                                 item.class, 
                                 item.brand_extended_description, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            Analytics.set('dimension7', customerNumber);
            Analytics.set('dimension6', branchId);
            Analytics.set('userId', SessionService.userProfile.displayRole + 
                                    '.' + 
                                    SessionService.userProfile.userid);
            
            Analytics.productClick(whatList);
        },
        
        recordViewDetail: function(customerNumber, branchId, item){
            var whatList = '';

            if(SessionService.sourceProductList.length>0){
              whatList = SessionService.sourceProductList.pop();
              SessionService.sourceProductList.push(whatList);
            }

            // Add Item Viewed
            Analytics.addProduct(item.itemnumber, 
                                 item.name, 
                                 item.class, 
                                 item.brand_extended_description, 
                                 '', 
                                 item.price, 
                                 item.quantity, 
                                 '', 
                                 item.position);

            // inject customernumber and branch into detail hit
            Analytics.set('dimension7', customerNumber);
            Analytics.set('dimension6', branchId);
            Analytics.set('userId', SessionService.userProfile.displayRole + 
                                    '.' + 
                                    SessionService.userProfile.userid);
              
            Analytics.trackDetail();
            Analytics.set('list', whatList);
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
                                    item.brand_extended_description, 
                                    item.class, 
                                    item.packsize, 
                                    pageIndex, 
                                    item.caseprice.toString());
            renderedIndex++;
            if(renderedIndex>Constants.Analytics.HowManyProductsToThisChunk){
                  Analytics.trackEvent('Search', 
                                       'Listing', 
                                       '', 
                                       0, 
                                       true, 
                                       {
                                          dimension6: branchId,
                                          dimension7: customerNumber,
                                          userId: SessionService.userProfile.displayRole + 
                                                  '.' + 
                                                  SessionService.userProfile.userid
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
                                  dimension7: customerNumber,
                                  userId: SessionService.userProfile.displayRole + 
                                          '.' + 
                                          SessionService.userProfile.userid
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
                                  dimension7: customerNumber,
                                  userId: SessionService.userProfile.displayRole + 
                                          '.' + 
                                          SessionService.userProfile.userid
                               });
        },
        
        recordPromotionClick: function(id, name, creative, position, customerNumber, branchId){

            // Add promotion
            Analytics.addPromo(id, name, creative, position);

            // inject customernumber and branch into detail hit
            Analytics.set('dimension7', customerNumber);
            Analytics.set('dimension6', branchId);
            Analytics.set('userId', SessionService.userProfile.displayRole + 
                                    '.' + 
                                    SessionService.userProfile.userid);
            
            Analytics.promoClick(name);
        }
    };
    return Service;

}]);