'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$state', '$modal', 'item', 'ProductService', 'AccessService', 'PricingService',
    function ($scope, $state, $modal, item, ProductService, AccessService, PricingService) {
    
    if(!item){
      $state.go('menu.home');
    }
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;

    $scope.canOrderItemInd = PricingService.canOrderItem(item);
    $scope.casePriceInd = PricingService.hasCasePrice(item);
    $scope.packagePriceInd = PricingService.hasPackagePrice(item);
    ProductService.getProductDetails(item.itemnumber, $scope.$state.params.catalogType).then(function(item) {
      $scope.item = item;
      $scope.item.quantity = 1;

       // used to determine if the item has order history in the view
      $scope.item.orderHistoryKeys = Object.keys(item.orderhistory);
    });

    if(!item.is_specialty_catalog){
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