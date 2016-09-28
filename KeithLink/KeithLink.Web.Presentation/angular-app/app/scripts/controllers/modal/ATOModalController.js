'use strict';

angular.module('bekApp')
.controller('ATOModalController', ['$scope', '$modalInstance', 'CartService', 'ListService', 'LocalStorage', 'UtilityService', 'SessionService',
  function ($scope, $modalInstance, CartService, ListService, LocalStorage, UtilityService, SessionService) {

  $scope.currentCustomer = LocalStorage.getCurrentCustomer();

  ListService.getListHeaders().then(function(resp){
    $scope.lists = resp;
  })

  CartService.getShipDates().then(function(resp){
    $scope.shipDates = resp;
    $scope.selectedCart.requestedshipdate = $scope.shipDates[0].shipdate;
  })

  CartService.getCartHeaders().then(function(cartHeaders){
    $scope.cartHeaders = cartHeaders;
  });

  $scope.selectedCart = {
    name: UtilityService.generateName(SessionService.userProfile.firstname, $scope.cartHeaders),
    requestedshipdate: '',
    ponumber: ''
  };
  $scope.selectedList = {
    name: 'History',
    listid: ''
  };

  $scope.createCart = function(cart) {

    CartService.createCart(cart.items, cart.requestedshipdate, cart.name, cart.ponumber).then(function(cart) {
      cart.listid = $scope.selectedList.listid;
      $modalInstance.close(cart);
      $scope.displayMessage('success', 'Successfully created new cart.');
    }, function(error) {
      $scope.displayMessage('error', error);
    });

  };

  $scope.close = function () {
    $modalInstance.dismiss('cancel');
  };

}]);