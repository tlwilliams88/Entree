'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
	.controller('SearchController', ['$scope', 'ProductService', 'CategoryService',
		function($scope, ProductService, CategoryService) {
			
			$scope.oneAtATime = true;
			$scope.items = ['Item 1', 'Item 2', 'Item 3'];
			$scope.selectedCategory = '';
			$scope.selectedBrands = [];
			$scope.selectedAllergens = [];
			$scope.isBrandShowing = false;
			$scope.brandHiddenNumber = 3;
			$scope.isAllergenShowing = false;
			$scope.allergenHiddenNumber = 3;
			$scope.hidden= true;

			$scope.products = [{'categoryId':'FS490','id':'101285','description':'Shrimp Raw Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'5 LB','upc':'00000000000000','manufacturer_number':'B-W-26/30','manufacturer_name':'Ellington Farms Seafood','cases':'0','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101286','description':'Shrimp Cooked Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'6 LB','upc':'00000000000001','manufacturer_number':'B-W-26/31','manufacturer_name':'Ellington Farms Seafood 2','cases':'1','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101285','description':'Shrimp Raw Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'5 LB','upc':'00000000000000','manufacturer_number':'B-W-26/30','manufacturer_name':'Ellington Farms Seafood','cases':'0','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101286','description':'Shrimp Cooked Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'6 LB','upc':'00000000000001','manufacturer_number':'B-W-26/31','manufacturer_name':'Ellington Farms Seafood 2','cases':'1','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101285','description':'Shrimp Raw Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'5 LB','upc':'00000000000000','manufacturer_number':'B-W-26/30','manufacturer_name':'Ellington Farms Seafood','cases':'0','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101286','description':'Shrimp Cooked Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'6 LB','upc':'00000000000001','manufacturer_number':'B-W-26/31','manufacturer_name':'Ellington Farms Seafood 2','cases':'1','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101285','description':'Shrimp Raw Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'5 LB','upc':'00000000000000','manufacturer_number':'B-W-26/30','manufacturer_name':'Ellington Farms Seafood','cases':'0','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101286','description':'Shrimp Cooked Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'6 LB','upc':'00000000000001','manufacturer_number':'B-W-26/31','manufacturer_name':'Ellington Farms Seafood 2','cases':'1','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101285','description':'Shrimp Raw Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'5 LB','upc':'00000000000000','manufacturer_number':'B-W-26/30','manufacturer_name':'Ellington Farms Seafood','cases':'0','kosher':'true','price':'325.00'},{'categoryId':'FS490','id':'101286','description':'Shrimp Cooked Hdls 25/30','ext_description':'Premium Wild Texas White','brand':'Cortona','size':'6 LB','upc':'00000000000001','manufacturer_number':'B-W-26/31','manufacturer_name':'Ellington Farms Seafood 2','cases':'1','kosher':'true','price':'325.00'}];
	    $scope.categories = [{'id':'85000','name':'Restaurant Supply','description':'Restaurant Supply','subcategories':null},{'id':'AB000','name':'Fresh Bread','description':'Fresh Bread','subcategories':null},{'id':'AD000','name':'Dairy','description':'Dairy','subcategories':null},{'id':'AP000','name':'All Produce','description':'All Produce','subcategories':null},{'id':'BE000','name':'Beverage Equipment','description':'Beverage Equipment','subcategories':null},{'id':'BF000','name':'Beverage & Fountain','description':'Beverage & Fountain','subcategories':null},{'id':'BP000','name':'Fresh Meat','description':'Fresh Meat','subcategories':null},{'id':'CC000','name':'Cookies, Crackers, Candies','description':'Cookies, Crackers, Candies','subcategories':null},{'id':'AB000','name':'Fresh Bread','description':'Fresh Bread','subcategories':null},{'id':'AD000','name':'Dairy','description':'Dairy','subcategories':null},{'id':'AP000','name':'All Produce','description':'All Produce','subcategories':null},{'id':'BE000','name':'Beverage Equipment','description':'Beverage Equipment','subcategories':null},{'id':'BF000','name':'Beverage & Fountain','description':'Beverage & Fountain','subcategories':null},{'id':'BP000','name':'Fresh Meat','description':'Fresh Meat','subcategories':null},{'id':'CC000','name':'Cookies, Crackers, Candies','description':'Cookies, Crackers, Candies','subcategories':null}];

			$scope.showBrand = function(){
				$scope.isBrandShowing = true;
				$scope.brandHiddenNumber = 100;
			};

			$scope.showAllergen = function(){
				$scope.isAllergenShowing = true;
				$scope.allergenHiddenNumber = 100;
			};

			ProductService.getProducts().then(function() {
				$scope.products = ProductService.products;
				$scope.predicate = 'id';
			});

			CategoryService.getCategories().then(function() {
				$scope.categories = CategoryService.categories;
			});

			$scope.toggleSelection = function toggleSelection(id, filter) {
				var idx;
				if (filter === 'brand') {
					idx = $scope.selectedBrands.indexOf(id);

					// is currently selected
					if (idx > -1) {
						$scope.selectedBrands.splice(idx, 1);
					}
					// is newly selected
					else {
						$scope.selectedBrands.push(id);
					}
				} else if(filter==='allergen') {
					idx = $scope.selectedAllergens.indexOf(id);

					// is currently selected
					if (idx > -1) {
						$scope.selectedAllergens.splice(idx, 1);
					}
					// is newly selected
					else {
						$scope.selectedAllergens.push(id);
					}
				}
				else{
					$scope.selectedCategory = id;
				}

			};

			$scope.brands = [{
				id: 1,
				name: 'Admiral Of The Fleet'
			}, {
				id: 2,
				name: 'Cortona'
			}, {
				id: 3,
				name: 'Ellington Farms'
			}, {
				id: 4,
				name: 'Golden Harvest'
			}, {
				id: 5,
				name: 'Philly'
			}, {
				id: 6,
				name: 'Markon Cooperative'
			}, {
				id: 7,
				name: 'Ceylon Tea Gardens'
			}, {
				id: 8,
				name: 'ComSource'
			}
			];

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