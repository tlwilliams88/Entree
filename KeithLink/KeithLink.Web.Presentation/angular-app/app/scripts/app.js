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
    'angular-loading-bar'
  ])
.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', 'localStorageServiceProvider', 'cfpLoadingBarProvider',
  function($stateProvider, $urlRouterProvider, $httpProvider, localStorageServiceProvider, cfpLoadingBarProvider) {
  
  // configure loading bar
  cfpLoadingBarProvider.includeBar = false;

  // the $stateProvider determines path urls and their related controllers
  $stateProvider
    // register
    .state('register', {
      url: '/register/',
      templateUrl: 'views/register.html',
      controller: 'RegisterController',
      resolve: {
        api: ['ApiService', function(ApiService) {
          return ApiService.getEndpointUrl();
        }]
      }
    })
    .state('menu', {
      abstract: true, // path that cannot be navigated to directly, it can only be accessed by child views
      templateUrl: 'views/menu.html',
      controller: 'MenuController',
      resolve: {
        api: ['ApiService', function(ApiService) {
          return ApiService.getEndpointUrl();
        }],
        branches: ['api', 'BranchService', function(api, BranchService) {
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
      }
    })
    .state('menu.catalog', {
      abstract: true,
      url: '/catalog/',
      template: '<div ui-view=""></div>'
    })
    // /catalog
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
    // /catalog/:type/:id
    .state('menu.catalog.products.list', {
      url: ':type/:id/?brands',
      templateUrl: 'views/searchresults.html',
      controller: 'SearchController',
      data: {
        authorize: 'canBrowseCatalog'
      }
    })
    // /catalog/products/:itemNumber (item details page)
    .state('menu.catalog.products.details', {
      url: ':itemNumber/',
      templateUrl: 'views/itemdetails.html',
      controller: 'ItemDetailsController',
      data: {
        authorize: 'canBrowseCatalog'
      },
      resolve: {
        item: ['$stateParams', 'api', 'ProductService', function($stateParams, api, ProductService) {
          return ProductService.getProductDetails($stateParams.itemNumber);
        }]
      }
    })
    .state('menu.lists', {
      url: '/lists/',
      controller: 'ListController',
      template: '<ui-view/>',
      data: {
        authorize: 'canManageLists'
      },
      resolve: {
        lists: ['$q', 'api', 'ListService', function ($q, api, ListService){
          return $q.all([
            ListService.getAllLists(),
            ListService.getAllLabels()
          ]);
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
        carts: ['api', 'CartService', function (api, CartService){
          return CartService.getAllCarts();
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
      templateUrl: 'views/addtoorder.html',
      controller: 'AddToOrderController',
      data: {
        authorize: 'canCreateOrders'
      },
      resolve: {
        lists: ['api', 'ListService', function (api, ListService){
          return ListService.getAllLists();
        }],
        carts: ['api', 'CartService', function(api, CartService) {
          return CartService.getAllCarts();
        }]
      }
    })
    .state('menu.addtoorder.items', {
      url: ':listId',
      templateUrl: 'views/addtoorder.html',
      controller: 'AddToOrderController',
      data: {
        authorize: 'canCreateOrders'
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

  // set local storage prefix
  localStorageServiceProvider.setPrefix('bek');

}])
.run(['$rootScope', '$state', 'ApiService', 'AccessService', 'AuthenticationService', function($rootScope, $state, ApiService, AccessService, AuthenticationService) {

  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    console.log(toState.name);
    console.log(toParams);
    // check if route is protected
    if (toState.data && toState.data.authorize) {
      // check if user's token is expired
      if (!AccessService.isLoggedIn()) {
        AuthenticationService.logout();
        $state.transitionTo('register');
        event.preventDefault();
      }

      // check if user has access to the route
      if (!AccessService[toState.data.authorize]()) {
        $state.transitionTo('register');
        event.preventDefault(); 
      }
    }

    // redirect register page to homepage if logged in
    if (toState.name === 'register' && AccessService.isLoggedIn()) {

      if ( AccessService.isOrderEntryCustomer() ) {
        $state.transitionTo('menu.home');  
      } else {
        $state.transitionTo('menu.catalog.home');
      }

      event.preventDefault(); 
    }

  });
}]);
