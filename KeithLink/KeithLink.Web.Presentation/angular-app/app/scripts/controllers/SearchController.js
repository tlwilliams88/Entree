'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:SearchController
 * @description
 * # SearchController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('SearchController', ['$scope', '$state', '$stateParams', '$modal', '$analytics', '$filter', '$timeout', 'ProductService', 'CategoryService', 'Constants', 'PricingService', 'CartService', 'ApplicationSettingsService', 'UtilityService', 'blockUI', 'LocalStorage', 'SessionService', 'campaignInfo', 'ENV', 'AnalyticsService',
    function(
      $scope, $state, $stateParams, // angular dependencies
      $modal, // ui bootstrap library
      $analytics, //google analytics
      $filter,
      $timeout,
      ProductService, CategoryService, Constants, PricingService, CartService, ApplicationSettingsService, UtilityService, // bek custom services
      blockUI,
      LocalStorage,
      SessionService,
      campaignInfo, 
      ENV,
      AnalyticsService
    ) {

    // $scope.$on('$stateChangeStart',
    //   function(){
    //     guiders.hideAll();
    // });

    //$scope.runTutorial is set in the loadProducts function -- Tutorial Ignored 09/25

    // guiders.createGuider({
    //   id: "searchpage_tutorial",
    //   title: "Updated Categories",
    //   description: "We've simplified our product categories to make it easier to find what you need. <br/><br/> As an example 'All Produce' is now 'Produce' and 'Frozen & Fresh Poultry' are under 'Center Of Plate'. <br/><br/> These categories are also available to choose from when searching or navigating from the product catalog. <br/><br/> To see sub-categories click the (+) icon.",
    //   buttons: [{name: "Close", onclick: setHideTutorial}],
    //   overlay: true,
    //   attachTo: "#categoriesSection",
    //   position: "right",
    //   offset: {left: 245, top: 321.3889},
    //   highlight: true
    // })

    var isMobile = UtilityService.isMobileDevice();
    var isMobileApp = ENV.mobileApp;

    function setHideTutorial(){
      LocalStorage.setHideTutorialSearch(true);
      $scope.tutorialRunning = false;
      guiders.hideAll();
    };

    // clear keyword search term at top of the page
    if ($scope.userBar) {
      $scope.userBar.universalSearchTerm = '';
    }

    // TODO: do not call these functions directly from view
    $scope.canOrderItem = PricingService.canOrderItem;
    $scope.hasCasePrice = PricingService.hasCasePrice;
    $scope.hasPrice = PricingService.hasPrice;
    $scope.hasPackagePrice = PricingService.hasPackagePrice;

    $scope.paramType = $stateParams.type; // Category, Search, Brand
    $scope.paramId = $stateParams.id; // search term, brand id, category id
    $scope.displayText = $stateParams.brand ? $stateParams.brand : $stateParams.category;
    if($scope.paramType != null &&
       $scope.paramId != null){
      LocalStorage.setSearchTerms($scope.paramType + ": " + $scope.paramId);
    }
    else{
      if($scope.$resolve != null &&
         $scope.$resolve.campaignInfo != null){
        LocalStorage.setSearchTerms("Campaign: " + $scope.$resolve.campaignInfo.description);
        AnalyticsService.recordPromotion($scope.$resolve.campaignInfo.uri,
                                         $scope.$resolve.campaignInfo.name,
                                         $scope.$resolve.campaignInfo.enddate.toString(),
                                         '',
                                         LocalStorage.getCustomerNumber(),
                                         LocalStorage.getBranchId());
      }
    }

    $scope.selectedSortParameter = 'Relevance';
    $scope.sortParametervalue = '';
    $scope.sortReverse = false;

    if(LocalStorage.getDefaultView()){
      $scope.defaultSearchView = LocalStorage.getDefaultView();
    } else {
      $scope.defaultSearchView = 'ViewWithImages';
    }

    $scope.itemsPerPage = LocalStorage.getPageSize();
    $scope.itemIndex = 0;

    $scope.numberFacetsToShow = 4;  // determines when to show the 'Show More' link for each facet
    $scope.maxSortCount = 200;      // max number of items that can be sorted by price

    $scope.sortDirection = 'asc';
    $scope.categoryIsOpen = false;

    $scope.hideMobileFilters = true;
    if ($state.params.catalogType == 'BEK') {
        $scope.pageTitle = 'Product Catalog';
    } else {
        if ($state.params.catalogType == "UNFI"){
            $scope.pageTitle = "Natural and Organic";
        } else if($state.params.campaign_id) {
          $scope.pageTitle = campaignInfo.description;
      } else {
            $scope.pageTitle = "Specialty Catalog";
        }
    }

	$scope.noFiltersSelected = true;

    $scope.products = [];
    $scope.aggregates = [{

      name: 'parentcategories',
      metrics: {
        available: [],
        selected: [],
        showMore: true
      }
    }, {
      name: 'subcategories',
      metrics: {
        selected: [],
        showMore: true
      }
    }, {
      name: 'brands',
      metrics: {
        available: [],
        selected: [],
        showMore: true // display Show More button for this facet
      }
    }, {
      name: 'manufacturers',
      metrics: {
        available: [],
        selected: [],
        showMore: true
      }
    }, {
      name: 'itemspecs',
      metrics: {
        available: [],
        selected: [],
        showMore: true
      }
    }, {
      name: 'temp_zones',
      metrics: {
        available: [],
        selected: [],
        showMore: true
      }
    }, {
      name: 'dietary',
      metrics: {
        available: [],
        selected: [],
        showMore: true
      }
    }, {
      name: 'specialfilters',
      metrics: {
        available: [],
        selected: [],
        showMore: true
      }
    }];

    $scope.parentcategories = $scope.aggregates[0].metrics;
    $scope.subcategories = $scope.aggregates[1].metrics;
    $scope.brands = $scope.aggregates[2].metrics;
    $scope.manufacturers = $scope.aggregates[3].metrics;
    $scope.temp_zones = $scope.aggregates[5].metrics;
    $scope.itemspecs = $scope.aggregates[4].metrics;
    $scope.dietary = $scope.aggregates[6].metrics;
    $scope.specialfilters = $scope.aggregates[7].metrics;

    $scope.initPagingValues = function(){
      $scope.visitedPages = [];
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.itemCountOffset = 0;
    };

    $scope.initPagingValues();

    /*************
    PAGINATION
    *************/

    //Toggles variable for determining Search Result View, and amount of results requested
    $scope.toggleListView = function(selectedview){
      var selectedSearchView = {key: 'defaultSearchView', value: selectedview};
      $scope.defaultSearchView = selectedview;
      LocalStorage.setDefaultView(selectedview);

      ApplicationSettingsService.saveApplicationSettings(selectedSearchView);
    };

    $scope.blockUIAndChangePage = function(page, toggleView){
        $scope.startingPoint = 0;
        $scope.endPoint = 0;
        $scope.toggleView = toggleView;

        var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});

        if ($scope.toggleView){
          visited = [];
          $scope.currentPage = 1;
        }

        $(document).ready(function(){
          $(document.activeElement).blur();
          $('html, body').animate({ scrollTop: 0 }, 500);
        });

        return blockUI.start('Loading Products...').then(function(){

          if(visited.length > 0){
            $timeout(function() {
              $scope.isChangingPage = true;
              $scope.pageChanged(page, visited);
            }, 100);
          }
          else{
              $scope.pageChanged(page, visited);
          }

        });

    };

    $scope.setStartAndEndPoints = function(page){
      var foundStartPoint = false;
        page.forEach(function(item, index){
          if(page && item.itemnumber === page[0].itemnumber){
            $scope.startingPoint = index;
            $scope.endPoint = angular.copy($scope.startingPoint + $scope.itemsPerPage);
            foundStartPoint = true;
          }
        });

        if(!foundStartPoint){
          appendProducts(page);
        }
        //We need two calls for stop here because we have two paging directives on the view. If the page change is triggered
        //automatically (deleting all items on page/saving) the event will fire twice and two loading overlays will be generated.
        blockUI.stop();
        blockUI.stop();
     };

     $scope.pageChanged = function(pages, visited) {
      var facets;
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;
      $scope.loadingPage = true;
      $scope.currentPage = pages.currentPage;
      $scope.startingPoint = ((pages.currentPage - 1)*$scope.itemsPerPage) + 1;
      $scope.startingResult = $scope.startingPoint;
      $scope.endPoint = angular.copy($scope.startingPoint + $scope.itemsPerPage);
      $scope.firstPageItem = ($scope.currentPage * $scope.itemsPerPage) - ($scope.itemsPerPage - 1);
      $scope.setRange();

      $scope.aggregateCount = ($scope.brands.selected.length + $scope.itemspecs.selected.length + $scope.dietary.selected.length + $scope.manufacturers.selected.length + $scope.temp_zones.selected.length + $scope.parentcategories.selected.length + $scope.subcategories.selected.length);

      if($scope.aggregateCount !== 0){
        facets = ProductService.getFacets(
          $scope.brands.selected,
          $scope.manufacturers.selected,
          $scope.dietary.selected,
          $scope.itemspecs.selected,
          $scope.temp_zones.selected,
          $scope.parentcategories.selected,
          $scope.subcategories.selected,
          $scope.specialfilters.selected
        );
      }
      
      var updatedPage = {
        type: $scope.paramType, 
        id: $scope.paramId, 
        deptName: $stateParams.deptName, 
        parentcategories: $scope.parentcategories.selected,
        subcategories: $scope.subcategories.selected,
        brands: $scope.brands.selected,
        manufacturers: $scope.manufacturers.selected,
        dietary: $scope.dietary.selected,
        itemspecs: $scope.itemspecs.selected,
        temp_zones: $scope.temp_zones.selected,
        specialfilters: $scope.specialfilters.selected,
        startingPoint: parseInt($scope.startingPoint),
        currentPage: parseInt($scope.currentPage)
      };
      if($scope.aggregateCount !== 0 || $scope.startingPoint > 0 || $scope.currentPage > 1){
          updatePage(updatedPage);
      }

      var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.startingPoint, $scope.sortField, $scope.sortDirection, facets, $stateParams.dept);
      ProductService.searchCatalog($scope.paramType, $scope.paramId, $scope.$state.params.catalogType, params, $stateParams.deptName, $stateParams.campaign_id).then(function(data){
        $scope.products = data.products;
        if($scope.toggleView){
          resetPage($scope.products, true);
        }

      AnalyticsService.recordSearchImpressions($scope.products, 
                                               LocalStorage.getCustomerNumber(),
                                               LocalStorage.getBranchId(),
                                               LocalStorage.getSearchTerms() + '-' + $scope.currentPage);

      blockUI.stop();
      blockUI.stop();
      });
     };

    $scope.setRange = function(){
      $scope.endPoint = $scope.endPoint;
      $scope.rangeStart = $scope.startingPoint;
      $scope.rangeEnd = ($scope.endPoint > $scope.totalProducts) ? $scope.totalProducts : $scope.endPoint - 1;
      if($scope.rangeStart === 0){
        $scope.rangeStart++;
        if($scope.rangeEnd === $scope.itemsPerPage - 1){
          $scope.rangeEnd ++;
        }
      }
    };

    function resetPage(results, initialPageLoad) {
      $scope.initPagingValues();
      $scope.activeElement = true;
      $scope.totalItems = $scope.totalProducts;
      $scope.rangeStartOffset = 0;
      $scope.rangeEndOffset = 0;

      if(initialPageLoad && !$stateParams.currentPage){
        $scope.currentPage = 1;
        $scope.firstPageItem = ($scope.currentPage * $scope.itemsPerPage) - ($scope.itemsPerPage);
        $scope.products = $scope.products.slice($scope.firstPageItem, ($scope.currentPage * $scope.itemsPerPage));
        $scope.setStartAndEndPoints($scope.products);
        $scope.visitedPages.push({page: 1, items: $scope.products, deletedCount: 0});
      }
      $scope.setRange();
    }
    
    function updatePage(updatedPage) {
        $state.go('menu.catalog.products.list',
            {   type: updatedPage.type, 
                id: updatedPage.id, 
                deptName: updatedPage.deptName, 
                parentcategories: updatedPage.parentcategories,
                subcategories: updatedPage.subcategories,
                brands: updatedPage.brands,
                manufacturers: updatedPage.manufacturers,
                dietary: updatedPage.dietary,
                itemspecs: updatedPage.itemspecs,
                temp_zones: updatedPage.temp_zones,
                specialfilters: updatedPage.specialfilters,
                startingPoint: updatedPage.startingPoint,
                currentPage: updatedPage.currentPage
            },
            {   location: true,
                reload: false,
                notify: false
            }
         )
    }

    function appendProducts(results) {
      $scope.visitedPages.push({page: $scope.currentPage, items: results, deletedCount: 0});
      //Since pages can be visited out of order, sort visited pages into numeric order.
      $scope.visitedPages = $scope.visitedPages.sort(function(obj1, obj2){
        var sorterval1 = obj1.page;
        var sorterval2 = obj2.page;
        return sorterval1 - sorterval2;
      });

      if($scope.totalProducts === 0){
        $scope.startingPoint = 0;
        $scope.endPoint = 0;
      }
      else{
       $scope.setStartAndEndPoints(results);
      }
    }

    function startLoading() {
      $scope.loadingResults = true;
    }

    function stopLoading() {
      $scope.loadingResults = false;
      blockUI.stop();
    }

    if($stateParams.sortingParams && $stateParams.sortingParams.sort.length){
      $scope.sort = $stateParams.sortingParams.sort;
    }

    $scope.breadcrumbs = [];

    /*************
    BREADCRUMBS
    *************/
    function setBreadcrumbs(data) {

      $scope.breadcrumbs = [];
      $scope.filterCount = 0;
      var displayText = $scope.displayText ? $scope.displayText : $scope.paramId;

      // search term
      if ($scope.paramType === 'search') {
       $scope.featuredBreadcrumb = {
          displayText: $stateParams.deptName
        };

        $scope.breadcrumbs.unshift($scope.featuredBreadcrumb);

        $scope.featuredBreadcrumb = {
          displayText: displayText
        };

        $scope.breadcrumbs.push($scope.featuredBreadcrumb);
      } else if($scope.paramId && !$stateParams.deptName) {
        $scope.featuredBreadcrumb = {
          displayText: displayText
        };

        $scope.breadcrumbs.push($scope.featuredBreadcrumb);
      }

      setBreadcrumbsForAggregates($scope.parentcategories, 'Categories', 'parentcategories');
      setBreadcrumbsForAggregates($scope.subcategories, 'Sub-Categories', 'subcategories');
      setBreadcrumbsForAggregates($scope.brands, 'Brands', 'brands');
      setBreadcrumbsForAggregates($scope.manufacturers, 'Manufacturers', 'manufacturers');
      setBreadcrumbsForAggregates($scope.dietary, 'Dietary', 'dietary');
      setBreadcrumbsForAggregates($scope.itemspecs, 'Item Specifications', 'itemspecs');
      setBreadcrumbsForAggregates($scope.temp_zones, 'Temp Zone', 'temp_zone');
      setBreadcrumbsForAggregates($scope.specialfilters, 'Special Filters', 'specialfilters');

    }

    function setBreadcrumbsForAggregates(aggregate, aggregatetext, apitext) {
      var aggregateData = aggregate,
          breadcrumbSeparator = ', ';

      if(aggregateData && aggregateData.selected.length > 0){
        var aggregateDataSelected = aggregateData.selected,
            multipleAggregatesSelected = aggregateDataSelected.length >= 3 ? true : false,
            displayText = multipleAggregatesSelected ? aggregatetext + ': ' + aggregateDataSelected.length + ' selected' : aggregatetext + ': ' + aggregateDataSelected.join(breadcrumbSeparator);

        if(aggregateDataSelected.length > 0 && aggregateDataSelected.length < 3) {
          $scope.breadcrumbs.push({
            apiText: apitext,
            clickData: aggregateDataSelected,
            displayText: displayText
          });
        } else if(aggregateDataSelected.length >= 3) {
          $scope.breadcrumbs.push({
            apiText: apitext,
            clickData: aggregateDataSelected,
            displayText: displayText
          });
        }

        $scope.filterCount += aggregateDataSelected.length;
      }
    }
    
    function updateAggregates(aggregate) {
        var aggregateState = $stateParams[aggregate],
            aggregateScope = $scope[aggregate].selected;
                                        
        if(Array.isArray(aggregateState) && aggregateState.length > 1) {
            aggregateState.forEach(function(category) {
                aggregateScope.push(category);
            })
        } else {
            aggregateScope.push(aggregateState);
        }
    }

    $scope.selectBreadcrumb = function(selectedaggregate) {

      var selectedAggregate = selectedaggregate;

      if(selectedAggregate){
        $scope.aggregates.forEach(function(aggregate, index){
          if(aggregate.name == selectedaggregate && selectedaggregate == 'parentcategories'){
            for(var i = index++; i < $scope.aggregates.length; i++){
              if(i > index){
                $scope.aggregates[i].metrics.selected = [];
              }
            }
          } else if(aggregate.name == selectedaggregate){
            for(var i = index; i < $scope.aggregates.length; i++){
              if(i > index){
                $scope.aggregates[i].metrics.selected = [];
              }
            }
          } else {
            return false;
          }

        });

        loadProducts();
      } else {
        $scope.clearFacets();
      }

    };

    /*************
    LOAD PRODUCT DATA
    *************/
    function getData() {
      var facets;
      $scope.userProfile = SessionService.userProfile;
      $scope.currentCustomer = LocalStorage.getCurrentCustomer();
            
      if($stateParams.parentcategories != undefined) {
          updateAggregates('parentcategories');
      }
      if($stateParams.subcategories != undefined) {
          updateAggregates('subcategories');
      }
      if($stateParams.brands != undefined) {
          updateAggregates('brands');
      }
      if($stateParams.manufacturers != undefined) {
          updateAggregates('manufacturers');
      }
      if($stateParams.dietary != undefined) {
          updateAggregates('dietary');
      }
      if($stateParams.itemspecs != undefined) {
          updateAggregates('itemspecs');
      }
      if($stateParams.specialfilters != undefined) {
          updateAggregates('specialfilters');
      }
      if($stateParams.temp_zones != undefined) {
          updateAggregates('temp_zones');
      }

      $scope.aggregateCount = ($scope.brands.selected.length + $scope.itemspecs.selected.length + $scope.temp_zones.selected.length + $scope.manufacturers.selected.length + $scope.dietary.selected.length + $scope.parentcategories.selected.length + $scope.subcategories.selected.length + $scope.specialfilters.selected.length);

      if($scope.aggregateCount !== 0){
        facets = ProductService.getFacets(
          $scope.brands.selected,
          $scope.manufacturers.selected,
          $scope.dietary.selected,
          $scope.itemspecs.selected,
          $scope.temp_zones.selected,
          $scope.parentcategories.selected,
          $scope.subcategories.selected,
          $scope.specialfilters.selected
        );
      }
      
      var updatedPage = {
        type: $scope.paramType, 
        id: $scope.paramId, 
        deptName: $stateParams.deptName, 
        parentcategories: $scope.parentcategories.selected,
        subcategories: $scope.subcategories.selected,
        brands: $scope.brands.selected,
        manufacturers: $scope.manufacturers.selected,
        dietary: $scope.dietary.selected,
        itemspecs: $scope.itemspecs.selected,
        temp_zones: $scope.temp_zones.selected,
        specialfilters: $scope.specialfilters.selected,
        startingPoint: $stateParams.startingPoint,
        currentPage: $stateParams.currentPage
      };
      if($scope.aggregateCount !== 0 || $scope.startingPoint > 0 || $scope.currentPage > 1){
          updatePage(updatedPage);
      }
       
      $scope.currentPage = $stateParams.currentPage ? parseInt($stateParams.currentPage) : 1;
      
      $scope.startingPoint = parseInt($stateParams.startingPoint);
      $scope.endPoint = angular.copy($scope.startingPoint + $scope.itemsPerPage);
      $scope.firstPageItem = ($scope.currentPage * $scope.itemsPerPage) - ($scope.itemsPerPage - 1);

      if($scope.sortField === 'itemnumber' && $stateParams.catalogType && $stateParams.catalogType != 'BEK'){
        $scope.sortField  = '';
      }

      var params = ProductService.getSearchParams($scope.itemsPerPage, $stateParams.startingPoint, $scope.sortField, $scope.sortDirection, facets, $stateParams.dept);
      return ProductService.searchCatalog($scope.paramType, $scope.paramId, $stateParams.catalogType, params, $stateParams.deptName, $stateParams.campaign_id);
    }

    //Load list of products and block UI with message
    function loadProducts(appendResults, fromFunction) {
      startLoading();
      return blockUI.start('Loading Products...').then(function(){
        return getData().then(function(data) {
        var page = 1;
        $scope.products = data.products;
        $scope.totalProducts = data.totalcount;
        // var hideTutorial = LocalStorage.getHideTutorialSearch(),
        //     runTutorial = data.facets.categories.length && !(hideTutorial || isMobileApp || isMobile) ? true : false;

        // if(runTutorial) {
        //   $scope.tutorialRunning = true;
        //   guiders.show('searchpage_tutorial');
        // }

        AnalyticsService.recordSearchImpressions($scope.products, 
                                                 LocalStorage.getCustomerNumber(),
                                                 LocalStorage.getBranchId(),
                                                 LocalStorage.getSearchTerms() + '-1');

        if(fromFunction !== 'sorting'){
          resetPage(data.products, true);
        }
        $scope.totalItems = data.totalcount;
        if (data.catalogCounts != null) {
            $scope.bekItemCount = data.catalogCounts.bek;
            $scope.unfiItemCount = data.catalogCounts.unfi;
        } else {
            $scope.bekItemCount = 0;
            $scope.unfiItemCount = 0;
        }
        // append results to existing data (for infinite scroll)
        if (appendResults) {
          $scope.products.push.apply($scope.products, data.products);
        // replace existing data (for sort, filter)
        } else {
          $scope.setStartAndEndPoints($scope.products);
        }
        if($scope.aggregates && $scope.aggregateCount !==0 || $scope.noFiltersSelected){
          updateFacetCount($scope.brands, data.facets.brands);
          updateFacetCount($scope.itemspecs, data.facets.itemspecs);
          updateFacetCount($scope.dietary, data.facets.dietary);
          updateFacetCount($scope.manufacturers, data.facets.mfrname);
          updateFacetCount($scope.temp_zones, data.facets.temp_zone);
          updateFacetCount($scope.specialfilters, data.facets.specialfilters);
          updateParentCategoryFacetCount($scope.parentcategories, data.facets.parentcategories);
        }

        setBreadcrumbs(data);
        stopLoading();

        blockUI.stop();

        delete $scope.searchMessage;

        return data.facets;
        });
      }, function(error) {
        $scope.searchMessage = 'Error loading products.';
      }).finally(function() {
      });
    }



    /*************
    FACETS
    *************/

    function updateFacetCount(facets, data){

      if(facets && facets.available.length > 0){

        facets.available.forEach(function(facet){

          var facetName = $filter('filter') (data, {name: facet.name, brand_control_label: facet.brand_control_label});
          facet.count = 0;

          if(facetName.length > 0 && facet.name){

            facet.count = facetName[0].count;

          }

        });

      }

      $scope.noFiltersSelected = false;
    }

    function updateParentCategoryFacetCount(categories, data){

      if (categories && categories.available){
        categories.available.forEach(function(category){
          var facetName = $filter('filter')(data, {name: category.name});
          category.count = 0;

          if (facetName && facetName.length > 0 && category.name){
            category.count = facetName[0].count;

            if (facetName[0].categories.length){
              category.categories.forEach(function(subcategory){
                  var foundSubcategory = $filter('filter')(facetName[0].categories, {name: subcategory.name});

                  if (foundSubcategory.length){
                    subcategory.count = foundSubcategory[0].count;
                  } else {
                    subcategory.count = 0;
                  }
              });
            }
          }
        });
      }
      $scope.noFiltersSelected = false;
    }

    $scope.clearFacets = function() {
      $scope.parentcategories.selected = [];
      $scope.subcategories.selected = [];
      $scope.brands.selected = [];
      $scope.manufacturers.selected = [];
      $scope.dietary.selected = [];
      $scope.temp_zones.selected = [];
      $scope.itemspecs.selected = [];
      $scope.specialfilters.selected = [];
      
      var updatedPage = {
        type: $scope.paramType, 
        id: $scope.paramId, 
        deptName: $stateParams.deptName, 
        parentcategories: undefined,
        subcategories: undefined,
        brands: undefined,
        manufacturers: undefined,
        dietary: undefined,
        itemspecs: undefined,
        temp_zones: undefined,
        specialfilters: undefined,
        startingPoint: undefined,
        currentPage: undefined
      };
      updatePage(updatedPage);

      loadProducts().then(refreshFacets);
      $scope.noFiltersSelected = true;
    };

    function refreshFacets(facets) {
      var selectedCategory;
      // set the $scope.aggregates object using the response data
      if(facets){

        $scope.brands.available = facets.brands;
        $scope.manufacturers.available = facets.mfrname;
        $scope.dietary.available = facets.dietary;
        $scope.itemspecs.available = addIcons(facets.itemspecs);
        $scope.temp_zones.available = facets.temp_zone;
        $scope.specialfilters.available = facets.specialfilters;
        $scope.parentcategories.available = facets.parentcategories;

        if($scope.parentcategories.available.length && $scope.subcategories.selected.length){

          $scope.parentcategories.available.forEach(function(parentcategory){

            parentcategory.categories.forEach(function(category){

              selectedCategory = $scope.subcategories.selected.indexOf(category.name);

              if(selectedCategory > -1){

                category.isSelected = true;

              }

            });

          });

        }

      }

    }

    /*************
    ITEM SPEC ICONS
    *************/
    function getIconDisplayInfo(name) {
      var itemSpec = {};
      switch (name) {
        case 'itembeingreplaced':
          itemSpec.displayname = 'Replacement Item';
          itemSpec.iconclass = 'text-green icon-cycle';
          break;
        case 'replacementitem':
          itemSpec.displayname = 'Item Being Replaced';
          itemSpec.iconclass = 'text-red icon-cycle';
          break;
        case 'childnutrition':
          itemSpec.displayname = 'Child Nutrition Sheet';
          itemSpec.iconclass = 'text-regular icon-apple';
          break;
        case 'sellsheet':
          itemSpec.displayname = 'Product Information Sheet';
          itemSpec.iconclass = 'text-regular icon-sellsheet';
          break;
        case 'nonstock':
          itemSpec.displayname = 'Non-Stock Item';
          itemSpec.iconclass = 'text-regular icon-user';
          break;
        // cannot filter by deviated cost
        // case 'deviatedcost':
        //   itemSpec.displayname = 'DeviatedCost';
        //   itemSpec.iconclass = 'text-regular icon-dollar';
        //   break;

        // case 'materialsafety':
        //   itemSpec.displayname = 'Material Safety Data Sheet';
        //   itemSpec.iconclass = 'text-regular icon-safety';
        //   break;
      }
      return itemSpec;
    }

    function addIcons(itemspecs) {
      var itemspecsArray = [];
      angular.forEach(itemspecs, function(item, index) {
        var itemSpec = getIconDisplayInfo(item.name);
        itemSpec.name = item.name;
        itemSpec.count = item.count;
        itemspecsArray.push(itemSpec);
      });
      return itemspecsArray;
    }

    /*************
    SORTING
    *************/
    $scope.selectSortParameter = function(parametername, parametervalue){
      if($scope.selectedSortParameter == parametername){
        return;
      } else {
        $scope.selectedSortParameter = parametername;
        $scope.sortField = parametervalue;

        if(parametername == 'Price Low to High') {
          $scope.sortDirection = 'asc';
        } else if (parametername == 'Price High to Low') {
          $scope.sortDirection = 'desc';
        } else {
          $scope.sortDirection = 'asc';
        }

        loadProducts('', 'sorting');
      }
    };

    $scope.sortParameters = [{
      name: 'Relevance',
    }, {
      name: 'Item #',
      value: 'keyword'
    }, {
      name: 'Name',
      value: 'name_not_analyzed'
    }, {
      name: 'Brand',
      value: 'brand_not_analyzed'
    }, {
      name: 'Price Low to High',
      value: 'caseprice'
    }, {
      name: 'Price High to Low',
      value: 'caseprice'
    }];

    /*************
    CLICK EVENTS
    *************/

    $scope.toggleSubCategories = function(category, index){
      var subCategories = $('.subcategories_' + index);
      category.IsOpen = !category.IsOpen;

      if (subCategories[0].style.display === 'block' && category.IsOpen == false){
        subCategories.css('display','none');
      } else {
        subCategories.css('display','block');
      }

    };

    $scope.toggleSelection = function(category, facetList, selectedFacet, parentcategory, index) {
      $scope.startingPoint = 0;
      $stateParams.startingPoint = 0;
      $stateParams.currentPage = 1;
      $scope.noFiltersSelected = !$scope.noFiltersSelected;
      var parentCategorySelected,
          parentCategoryAvailableIdx,
          parentCategorySelectedIdx,
          subcategorySelected,
          totalSelectedSubCategories,
          isSelected,
          unSelectedCategory,
          idx;

      idx = facetList.indexOf(selectedFacet);

      if (idx > -1) {
        parentCategorySelected = $filter('filter')($scope.parentcategories.available, {name:selectedFacet});

        if (parentCategorySelected.length){ // Parent Category is un-selected here

          if(parentCategorySelected[0].IsOpen){ // Toggles sub-categories display to none if category is open
            $scope.toggleSubCategories(parentCategorySelected[0], index); // Closes Category when parent category is unselected
          }

          isSelected = document.activeElement.checked;

          parentCategorySelected[0].categories.forEach(function(category){ //unselects and removes sub-category when parent Category is unselected
            category.isSelected = isSelected;
            unSelectedCategory = $scope.subcategories.selected.indexOf(category.name);

            if ($scope.parentcategories.selected.length > 0 && unSelectedCategory > -1){
              $scope.subcategories.selected.splice(unSelectedCategory, 1); // Removes only specific sub-categories associated with category
            } else if ($scope.parentcategories.selected.length == 0) {
              $scope.subcategories.selected = []; // Removes all remaining sub-categories for last remaining category

            }

          });

          facetList.splice(idx, 1);
        } else { // All other aggregates are un-selected here

          if(parentcategory){
            // Identify parent category selected
            parentCategorySelected = $filter('filter')($scope.parentcategories.available, {name:parentcategory});

            // Find index of parent category in available parent categories
            parentCategoryAvailableIdx = $scope.parentcategories.available.indexOf(parentCategorySelected[0]);

            // Find index of parent category in selected parent categories
            parentCategorySelectedIdx = $scope.parentcategories.selected.indexOf(parentCategorySelected[0].name);

            // Identify sub-category selected within selected parent category
            subcategorySelected = $filter('filter')(parentCategorySelected[0].categories, {name:selectedFacet});

            // Find total count of selected sub-categories within selected parent category
            totalSelectedSubCategories = $filter('filter')(parentCategorySelected[0].categories, {isSelected:true});// Finds all selected sub-categories

            if(totalSelectedSubCategories.length == 1){
              // If only one selected sub-category remains un-select parent category and collapses category
              $scope.parentcategories.selected.splice(parentCategorySelectedIdx, 1);
              $scope.toggleSubCategories(parentCategorySelected[0], parentCategoryAvailableIdx);
            }
            subcategorySelected[0].isSelected = false;// Sets selected sub-category isSelected to false
          }

          facetList.splice(idx, 1);
        }
      } else {

        if (parentcategory){ // Runs if the aggregate selected is a sub-category

          if ($scope.parentcategories.selected.length == 0){ // Runs if parent category selected array is empty
            $scope.parentcategories.selected.push(parentcategory);
          } else { // Runs if parent category selected array is not empty

            if ($scope.parentcategories.selected.indexOf(parentcategory) == -1){ // Only push to $scope.parentcategories.selected array if parentcategory does not exist
              $scope.parentcategories.selected.push(parentcategory);
            }
          }

          facetList.push(selectedFacet); // In this case this pushes the sub-category to the facetList of selected facets
        } else { // All other aggregate selections other than sub-category run here
          facetList.push(selectedFacet);

          if(category == true){
            parentCategorySelected = $filter('filter')($scope.parentcategories.available, {name:selectedFacet});
          }

          if (parentCategorySelected && parentCategorySelected.length){
            isSelected = document.activeElement.checked;

            parentCategorySelected[0].categories.forEach(function(category){
              category.isSelected = isSelected;
              $scope.subcategories.selected.push(category.name);
            });
          }
        }
      }

      loadProducts();
    };

    $scope.goToItemDetails = function(item) {
      ProductService.selectedProduct = item;
      $state.go('menu.catalog.products.details', {
        itemNumber: item.itemnumber
      });
    };

    $scope.openExportModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/exportmodal.html',
        controller: 'ExportModalController',
        resolve: {
          location: function() {
            return {category:'Search', action:'Export Search Results'};
          },
          headerText: function () {
            return 'Product Catalog (limited to 500 items)';
          },
          exportMethod: function() {
            return ProductService.exportProducts;
          },
          exportConfig: function() {
            return ProductService.getExportConfig();
          },
          exportParams: function() {
            // return search url with params
            var sortDirection = $scope.sortReverse ? 'desc' : 'asc';
            var facets = ProductService.getFacets(
              $scope.brands.selected,
              $scope.manufacturers.selected,
              $scope.dietary.selected,
              $scope.itemspecs.selected,
              $scope.temp_zones.selected,
              $scope.parentcategories.selected,
              $scope.subcategories.selected
            );

            var params = ProductService.getSearchParams($scope.itemsPerPage, $scope.itemIndex, $scope.sortField, sortDirection, facets, $stateParams.dept);
            return ProductService.getSearchUrl($scope.paramType, $scope.paramId, $scope.$state.params.catalogType) + '?' + jQuery.param(params); // search query string param
          },
          exportType: function() {
             return Constants.exportType.searchExport;
          }
        }
      });
    };

    $scope.bekCatalogSwitch = function () {
        //change state to unfi
        $state.go($state.current,{catalogType: 'BEK'}, {reload: true});
    };

    $scope.unfiCatalogSwitch = function () {
        //change state to unfi
        $state.go($state.current,{catalogType: 'UNFI'}, {reload: true});
    };
    // INIT
    loadProducts().then(refreshFacets);

  }]);
