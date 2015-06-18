'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$state', '$modal', '$filter', 'CartService', 'OrderService', 'MarketingService', 'NotificationService', 'CustomerService',
    function($scope, $state, $modal, $filter, CartService, OrderService, MarketingService, NotificationService, CustomerService) {
    
    if(CartService.cartHeaders.length > 0){
      $scope.cartHeaders = CartService.cartHeaders;
    }
    else{
      CartService.getCartHeaders().then(function(cartHeaders){
        $scope.cartHeaders = cartHeaders;
      })
    }

    // get orders
    $scope.orders = [];
    if ($scope.canViewOrders) {
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
      }, function(errorMessage) {
        $scope.ordersMessage = errorMessage;
      }).finally(function() {
        $scope.loadingOrders = false;
      });
    }
 
    // get promo/marketing items
    $scope.loadingPromoItems = true;
    MarketingService.getPromoItems().then(function(items) {
      $scope.promoItems = items;
      delete $scope.promoMessage;
    }, function(errorMessage) {
      $scope.promoMessage = errorMessage;
    }).finally(function() {
      $scope.loadingPromoItems = false;
    });
 
    // get account info
    $scope.loadingAccountBalance = true;
    CustomerService.getAccountBalanceInfo().then(function(data) {
      $scope.selectedUserContext.customer.balance = data.balance;
      $scope.selectedUserContext.customer.lastorderupdate = data.lastorderupdate;
    }, function(errorMessage) {
      $scope.accountBalanceMessage = errorMessage;
    }).finally(function() {
      $scope.loadingAccountBalance = false;
    });
 
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