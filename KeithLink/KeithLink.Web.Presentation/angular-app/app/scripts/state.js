'use strict';

angular.module('bekApp')
.config([ '$stateProvider', '$urlRouterProvider', 
  function ($stateProvider, $urlRouterProvider) {

  // the $stateProvider determines path urls and their related controllers
  /*
  data
    authorize: matches a function in AccessService and checks the user has access to the route
  */

  $stateProvider
    // register
    .state('register', {
      url: '/register/',
      templateUrl: 'views/register.html',
      controller: 'RegisterController',
      data: {}
    })
    .state('changepassword', {
        url: '/changepassword/',
        templateUrl: 'views/changepassword.html',
        controller: 'ChangePasswordController',
        data: {},
        resolve: {
          userProfile: ['UserProfileService', function(UserProfileService) {
            return UserProfileService.getCurrentUserProfile();
          }]
        }
    })
    .state('menu', {
      abstract: true, // path that cannot be navigated to directly, it can only be accessed by child views
      templateUrl: 'views/menu.html',
      controller: 'MenuController',
      resolve: {
        branches: ['BranchService', function(BranchService) {
          // guest users must have branches to load the page (but non-guest users do not)
          // also needed for tech support
          return BranchService.getBranches();
        }],
        userProfile: ['SessionService', function(SessionService) {
          return SessionService.userProfile;
        }],
        selectedUserContext: ['LocalStorage', function(LocalStorage) {
          if (LocalStorage.getTempContext()) {
            LocalStorage.setSelectedCustomerInfo(LocalStorage.getTempContext());
          } else if (LocalStorage.getTempBranch()) {
            LocalStorage.setSelectedBranchInfo(LocalStorage.getTempBranch());
          }
        }]
      }
    })
    // /home
    .state('menu.home', {
      url: '/home/',
      templateUrl: 'views/home.html',
      controller: 'HomeController',
      data: {
        authorize: 'isOrderEntryCustomer',
        saveCarts: true
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
        authorize: 'canManageLists',
        saveLists: true
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
      url: ':listId/',
      templateUrl: 'views/lists.html',
      controller: 'ListController',
      data: {
        authorize: 'canManageLists',
        saveLists: true
      },
      resolve: {
        validListId: ['$stateParams', 'lists', 'ResolveService', function($stateParams, lists, ResolveService) {
          return ResolveService.validateList($stateParams.listId);
        }],
        originalList: ['$stateParams', 'validListId', 'lists', 'ListService', function($stateParams, validListId, lists, ListService) {
          return ListService.getList(validListId);
        }]
      }
    })

    .state('menu.organizelist', {
      url: '/lists/:listId/organize',
      templateUrl: 'views/listsorganize.html',
      controller: 'ListOrganizeController',
      data: {
        authorize: 'canManageLists',
        saveLists: true
      },
      resolve: {
        list: ['$stateParams', 'ListService', function ($stateParams, ListService) {
          return ListService.getListWithItems($stateParams.listId, {includePrice: false});
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
        authorize: 'canCreateOrders',
        saveCarts: true
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
      url: ':cartId',
      templateUrl: 'views/cartitems.html',
      controller: 'CartItemsController',
      data: {
        authorize: 'canCreateOrders',
        saveCarts: true
      },
      resolve: {
        validBasketId: ['$stateParams', 'carts', 'changeOrders', 'ResolveService', function($stateParams, carts, changeOrders, ResolveService) {
          return ResolveService.validateBasket($stateParams.cartId, changeOrders);
        }],
        originalBasket: ['$stateParams', 'carts', 'changeOrders', 'validBasketId', 'ResolveService', function($stateParams, carts, changeOrders, validBasketId, ResolveService) {
          return ResolveService.selectValidBasket(validBasketId, changeOrders);
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
        authorize: 'canCreateOrders',
        saveCarts: true,
        saveLists: true
      },
      resolve: {
        lists: ['ListService', function (ListService){
          return ListService.getListHeaders();
        }],
        // carts: ['CartService', function(CartService) {
        //   return CartService.getCartHeaders();
        // }],
        // changeOrders: ['OrderService', function(OrderService) {
        //   return OrderService.getChangeOrders();
        // }],
        shipDates: ['CartService', function(CartService) {
          return CartService.getShipDates();
        }]
      }
    })
    .state('menu.addtoorder.items', {
      url: ':cartId/list/:listId/?useParlevel',
      templateUrl: 'views/addtoorder.html',
      controller: 'AddToOrderController',
      data: {
        authorize: 'canCreateOrders',
        saveCarts: true,
        saveLists: true
      },
      resolve: {
        // validBasketId: ['$stateParams', 'carts', 'changeOrders', 'ResolveService', function($stateParams, carts, changeOrders, ResolveService) {
        //   return ResolveService.validateBasket($stateParams.cartId, changeOrders);
        // }],
        // selectedCart: ['$stateParams', 'carts', 'changeOrders', 'validBasketId', 'ResolveService', function($stateParams, carts, changeOrders, validBasketId, ResolveService) {
        //   if ($stateParams.cartId !== 'New') {
        //     return ResolveService.selectValidBasket(validBasketId, changeOrders);
        //   }
        // }],
        selectedCart: ['$stateParams', 'CartService', 'OrderService', function($stateParams, CartService, OrderService) {
          if ($stateParams.cartId !== 'New') {
            // determine if id is a change order or a cart, carts are guids show they have dashes
            if ($stateParams.cartId.indexOf('-') > -1) {
              return CartService.getCart($stateParams.cartId);
            } else {
              return OrderService.getOrderDetails($stateParams.cartId);
            }
          }
        }],
        validListId: ['$stateParams', 'lists', 'ResolveService', function($stateParams, lists, ResolveService) {
          return ResolveService.validateList($stateParams.listId, 'isworksheet');
        }],
        selectedList: ['$stateParams', 'lists', 'validListId', 'ListService', function($stateParams, lists, validListId, ListService) {
          return ListService.getList(validListId);
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
        authorize: 'canViewOrders'
      }
    })
    .state('menu.orderitems', {
      url: '/order/:invoiceNumber/',
      templateUrl: 'views/orderitems.html',
      controller: 'OrderItemsController',
      data: {
        authorize: 'canViewOrders'
      },
      resolve: {
        order: [ '$stateParams', 'OrderService', function($stateParams, OrderService) {
          return OrderService.getOrderDetails($stateParams.invoiceNumber);
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
    .state('invoiceimage', {
      url: '/invoice/image/:invoiceNumber/',
      templateUrl: 'views/invoiceimage.html',
      controller: 'InvoiceImageController',
      data: {
        authorize: 'canPayInvoices'
      },
      resolve: {
        selectedUserContext: ['LocalStorage', function(LocalStorage) {
          if (LocalStorage.getTempContext()) {
            LocalStorage.setSelectedCustomerInfo(LocalStorage.getTempContext());
          } else if (LocalStorage.getTempBranch()) {
            LocalStorage.setSelectedBranchInfo(LocalStorage.getTempBranch());
          }
        }],
        images: ['$stateParams', 'InvoiceService', 'selectedUserContext', function($stateParams, InvoiceService, selectedUserContext) {
          return InvoiceService.getInvoiceImage($stateParams.invoiceNumber);
        }]
      }
    })

    /**********
    TRANSACTION
    **********/
    .state('menu.transaction', {
      url: '/transactions/',
      templateUrl: 'views/transaction.html',
      controller: 'TransactionController',
      data: {
        authorize: 'canPayInvoices'
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
      }
    })

    /**********
    REPORT
    **********/
    .state('menu.reports', {
      url: '/reports/',
      templateUrl: 'views/reports.html',
      controller: 'ReportsController',
      // data: {
      //     authorize: 'canPayInvoices'
      // }
    })
    .state('menu.itemusagereport', {
      url: '/reports/itemusage',
      templateUrl: 'views/itemusagereport.html',
      controller: 'ItemUsageReportController',
      data: {
        authorize: 'canPayInvoices'
      }
    })
    .state('menu.inventoryreport', {
      url: '/reports/inventory',
      templateUrl: 'views/inventoryreport.html',
      controller: 'InventoryReportController',
      resolve: {
        reports: ['ListService', function(ListService) {
          return ListService.getListsByType('inventoryValuation');
        }]
      },
      data: {
        authorize: 'canManageLists'
      }
    })


    /**********
    ADMIN
    **********/
    .state('menu.admin', {
      url: '/admin/',
      template: '<ui-view>'
    })

    .state('menu.admin.editinternaluser', {
      url: 'user/:email/',
      templateUrl: 'views/admin/editinternaluser.html',
      controller:'EditInternalUserController',
      data: {
        authorize: 'canEditInternalUsers'
      },
      resolve: {
        userProfile : ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
          return UserProfileService.getUserProfile($stateParams.email);
        }]
      }
    })
    .state('menu.admin.user', {
      url: 'customergroup/:groupId/user/:email/',
      abstract: true,
      template: '<ui-view/>',
      data: {
        authorize: 'canViewCustomerGroupDashboard'
      }
    })
    .state('menu.admin.user.edit', {
      url: 'edit/',
      templateUrl: 'views/admin/edituserdetails.html',
      controller: 'EditUserDetailsController',
      data: {
        authorize: 'canEditUsers'
      },
      resolve: {
        userProfile: ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
          return UserProfileService.getUserProfile($stateParams.email);
        }],
        userCustomers: ['UserProfileService', 'userProfile', function(UserProfileService, userProfile) {
          return UserProfileService.getAllUserCustomers(userProfile.userid);
        }]
      }
    })
    .state('menu.admin.user.view', {
      url: 'view/',
      templateUrl: 'views/admin/viewuserdetails.html',
      controller: 'ViewUserDetailsController',
      data: {
        authorize: 'canViewCustomerGroupDashboard'
      },
      resolve: {
        userProfile: ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
          return UserProfileService.getUserProfile($stateParams.email);
        }],
        userCustomers: ['UserProfileService', 'userProfile', function(UserProfileService, userProfile) {
          return UserProfileService.getAllUserCustomers(userProfile.userid);
        }]
      }
    })

    .state('menu.admin.customer', {
      url: 'customers/:customerNumber/:branchNumber/',
      templateUrl: 'views/admin/customerdetails.html',
      controller: 'CustomerDetailsController',
      data: {
        authorize: 'canViewCustomerGroupDashboard'
      }
    })
    .state('menu.admin.ordermanagement', {
      url: 'ordermanagement/',
      templateUrl: 'views/admin/ordermanagement.html',
      controller: 'OrderManagementController',
      resolve: {}
    })

    /*************
    ADMIN CUSTOMER GROUPS
    *************/
    .state('menu.admin.customergroupdashboard',{
      url: 'customergroup/dashboard/?customerGroupId',
      templateUrl: 'views/admin/customergroupdashboard.html',
      controller: 'CustomerGroupDashboardController',
      data: {
        authorize: 'canViewCustomerGroupDashboard'
      }
    })
    .state('menu.admin.customergroup', {
      url: 'customergroup/',
      templateUrl: 'views/admin/customergroups.html',
      controller: 'CustomerGroupsController',
      data: {
        authorize: 'canViewCustomerGroups'
      }
    })
    .state('menu.admin.customergroupdetails', {
      url: 'customergroup/new/',
      templateUrl: 'views/admin/customergroupdetails.html',
      controller: 'CustomerGroupDetailsController',
      data: {
        authorize: 'canManageCustomerGroups'
      }
    })

    /************************
    /* User Password
    /************************/
    .state('forgotpassword', {
        url: '/forgotpassword/?t',
        templateUrl: 'views/forgotpassword.html',
        controller: 'ForgotPasswordController',
        data: {},
        resolve: {
        validToken: ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
         return UserProfileService.validateToken($stateParams.t);
        }]
      }
    })
    .state('setpassword', {
        url: '/setpassword/?t',
        templateUrl: 'views/forgotpassword.html',
        controller: 'ForgotPasswordController',
        data: {},
        resolve: {
        validToken: ['$stateParams', 'UserProfileService', function($stateParams, UserProfileService) {
         return UserProfileService.validateToken($stateParams.t);
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
  
  // redirect when user tries to go to an abstract state
  // $urlRouterProvider.when('/lists', '/lists/0');
  // $urlRouterProvider.when('/lists/', '/lists/0');
  $urlRouterProvider.when('/cart/', '/cart/0');
  $urlRouterProvider.when('/cart/', '/cart/0');

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
