'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:featuredItemCarousel
 * @description
 * loops through items in a list and creates the carousel slides for mobile devices
 *
 * Inputs
 * items: array of objects, each object is a slide, the description becomes the slide text and the imageUrl is used for the image
 * link: the angular ui router ui-sref attribute value for the slide link, if passing a url paramater, YOU MUST USE 'item' AS THE PARENT OBJECT (see example)
 *
 * Example
 * <div featured-item-carousel items="items" link="'menu.catalog.products.details({ itemNumber : item.itemnumber })'"></div>
 */
angular.module('bekApp')
  .directive('featuredItemCarousel', ['$rootScope', 'MarketingService', 'UtilityService', function ($rootScope, MarketingService, UtilityService) {

    return {
      restrict: 'A',
      scope: {
        itemList: '=featuredItemCarousel',
        template: '@template',
        attribute: '@attr'
      },
      transclude: true,
      templateUrl: function(scope, elem) {
        // if(elem.$attr.catalog == 'catalog'){
        //   return 'views/directives/featureditemcarouselcatalog.html';
        // } else {
          return 'views/directives/featureditemcarousel.html';
        // }
      },
      link: function(scope, elem, attr) {
        scope.openExternalLink = $rootScope.openExternalLink;
        scope.isCatalog = attr.catalog == 'true' ? true : false;
        scope.isMarketing = attr.marketing == 'true' ? true : false;
        scope.isMobile = UtilityService.isMobileDevice();
        scope.storePromoItemInformation = function(targeturltext, id){
          MarketingService.storeMarketingUserInteractionInformation(targeturltext, id);
        };
      }
    };

  }]);
