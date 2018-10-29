'use strict';

angular.module('bekApp')
  .directive('navigationBar', [function () {
    return {
      restrict: 'E',
      // replace : true,
      scope: {
        isSidebarOpen: '=isSidebarOpen',  //Two-way data binding
        userContext: '=userContext',      //Two-way data binding
        userProfile: '=userProfile',      //Two-way data binding
      },
      templateUrl: 'views/directives/navigationBar.html',
      controller: ['$scope', '$rootScope', '$stateParams', '$state', 'Constants',
        'AccessService', 'NotificationService', 'UserProfileService', 'ENV', 
        function ($scope, $rootScope, $stateParams, $state, Constants,
          AccessService, NotificationService, UserProfileService, ENV)
        {

          if (typeof $scope.isSidebarOpen === "undefined")
            alert("The attribute, 'is-sidebar-open', was not defined in the element, 'navigation-bar'.");
          if (typeof $scope.userContext === "undefined")
            alert("The attribute, 'user-context', was not defined in the element, 'navigation-bar'.");
          if (typeof $scope.userProfile === "undefined")
            alert("The attribute, 'user-profile', was not defined in the element, 'navigation-bar'.");

          $scope.$state = $state;
          $scope.constants = Constants;

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
          $scope.flipsnackUrl = ENV.flipsnackUrl;

          $scope.openExternalLink = $rootScope.openExternalLink;
          $scope.openExternalLinkWithPost = $rootScope.openExternalLinkWithPost;

          $scope.userBar = {};
          $scope.userBar.universalSearchTerm = '';
          $scope.userBar.userNotificationsCount = NotificationService.userNotificationsCount;
          
          //Used for Kbit access
          var usernameToken = $scope.userProfile.usernametoken;
          $scope.cognosUrl = ENV.cognosUrl + '?username=' + usernameToken;

          $scope.toggleSidebarMenu = function () {
            $scope.isSidebarOpen = !$scope.isSidebarOpen;
          };

          //Submenu for specialty catalogs
          $scope.toggleSpecialCatalogSubmenu = function () {
            $scope.specialCatalogOpen = !$scope.specialCatalogOpen;
          };

          $scope.toggleReportsSubmenu = function() {
            $scope.reportsOpen = !$scope.reportsOpen;
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
