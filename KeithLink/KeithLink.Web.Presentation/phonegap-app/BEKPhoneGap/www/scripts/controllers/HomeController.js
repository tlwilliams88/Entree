'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$state', '$modal', '$filter', 'CartService', 'OrderService', 'MarketingService', 'NotificationService', 'CustomerService', 'Constants',
    function($scope, $state, $modal, $filter, CartService, OrderService, MarketingService, NotificationService, CustomerService, Constants) {
    
    // get carts
    CartService.getCartHeaders().then(function(carts) {
      $scope.homeCarts = CartService.carts;
    });

    // get orders
    $scope.loadingOrders = true;
    OrderService.getOrders({
      from: 0,
      size: 6,
      sort:  [{
        field: 'createddate',
        order: 'desc'
      }]
    }).then(function(data) {
      $scope.orders = data.results;
      delete $scope.ordersMessage;
    }, function(error) {
      $scope.ordersMessage = 'Error loading orders.';
    }).finally(function() {
      $scope.loadingOrders = false;
    });

    // get promo/marketing items
    $scope.loadingPromoItems = true;
    MarketingService.getPromoItems().then(function(items) {
      $scope.promoItems = items;
      delete $scope.promoMessage;
    }, function(error) {
      $scope.promoMessage = 'Error loading promo items.';
    }).finally(function() {
      $scope.loadingPromoItems = false;
    });

    // get account info
    $scope.loadingAccountBalance = true;
    CustomerService.getAccountBalanceInfo().then(function(data) {
      $scope.selectedUserContext.customer.balance = data.balance;
      $scope.selectedUserContext.customer.lastorderupdate = data.lastorderupdate;
      $scope.loadingAccountBalance = false;
    });
 
    // $scope.createNewCart = function() {
    //   return CartService.createCart().then(function(cartId) {
    //     CartService.setActiveCart(cartId);
    //     $state.go('menu.cart.items', {cartId: cartId, renameCart: true});
    //   });
    // };

    $scope.showPromoItemContent = function(promoItem) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/promoitemcontentmodal.html',
        controller: 'PromoItemContentModalController',
        resolve: {
          promoItem: function() {
            return promoItem;
          }
        }
      });
    };

    $scope.openQuickAddModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/cartquickaddmodal.html',
        controller: 'CartQuickAddModalController'
      });

      modalInstance.result.then(function(cartId) {
        $state.go('menu.cart.items', {cartId: cartId});
      });
    };

    $scope.openOrderImportModal = function () {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/orderimportmodal.html',
        controller: 'ImportModalController'
      });
    };

    $scope.showAdditionalInfo = function(notification) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/notificationdetailsmodal.html',
        controller: 'NotificationDetailsModalController',
        windowClass: 'color-background-modal',
        scope: $scope,
        resolve: {
          notification: function() {
            return notification;
          }
        }
      });
    };

    $scope.notificationParams = {
      size: 6,
      from:0,
      sort: [{
        field: 'messagecreatedutc',
        order: 'desc'
      }]
    };

    $scope.loadingRecentActivity = true;
      NotificationService.getMessages($scope.notificationParams).then(function(data) {
        var notifications =data.results,
        notificationDates ={},
        dates = [];
        notifications.forEach(function(notification){
         var date = moment(notification.messagecreatedutc).format('YYYY-MM-DD');
         if(notificationDates[date]){
          notificationDates[date].push(notification);
          }
          else{
            dates.push(date);
            notificationDates[date] = [notification];
          }          
         
        });
      $scope.notificationDates = notificationDates;
      $scope.dates = dates;      
      $scope.loadingRecentActivity = false;
    });

  }]);