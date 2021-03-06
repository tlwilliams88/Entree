'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$state', '$modal', 'item', 'ProductService', 'AccessService', 'PricingService', 'LocalStorage', 'AnalyticsService', 'ENV', 'Constants',
    function ($scope, $state, $modal, item, ProductService, AccessService, PricingService, LocalStorage, AnalyticsService, ENV, Constants) {
    
    if(!item){
      $state.go('menu.home');
    }
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;

    $scope.showRecommendedItems = ENV.showRecommendedItems;

    if($scope.showRecommendedItems == true) {
      var pagesize = ENV.isMobileApp == 'true' ? Constants.recommendedItemParameters.Mobile.pagesize : Constants.recommendedItemParameters.Desktop.ItemDetails.pagesize,
          getimages = ENV.isMobileApp == 'true' ? Constants.recommendedItemParameters.Mobile.getimages : Constants.recommendedItemParameters.Desktop.getimages;
      ProductService.getRecommendedItems($scope.item.itemnumber, pagesize, getimages).then(function(resp) {
        $scope.recommendedItems = resp;
      });
    }

    $scope.item.orderHistoryKeys = Object.keys(item.orderhistory);

    $scope.canOrderItemInd = PricingService.canOrderItem(item);
    $scope.casePriceInd = PricingService.hasCasePrice(item);
    $scope.packagePriceInd = PricingService.hasPackagePrice(item);
    
    AnalyticsService.recordProductClick(LocalStorage.getCustomerNumber(), 
                                        LocalStorage.getBranchId(),
                                        $scope.item);

    AnalyticsService.recordViewDetail(LocalStorage.getCustomerNumber(), 
                                      LocalStorage.getBranchId(),
                                      $scope.item);

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