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
			
			var branchId = $scope.currentUser.currentLocation.branchId;
			var type = $stateParams.type;

			$scope.breadcrumbs = [];
			$scope.loadingResults = true;

		  	$scope.totalItems = 175;
		  	$scope.currentPage = 1;
		  	$scope.itemsPerPage = 30;
		  	$scope.itemIndex = 0;


			$scope.loadMore = function() {


		    	$scope.itemIndex += $scope.itemsPerPage;
		    	$scope.currentPage++;
		        
		    	if ($scope.products.length >= $scope.totalItems) {
		    		return;
		    	}

		        ProductService.getProducts(branchId, searchTerm, $scope.itemsPerPage, $scope.itemIndex).then(function(data) {
		        	$scope.products.push.apply($scope.products, data.products);
		        	console.log('reload: ' + $scope.currentPage);

					$scope.predicate = 'id';
					$scope.loadingResults = false;
				});
		    };


			if (type === 'category') {
				$scope.breadcrumbs[0] = 'Category';

				ProductService.getProductsByCategory(branchId, $stateParams.id, $scope.itemsPerPage).then(function(data) {
					$scope.products = data.products;
					$scope.categories = data.facets[0].facetvalues;
					$scope.brands = data.facets[1].facetvalues;
					$scope.totalItems = data.totalcount;

					$scope.predicate = 'id';
					$scope.loadingResults = false;
				});

			} else if (type === 'search') {
				var searchTerm =  $stateParams.id;

				$scope.breadcrumbs.search = searchTerm;

				ProductService.getProducts(branchId, searchTerm, $scope.itemsPerPage).then(function(data) {
					$scope.products = data.products;
					$scope.categories = data.facets[0].facetvalues;
					$scope.brands = data.facets[1].facetvalues;
					$scope.totalItems = data.totalcount;

					$scope.predicate = 'id';
					$scope.loadingResults = false;
				});

			} else if (type === 'brand') {
				$scope.breadcrumbs[0] = 'Brand';

			}

			$scope.pageChanged = function(pageNum, itemsPerPage) {
				var itemIndex = $scope.itemIndex = (pageNum - 1) * itemsPerPage;

				$scope.loadingResults = true;
		   		ProductService.getProducts(branchId, searchTerm, $scope.itemsPerPage, itemIndex).then(function(response) {
					$scope.products = response.data.products;
					$scope.predicate = 'id';
					$scope.loadingResults = false;
				});
		  };

		  



			
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


				ProductService.getProductsByCategory(branchId, $stateParams.id, $scope.itemsPerPage, 0, $scope.selectedBrands, $scope.selectedCategory).then(function(data) {
					$scope.products = data.products;
					$scope.categories = data.facets[0].facetvalues;
					$scope.brands = data.facets[1].facetvalues;
					$scope.totalItems = data.totalcount;

					$scope.predicate = 'id';
					$scope.loadingResults = false;
				});
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