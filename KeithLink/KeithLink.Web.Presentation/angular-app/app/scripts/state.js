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
    /**********
    REGISTER
    **********/
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
        branches: ['BranchService', 'localStorageService', function(BranchService, localStorageService) {
          // guest users must have branches to load the page (but non-guest users do not)
          // also needed for tech support
          var branches = localStorageService.get('branches');

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
        }],
        mandatoryMessages: ['NotificationService', function(NotificationService) {
          return NotificationService.mandatoryMessages;
        }],
        isHomePage: ['$stateParams', function($stateParams) {
          return $stateParams.isHomePage = false;
        }]
      }
    })
    /**********
    HOME
    **********/
    .state('menu.home', {
      url: '/home/',
      templateUrl: 'views/home.html',
      controller: 'HomeController',
      data: {
        authorize: 'isOrderEntryCustomer',
        saveCarts: true,
      },
      resolve: {
        isHomePage: ['$stateParams', '$rootScope', function($stateParams, $rootScope) {
          $rootScope.returnToStateItemNumber = '';
          $rootScope.returnToStateName = '';
          return $stateParams.isHomePage = true;
        }]
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
        branches: ['BranchService', 'localStorageService', function(BranchService, localStorageService) {
          return localStorageService.get('branches');
        }]
      }
    })
    .state('menu.applicationsettings', {
      url: '/applicationsettings/',
      templateUrl: 'views/applicationsettings.html',
      controller: 'ApplicationSettingsController',
      data: {
        authorize: 'isLoggedIn'
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
    CAMPAIGN CATALOG
    **********/
    .state('menu.campaign', {
      url: '/catalog/campaign/:campaign_id',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        campaignInfo: ['$stateParams', 'ProductService', function($stateParams, ProductService) {
          return ProductService.getCampaignDetails($stateParams.campaign_id);
        }]
      }
    })

    /**********
    CATALOG
    **********/
    .state('menu.catalog', {
      abstract: true,
      url: '/catalog/:catalogType/',
      template: '<div ui-view=""></div>',
      params: {
        catalogType: {
          value: 'BEK',
          squash: false
        }
      }
    })
    .state('menu.catalog.home', {
      url: '',
      templateUrl: function($stateParams) {
          if ($stateParams.catalogType == 'UNFI') {
              return 'views/unfi-catalog.html';
          } else {
              return 'views/catalog.html';
          }
      },
      controller: 'CatalogController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        security: ['LocalStorage', '$q', '$stateParams', function(LocalStorage, $q, $stateParams) {
          var customerRecord = LocalStorage.getCurrentCustomer();

          if( $stateParams.catalogType == 'UNFI' && customerRecord.customer.canViewUNFI == false){
            return $q.reject('Customer Cannot View UNFI Catalog.');
          }

        }]
      }
    })
    .state('menu.catalog.products', {
      abstract: true,
      url: 'products/',
      template: '<div ui-view=""></div>',
      resolve: {
        security: ['LocalStorage', '$q', '$stateParams', function(LocalStorage, $q, $stateParams) {
          var customerRecord = LocalStorage.getCurrentCustomer();

          if( $stateParams.catalogType == 'UNFI' && customerRecord.customer.canViewUNFI == false){
            return $q.reject('Customer Cannot View UNFI Items.');
          }

        }]
      }
    })
    .state('menu.catalog.products.list', {
      url: ':type/:id/:deptName/?currentPage/?startingPoint/?parentcategories/?subcategories/?brands/?manufacturers/?dietary/?itemspecs/?temp_zones/?specialfilters',
      params: {
        brand: null,
        category: null,
        dept: null,
        deptName: null
      },
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        security: ['LocalStorage', '$q', '$stateParams', 'ListService', function(LocalStorage, $q, $stateParams, ListService) {
          var customerRecord = LocalStorage.getCurrentCustomer();

          ListService.getListHeaders();

          if( $stateParams.catalogType == 'UNFI' && customerRecord.customer.canViewUNFI == false){
            return $q.reject('Customer Cannot View UNFI Items.');
          }
        }],
        campaignInfo: [ function() {
          return false;
        }]
      }
    })
    .state('menu.catalog.products.brand', {
      url: 'catalog/:catalogType/search/:type/:id/products',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        security: ['LocalStorage', '$q', '$stateParams', function(LocalStorage, $q, $stateParams) {
          var customerRecord = LocalStorage.getCurrentCustomer();

          if( $stateParams.catalogType == 'UNFI' && customerRecord.customer.canViewUNFI == false){
            return $q.reject('Customer Cannot View UNFI Items.');
          }

        }],
        campaignInfo: [ function() {
          return false;
        }]
      }
    })
    .state('menu.catalog.products.details', {
      url: ':itemNumber',
      templateUrl: 'views/itemdetails.html',
      controller: 'ItemDetailsController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        item: ['$stateParams', 'ProductService', function($stateParams, ProductService) {
          return ProductService.getProductDetails($stateParams.itemNumber, $stateParams.catalogType);
        }],

        security: ['LocalStorage', '$q', '$stateParams', function(LocalStorage, $q, $stateParams) {
          var customerRecord = LocalStorage.getCurrentCustomer();

          if( $stateParams.catalogType == 'UNFI' && customerRecord.customer.canViewUNFI == false){
            return $q.reject('Customer Cannot View UNFI Items.');
          }

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
      url: ':listId/:listType/?renameList',
      templateUrl: 'views/lists.html',
      controller: 'ListController',
      data: {
        authorize: 'canManageLists',
        saveLists: true
      },
      params: {
        noAvailableLists: false
      },
      resolve: {
        originalList: ['$stateParams', '$filter', 'lists', 'ListService', 'DateService', 'Constants', 'LocalStorage', 'ENV',
         function($stateParams, $filter, lists, ListService, DateService, Constants, LocalStorage, ENV) {

          var last = LocalStorage.getLastList(),
              stillExists = false,
              pageSize = $stateParams.pageSize = LocalStorage.getPageSize(),
              params = {size: pageSize, from: 0, sort: [], message: 'Loading List...'},
              listToBeUsed = {},
              listHeader = {},
              selectedList = {
                  listId: $stateParams.listId,
                  listType: $stateParams.listType
              };

          ListService.listHeaders.forEach(function(list){
            if(last && last.listId && (last.listId == list.listid)){
              stillExists = true;
              listHeader = $filter('filter')(ListService.listHeaders, {listid: last.listId, type: last.listType})[0];
            }
          });

          if((last && stillExists && (!$stateParams.renameList || $stateParams.renameList === 'false')) || last && last.listId == 'nonbeklist'){
            listToBeUsed.listId = last.listId;
            listToBeUsed.listType = last.listType;
          }

          listToBeUsed.listId = listToBeUsed.listId != 'nonbeklist' ? parseInt(listToBeUsed.listId, 10) : listToBeUsed.listId;
          listToBeUsed.listType = listHeader ? listHeader.type : listToBeUsed.listType;

          if((isNaN(listToBeUsed.listId) || !listHeader) && listToBeUsed.listId != 'nonbeklist'){
            var contractList = $filter('filter')(ListService.listHeaders, { type: 2 }),
                historyList = $filter('filter')(ListService.listHeaders, { type: 5 }),
                favoritesList = $filter('filter')(ListService.listHeaders, { type: 1 }),
                customList = $filter('filter')(ListService.listHeaders, { type: 0 });

            if(contractList.length > 0){
              listToBeUsed = contractList[0];
            }
            else if(historyList.length > 0){
              listToBeUsed = historyList[0];
            }
            else if(favoritesList.length > 0){
              listToBeUsed = favoritesList[0];
            }
            else if(customList.length > 0){
              listToBeUsed = customList[0];
            }
             else{
              var defaultList = {type:1};
              return ListService.createList([], defaultList).then(function(resp){
                $stateParams.noAvailableLists = true;
                return ListService.updateListPermissions(resp);
              });
            }
          }
          
          $stateParams.listId = listToBeUsed.listid ? listToBeUsed.listid : listToBeUsed.listId;
          $stateParams.listType = listToBeUsed.type ? listToBeUsed.type : listToBeUsed.listType;
          
          if(listToBeUsed && ($stateParams.listType == Constants.listType.Worksheet || $stateParams.listType == Constants.listType.Contract || $stateParams.listType == Constants.listType.Recommended || $stateParams.listType == Constants.listType.Mandatory)){
            ListService.getParamsObject(params, 'lists').then(function(storedParams){
              $stateParams.sortingParams = storedParams;
              params = storedParams;
            });
          }

          if(listToBeUsed.listId == 'nonbeklist'){
            return ListService.getCustomInventoryList();
          } else {
            LocalStorage.setLastList(listToBeUsed);
            if($stateParams.listType == Constants.listType.Contract) {
                params.filter = {
                          field: 'delta',
                          value: 'active'
                        }
            }
            return ListService.getList(listToBeUsed, params);
          }

        }]
      }
    })

    .state('menu.organizelist', {
      url: '/lists/:listType/:listId/organize',
      templateUrl: 'views/listsorganize.html',
      controller: 'ListOrganizeController',
      data: {
        authorize: 'canManageLists',
        saveLists: true
      },
      resolve: {
        list: ['$stateParams', 'ListService', function ($stateParams, ListService) {
          var list = {
            listId : $stateParams.listId,
            listType : $stateParams.listType
          }
          return ListService.getListWithItems(list, {includePrice: false});
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
      url: ':cartId/list/:listId/:listType/?useParlevel/?continueToCart/?searchTerm/?createdFromPrint/?currentPage/?pageLoaded',
      params: {
        listItems: null
      },
      templateUrl: 'views/addtoorder.html',
      controller: 'AddToOrderController',
      data: {
        authorize: 'canCreateOrders',
        saveCarts: true,
        saveLists: true
      },
      resolve: {
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
        selectedList: ['$stateParams', '$filter', 'ListService', 'DateService', 'Constants', 'LocalStorage', 'ENV',
         function($stateParams, $filter, ListService, DateService, Constants, LocalStorage, ENV) {

          var pageSize = $stateParams.pageSize = LocalStorage.getPageSize(),
              params = {size: pageSize, from: 0, sort: []},
              listToBeUsed = {
                 listId: $stateParams.listId,
                 listType: $stateParams.listType
              },
              lists = ListService.listHeaders,
              lastOrderList = LocalStorage.getLastOrderList();
          
          listToBeUsed = listToBeUsed.listId && listToBeUsed.listType ? listToBeUsed : $filter('filter')(lists, {listid: lastOrderList.listId, type: lastOrderList.listType})[0];
          
          if(listToBeUsed == undefined){
            if(lists.length == 0) {
              listToBeUsed = {
                  listId: $stateParams.listId,
                  listType: $stateParams.listType
              }
            } else if (list.length > 0) {
              var contractList = $filter('filter')(lists, { type: 2 }),
                  historyList = $filter('filter')(lists, { type: 5 }),
                  favoritesList = $filter('filter')(lists, { type: 1 }),
                  customList = $filter('filter')(lists, { type: 0 });

              if(contractList.length > 0){
                listToBeUsed = contractList[0];
              }
              else if(historyList.length > 0){
                listToBeUsed = historyList[0];
              }
              else if(favoritesList.length > 0){
                listToBeUsed = favoritesList[0];
              }
              else if(customList.length > 0){
                listToBeUsed = customList[0];
              }
            }

          }
          
          $stateParams.listId = listToBeUsed.listid ? listToBeUsed.listid : listToBeUsed.listId;
          $stateParams.listType = listToBeUsed.type ? listToBeUsed.type : listToBeUsed.listType;

          if(listToBeUsed && ($stateParams.listType == Constants.listType.Worksheet || $stateParams.listType == Constants.listType.Contract || $stateParams.listType == Constants.listType.Recommended || $stateParams.listType == Constants.listType.Mandatory)){
            ListService.getParamsObject(params, 'addToOrder').then(function(storedParams){
              $stateParams.sortingParams = storedParams;
              params = storedParams;
            });
          }

          LocalStorage.setLastOrderList(listToBeUsed);
          if($stateParams.listType == Constants.listType.Contract) {
              params.filter = {
                        field: 'delta',
                        value: 'active'
                      }
          }
          return ListService.getList(listToBeUsed, params);
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
        authorize: 'canViewInvoices'
      },
      resolve:{
        canPayInvoices: [ 'AccessService', function(AccessService) {
          return AccessService.canPayInvoices();
        }]
      }
    })
    .state('menu.invoiceitems', {
      url: '/invoice/:invoiceNumber/',
      templateUrl: 'views/invoiceitems.html',
      controller: 'InvoiceItemsController',
      data: {
        authorize: 'canViewInvoices'
      },
      resolve: {
        invoice: [ '$stateParams', 'InvoiceService', function($stateParams, InvoiceService) {
          return InvoiceService.getInvoice($stateParams.invoiceNumber);
        }]
      }
    })
    .state('invoiceimage', {
      url: '/invoice/image/{invoiceNumber}?={customerid}&={branchid}',
      templateUrl: 'views/invoiceimage.html',
      controller: 'InvoiceImageController',
      data: {
        authorize: 'canViewInvoices'
      },
      resolve: {
        images: ['$stateParams', 'InvoiceService', function($stateParams, InvoiceService) {
          return InvoiceService.getInvoiceImage($stateParams.invoiceNumber, $stateParams.customerid, $stateParams.branchid);
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
        authorize: 'canViewMarketing'
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
        authorize: 'canRunReports'
      }
    })
    .state('menu.inventoryreport', {
      url: '/reports/inventory/:listid/',
      templateUrl: 'views/inventoryreport.html',
      controller: 'InventoryReportController',
      resolve: {
        reports: ['ListService', function(ListService) {
          return ListService.getListsByType('inventoryValuation');
        }]
      },
      data: {
        authorize: 'canRunReports'
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
      },
      resolve : {
        currentUserProfile : ['UserProfileService', function(UserProfileService){
        return UserProfileService.getCurrentUserProfile();
      }]
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

  $stateProvider
    .state('menu.configsettings', {
      url: '/configsettings',
      templateUrl: 'views/admin/configsettings.html',
      controller: 'ConfigSettingsController',
      resolve: {
        security: ['UserProfileService', '$q', function(UserProfileService, $q) {
          return UserProfileService.getCurrentUserProfile().then(function(resp){
            var profile = resp;

          if(profile.rolename !== 'beksysadmin'){
            return $q.reject('User Not Authorized');
          }
          });
        }]

      }
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
