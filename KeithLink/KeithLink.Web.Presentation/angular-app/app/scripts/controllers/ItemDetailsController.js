'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$modal', 'item', 'ProductService', 'ListService', 'CartService', 'OrderService',
    function ($scope, $modal, item, ProductService, ListService, CartService, OrderService) {
    
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;
    
    ProductService.getProductDetails(item.itemnumber).then(function(item) {
      $scope.item = item;
      $scope.item.quantity = 1;
      // $scope.item.orderhistory = {
      //      "10/31/2014": 1,
      //      "11/2/2014": 2,
      //      "11/3/2014": 4,
      //      "11/4/2014": 7,
      //      "11/5/2014": 10
      //  };
      // $scope.item.orderhistory = null;
    });

    ProductService.saveRecentlyViewedItem(item.itemnumber);

    // TODO: move into context menu controller
    $scope.lists = ListService.lists;
    ListService.getListHeaders();

    $scope.carts = CartService.carts;
    CartService.getCartHeaders();

    OrderService.getChangeOrders().then(function(orders) {
      $scope.changeOrders = orders;
    });

    $scope.canOrderProduct = function(item) {
      return ProductService.canOrderProduct(item);
    };

    $scope.openNotesModal = function (item) {

      var modalInstance = $modal.open({
        templateUrl: 'views/itemnotesmodal.html',
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