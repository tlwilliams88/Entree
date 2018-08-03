'use strict';

angular.module('bekApp')
.controller('ContextMenuModalController', ['$scope', '$state', '$modalInstance', 'item', 'CartService', 'carts', 'lists', 'changeOrders', 'recommendationType', 'trackingkey', '$q', 'ListService', '$rootScope', 'OrderService', 'AnalyticsService', 'Constants', '$filter', 'SessionRecordingService',
  function ($scope, $state, $modalInstance, item, CartService, carts, lists, changeOrders, recommendationType, trackingkey, $q, ListService, $rootScope, OrderService, AnalyticsService, Constants, $filter, SessionRecordingService) {

	$scope.currentLocation = $state.current.name;
	
	$scope.item = item;

	$scope.lists = lists;

	$scope.carts = carts;

	$scope.changeOrders = changeOrders;

	CartService.getShipDates(); // needed if user creates a cart using the context menu
	
	/*************
    LISTS
    *************/

	 $scope.addItemToList = function(item, selectedList) {
		var newItem = angular.copy(item);
		  if(selectedList && selectedList.iscustominventory){
			var newItem = [
			  item
			];
			$q.all([
			  ListService.addNewItemsFromCustomInventoryList(selectedList, newItem),
			]).then(function(data) {
			  item.favorite = true;
			  $scope.cancel();
			});
		  } else {
			$scope.cancel();
  
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
			ListService.createList(newListItem),
			ListService.addItemToFavorites(newListItem)
		  ]).then(function(data) {
			ListService.getListHeaders().then(function(listheaders){
			  $rootScope.listHeaders = listheaders;
			})
			$rootScope.$broadcast('ListCreatedFromContextMenu');
			$scope.cancel();
		  });
		};
  
		/*************
		CARTS
		*************/
  
		$scope.addItemToCart = function(cartName, cartId, item) {
			var newItem = angular.copy(item);
	
			if(recommendationType && recommendationType != undefined && newItem.orderedfromsource == null) {
			  newItem.orderedfromsource = recommendationType;
				newItem.trackingkey = trackingkey;
				
				SessionRecordingService.tagAddRecommendedItem(recommendationType + ';' + trackingkey + '=' + item.itemNumber);
			}
			CartService.addItemToCart(cartId, newItem).then(function(data) {
			  $scope.cancel();
			  $scope.displayMessage('success', 'Successfully added item to cart ' + cartName + '.');
			}, function() {
			  $scope.displayMessage('error', 'Error adding item to cart ' + cartName + '.');
			});
		};
  
		$scope.createCartWithItem = function(item) {
		  if(recommendationType && recommendationType != undefined && item.orderedfromsource == null) {
			item.orderedfromsource = recommendationType;
			item.trackingkey = trackingkey;
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
		  
			SessionRecordingService.tagAddRecommendedItem(recommendationType + ';' + trackingkey + '=' + items[0].itemNumber);

			CartService.createCart(items, null, null, null, listToBeUsed.listid, listToBeUsed.type).then(function(data) {
			$scope.cancel();
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
  
		  if(recommendationType && recommendationType != undefined && newItem.orderedfromsource == null) {
			orderItem.orderedfromsource = recommendationType;
			orderItem.trackingkey = trackingkey;
		  }
		  orderItem.quantity = (orderItem.quantity && orderItem.quantity > 0) ? orderItem.quantity : 1;
		  orderItem.each = (orderItem.each) ? true : false;
		  order.items.push(orderItem);
  
		  var params = {
			deleteOmitted: false
		  };
  
		  OrderService.updateOrder(order, params).then(function(data) {
			$scope.cancel();
			$scope.displayMessage('success', 'Successfully added item to Order #' + order.invoicenumber + '.');
		  }, function() {
			$scope.displayMessage('error', 'Error adding item to Order #' + order.invoicenumber + '.');
		  });
		};

	$scope.cancel = function () {
		$modalInstance.dismiss('cancel');
	};

}]);