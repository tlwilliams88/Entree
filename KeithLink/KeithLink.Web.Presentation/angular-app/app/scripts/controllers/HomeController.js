'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:HomeController
 * @description
 * # HomeController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('HomeController', [ '$scope', '$rootScope', '$state', '$stateParams', '$modal', '$filter', 'Constants', 'CartService', 'OrderService', 'MarketingService', 'DateService', 'NotificationService', 'CustomerService', 'isHomePage', 'LocalStorage', 'UtilityService', 'ENV', 'ListService', 'ProductService', 'CategoryService', 'BrandService', 'SessionService', 'AnalyticsService',
    function($scope, $rootScope, $state, $stateParams, $modal, $filter, Constants, CartService, OrderService, MarketingService, DateService, NotificationService, CustomerService, isHomePage, LocalStorage, UtilityService, ENV, ListService, ProductService, CategoryService, BrandService, SessionService, AnalyticsService) {

    AnalyticsService.setUserProperties(SessionService.userProfile.userid, 
                                       SessionService.userProfile.displayRole, 
                                       (SessionService.userProfile.emailaddress.indexOf('@benekeith.com') > -1).toString(), 
                                       SessionService.userProfile.iskbitcustomer.toString(), 
                                       SessionService.userProfile.ispowermenucustomer.toString());


    $scope.isHomePage = isHomePage;

    $scope.$on('$stateChangeStart',
      function(){
        guiders.hideAll();
    });

    var isMobile = UtilityService.isMobileDevice();
    var isMobileApp = ENV.mobileApp;

    CartService.getCartHeaders().then(function(carts){
        $scope.cartHeaders = carts;
    })

    OrderService.getChangeOrders();

    // Tutorial -- Tutorial Ignored 09/25
    // var hideTutorial = LocalStorage.getHideTutorialHomePage(),
    //     runTutorial =  hideTutorial || isMobileApp || isMobile ? false : true;
    // 
    // guiders.createGuider({
    //   id: "homepage_tutorial",
    //   title: "New Menu Location",
    //   description: "Where did the menu go? <br/><br/> In order to give you more space to work we've hidden the menu.  <br/><br/> When you need it click on the menu icon in the top left corner.",
    //   buttons: [{name: "Close", onclick: setHideTutorial}],
    //   overlay: true,
    //   attachTo: "#menuIcon",
    //   position: "right",
    //   offset: {left: -70, top: 64.11},
    //   highlight: true
    // })
    // 
    // function setHideTutorial(){
    //   LocalStorage.setHideTutorialHomePage(true);
    //   $rootScope.tutorialRunning = false;
    //   guiders.hideAll();
    // };

    // if(runTutorial) {
    //   $rootScope.tutorialRunning = true;
    //   guiders.show('homepage_tutorial');
    // }

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

    ProductService.getCampaigns().then(function(resp) {
        $scope.promoItems = resp;

        AnalyticsService.recordPromotion(
          $scope.promoItems,
          LocalStorage.getCustomerNumber(),
          LocalStorage.getBranchId());

    });

    // get promo/marketing items
    $scope.loadingPromoItems = true;
    MarketingService.getPromoItems().then(function(items) {
      // if(items[0].targeturltext != 'Admiral of the Fleet Salmon') { //Added to stop the salmon photo from displaying
        $scope.marketingPromoItems = items;
      // }
      delete $scope.promoMessage;
    }, function(errorMessage) {
      $scope.promoMessage = errorMessage;
    }).finally(function() {
      $scope.loadingPromoItems = false;

      // If Tutorial Should not show remove onboarding-focus class for icon element
    //   if(!$scope.runTutorial){
    //     $('.onboarding-focus').removeClass('onboarding-focus');
    //   }
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

    $scope.loadingRecentlyViewedItems = true;
    $scope.loadingrecentlyOrderedUnfiItems = true;
    $scope.loadingCategories = true;
    $scope.loadingBrands = true;
    $scope.loadingRecommendedItems = true;

    ProductService.getRecentlyViewedItems().then(function(recentitems) {
      $scope.recentlyViewedItems = recentitems;
      $scope.loadingRecentlyViewedItems = false;

      CategoryService.getCategories($state.params.catalogType).then(function(categories) {
        $scope.showRecommendedItems = ENV.showRecommendedItems;

        $scope.recommendedItems = [
          {"categoryid":"11","name":"Apples,Bananas","search_name":"applesbananas","description":null,"ppicode":null, category_image:{url: "images/ap000.jpg"}, parent_category: "produce"},
          {"categoryid":"20","name":"Fruit-frozen","search_name":"fruit-frozen","description":null,"ppicode":null, category_image:{url: "images/ff000.jpg"}, parent_category: "frozen_food"},
          {"categoryid":"30","name":"Beef Commodity","search_name":"beef_commodity","description":null,"ppicode":null, category_image:{url: "images/fm000.jpg"}, parent_category: "center_of_plate"},
          {"categoryid":"41","name":"Condiments & Dressings","search_name":"condiments_and_dressings","description":null,"ppicode":null, category_image:{url: "images/cx000.jpg"}, parent_category: "grocery"},
          {"categoryid":"60","name":"Dairy","search_name":"dairy","description":null,"ppicode":null, category_image:{url: "images/ad000.jpg"}, parent_category: "dairy"},
          {"categoryid":"71","name":"Paper & Plastics","search_name":"paper_and_plastics","description":null,"ppicode":null, category_image:{url: "images/pd000.jpg"}, parent_category: "non-food"},
          {"categoryid":"85","name":"Restaurant Supply","search_name":"restaurant_supply","description":null,"ppicode":null, category_image:{url: "images/ks000.jpg"}, parent_category: "equip_and_supply"}
        ];
        $scope.loadingRecommendedItems = false;
        
        $scope.categories = categories;
        $scope.loadingCategories = false;

        BrandService.getHouseBrands().then(function(brands){
          $scope.brands = brands;
          $scope.loadingBrands = false;
        });
      });
    });

    $scope.clearRecentlyViewedItems = function(){
      ProductService.clearRecentlyViewedItems().then(function() {
        $scope.recentlyViewedItems = null;
        $scope.displayMessage('success', 'Successfully cleared recently viewed items.');
      });
    };

    $scope.clearRecentlyOrderedItems = function(){
      OrderService.clearRecentlyOrderedUNFIItems().then(function(){
        $scope.recentlyOrderedUnfiItems = '';
        $scope.displayMessage('success', 'Successfully cleared recently ordered items.');
      });
    };

  }]);
