'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
.controller('SearchController', ['$scope', 'ProductService', 'CategoryService', '$stateParams',
   function($scope, ProductService, CategoryService, $stateParams) {
       // clear keyword search term at top of the page
       if ($scope.userBar) {
           $scope.userBar.universalSearchTerm = '';
       }
       
       $scope.breadcrumbs = [];
       $scope.loadingResults = true;

         $scope.itemsPerPage = 30;
         $scope.itemIndex = 0;

                $scope.oneAtATime = true;
       $scope.items = ['Item 1', 'Item 2', 'Item 3'];
       $scope.selectedCategory = '';
       $scope.selectedSubcategory = '';
       $scope.selectedBrands = [];
       $scope.selectedAllergens = [];
       $scope.isBrandShowing = false;
       $scope.brandHiddenNumber = 3;
       $scope.isAllergenShowing = false;
       $scope.allergenHiddenNumber = 3;
       $scope.hidden= true;

       function getData() {
           var type = $stateParams.type;
           var branchId = $scope.currentUser.currentLocation.branchId;

           if (type === 'category') {

               var categoryId = $stateParams.id;
               return ProductService.getProductsByCategory(branchId, categoryId, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory);

           } else if (type === 'search') {

               var searchTerm = $stateParams.id;
               return ProductService.getProducts(branchId, searchTerm, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory);
           } else if (type === 'brand') {

               var searchTerm = $stateParams.id;
               return ProductService.getProducts(branchId, searchTerm, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory);
           }
       }

       function loadProducts(appendResults) {
           $scope.loadingResults = true;

           getData().then(function(data) {

               if (appendResults) {
                   $scope.products.push.apply($scope.products, data.products);
               } else {
                   $scope.products = data.products;
               }

               $scope.categories = data.facets.categories;
               if($scope.selectedBrands.length===0){
                $scope.brands = data.facets.brands;
              }
               $scope.totalItems = data.totalcount;
               console.log('totalitems: ' + data.totalcount);
               console.log('products: ' + $scope.products.length);

               $scope.predicate = 'id';
               $scope.loadingResults = false;
           });
       }

       loadProducts();

       $scope.infiniteScrollLoadMore = function() {

           if ($scope.products.length >= $scope.totalItems || $scope.loadingResults) {
               return;
           }

           $scope.itemIndex += $scope.itemsPerPage;

           console.log('more: ' + $scope.itemIndex);
           loadProducts(true);
       };

       


       $scope.showContextMenu = function(e, idx) {
           $scope.contextMenuLocation = { 'top': e.y, 'left': e.x };
           $scope.isContextMenuDisplayed = true;
       };

          $scope.showBrand = function(){
           $scope.isBrandShowing = true;
           $scope.brandHiddenNumber = 100;
       };

       $scope.showAllergen = function(){
           $scope.isAllergenShowing = true;
           $scope.allergenHiddenNumber = 100;
       };

       $scope.toggleSelection = function toggleSelection(selectedFacet, filter) {
           selectedFacet.show = !selectedFacet.show;
           var idx;
           if (filter === 'brand') {
               idx = $scope.selectedBrands.indexOf(selectedFacet);

               // is currently selected
               if (idx > -1) {
                   $scope.selectedBrands.splice(idx, 1);
               }
               // is newly selected
               else {
                   $scope.selectedBrands.push(selectedFacet);
               }
           } else if(filter==='allergen') {
               idx = $scope.selectedAllergens.indexOf(selectedFacet);

               // is currently selected
               if (idx > -1) {
                   $scope.selectedAllergens.splice(idx, 1);
               }
               // is newly selected
               else {
                   $scope.selectedAllergens.push(selectedFacet);
               }
           } else if(filter==='subcategory'){
               $scope.selectedSubcategory = selectedFacet.id;
           }
           else{
               $scope.selectedCategory = selectedFacet.name;
               $scope.selectedSubcategory = '';
           }

           loadProducts();
       };

       $scope.allergens = [{
           id: 1,
           name: 'No Eggs'
       }, {
           id: 2,
           name: 'No Soy'
       }, {
           id: 3,
           name: 'No Fish'
       }, {
           id: 4,
           name: 'No Milk'
       }, {
           id: 5,
           name: 'No Wheat'
       }, {
           id: 6,
           name: 'No Shellfish'
       }, {
           id: 7,
           name: 'No Peanuts'
       }, {
           id: 8,
           name: 'No TreeNuts'
       }];

   }
]);
