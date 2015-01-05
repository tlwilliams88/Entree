'use strict';

angular.module('bekApp')
.config([ '$stateProvider', '$urlRouterProvider', 
  function ($stateProvider, $urlRouterProvider) {

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
    .state('menu.userprofile', {
      url: '/profile/',
      templateUrl: 'views/userprofile.html',
      controller: 'UserProfileController',
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
        originalBasket: ['$state', '$stateParams', 'carts', 'changeOrders', 'ResolveService', 'CartService', function($state, $stateParams, carts, changeOrders, ResolveService, CartService) {
          var selectedBasket = ResolveService.selectDefaultBasket($stateParams.cartId, changeOrders);
          if (selectedBasket) {
            return selectedBasket;
          } else {
            return CartService.createCart();
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
        // invoices: [ 'InvoiceService', function(InvoiceService) {
        //   return InvoiceService.getAllInvoices();
        // }],
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
    REPORT
    **********/
    .state('menu.reports', {
        url: '/reports/',
        templateUrl: 'views/reports.html',
        controller: 'ReportsController',
        data: {
            authorize: 'canPayInvoices'
        },
        resolve: {
            /*items: [ '$stateParams', 'ReportService', function($stateParams, ReportService) {
              return loadItemUsage();
            }]*/
        }
    })

    /**********
    ADMIN
    **********/
    .state('menu.admin', {
      url: '/admin/',
      template: '<ui-view>'
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
        userProfile: ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
          return UserProfileService.getUserProfile($stateParams.email);
        }]
      }
    })
    .state('menu.admin.customer', {
      url: 'customers/:customerNumber/',
      templateUrl: 'views/admin/customerdetails.html',
      controller: 'CustomerDetailsController',
      data: {
        authorize: 'canManageAccount'
      }
    })

    /*************
    ADMIN ACCOUNTS
    *************/
    .state('menu.admin.account', {
      url: 'accounts/',
      templateUrl: 'views/admin/accounts.html',
      controller: 'AccountsController',
      data: {
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.accountdetails', {
      url: 'accounts/:accountId/',
      templateUrl: 'views/admin/accountdetails.html',
      controller: 'AdminAccountDetailsController',
      data: {
        authorize: 'canManageAccount'
      },
      resolve: {
        originalAccount: ['$stateParams', 'AccountService', function($stateParams, AccountService) {
          if ($stateParams.accountId === 'new') {
            return {};
          } else {
            return AccountService.getAccountDetails($stateParams.accountId);
          }
        }]
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

}]);