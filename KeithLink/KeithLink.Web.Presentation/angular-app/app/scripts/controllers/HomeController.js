'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$rootScope', '$state', '$stateParams', '$modal', '$filter', 'Constants', 'CartService', 'OrderService', 'MarketingService', 'DateService', 'NotificationService', 'CustomerService', 'isHomePage', 'LocalStorage', 'UtilityService', 'ENV', 'ListService',
    function($scope, $rootScope, $state, $stateParams, $modal, $filter, Constants, CartService, OrderService, MarketingService, DateService, NotificationService, CustomerService, isHomePage, LocalStorage, UtilityService, ENV, ListService) {

    $scope.isHomePage = isHomePage;

    var isMobile = UtilityService.isMobileDevice();
    var isMobileApp = ENV.mobileApp;
    var hideTutorial = LocalStorage.getHideTutorialHomePage();
    $scope.runTutorial =  hideTutorial || isMobileApp || isMobile ? false : true;

    CartService.getCartHeaders();

    ListService.getListHeaders();

    OrderService.getChangeOrders();

    $scope.setHideTutorial = function(){
      LocalStorage.setHideTutorialHomePage(true);
    };

    $scope.menuOnboardingSteps = [
      {
        title: "New Menu Location",
        position: "bottom",
        description: "Where did the menu go? <br/><br/> In order to give you more space to work we've hidden the menu.  <br/><br/> When you need it click on the menu icon in the top left corner.",
        attachTo: "#menuIcon",
        width: 400
      }
    ];

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
      if(data){
        $scope.selectedUserContext.customer.balance = data.balance ? data.balance : 0;
        $scope.selectedUserContext.customer.lastorderupdate = data.lastorderupdate ? data.lastorderupdate : '';
      }
    }, function(errorMessage) {
      $scope.accountBalanceMessage = errorMessage;
    }).finally(function() {
      $scope.loadingAccountBalance = false;
    });

    $scope.hidePayNowButton = ($scope.selectedUserContext.customer.termcode === '50' || $scope.selectedUserContext.customer.termcode === '51');

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
        field: 'messagecreated',
        order: 'desc'
      }]
    };

    $scope.loadingRecentActivity = true;
      NotificationService.getMessages($scope.notificationParams).then(function(data) {
        var notifications =data.results,
        notificationDates ={},
        dates = [];
        notifications.forEach(function(notification){
         var date = DateService.momentObject(notification.messagecreated).format(Constants.dateFormat.yearMonthDayDashes);

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
