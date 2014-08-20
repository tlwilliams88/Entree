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
    'infinite-scroll'
  ])
.config(['$stateProvider', '$urlRouterProvider', '$httpProvider', 'localStorageServiceProvider', function($stateProvider, $urlRouterProvider, $httpProvider, localStorageServiceProvider) {
  // the $stateProvider determines path urls and their related controllers
  $stateProvider
    .state('login', {
      url: '/login/',
      templateUrl: 'views/login.html'
      // controller: 'MenuController'
    })
    .state('menu', {
      abstract: true, // path that cannot be navigated to directly, it can only be accessed by child views
      templateUrl: 'views/menu.html',
      controller: 'MenuController'
    })
    // register
    .state('menu.register', {
      url: '/register/',
      templateUrl: 'views/register.html',
      controller: 'RegisterController'
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
      }
    })
    .state('menu.lists', {
      url: '/lists/',
      templateUrl: 'views/lists.html',
      controller: 'ListController',
      data: {
        authorize: 'canManageLists'
      }
    })
    .state('menu.listitems', {
      url: '/lists/:listId/?renameList',
      templateUrl: 'views/lists.html',
      controller: 'ListController',
      data: {
        authorize: 'canManageLists'
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

  // add authorization headers
  $httpProvider.interceptors.push('AuthenticationInterceptorService');

  // append correct api url to all relevant http requests
  $httpProvider.interceptors.push(['$q', '$injector', function($q, $injector) {
    return {
     'request': function(config) {
        $injector.invoke(['$http', 'ApiService', function($http, ApiService) {
          if (config.url[0] === '/') {
            config.url = ApiService.endpointUrl + config.url;

            console.log('url: ' + config.url);
            console.log(config.params);
            
          }
       }]);
       return config || $q.when(config);
      }
    };
  }]);

  // set local storage prefix
  localStorageServiceProvider.setPrefix('bek');

}])
.run(['$rootScope', '$state', 'ApiService', 'AccessService', function($rootScope, $state, ApiService, AccessService) {

  // ApiService.endpointUrl = 'http://devapi.bekco.com';
  ApiService.getEndpointUrl().then(function(response) {
    ApiService.endpointUrl = location.protocol + '//' + response.data.ClientApiEndpoint;
  });

  $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
    if (toState.data && toState.data.authorize && !AccessService[toState.data.authorize]()) {
      $state.transitionTo('menu.register');
      event.preventDefault(); 
    }

    if (toState.name === 'menu.register' && AccessService.isLoggedIn()) {
      $state.transitionTo('menu.home');
      event.preventDefault(); 
    }

  });
}]);
