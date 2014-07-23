'use strict';

/**
 * @ngdoc function
 * @name bekApp.directive:pageHeaderBar
 * @description
 * loops through items in a list and creates the carousel slides for mobile devices
 * 
 * Inputs 
 * message: title or message displayed at the top of the page
 * this directive will also tranclude whatever is included within the directive tags
 * 
 * Example
 * <div page-header-bar message="'Product Catalog'">
    <button class="btn page-header__export-button"><span class="icon-printer"></span></button>
    <button class="btn page-header__export-button"><span class="icon-disk"></span></button>
   </div>
 */
angular.module('bekApp')
  .directive('pageHeaderBar', function () {
    
    return {
      restrict: 'A',
      transclude: true,
      scope: {
        pageTitle: '=message'
      },
      templateUrl: 'views/directives/pageheaderbar.html'
    };

  });