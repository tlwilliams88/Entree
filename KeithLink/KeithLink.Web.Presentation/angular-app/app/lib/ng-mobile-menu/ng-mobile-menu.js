angular.module('shoppinpal.mobile-menu', [])
    .run(['$rootScope', '$spMenu', function($rootScope, $spMenu){
        $rootScope.$spMenu = $spMenu;
    }])
    .provider("$spMenu", function(){
        this.$get = [function(){
           var menu = {};

           menu.show = function show(){
               var menu = angular.element(document.querySelector('#sp-nav'));
               menu.addClass('show');

               var page = angular.element(document.querySelector('.container-fluid'));
               page.addClass('noscroll');
           };

           menu.hide = function hide(){
               var menu = angular.element(document.querySelector('#sp-nav'));
               menu.removeClass('show');

               var page = angular.element(document.querySelector('.container-fluid'));
               page.removeClass('noscroll');
           };

           menu.toggle = function toggle() {
               var menu = angular.element(document.querySelector('#sp-nav'));
               menu.toggleClass('show');
               
               var page = angular.element(document.querySelector('.container-fluid'));
               page.toggleClass('noscroll');
           };

           return menu;
        }];
    });