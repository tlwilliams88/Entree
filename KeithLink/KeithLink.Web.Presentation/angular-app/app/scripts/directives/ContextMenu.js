'use strict';

/*
to be used in conjunction with contextMenuTemplate directive
*/

angular.module('bekApp')
.directive('contextMenu', [ function(){
  return {
    restrict: 'A',
    // scope: true,
    controller: ['$scope', '$rootScope', '$state', '$q', '$modal', 'toaster', 'ListService', 'CartService', 'OrderService', 'ContextMenuService', '$filter', 'AnalyticsService', 'Constants', '$stateParams', 'PricingService',
    function($scope, $rootScope, $state, $q, $modal, toaster, ListService, CartService, OrderService, ContextMenuService, $filter, AnalyticsService, Constants, $stateParams, PricingService){

      $scope.currentLocation = $state.current.name;
      $scope.recommendationType = $stateParams.recommendationType;
      $scope.canOrderItem = PricingService.canOrderItem;

      if ($scope.isOrderEntryCustomer) {
        $scope.lists = ListService.listHeaders;
        $scope.changeOrders = OrderService.changeOrderHeaders;

      if($scope.changeOrders == null || $scope.changeOrders.length == 0) {
          OrderService.getChangeOrders().then(function(resp){
            $scope.changeOrders = resp;
          });
      }

        if ($scope.canCreateOrders) {
          CartService.getShipDates(); // needed if user creates a cart using the context menu

          $scope.carts = $scope.cartsHeaders;

        }

      }

      function closeModal() {
        $scope.$broadcast('closeContextMenu');

        // if (ContextMenuService.menuElement) {
        //   ContextMenuService.menuElement.removeClass('open');
        // }

        if (ContextMenuService.modalElement) {
          ContextMenuService.modalElement.close();
        }
      }

      /*************
      LISTS
      *************/

      $scope.addItemToList = function(item, selectedList) {
      var newItem = angular.copy(item);
        if(selectedList && (selectedList.iscustominventory || $scope.isCustomInventoryList)){
          var newItem = [
            item
          ];
          $q.all([
            ListService.addNewItemsFromCustomInventoryList(selectedList, newItem),
          ]).then(function(data) {
            item.favorite = true;
            closeModal();
          });
        } else {
          closeModal();

          if(item.favorite == false) {
              item.favorite = true;              
              $q.all([
                ListService.addItem(selectedList, newItem),
                ListService.addItemToFavorites(newItem)
              ]);
          } else {
              ListService.addItem(selectedList, newItem);
          }
        }
      };

      $scope.createListWithItem = function(item) {
        var newListItem = item;
        $q.all([
          ListService.createList(newListItem, {isCustomInventory: $scope.isCustomInventoryList}),
          ListService.addItemToFavorites(newListItem)
        ]).then(function(data) {
          ListService.getListHeaders().then(function(listheaders){
            $rootScope.listHeaders = listheaders;
          })
          $rootScope.$broadcast('ListCreatedFromContextMenu');
          closeModal();
        });
      };

      /*************
      CARTS
      *************/

      $scope.addItemToCart = function(cartName, cartId, item) {
        var newItem = angular.copy(item);

        if($stateParams.recommendationType && $stateParams.recommendationType != undefined && newItem.orderedfromsource == null) {
          newItem.orderedfromsource = $stateParams.recommendationType;
          newItem.trackingkey = $stateParams.trackingkey;
        }
        CartService.addItemToCart(cartId, newItem).then(function(data) {
          closeModal();
          $scope.displayMessage('success', 'Successfully added item to cart ' + cartName + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to cart ' + cartName + '.');
        });
      };

      $scope.createCartWithItem = function(item) {
        if($stateParams.recommendationType && $stateParams.recommendationType != undefined && item.orderedfromsource == null) {
          item.orderedfromsource = $stateParams.recommendationType;
          item.trackingkey = $stateParams.trackingkey;
        }

        var items = [item];
        CartService.renameCart = true;
        
        AnalyticsService.recordCheckout(null, 
                                        Constants.checkoutSteps.CreateCart, // step
                                        "From Context Menu"); //option

        var contractList = $filter('filter')($scope.lists, { type: 2 }),
            historyList = $filter('filter')($scope.lists, { type: 5 }),
            favoritesList = $filter('filter')($scope.lists, { type: 1 }),
            customList = $filter('filter')($scope.lists, { type: 0 }),
            listToBeUsed = {};

        if(contractList && contractList.length > 0){
          listToBeUsed = contractList[0];
        }
        else if(historyList && historyList.length > 0){
          listToBeUsed = historyList[0];
        }
        else if(favoritesList && favoritesList.length > 0){
          listToBeUsed = favoritesList[0];
        }
        else if(customList && customList.length > 0){
          listToBeUsed = customList[0];
        }
        
        CartService.createCart(items, null, null, null, listToBeUsed.listid, listToBeUsed.type).then(function(data) {
          closeModal();
          $scope.displayMessage('success', 'Successfully created new cart ' + data.name + '.');
        }, function() {
          $scope.displayMessage('error', 'Error creating new cart.');
        });
      };

      /*************
      CHANGE ORDERS
      *************/

       $scope.addItemToChangeOrder = function(order, item) {
        var orderItem = angular.copy(item);

        if($stateParams.recommendationType && $stateParams.recommendationType != undefined && item.orderedfromsource == null) {
          orderItem.orderedfromsource = $stateParams.recommendationType;
          orderItem.trackingkey = $stateParams.trackingkey;
        }
        orderItem.quantity = (orderItem.quantity && orderItem.quantity > 0) ? orderItem.quantity : 1;
        orderItem.each = (orderItem.each) ? true : false;
        order.items.push(orderItem);

        var params = {
          deleteOmitted: false
        };

        OrderService.updateOrder(order, params).then(function(data) {
          closeModal();
          $scope.displayMessage('success', 'Successfully added item to Order #' + order.invoicenumber + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding item to Order #' + order.invoicenumber + '.');
        });
      };

    }]
  };
}]);
