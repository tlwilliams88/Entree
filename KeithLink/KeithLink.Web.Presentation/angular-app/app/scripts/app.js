'use strict';

/**
 * @ngdoc overview
 * @name bekApp
 * @description
 * # bekApp
 *
 * Main module of the application.
 */
angular
  .module('bekApp', [
    'ngAnimate', // needed for ui-bootstrap carousel
    // 'ngCookies',
    'ngResource',
    'ngTouch',
    'LocalStorageModule',
    'ui.router',
    'ui.bootstrap',
    'ui.sortable',
    'shoppinpal.mobile-menu',
    'ngDragDrop',
    'infinite-scroll',
    'unsavedChanges',
    'toaster',
    'angular-loading-bar',
    'configenv'
  ])
.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', '$logProvider', 'localStorageServiceProvider', 'cfpLoadingBarProvider', 'ENV',
  function($stateProvider, $urlRouterProvider, $httpProvider, $logProvider, localStorageServiceProvider, cfpLoadingBarProvider, ENV) {
  
  // configure loading bar
  cfpLoadingBarProvider.includeBar = false;

  // configure logging
  $logProvider.debugEnabled(ENV.loggingEnabled);

  // set local storage prefix
  localStorageServiceProvider.setPrefix('bek');

  // the $stateProvider determines path urls and their related controllers
  $stateProvider
    // register
    .state('register', {
      url: '/register/',
      templateUrl: 'views/register.html',
      controller: 'RegisterController'
    })
    .state('menu', {
      abstract: true, // path that cannot be navigated to directly, it can only be accessed by child views
      templateUrl: 'views/menu.html',
      controller: 'MenuController',
      resolve: {
        branches: ['BranchService', function(BranchService) {
          return BranchService.getBranches();
        }]
      }
    })
    // /home
    .state('menu.home', {
      url: '/home/',
      templateUrl: 'views/home.html',
      controller: 'HomeController',
      data: {
        authorize: 'isOrderEntryCustomer'
      }
    })
    .state('menu.accountdetails', {
      url: '/account/',
      templateUrl: 'views/accountdetails.html',
      controller: 'AccountDetailsController',
      data: {
        authorize: 'isLoggedIn'
      },
      resolve: {
        branches: ['BranchService', function(BranchService) {
          return BranchService.getBranches();
        }]
      }
    })

    /**********
    CATALOG
    **********/
    .state('menu.catalog', {
      abstract: true,
      url: '/catalog/',
      template: '<div ui-view=""></div>'
    })
    .state('menu.catalog.home', {
      url: '',
      templateUrl: 'views/catalog.html',
      controller: 'CatalogController',
      data: {
        authorize: 'canBrowseCatalog'
      }
    })
    .state('menu.catalog.products', {
      abstract: true,
      url: 'products/',
      template: '<div ui-view=""></div>'
    })
    .state('menu.catalog.products.list', {
      url: ':type/:id/?brands',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController',
      data: {
        authorize: 'canBrowseCatalog'
      }
    })
    .state('menu.catalog.products.details', {
      url: ':itemNumber/',
      templateUrl: 'views/itemdetails.html',
      controller: 'ItemDetailsController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        item: ['$stateParams', 'ProductService', function($stateParams, ProductService) {
          return ProductService.getProductDetails($stateParams.itemNumber);
        }]
      }
    })


    .state('menu.lists', {
      url: '/lists/',
      abstract: true,
      template: '<ui-view/>',
      data: {
        authorize: 'canManageLists'
      },
      resolve: {
        lists: ['$q', 'ListService', function ($q, ListService) {
          return $q.all([
            ListService.getAllLists(),
            ListService.getAllLabels()
          ]);
        }],
        carts: ['CartService', function(CartService) {
          return CartService.getCartHeaders();
        }]
      }
    })
    .state('menu.lists.items', {
      url: ':listId/?renameList',
      templateUrl: 'views/lists.html',
      controller: 'ListController',
      data: {
        authorize: 'canManageLists'
      }
    })
    .state('menu.cart', {
      url: '/cart/',
      templateUrl: 'views/cart.html',
      controller: 'CartController',
      data: {
        authorize: 'canCreateOrders'
      },
      resolve: {
        carts: ['CartService', function (CartService){
          return CartService.getAllCarts();
        }],
        shipDates: ['CartService', function (CartService){
          return CartService.getShipDates();
        }]
      }
    })
    .state('menu.cart.items', {
      url: ':cartId/?renameCart',
      templateUrl: 'views/cartitems.html',
      controller: 'CartItemsController',
      data: {
        authorize: 'canCreateOrders'
      }
    })
    .state('menu.addtoorder', {
      url: '/add-to-order/',
      abstract: true,
      template: '<ui-view/>',
      data: {
        authorize: 'canCreateOrders'
      },
      resolve: {
        lists: ['ListService', function (ListService){
          return ListService.getAllLists();
        }],
        carts: ['CartService', function(CartService) {
          return CartService.getAllCarts();
        }],
        shipDates: ['CartService', function(CartService) {
          return CartService.getShipDates();
        }]
      }
    })
    .state('menu.addtoorder.items', {
      url: ':listId/?cartId&useParlevel',
      templateUrl: 'views/addtoorder.html',
      controller: 'AddToOrderController',
      data: {
        authorize: 'canCreateOrders'
      }
    })
    .state('menu.order', {
      url: '/orders/',
      templateUrl: 'views/order.html',
      controller: 'OrderController',
      data: {
        authorize: 'canSubmitOrders'
      },
      resolve: {
        orders: [ 'OrderService', function(OrderService) {
          return OrderService.getAllOrders();
        }]
      }
    })
    .state('menu.orderitems', {
      url: '/orders/:orderNumber/',
      templateUrl: 'views/orderitems.html',
      controller: 'OrderItemsController',
      data: {
        authorize: 'canSubmitOrders'
      },
      resolve: {
        order: [ '$stateParams', 'OrderService', function($stateParams, OrderService) {
          return OrderService.getOrderDetails($stateParams.orderNumber);
        }]
      }
    })

    /**********
    ADMIN
    **********/
    .state('menu.admin', {
      url: '/admin/',
      templateUrl: 'views/admin/menu.html'
    })
    .state('menu.admin.user', {
      url: 'users/',
      templateUrl: 'views/admin/users.html', //'views/adminusers.html',
      controller: 'UsersController',
      resolve: {
        users: [ 'UserProfileService', function(UserProfileService) {
          return [{name: 'Maria'}, {name: 'Andrew'}, {name: 'Josh'}]; //UserProfileService.getAllUsers();
        }]
      }
    })
    .state('menu.admin.adduser', {
      url: 'users/add/',
      templateUrl: 'views/admin/adduserdetails.html',
      controller: 'AddUserDetailsController'
    })
    .state('menu.admin.edituser', {
      url: 'users/:userId/',
      templateUrl: 'views/admin/edituserdetails.html',
      controller: 'EditUserDetailsController'
    })
    .state('menu.admin.customer', {
      url: 'customers/',
      templateUrl: 'views/admin/customers.html',
      controller: 'CustomersController',
      resolve: {
        customers: [ 'CustomerService', function(CustomerService) {
          return CustomerService.getCustomers();
        }]
      }
    })
    .state('menu.admin.account', {
      url: 'accounts/',
      templateUrl: 'views/admin/accounts.html',
      controller: 'AccountsController',
      resolve: {
        accounts: [ 'AccountService', function(AccountService) {
          return AccountService.getAccounts();
        }]
      }
    })
    .state('menu.admin.account.details', {
      url: ':accountId/',
      templateUrl: 'views/admin/accountdetails.html',
      controller: 'AdminAccountDetailsController'
    });

  $stateProvider
    .state('404', { 
      url: '/404/',
      templateUrl: 'views/404.html'
    });
  // redirect to /home route when going to '' or '/' paths
  $urlRouterProvider.when('', '/register');
  $urlRouterProvider.when('/', '/register');
  $urlRouterProvider.when('/lists', '/lists/1');
  $urlRouterProvider.when('/lists/', '/lists/1');
  $urlRouterProvider.otherwise('/404');

  // allow user to access paths with or without trailing slashes
  $urlRouterProvider.rule(function ($injector, $location) {
    var path = $location.url();

    // check to see if the path already has a slash where it should be
    if ('/' === path[path.length - 1] || path.indexOf('/?') > -1) {
        return;
    }
    if (path.indexOf('?') > -1) {
        return path.replace('?', '/?');
    }

    return path + '/';
  });

  // add authentication headers and Api Url
  $httpProvider.interceptors.push('AuthenticationInterceptor');

}])
.run(['$rootScope', '$state', '$log', 'toaster','AccessService', 'AuthenticationService',
  function($rootScope, $state, $log, toaster, AccessService, AuthenticationService) {

  $rootScope.displayMessage = function(type, message) {
    toaster.pop(type, null, message);
  };

  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    $log.debug('route: ' + toState.name);

    // check if route is protected
    if (toState.data && toState.data.authorize) {
      // check if user's token is expired
      if (!AccessService.isLoggedIn()) {
        AuthenticationService.logout();
        $state.go('register');
        event.preventDefault();
      }

      // check if user has access to the route
      if (!AccessService[toState.data.authorize]()) {
        $state.go('register');
        event.preventDefault(); 
      }
    }

    // redirect register page to homepage if logged in
    if (toState.name === 'register' && AccessService.isLoggedIn()) {

      if ( AccessService.isOrderEntryCustomer() ) {
        $state.go('menu.home');  
      } else {
        $state.go('menu.catalog.home');
      }

      event.preventDefault(); 
    }

  });
}]);
