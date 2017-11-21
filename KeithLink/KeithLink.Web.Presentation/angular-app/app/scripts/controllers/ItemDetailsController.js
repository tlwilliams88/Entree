'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$state', '$modal', 'item', 'ProductService', 'AccessService', 'PricingService', 'AnalyticsService',
    function ($scope, $state, $modal, item, ProductService, AccessService, PricingService, AnalyticsService) {
    
    if(!item){
      $state.go('menu.home');
    }
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;

    $scope.item.orderHistoryKeys = Object.keys(item.orderhistory);

    $scope.canOrderItemInd = PricingService.canOrderItem(item);
    $scope.casePriceInd = PricingService.hasCasePrice(item);
    $scope.packagePriceInd = PricingService.hasPackagePrice(item);
    
    AnalyticsService.recordViewDetail($scope.item);

    if(!(item.is_specialty_catalog || item.isvalid === false)){
      ProductService.saveRecentlyViewedItem(item.itemnumber);
    }

    $scope.openNotesModal = function (item) {

      var modalInstance = $modal.open({
        templateUrl: 'views/modals/itemnotesmodal.html',
        controller: 'ItemNotesModalController',
        resolve: {
          item: function() {
            return angular.copy(item);
          }
        }
      });

      modalInstance.result.then(function(item) {
        $scope.item = item;
      });
    };

  }]);