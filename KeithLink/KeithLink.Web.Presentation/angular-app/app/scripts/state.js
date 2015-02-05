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
      controller: 'RegisterController'
    })
    .state('changepassword', {
        url: '/changepassword/',
        templateUrl: 'views/changepassword.html',
        controller: 'ChangePasswordController'
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
      },
      controller: ['$scope', 'ListService', function($scope, ListService) {
        $scope.$on('$destroy', function() {
          ListService.lists = [];
          ListService.labels = [];
        });
      }]
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
      },
      controller: ['$scope', 'CartService', function($scope, CartService) {
        $scope.$on('$destroy', function() {
          CartService.carts = [];
        });
      }]
    })
    .state('menu.cart.items', {
      url: ':cartId/?renameCart',
      templateUrl: 'views/cartitems.html',
      controller: 'CartItemsController',
      data: {
        authorize: 'canCreateOrders'
      },
      resolve: {
        originalBasket: ['$stateParams', 'carts', 'changeOrders', 'ResolveService', 'CartService', function($stateParams, carts, changeOrders, ResolveService, CartService) {
          var selectedBasket = ResolveService.selectDefaultBasket($stateParams.cartId, changeOrders);
          if (selectedBasket) {
            return selectedBasket;
          } else {
            // no existing carts found, create a new cart and redirect to it
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
      // ,
      // controller: ['$state', 'carts', function($state, carts) {
      //   $state.go('menu.addtoorder.items', { listId: 123, cartId: 'new' });
      // }]
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
        selectedCart: ['$stateParams', 'carts', 'changeOrders', 'ResolveService', function($stateParams, carts, changeOrders, ResolveService) {
          var selectedBasket = ResolveService.selectDefaultBasket($stateParams.cartId, changeOrders);
          return selectedBasket;
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
    TRANSACTION
    **********/
    .state('menu.transaction', {
      url: '/transactions/',
      templateUrl: 'views/transaction.html',
      controller: 'TransactionController',
      data: {
        authorize: 'canPayInvoices'
      },
      resolve: {
        accounts: ['BankAccountService', function(BankAccountService) {
          return BankAccountService.getAllBankAccounts();
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
    .state('menu.admin.edituser', {
      url: 'customergroup/:groupId/edituser/:email/',
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
      url: 'customers/:customerNumber/:branchNumber/',
      templateUrl: 'views/admin/customerdetails.html',
      controller: 'CustomerDetailsController',
      data: {
        authorize: 'canManageAccount'
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
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.customergroup', {
      url: 'customergroup/',
      templateUrl: 'views/admin/customergroups.html',
      controller: 'CustomerGroupsController',
      data: {
        authorize: 'canManageAccount'
      }
    })
    .state('menu.admin.customergroupdetails', {
      url: 'customergroup/:groupId/',
      templateUrl: 'views/admin/customergroupdetails.html',
      controller: 'CustomerGroupDetailsController',
      data: {
        authorize: 'canManageAccount'
      },
      resolve: {
        originalCustomerGroup: ['$stateParams', 'CustomerGroupService', function($stateParams, CustomerGroupService) {
          if ($stateParams.groupId === 'new') {
            return {};
          } else {
            return CustomerGroupService.getGroupDetails($stateParams.groupId);
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
  
  // redirect when user tries to go to an abstract state
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
