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

			$scope.selectedCategory = '';
			$scope.selectedBrands = [];
			$scope.selectedAllergens = [];
			$scope.isBrandShowing = false;
			$scope.brandHiddenNumber = 3;
			$scope.isAllergenShowing = false;
			$scope.allergenHiddenNumber = 3;

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