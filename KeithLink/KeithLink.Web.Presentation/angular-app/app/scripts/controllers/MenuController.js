'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:MenuController
 * @description
 * # MenuController
 * Controller of the bekApp
 */

angular.module('bekApp')
  .controller('MenuController', ['$scope', '$timeout', '$rootScope', '$modalStack', '$state', '$q', '$log', '$window', '$modal', '$filter', 'ENV', 'branches', 'CustomerService', 'AuthenticationService', 'AccessService', 'UtilityService', 'LocalStorage', 'NotificationService', 'ProductService', 'ListService', 'CartService', 'userProfile', 'ApplicationSettingsService',
    function (
      $scope, $timeout, $rootScope, $modalStack, $state, $q, $log, $window,  // built in angular services
      $modal,   // ui-bootstrap library
      $filter,
      ENV,      // environment config, see configenv.js file which is generated from Grunt
      branches, // state resolve
      CustomerService, AuthenticationService, AccessService, UtilityService, LocalStorage, NotificationService, ProductService, ListService, CartService, userProfile, ApplicationSettingsService // bek custom services
    ) {

  $scope.$state = $state;
  $scope.isMobile = UtilityService.isMobileDevice();
  $scope.isMobileApp = ENV.mobileApp;
  $scope.mandatoryMessages = NotificationService.mandatoryMessages;
  // define search term in user bar so it can be cleared in the SearchController after a user searches
  $scope.userBar = {};
  $scope.userBar.universalSearchTerm = '';
  $scope.branches = branches;
  $scope.userGuideUrl = "/Assets/help/User_Guide.pdf";
 

  // global notification at the top of all pages
  // TODO: Global messaging backend?
  $scope.messageText = 'Hello world!';
  $scope.displayGlobalMessage = true;
  $scope.userProfile = userProfile;
  refreshAccessPermissions($scope.userProfile);

  // Application version for use on sidebar menu
  // Using 3 different values for potential hotfix mobile submissions
  $scope.iOS = (/iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream && $scope.isMobileApp);
  $scope.Android = (!(/iPad|iPhone|iPod/.test(navigator.userAgent)) && !window.MSStream && $scope.isMobileApp);
 
  $scope.webVersionNum = '1.6.0';
  $scope.androidVersionNum = '1.5.4';
  $scope.iOSVersionNum = '1.5.4';

  // KBIT ACCESS
  var usernameToken = $scope.userProfile.usernametoken;
  $scope.cognosUrl = ENV.cognosUrl + '?username=' + usernameToken;
  
  $scope.userBar.userNotificationsCount = NotificationService.userNotificationsCount;
  $scope.specialCatalogOpen = false;
  $scope.showSpecialtyCatalogs = true;

  if (AccessService.isOrderEntryCustomer()) {

    $scope.cartHeaders = CartService.cartHeaders;
    $scope.numOrdersToDisplay = 6;
    $scope.numCartsToDisplay = 4;

    if (CartService.cartHeaders.length === 0 && $scope.canCreateOrders) {
      $scope.loadingCarts = true;
      delete $scope.cartMessage;
      CartService.getCartHeaders().then(
        function(carts) {
          $scope.numCartsToDisplay = carts.length <= 4 ? carts.length : 4;
          $scope.numOrdersToDisplay = 6 - $scope.numCartsToDisplay;
        },
        function() {
          $scope.cartMessage = 'Error loading carts.';
        })
      .finally(function() {
        $scope.loadingCarts = false;
      });
    }
  }

  $scope.checkModal = function(){   
    if($modal){
      $modalStack.dismissAll();
    }
  }
  //Check stored settings. If they exist, update local storage, if not, clear out local storage values.
   ApplicationSettingsService.getApplicationSettings('').then(function(settings){
      if(settings.length > 0){
        settings.forEach(function(setting){
          if(setting.key === 'pageLoadSize'){
            LocalStorage.setPageSize(setting.value);
          }
          if(setting.key === 'sortPreferences'){
            LocalStorage.setDefaultSort(setting.value);
          }
        })
      }
      else{
          LocalStorage.setPageSize('');
          LocalStorage.setDefaultSort('');
      }
   });
  /**********
  PHONEGAP OFFLINE STORAGE
  **********/

  var db_table_name_lists = 'lists',
    db_table_name_carts = 'carts';

  function downloadDataForOfflineStorage() {
    $q.all([
      ListService.getAllListsForOffline(),
      CartService.getAllCartsForOffline()
    ]).then(function() {
      // console.log('Downloaded data for offline use.');
    });
  }

  if (ENV.mobileApp && AccessService.isOrderEntryCustomer() && AccessService.canCreateOrders()) {
    // console.log('downloading data');
    downloadDataForOfflineStorage();
  }

  /**********
  SELECTED USER CONTEXT
  **********/

  $scope.setSelectedUserContext = function(selectedUserContext) {
    $scope.selectedUserContext = selectedUserContext;
  };

  $scope.departments = [
    { "value": '', "text": 'All'},
    { "value": '1', "text": 'Produce'},
    { "value": '2', "text": 'Frozen'},
    { "value": '3', "text": 'Frozen Meat'},
    { "value": '4', "text": 'Grocery'},
    { "value": '5', "text": 'Boxed Beef'},
    { "value": '6', "text": 'Dairy'},
    { "value": '7', "text": 'Non-Food'},
    { "value": '8', "text": 'Rest. Supply'}];


  $scope.selectDepartment = function(dept){
    $scope.departmentNum = dept;
    $scope.department = $filter('filter')($scope.departments, {value: dept});
  };

  $scope.selectDepartment('');

  // get default selected user context
  if ($scope.isOrderEntryCustomer) {
    $scope.setSelectedUserContext(LocalStorage.getCurrentCustomer());
  } else {
    $scope.setSelectedUserContext(LocalStorage.getBranchId());
  }

  $scope.customerInfiniteScroll = {
    from: 0,
    size: 15
  };

  // list of state names where a user has the possibility of viewing info from multiple customers
  var statesWithViewingAllCustomers = ['menu.invoice', 'menu.transaction'];

  // listens for state change event to restore selectedUserContext
  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {

    // if users is viewing all customers
    // change selected user context back to the one stored in LocalStorage here
    if (statesWithViewingAllCustomers.indexOf(fromState.name) > -1 && !$scope.selectedUserContext.id) {
      $scope.setSelectedUserContext(LocalStorage.getCurrentCustomer());
    }
  });

  var firstPageCustomers; // used to cache the first page of customer results
  // populates upper-left customer dropdown infinite scroll
  var searchTerm;

  var getData = function(query) {
    $scope.customerInfiniteScroll.from = (query.page - 1) * $scope.customerInfiniteScroll.size;

    if (query.page === 1 && firstPageCustomers && !query.term) { // use cache if getting first page
      query.callback(firstPageCustomers);
    } else {
      CustomerService.getCustomers(
        query.term,
        $scope.customerInfiniteScroll.size,
        $scope.customerInfiniteScroll.from
      ).then(function(data) {
        // convert data to match select2 data object
        var customerList = {
          results: [],
          more: query.page * $scope.customerInfiniteScroll.size < data.totalResults // boolean if there are more results to display using infinite scroll
        };

        data.results.forEach(function(customer) {
          customerList.results.push({
            id: customer.displayname, // value
            text: customer.displayname,  // display text
            customer: customer
          });
        });

        if (query.page === 1 && !query.term) {
          firstPageCustomers = customerList;
        }
        query.callback(customerList);
      });
    }
  };

  $scope.displayUserMenu  = false;
  document.onclick = function(element){
    $timeout(function() {
      if($scope.displayUserMenu && !$scope.mouseOverDropdown){
        $scope.displayUserMenu = !$scope.displayUserMenu; 
      }
    }, 0);
  };

  $scope.customerSelectOptions = {
    query: function (query){
      $timeout(function() {
        if (searchTerm === query.term) {
          getData(query);
        }
      }, 500);
      searchTerm = query.term;
    }
  };

  $scope.showNotification = function(notification) {
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
     $scope.dismissNotification(notification);
  };

  $scope.dismissNotification = function(notification) {
    var messageRead = $scope.mandatoryMessages.slice(0,1);
    NotificationService.updateUnreadMessages(messageRead);
    $scope.mandatoryMessages.splice(0,1);
  };

  $scope.goToAdminLandingPage = function() {
    // internal bek admin user
    if ($scope.canViewCustomerGroups) {
      $state.go('menu.admin.customergroup');

    // external owner admin
    } else {
      $state.go('menu.admin.customergroupdashboard', { customerGroupId: null });
    }
  };

  function refreshPage() {
    location.reload();
  }

  // change context menu selection for guest users
  $scope.changeBranch = function(branchId) {
    LocalStorage.setTempBranch(branchId);
    refreshPage();
  };

  // change context menu selection for order-entry customers
  $scope.changeCustomerLocation = function(selectedUserContext) {
    LocalStorage.setTempContext(selectedUserContext);
    refreshPage();
  };

  //Submenu for specialty catalogs
  $scope.toggleSpecialCatalogSubmenu = function() {
    if ($scope.$state !== undefined) {       
      $scope.specialCatalogOpen = !$scope.specialCatalogOpen;
    }
  };

  $scope.toggleSidebarMenu = function() {
      $scope.isSidebarOpen = !$scope.isSidebarOpen;
  };

  /**********
  MENU BUTTON CLICK HANDLERS
  **********/

  $scope.logout = function() {
    AuthenticationService.logout();

    $state.transitionTo('register');
    $scope.displayUserMenu = false;
  };

  $scope.print = function () {
    $window.print();
  };

  $scope.navigateBack = function(){
    $window.history.back();
  };

  $scope.openTechnicalSupportModal = function() {
    var modalInstance = $modal.open({
      templateUrl: 'views/modals/technicalsupportmodal.html',
      controller: 'TechnicalSupportModalController',
      windowClass: 'color-background-modal',
      resolve: {
        branch: function() {
          var branchFound,
            branchId = LocalStorage.getBranchId();
          angular.forEach(branches, function(branch) {
            if (branch.id.toUpperCase() === branchId.toUpperCase()) {
              branchFound = branch;
            }
          });
          return branchFound;
        }
      }
    });
  };

  // PHONEGAP Feature
  $scope.scanBarcode = function() { // TODO: should this work with UNFI?
    cordova.plugins.barcodeScanner.scan(
      function (result) {
        var scannedText = result.text;
        $log.debug(scannedText);
        ProductService.scanProduct(scannedText).then(function(item) {
          if (item) {
            ProductService.selectedProduct = item;
            $state.go('menu.catalog.products.details', { itemNumber: item.itemnumber });
          } else {
            $state.go('menu.catalog.products.list', { type: 'search', id: scannedText });
          }
        }, function (error) {
          $scope.displayMessage('error', 'Error with scan product request.');
        });
    }, function (error) {
      $log.debug('Scanning failed: ' + error);
    });
  };

  /**********
  SET USER PERMISSIONS
  these are used in all sub-states to show/hide accessible info
  also used in state.js to determine if the user has access to a state
  **********/

  function refreshAccessPermissions(userProfile) {
    $scope.isLoggedIn = AccessService.isLoggedIn();
    $scope.isOrderEntryCustomer = AccessService.isOrderEntryCustomer();
    $scope.isInternalAccountAdminUser = AccessService.isInternalAccountAdminUser();
    $scope.isInternalUser = AccessService.isInternalUser();
    $scope.isDemo = AccessService.isDemo();

    $scope.canBrowseCatalog = AccessService.canBrowseCatalog();
    $scope.canSeePrices = AccessService.canSeePrices();
    $scope.canManageLists = AccessService.canManageLists();
    $scope.canViewOrders = AccessService.canViewOrders();
    $scope.canCreateOrders = AccessService.canCreateOrders();
    $scope.canSubmitOrders = AccessService.canSubmitOrders();
    $scope.canPayInvoices = AccessService.canPayInvoices();
    $scope.canManageCustomerGroups = AccessService.canManageCustomerGroups();
    $scope.canViewCustomerGroups = AccessService.canViewCustomerGroups();
    $scope.canViewCustomerGroupDashboard = AccessService.canViewCustomerGroupDashboard();
    $scope.canEditUsers = AccessService.canEditUsers();
    $scope.canEditInternalUsers = AccessService.canEditInternalUsers();
    $scope.canGrantAccessToOtherServices = AccessService.canGrantAccessToOtherServices();
    $scope.canMoveUserToAnotherGroup = AccessService.canMoveUserToAnotherGroup();
    $scope.canViewMarketing = AccessService.canViewMarketing();
	$scope.canGrantAccessToKbit = AccessService.canGrantAccessToKbit();
	$scope.canGrantAccessToEmenuManage = AccessService.canGrantAccessToEmenuManage();
    $scope.canRunReports = AccessService.canRunReports();
	$scope.isSysAdmin = AccessService.isSysAdmin();
  }
}]);
