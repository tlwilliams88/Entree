angular.module('shoppinpal.mobile-menu', [])
    .run(['$rootScope', '$spMenu', function($rootScope, $spMenu){
        $rootScope.$spMenu = $spMenu;
    }])
    .provider("$spMenu", function(){
        this.$get = [function(){
           var menu = {};

           menu.show = function show(){
              if (window.innerWidth < 992) {
               var menu = angular.element(document.querySelector('#sp-nav'));
               menu.addClass('show');


               var page = angular.element(document.querySelector('body'));
               page.addClass('noscroll');
              }
           };

           menu.hide = function hide(){
              if (window.innerWidth < 992) {
               var menu = angular.element(document.querySelector('#sp-nav'));
               menu.removeClass('show');

               var page = angular.element(document.querySelector('body'));
               page.removeClass('noscroll');
              }
           };

           menu.toggle = function toggle() {
              if (window.innerWidth < 992) {
               var menu = angular.element(document.querySelector('#sp-nav'));
               menu.toggleClass('show');

               var page = angular.element(document.querySelector('body'));
               page.toggleClass('noscroll');
              }
           };

           return menu;
        }];
    });
