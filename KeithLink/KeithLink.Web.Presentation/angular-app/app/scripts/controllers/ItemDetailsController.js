'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:ProductDetailsController
 * @description
 * # ProductDetailsController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('ItemDetailsController', ['$scope', '$modal', 'item', 'ProductService', 'ListService', 'CartService', 
    function ($scope, $modal, item, ProductService, ListService, CartService) {
    
    var originalItemNotes = item.notes;

    $scope.item = item;
    $scope.item.quantity = 1;
    
    ProductService.getProductDetails(item.itemnumber).then(function(item) {
      $scope.item.productimages = item.productimages;
    });

    ProductService.saveRecentlyViewedItem(item.itemnumber);

    // TODO: move into context menu controller
    $scope.lists = ListService.lists;
    ListService.getAllLists({
      'header': true
    });

    $scope.carts = CartService.carts;
    CartService.getAllCarts({
      'header': true
    });

    $scope.canOrderProduct = function(item) {
      return ProductService.canOrderProduct(item);
    };

    $scope.openContextMenu = function (e, item) {

      // if (window.innerWidth <= 991) {
        var modalInstance = $modal.open({
          templateUrl: 'views/contextmenumodal.html',
          controller: 'ContextMenuModalController',
          resolve: {
            lists: function () {
              return $scope.lists;
            },
            carts: function () {
              return $scope.carts;
            },
            item: function() {
              return item;
            }
          }
        });  
      // } else {
      //   $scope.displayedItems = {};
      //   $scope.displayedItems.isContextMenuDisplayed = true;
      // }

      

      // modalInstance.result.then(function (object) {
      //   $scope.selected = object;
      // }, function () {
      //   console.log('Modal dismissed at: ' + new Date());
      // });
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