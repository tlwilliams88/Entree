'use strict';

angular.module('bekApp')
.controller('ContextMenuModalController', ['$scope', '$modalInstance', 'item', 'ListService', 'CartService', 'OrderService',
  function ($scope, $modalInstance, item, ListService, CartService, OrderService) {

	$scope.item = item;

	$scope.cancel = function () {
		$modalInstance.dismiss('cancel');
	};

	ListService.getListHeaders().then(function(lists) {
		$scope.lists = lists;
	});

	CartService.getShipDates(); // needed if user creates a cart using the context menu

	CartService.getCartHeaders().then(function(carts) {
		$scope.carts = carts;
	});

	OrderService.getChangeOrders().then(function(orders) {
		$scope.changeOrders = orders;
	});

}]);