'use strict';

angular.module('bekApp')
  .controller('MarketingController', ['$scope', 'MarketingService',
    function ($scope, MarketingService) {
    
      $scope.addContentItem = function(contentItem) {
        var testContentItem = {
          "branchid":"fdf",
          "tagline":"my first json tagline",
          "targeturltext":"targurltxt",
          "targeturl":"",
          "campaignid":"mcampid",
          "content":"some content goes here, yay!",
          "iscontenthtml":false,
          "productid":"908762",
          "activesdatestart":"11/1/2014",
          "activedateend":"1/1/2015"
        };

        testContentItem.imagedata = contentItem.image.base64;

        MarketingService.createItem(testContentItem);
      };

  }]);