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
    // 'ngAnimate',
    // 'ngCookies',
    'ngResource',
    'ngSanitize',
    'ngTouch',
    'LocalStorageModule', // HTML5 local storage
    'ui.router',
    'ui.bootstrap',
    'ui.sortable', // jquery ui list sorting (used on lists page)
    'shoppinpal.mobile-menu', // mobile sidebar menu
    'ngDragDrop', // jquery ui drag and drop (used on lists page)
    'infinite-scroll',
    'unsavedChanges', // throws warning to user when navigating away from an unsaved form
    'toaster', // user notification messages
    'angular-loading-bar', // loading indicator in the upper left corner
    'angularFileUpload', // csv file uploads for lists and orders
    'naif.base64', // base64 file uploads for images
    'fcsa-number',
    'configenv'
  ])
.config(['$stateProvider', '$compileProvider', '$tooltipProvider', '$urlRouterProvider', '$httpProvider', '$logProvider', 'localStorageServiceProvider', 'cfpLoadingBarProvider', 'ENV',
  function($stateProvider, $compileProvider, $tooltipProvider, $urlRouterProvider, $httpProvider, $logProvider, localStorageServiceProvider, cfpLoadingBarProvider, ENV) {

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
        // guest users must have branches to load the page (but non-guest users do not?)
        branches: ['BranchService', function(BranchService) {
          return BranchService.getBranches();
        }]
        // get SHIP DATES: I originally put this here so I could set the default ship date of new carts
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
    .state('menu.notifications', {
      url: '/notifications/',
      templateUrl: 'views/notifications.html',
      controller: 'NotificationsController',
      data: {
        authorize: 'isOrderEntryCustomer'
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

    /**********
    LISTS
    **********/
    .state('menu.lists', {
      url: '/lists/',
      abstract: true,
      template: '<ui-view/>',
      data: {
        authorize: 'canManageLists'
      },
      resolve: {
        lists: ['ListService', function (ListService) {
          return ListService.getListHeaders();
        }],
        labels: ['ListService', function(ListService) {
          return ListService.getAllLabels();
        }]
      }
    })
    .state('menu.lists.items', {
      url: ':listId/?renameList',
      templateUrl: 'views/lists.html',
      controller: 'ListController',
      data: {
        authorize: 'canManageLists'
      },
      resolve: {
        originalList: [ '$stateParams', 'lists', 'ResolveService', function($stateParams, lists, ResolveService) {
          return ResolveService.selectDefaultList($stateParams.listId);
        }]
      }
    })

    /**********
    CART
    **********/
    .state('menu.cart', {
      url: '/cart/',
      abstract: true,
      template: '<ui-view/>',
      data: {
        authorize: 'canCreateOrders'
      },
      resolve: {
        carts: ['CartService', function (CartService){
          return CartService.getCartHeaders();
        }],
        changeOrders: ['OrderService', function(OrderService) {
          return OrderService.getChangeOrders();
        }],
        criticalItemsLists: ['ListService', function(ListService) {
          return ListService.getCriticalItemsLists();
        }],
        shipDates: ['CartService', function(CartService) {
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
      },
      resolve: {
        originalBasket: ['$state', '$stateParams', 'carts', 'changeOrders', 'ResolveService', function($state, $stateParams, carts, changeOrders, ResolveService) {
          var selectedBasket = ResolveService.selectDefaultBasket($stateParams.cartId, changeOrders);
          if (selectedBasket) {
            return selectedBasket;
          } else {
            $state.go('menu.home');
          }
        }]
      }
    })

    /**********
    ADD TO ORDER
    **********/
    .state('menu.addtoorder', {
      url: '/add-to-order/',
      abstract: true,
      template: '<ui-view/>',
      data: {
        authorize: 'canCreateOrders'
      },
      resolve: {
        lists: ['ListService', function (ListService){
          return ListService.getListHeaders();
        }],
        carts: ['CartService', function(CartService) {
          return CartService.getCartHeaders();
        }],
        changeOrders: ['OrderService', function(OrderService) {
          return OrderService.getChangeOrders();
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
      },
      resolve: {
        selectedList: [ '$stateParams', 'lists', 'ResolveService', function($stateParams, lists, ResolveService) {
          return ResolveService.selectDefaultList($stateParams.listId);
        }],
        selectedCart: ['$state', '$stateParams', 'carts', 'changeOrders', 'ResolveService', function($state, $stateParams, carts, changeOrders, ResolveService) {
          var selectedBasket = ResolveService.selectDefaultBasket($stateParams.cartId, changeOrders);
          if (selectedBasket) {
            return selectedBasket;
          } else {
            $state.go('menu.home');
          }
        }]
      }
    })

    /**********
    ORDER HISTORY
    **********/
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
      url: '/order/:orderNumber/',
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
    INVOICE
    **********/
    .state('menu.invoice', {
      url: '/invoices/',
      templateUrl: 'views/invoice.html',
      controller: 'InvoiceController',
      data: {
        authorize: 'canPayInvoices'
      },
      resolve: {
        invoices: [ 'InvoiceService', function(InvoiceService) {
          return InvoiceService.getAllInvoices();
        }],
        accounts: ['BankAccountService', function(BankAccountService) {
          return BankAccountService.getAllBankAccounts();
        }]
      }
    })
    .state('menu.invoiceitems', {
      url: '/invoice/:invoiceNumber/',
      templateUrl: 'views/invoiceitems.html',
      controller: 'InvoiceItemsController',
      data: {
        authorize: 'canPayInvoices'
      },
      resolve: {
        invoice: [ '$stateParams', 'InvoiceService', function($stateParams, InvoiceService) {
          return InvoiceService.getInvoice($stateParams.invoiceNumber);
        }]
      }
    })

    /**********
    MARKETING CMS
    **********/
    .state('menu.marketing', {
      url: '/marketing/',
      templateUrl: 'views/marketing.html',
      controller: 'MarketingController',
      data: {
        authorize: 'canPayInvoices'
      },
      resolve: {
        // invoice: [ '$stateParams', 'InvoiceService', function($stateParams, InvoiceService) {
        //   return InvoiceService.getInvoiceDetails($stateParams.invoiceNumber);
        // }]
      }
    })

    /**********
    ADMIN
    **********/
    .state('menu.admin', {
      url: '/admin/',
      templateUrl: 'views/admin/menu.html',
      data: {
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.user', {
      url: 'users/',
      templateUrl: 'views/admin/users.html', //'views/adminusers.html',
      controller: 'UsersController',
      data: {
        authorize: 'canManageAccount'
      },
      resolve: {
        users: [ 'UserProfileService', function(UserProfileService) {
          return [{name: 'Maria'}, {name: 'Andrew'}, {name: 'Josh'}]; //UserProfileService.getAllUsers();
        }]
      }
    })
    .state('menu.admin.adduser', {
      url: 'users/add/',
      templateUrl: 'views/admin/adduserdetails.html',
      controller: 'AddUserDetailsController',
      data: {
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.accountadmin',{
      url: 'account/',
      templateUrl: 'views/admin/accountadmin.html',
      controller: 'AccountAdminController',
      data: {
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.edituser', {
      url: 'edituser/:email/',
      templateUrl: 'views/admin/edituserdetails.html',
      controller: 'EditUserDetailsController',
      data: {
        authorize: 'canManageAccount'
      },
      resolve: {
        returnedProfile: ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
          return UserProfileService.getUserProfile($stateParams.email);
        }]
      }
    })
    .state('menu.admin.customer', {
      url: 'customers/:customerNumber/',
      templateUrl: 'views/admin/customers.html',
      controller: 'CustomersController',
      data: {
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.account', {
      url: 'accounts/',
      templateUrl: 'views/admin/accounts.html',
      controller: 'AccountsController',
      data: {
        authorize: 'canManageAccount'
      },
      resolve: {
        accounts: [ 'AccountService', function(AccountService) {
          return AccountService.getAccounts();
        }]
      }
    })
    .state('menu.admin.account.details', {
      url: ':accountId/',
      templateUrl: 'views/admin/accountdetails.html',
      controller: 'AdminAccountDetailsController',
      data: {
        authorize: 'canManageAccount'
      }
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
  $urlRouterProvider.when('/cart/', '/cart/1');
  $urlRouterProvider.when('/cart/', '/cart/1');
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

  // group multiple aysnc methods together to only run through one digest cycle
  $httpProvider.useApplyAsync(true);

  $compileProvider.debugInfoEnabled(false);

  // fix for ngAnimate and ui-bootstrap tooltips
  $tooltipProvider.options({animation: false});

}])
.run(['$rootScope', '$state', '$log', 'toaster', 'AccessService', 'AuthenticationService', 'NotificationService',
  function($rootScope, $state, $log, toaster, AccessService, AuthenticationService, NotificationService) {

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

  $rootScope.$on('$stateChangeSuccess', function(event, toState, toParams, fromState, fromParams) {
    if (AccessService.isOrderEntryCustomer()) {
      NotificationService.getUnreadMessageCount();
    }
  });

}]);
