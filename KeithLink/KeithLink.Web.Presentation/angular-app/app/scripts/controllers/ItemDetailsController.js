'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$modal', 'item', 'ProductService', 'PricingService',
    function ($scope, $modal, item, ProductService, PricingService) {
    
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;

    $scope.canOrderItemInd = PricingService.canOrderItem(item)
    $scope.casePriceInd = PricingService.hasCasePrice(item)
    $scope.packagePriceInd = PricingService.hasPackagePrice(item)
    
    ProductService.getProductDetails(item.itemnumber).then(function(item) {
      $scope.item = item;
      $scope.item.quantity = 1;

       // used to determine if the item has order history in the view
      $scope.item.orderHistoryKeys = Object.keys(item.orderhistory);
    });

    ProductService.saveRecentlyViewedItem(item.itemnumber);

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