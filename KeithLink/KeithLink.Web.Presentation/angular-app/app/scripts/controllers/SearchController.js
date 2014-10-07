'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
    .controller('SearchController', ['$scope', '$state', '$stateParams', 'ProductService', 'CategoryService', 'ListService', 'CartService', 'BrandService',
        function($scope, $state, $stateParams, ProductService, CategoryService, ListService, CartService, BrandService) {
            // clear keyword search term at top of the page
            if ($scope.userBar) {
                $scope.userBar.universalSearchTerm = '';
            }

            $scope.loadingResults = true;

            $scope.itemsPerPage = 50;
            $scope.itemIndex = 0;

            $scope.oneAtATime = true;
            $scope.selectedCategory = '';
            $scope.selectedSubcategory = '';
            $scope.selectedBrands = [];
            $scope.selectedDietary = [];
            $scope.selectedSpecs = [];
            $scope.selectedNonstock = [];
            $scope.isBrandShowing = false;
            $scope.isDietaryShowing = false;
            $scope.isSpecShowing = false;
            $scope.brandHiddenNumber = 3;
            $scope.dietaryHiddenNumber = 3;
            $scope.specHiddenNumber = 3;
            $scope.constantHiddenNumber = 4;
            $scope.brandCount = 0;
            $scope.dietaryCount = 0;
            $scope.specCount = 0;
            $scope.hidden = true;
            $scope.sortField = '';
            $scope.sortDirection = '';
            $scope.asc = true;
            $scope.paramType = $stateParams.type;
            $scope.categoryName = '';

            function getCategoryBySearchName(categorySearchName) {
                return CategoryService.getCategories().then(function(data) {
                    angular.forEach(data.categories, function(item, index) {
                        if (item.search_name === categorySearchName) {
                            $scope.categoryName = item.name;
                        }
                    });
                    return ProductService.getProductsByCategory(categorySearchName, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedDietary, $scope.selectedSpecs, $scope.selectedNonstock, $scope.sortField, $scope.sortDirection);
                });
            }

            function getHouseBrandById(houseBrandId) {
                return BrandService.getHouseBrands().then(function(brands) {
                    angular.forEach(brands, function(item, index) {
                        if (item.brand_control_label === houseBrandId) {
                            $scope.categoryName = item.extended_description;
                        }

                    });
                    return ProductService.getProductsByHouseBrand(houseBrandId, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedDietary, $scope.selectedSpecs, $scope.selectedNonstock, $scope.sortField, $scope.sortDirection);
                });
            }

            function getData() {
                var type = $stateParams.type;

                if (type === 'category') {
                    var categorySearchName = $stateParams.id;
                    return getCategoryBySearchName(categorySearchName);

                } else if (type === 'search') {

                    var searchTerm = $stateParams.id;
                    $scope.searchTerm = '\"' + searchTerm + '\"';
                    return ProductService.getProducts(searchTerm, $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedDietary, $scope.selectedSpecs, $scope.selectedNonstock, $scope.sortField, $scope.sortDirection);
                } else if (type === 'brand') {
                    var houseBrandId = $stateParams.id;
                    return getHouseBrandById(houseBrandId);

                    //$scope.selectedBrands.push(brandName);
                    //return ProductService.getProducts('', $scope.itemsPerPage, $scope.itemIndex, $scope.selectedBrands, $scope.selectedCategory, $scope.selectedDietary, $scope.selectedSpecs, $scope.selectedNonstock, $scope.sortField, $scope.sortDirection);
                }
            }

            function loadProducts(appendResults) {
                $scope.loadingResults = true;

                return getData().then(function(data) {
                    $scope.filterCount = 0;

                    // append results to existing data
                    if (appendResults) {
                        $scope.products.push.apply($scope.products, data.products);
                        // replace existing data
                    } else {
                        $scope.products = data.products;
                    }
                    $scope.totalItems = data.totalcount;

                    $scope.breadcrumbs = [];
                    //check initial page view
                    if ($stateParams.type === 'category') {
                        $scope.breadcrumbs.push({
                            type: 'topcategory',
                            id: $stateParams.id,
                            name: $scope.categoryName
                        });
                        $scope.filterCount++;
                        $scope.houseBrand = '';
                    }
                    if ($stateParams.type === 'search') {
                        $scope.breadcrumbs.push({
                            type: 'allcategories',
                            id: $stateParams.id,
                            name: 'All Categories'
                        });
                        $scope.houseBrand = '';
                    }
                    if ($stateParams.type === 'brand') {
                        $scope.breadcrumbs.push({
                            type: 'allcategories',
                            id: $stateParams.id,
                            name: $scope.categoryName
                        });
                        //check and disable selected brand
                        $scope.houseBrand = $scope.categoryName;
                    }

                    //check for selected facets
                    if ($scope.selectedCategory) {
                        $scope.breadcrumbs.push({
                            type: 'category',
                            id: $scope.selectedCategory.name,
                            name: $scope.selectedCategory.categoryname
                        });
                        $scope.filterCount++;
                    }
                    var brandsBreadcrumb = 'Brand: ';
                    angular.forEach($scope.selectedBrands, function(item, index) {
                        brandsBreadcrumb += item + ' or ';
                        $scope.filterCount++;
                    });
                    if (brandsBreadcrumb !== 'Brand: ') {
                        $scope.breadcrumbs.push({
                            type: 'brand',
                            id: $scope.selectedBrands,
                            name: brandsBreadcrumb.substr(0, brandsBreadcrumb.length - 4)
                        });
                    }
                    var dietaryBreadcrumb = 'Dietary: ';
                    angular.forEach($scope.selectedDietary, function(item, index) {
                        dietaryBreadcrumb += item + ' or ';
                        $scope.filterCount++;
                    });
                    if (dietaryBreadcrumb !== 'Dietary: ') {
                        $scope.breadcrumbs.push({
                            type: 'dietary',
                            id: $scope.selectedDietary,
                            name: dietaryBreadcrumb.substr(0, dietaryBreadcrumb.length - 4)
                        });
                    }
                    var specBreadcrumb = 'Item Specifications: ';
                    angular.forEach($scope.selectedSpecs, function(item, index) {
                        specBreadcrumb += changeSpecDisplayName(item) + ' or ';
                        $scope.filterCount++;
                    });
                    angular.forEach($scope.selectedNonstock, function(item, index) {
                        specBreadcrumb += changeSpecDisplayName(item) + ' or ';
                        $scope.filterCount++;
                    });
                    if (specBreadcrumb !== 'Item Specifications: ') {
                        $scope.breadcrumbs.push({
                            type: 'spec',
                            id: $scope.selectedSpecs,
                            name: specBreadcrumb.substr(0, specBreadcrumb.length - 4)
                        });
                    }
                    if ($stateParams.type === 'search') {
                        $scope.breadcrumbs.push({
                            type: 'search',
                            id: 'search',
                            name: '\"' + $stateParams.id + '\"'
                        });
                    }
                    $scope.loadingResults = false;
                    $scope.brandCount = data.facets.brands.length;
                    $scope.dietaryCount = data.facets.dietary.length;
                    $scope.specCount = data.facets.itemspecs.length + data.facets.nonstock.length;

                    return data.facets;
                });
            }

            loadProducts().then(function(facets) {
                refreshScopeFacets(facets);
            });

            $scope.goToItemDetails = function(item) {
                ProductService.selectedProduct = item;
                $state.go('menu.catalog.products.details', {
                    itemNumber: item.itemnumber
                });
            };

            $scope.infiniteScrollLoadMore = function() {
                console.log('infinite scroll');

                if (($scope.products && $scope.products.length >= $scope.totalItems) || $scope.loadingResults) {
                    return;
                }

                $scope.itemIndex += $scope.itemsPerPage;

                console.log('more: ' + $scope.itemIndex);
                loadProducts(true);
            };

            $scope.breadcrumbClickEvent = function(type, id) {
                $scope.loadingResults = true;
                if (type === 'topcategory' || type === 'allcategories') {
                    $scope.selectedBrands = [];
                    $scope.selectedSpecs = [];
                    $scope.selectedNonstock = [];
                    $scope.selectedDietary = [];
                    $scope.selectedCategory = '';
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                }
                if (type === 'category') {
                    $scope.selectedBrands = [];
                    $scope.selectedSpecs = [];
                    $scope.selectedNonstock = [];
                    $scope.selectedDietary = [];
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                }
                if (type === 'brand') {
                    $scope.selectedSpecs = [];
                    $scope.selectedNonstock = [];
                    $scope.selectedDietary = [];
                    $scope.selectedBrands = id;
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                }
                if (type === 'dietary') {
                    $scope.selectedBrands = [];
                    $scope.selectedSpecs = [];
                    $scope.selectedNonstock = [];
                    $scope.selectedDietary = id;
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                }
                if (type === 'spec') {
                    $scope.selectedBrands = [];
                    $scope.selectedDietary = [];
                    $scope.selectedSpecs = id;
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                }
            };

            $scope.showContextMenu = function(e, idx) {
                $scope.contextMenuLocation = {
                    'top': e.y,
                    'left': e.x
                };
                $scope.isContextMenuDisplayed = true;
            };

            $scope.showBrand = function() {
                $scope.isBrandShowing = true;
                $scope.brandHiddenNumber = 500;
            };
            $scope.hideBrand = function() {
                $scope.isBrandShowing = false;
                $scope.brandHiddenNumber = 3;
            };

            $scope.showDietary = function() {
                $scope.isDietaryShowing = true;
                $scope.dietaryHiddenNumber = 500;
            };
            $scope.hideDietary = function() {
                $scope.isDietaryShowing = false;
                $scope.dietaryHiddenNumber = 3;
            };

            $scope.showSpec = function() {
                $scope.isSpecShowing = true;
                $scope.specHiddenNumber = 500;
            };
            $scope.hideSpec = function() {
                $scope.isSpecShowing = false;
                $scope.specHiddenNumber = 3;
            };

            $scope.sortTable = function sortTable(field) {
                $scope.itemsPerPage = 50;
                $scope.itemIndex = 0;
                if (field !== 'caseprice' || $scope.totalItems < 201) {
                    if ($scope.sortField !== field) {
                        $scope.sortField = field;
                        $scope.sortDirection = 'asc';
                        $scope.asc = true;
                    } else {
                        if ($scope.sortDirection === 'asc') {
                            $scope.sortDirection = 'desc';
                            $scope.asc = false;
                        } else {
                            $scope.sortDirection = 'asc';
                            $scope.asc = true;
                        }
                    }
                    loadProducts();
                }
            };

            $scope.toggleSelection = function toggleSelection(selectedFacet, filter) {
                $scope.loadingResults = true;
                $scope.itemsPerPage = 50;
                $scope.itemIndex = 0;
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

                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                } else if (filter === 'dietary') {
                    idx = $scope.selectedDietary.indexOf(selectedFacet);

                    // is currently selected
                    if (idx > -1) {
                        $scope.selectedDietary.splice(idx, 1);
                    }
                    // is newly selected
                    else {
                        $scope.selectedDietary.push(selectedFacet);
                    }
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                } else if (filter === 'spec') {
                    idx = $scope.selectedSpecs.indexOf(selectedFacet);

                    // is currently selected
                    if (idx > -1) {
                        $scope.selectedSpecs.splice(idx, 1);
                    }
                    // is newly selected
                    else {
                        $scope.selectedSpecs.push(selectedFacet);
                    }
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                } else if (filter === 'nonstock') {
                    idx = $scope.selectedNonstock.indexOf(selectedFacet);

                    // is currently selected
                    if (idx > -1) {
                        $scope.selectedNonstock.splice(idx, 1);
                    }
                    // is newly selected
                    else {
                        $scope.selectedNonstock.push(selectedFacet);
                    }
                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                } else if (filter === 'subcategory') {
                    $scope.selectedSubcategory = selectedFacet.id;
                } else {
                    $scope.selectedCategory = selectedFacet;
                    $scope.selectedSubcategory = '';

                    loadProducts().then(function(facets) {
                        refreshScopeFacets(facets);
                    });
                }
            };

            $scope.canOrderProduct = function(item) {
                return ProductService.canOrderProduct(item);
            };

            function refreshScopeFacets(facets) {
                $scope.categories = facets.categories;
                $scope.brands = facets.brands;
                $scope.dietary = facets.dietary;
                if (facets.itemspecs && facets.itemspecs.length > 0) {
                    $scope.itemspecs = addIcons(facets.itemspecs);
                } else {
                    $scope.itemspecs = [];
                }
                var hasNonstock = false;
                if (facets.nonstock && facets.nonstock.length > 0) {
                    angular.forEach(facets.nonstock, function(item, index) {
                        if (item.name === 'y') {
                            $scope.nonstock = {
                                name: 'nonstock',
                                displayname: 'Non-Stock Item',
                                iconclass: 'text-regular icon-user',
                                count: item.count
                            };
                            hasNonstock = true;
                            $scope.hasNonstock = true;
                            $scope.specHiddenNumber = 2;
                        }
                    });
                    if (hasNonstock === false) {
                        $scope.nonstock = '';
                        $scope.specHiddenNumber = 3;
                        $scope.hasNonstock = false;
                    }

                } else {
                    $scope.nonstock = '';
                    $scope.specHiddenNumber = 3;
                    $scope.hasNonstock = true;
                }
            }

            function addIcons(itemspecs) {
                var itemspecsArray = [];
                angular.forEach(itemspecs, function(item, index) {
                    var itemname = '';
                    var itemcount = 0;
                    //if coming from bookmark, set item name
                    if (!item.name) {
                        if (item) {
                            itemname = item;
                        }
                    } else {
                        itemname = item.name;
                        itemcount = item.count;
                    }

                    if (itemname === 'itembeingreplaced') {
                        itemspecsArray.push({
                            name: itemname,
                            displayname: 'Item Being Replaced',
                            iconclass: 'text-red icon-cycle',
                            count: itemcount
                        });
                    }
                    if (itemname === 'replacementitem') {
                        itemspecsArray.push({
                            name: itemname,
                            displayname: 'Replacement Item',
                            iconclass: 'text-green icon-cycle',
                            count: itemcount
                        });
                    }
                    if (itemname === 'childnutrition') {
                        itemspecsArray.push({
                            name: itemname,
                            displayname: 'Child Nutrition Sheet',
                            iconclass: 'text-regular icon-apple',
                            count: itemcount
                        });
                    }
                    if (itemname === 'sellsheet') {
                        itemspecsArray.push({
                            name: itemname,
                            displayname: 'Product Information Sheet',
                            iconclass: 'text-regular icon-sellsheet',
                            count: itemcount
                        });
                    }
                    //THESE ITEM.NAMES ARE CURRENTLY JUST GUESSES --- I HAVE NOT SEEN WHAT THESE 4 ARE CALLED YET
                    if (itemname === 'DeviatedCost') {
                        itemspecsArray.push({
                            name: itemname,
                            displayname: 'DeviatedCost',
                            iconclass: 'text-regular icon-dollar',
                            count: itemcount
                        });
                    }
                    if (item.name === 'MaterialSafety') {
                        itemspecsArray.push({
                            name: itemname,
                            displayname: 'Material Safety Data Sheet',
                            iconclass: 'text-regular icon-safety',
                            count: itemcount
                        });
                    }
                });
                return itemspecsArray;
            }

            function changeSpecDisplayName(name) {
                if (name === 'itembeingreplaced') {
                    return 'Item Being Replaced';
                }
                if (name === 'replacementitem') {
                    return 'Replacement Item';
                }
                if (name === 'childnutrition') {
                    return 'Child Nutrition Sheet';
                }
                if (name === 'sellsheet') {
                    return 'Product Information Sheet';
                }
                if (name === 'nonstock') {
                    return 'Non-Stock Item';
                }
            }

            // TODO: move into context menu controller
            // $scope.lists = ListService.lists;
            // ListService.getListHeaders();

            $scope.carts = CartService.carts;
            CartService.getCartHeaders();
        }
    ]);