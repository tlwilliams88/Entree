'use strict';

angular.module('bekApp')
  .directive('navigationBar', [function () {
    return {
      restrict: 'E',
      // replace : true,
      scope: {
        openScope: '=',
        isDisabled: '='
      },
      templateUrl: 'views/directives/navigatioBar.html',
      controller: ['$scope', '$stateParams', '$modal', 'OrderService', '$state', 'ApplicationSettingsService', 'UtilityService', 'LocalStorage', 'ListService', 'CartService',
        function ($scope, $stateParams, $modal, OrderService, $state, ApplicationSettingsService, UtilityService, LocalStorage, ListService, CartService) {

          $scope.isOrderEntryCustomer = AccessService.isOrderEntryCustomer();
          $scope.canBrowseCatalog = AccessService.canBrowseCatalog();
          $scope.canViewOrders = AccessService.canViewOrders();
          $scope.canRunReports = AccessService.canRunReports();
          $scope.canSeePrices = AccessService.canSeePrices();
          $scope.canViewInvoices = AccessService.canViewInvoices();
          $scope.canManageLists = AccessService.canManageLists();
          $scope.canViewCustomerGroups = AccessService.canViewCustomerGroups();
          $scope.canViewCustomerGroupDashboard = AccessService.canViewCustomerGroupDashboard();
          $scope.canViewMarketing = AccessService.canViewMarketing();

          $scope.specialCatalogOpen = false;
          $scope.showSpecialtyCatalogs = true;

          $scope.userBar = {};
          $scope.userBar.universalSearchTerm = '';
          $scope.userBar.userNotificationsCount = NotificationService.userNotificationsCount;



          $scope.toggleSidebarMenu = function () {
            $scope.isSidebarOpen = !$scope.isSidebarOpen;
          };

          $scope.checkModal = function () {
            if ($modal) {
              $modalStack.dismissAll();
            }
          };

          //Submenu for specialty catalogs
          $scope.toggleSpecialCatalogSubmenu = function () {
            if ($scope.$state !== undefined) {
              $scope.specialCatalogOpen = !$scope.specialCatalogOpen;
            }
          };

          // Menumax
          $scope.redirectToMenumax = function () {
            UserProfileService.generateMenuMaxAuthToken().then(function (resp) {

              var payload = '{"email":"' + $scope.userProfile.emailaddress + '",' + '"entreeSSOToken":"' + resp + '"}';
              var url = ENV.menuMaxUrl;

              $scope.openExternalLinkWithPost(url, "_blank", payload);

            })
          };

          $scope.goToAdminLandingPage = function () {
            // internal bek admin user
            if ($scope.canViewCustomerGroups) {
              $state.go('menu.admin.customergroup');

              // external owner admin
            } else {
              $state.go('menu.admin.customergroupdashboard', { customerGroupId: null });
            }
          };





        }]
    };
  }]);
