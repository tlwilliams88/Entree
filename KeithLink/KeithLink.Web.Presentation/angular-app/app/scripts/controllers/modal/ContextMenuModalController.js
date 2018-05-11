'use strict';

angular.module('bekApp')
.controller('ContextMenuModalController', ['$scope', '$modalInstance', 'item', 'CartService', 'carts', 'lists', 'changeOrders',
  function ($scope, $modalInstance, item, CartService, carts, lists, changeOrders) {

	$scope.item = item;

	$scope.lists = lists;

	$scope.carts = carts;

	$scope.changeOrders = changeOrders;

	CartService.getShipDates(); // needed if user creates a cart using the context menu

	$scope.cancel = function () {
		$modalInstance.dismiss('cancel');
	};

}]);