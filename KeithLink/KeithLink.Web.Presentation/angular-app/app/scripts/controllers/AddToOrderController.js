'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$modal', '$q', '$stateParams', '$filter', '$timeout', 'blockUI', 'lists', 'selectedList', 'selectedCart', 'CartService', 'ListService', 'OrderService', 'UtilityService', 'PricingService', 'ListPagingModel', 'LocalStorage', '$analytics', 'toaster',
    function ($scope, $state, $modal, $q, $stateParams, $filter, $timeout, blockUI, lists, selectedList, selectedCart, CartService, ListService, OrderService, UtilityService, PricingService, ListPagingModel, LocalStorage, $analytics, toaster) {
    
    CartService.getCartHeaders().then(function(cartHeaders){
      $scope.cartHeaders = cartHeaders;
    });

    $scope.calculatePieces = function(items){
      //total piece count for cart info box
      $scope.piecesCount = 0;
        items.forEach(function(item){
          $scope.piecesCount = $scope.piecesCount + item.quantity;
        })
    }

    // redirect to url with correct parameters
    var basketId;
    if ($stateParams.cartId !== 'New') {
      basketId = selectedCart.id || selectedCart.ordernumber;
      selectedCart.items = OrderService.filterDeletedOrderItems(selectedCart);
      $scope.calculatePieces(selectedCart.items);
      $scope.origItemCount = selectedCart.items.length;

      if($stateParams.continueToCart){
      //continueToCart indicates the Proceed to Checkout button was pressed.
      $state.go('menu.cart.items', {cartId: basketId});
      }
    } else {
      basketId = 'New';
    }
    if ($stateParams.cartId !== basketId.toString() || $stateParams.listId !== selectedList.listid.toString()) {
      $state.go('menu.addtoorder.items', {cartId: basketId, listId: selectedList.listid}, {location:'replace', inherit:false, notify: false});
    }
    $scope.confirmQuantity = ListService.confirmQuantity;
    $scope.basketId = basketId;   
    $scope.indexOfSDestroyedRow = '';
    $scope.destroyedOnField = '';
    $scope.useParLevel = false;
    $scope.visitedPages = [];
        
    $scope.removeRowHighlightParLevel = function(){
        $('.ATOrowHighlight').removeClass('ATOrowHighlight');
    };

    $scope.openQuickAddModal = function() {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/cartquickaddmodal.html',
        controller: 'CartQuickAddModalController'
      });
   
      modalInstance.result.then(function(cartId) {
        $state.go('menu.cart.items', {cartId: cartId});
      });
    };

    function onItemQuantityChanged(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var object = changedExpression.substr(0, changedExpression.indexOf('.'));
      var item = $scope[object].items[idx];      
 
      if(newVal !== oldVal && item){        
        refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);        
        var combinedItems = getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items);        
        $scope.calculatePieces(combinedItems);
        $scope.itemCount = combinedItems.length;
      }
      if(item !== undefined){
        item.extPrice = PricingService.getPriceForItem(item);
      }  
    }

    var watches = [];
    $scope.addItemWatches = function(startingIndex, endingIndex) {
      watches = [];
      endingIndex = ($scope.selectedList.itemCount < (startingIndex + (endingIndex - startingIndex))) ? $scope.selectedList.itemCount : endingIndex;
      for (var i = startingIndex; i < endingIndex; i++) {
        watches.push($scope.$watch('selectedList.items[' + i + '].quantity', onItemQuantityChanged));
        watches.push($scope.$watch('selectedList.items[' + i + '].each', onItemQuantityChanged));
      }
    }
    function clearItemWatches(watchers) {
      watchers.forEach(function(watch) {
        watch();
      });
      watchers = [];
    }

    var cartWatches = [];
    $scope.addCartWatches = function() {
      for (var i = 0; i < $scope.selectedCart.items.length; i++) {       
        cartWatches.push($scope.$watch('selectedCart.items[' + i + '].quantity', onItemQuantityChanged));
        cartWatches.push($scope.$watch('selectedCart.items[' + i + '].each', onItemQuantityChanged));
      }
    }
    
        // combine cart and list items and total their quantities
    function getCombinedCartAndListItems(cartItems, listItems) {    
      var items = angular.copy(cartItems.concat(listItems));
      // combine quantities if itemnumber is a duplicate
      var newCartItems = [];
      
      angular.forEach(items, function(item, index) {
        var duplicateItem = UtilityService.findObjectByField(newCartItems, 'itemnumber', item.itemnumber);
        item.quantity = parseInt(item.quantity, 10);
        if (duplicateItem) {
          if(item.quantity){
            duplicateItem.quantity = duplicateItem.quantity ? duplicateItem.quantity += item.quantity : item.quantity;
            duplicateItem.extPrice = (duplicateItem.quantity === item.quantity) ? item.extPrice : PricingService.getPriceForItem(duplicateItem);
            duplicateItem.each = item.each;
            if(item.quantity > 0){
              duplicateItem.iscombinedquantity = true;
            }
          }
        } else {
          // do not double-count items in both the list and cart 
          if (item.isHidden === true) {
            item.quantity = 0;
          }
          newCartItems.push(item);
        }
      });
        // remove items with 0 quantity
        newCartItems = $filter('filter')(newCartItems, function(item) {
        return item.quantity > 0;
      });
      return newCartItems;
    }

     function flagDuplicateCartItems(cartItems, listItems) {
      angular.forEach(cartItems, function(cartItem) {
        var existingItem = UtilityService.findObjectByField(listItems, 'itemnumber', cartItem.itemnumber);
        if (existingItem) {
          cartItem.isHidden = true;
          // flag cart items that are in the list multiple times
          // hide those duplicate cart items from ui
          //$stateParams.listItems and testDuplicates will indicate whether or not the ATO page is being displayed after saving or after returning to the page from a state change.
          //testDuplicates will tell you if the totals for duplicate items have been combined or not
          var testDuplicates = 0;
          var lastDupeInDisplayedList = {};
          listItems.forEach(function(item){
            if(item.itemnumber === existingItem.itemnumber){
                testDuplicates = testDuplicates + 1;
                lastDupeInDisplayedList = item;
            }
          })

          if($scope.appendedItems && $scope.appendedItems.length > 0){
            var lastInstanceInAppendedItems = {};
            $scope.appendedItems.forEach(function(appendedItem){
              if(appendedItem.itemnumber === cartItem.itemnumber){
                lastInstanceInAppendedItems = appendedItem;
              }
            })
            if(lastInstanceInAppendedItems && lastInstanceInAppendedItems.name){
              var alreadyAccountedFor = false;
              listItems.forEach(function(listItem){
                if(listItem.itemnumber === lastInstanceInAppendedItems.itemnumber && listItem.listitemid !== lastInstanceInAppendedItems.listitemid){
                  alreadyAccountedFor = true;
                }
              })
              lastInstanceInAppendedItems.quantity = alreadyAccountedFor ? '' : cartItem.quantity;
            }
          }
          else{
          if(testDuplicates===0 && !$stateParams.listItems){
            existingItem.quantity = cartItem.quantity; // set list item quantity
          }
          else{   
            if(!$stateParams.listItems){
              $scope.selectedList.items.forEach(function(listItem, index){
              if(listItem.itemnumber === lastDupeInDisplayedList.itemnumber && listItem.listitemid !== lastDupeInDisplayedList.listitemid){
                $scope.selectedList.items[index].quantity = 0;
              }
                if(listItem.listitemid === lastDupeInDisplayedList.listitemid){
                  $scope.selectedList.items[index].quantity = cartItem.quantity;
                  $scope.selectedList.items[index].each = cartItem.each;
                }
              })
            }
          }
        }
        } else {
          cartItem.isHidden = false;
        }
      });
        $scope.appendedItems = [];           
        refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
    }

    $scope.blockUIAndChangePage = function(page){
      //Check if items for page already exist in the controller     
      $scope.startingPoint = 0;
      $scope.endPoint = 0;    
       var visited = $filter('filter')($scope.visitedPages, {page: page.currentPage});
         blockUI.start("Loading List...").then(function(){
          if(visited.length > 0){
            $timeout(function() {
              $scope.pageChanged(page, visited);
            }, 100);
          }
          else{
            $scope.pageChanged(page, visited);
          }
        })     
    }

    
    $scope.setCartItemsDisplayFlag = function (){
      if($scope.selectedCart.items && $scope.selectedCart.items.length > 0){        
        $scope.selectedCart.items.forEach(function(item){
          if($filter('filter')($scope.selectedList.items.slice($scope.startingPoint, $scope.endPoint), {itemnumber: item.itemnumber}).length > 0){
            item.isShown = true;
          }
          else{
            item.isShown = false;
          }      
      })
      }
    }

  $scope.pagingPageSize = LocalStorage.getPageSize();
  $scope.pageChanged = function(page, visited) {
      $scope.currentPage = page.currentPage
      $scope.startingPoint = ((page.currentPage - 1)*parseInt($scope.pagingPageSize));
      $scope.endPoint = $scope.startingPoint + parseInt($scope.pagingPageSize);
      $scope.setRange();
        if(!visited.length){
          listPagingModel.loadMoreData($scope.startingPoint, $scope.endPoint - 1, $scope.loadingResults, []);
        }
        else{
          var foundStartPoint = false;
          $scope.selectedList.items.forEach(function(item, index){
            if(item.listitemid && item.listitemid === visited[0].items[0].listitemid){
              $scope.startingPoint = index;
              $scope.endPoint = angular.copy($scope.startingPoint + parseInt($scope.pagingPageSize));
              foundStartPoint = true;
              $scope.addItemWatches($scope.startingPoint, $scope.endPoint)
              $scope.setCartItemsDisplayFlag();
            }
          })

          if(!foundStartPoint && visited[0].items.length > 0){
            appendListItems(visited[0].items);
          }
           blockUI.stop();
        }        

  };

$scope.setCurrentPageAfterRedirect = function(pageToSet){
    var visited = [];
    if(!pageToSet && $stateParams.currentPage){
    var page = $stateParams.currentPage;
      }
       else{
        $stateParams.currentPage = '';
        var page = pageToSet || 1;
       }
       
      if($scope.visitedPages[0]){
        visited = $filter('filter')($scope.visitedPages, {page: page});
      }
   var selectedPage = {
      currentPage: page   
     };
     $scope.pageChanged(selectedPage, visited);
}

  $scope.setRange = function(){
    $scope.endPoint = $scope.endPoint;
    $scope.rangeStart = $scope.startingPoint + 1;
    $scope.rangeEnd = ($scope.endPoint > $scope.selectedList.itemCount) ? $scope.selectedList.itemCount : $scope.endPoint;
  }

    function setSelectedCart(cart) {
      $scope.selectedCart = cart;

      $scope.addCartWatches();
    }
    function setSelectedList(list) {
      $scope.selectedList = list;
      $scope.startingPoint = 0;
      $scope.visitedPages = [];
      $scope.visitedPages.push({page: 1, items: $scope.selectedList.items});
      $scope.endPoint = parseInt($scope.pagingPageSize);
      $scope.setCurrentPageAfterRedirect();
      $scope.setRange();
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);

      if($stateParams.listItems){
       $stateParams.listItems.forEach(function(item){
         $scope.selectedList.items.forEach(function(selectedlistitem){
          if(item.listitemid === selectedlistitem.listitemid){
            selectedlistitem.quantity = item.quantity; 
            selectedlistitem.onhand = item.onhand; 
          }
         })
        })       
       $stateParams.listItems = undefined;
      }
      $scope.setCartItemsDisplayFlag();
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items);
      $scope.addItemWatches($scope.startingPoint, $scope.endPoint);
    }
    function appendListItems(list) {
      $stateParams.listItems = $scope.selectedList.items;
      var originalItemCount = $scope.selectedList.items.length;
      var entireListReturned = (list.items.length === $scope.selectedList.itemCount) ? true : false;
      if(entireListReturned){       
        $scope.visitedPages = [];
        var continueLoop = true;
        var numberOfPages = parseInt(list.items.length/$scope.pagingPageSize);
        for(var i = 1; continueLoop; i++){
          var start = (i -1) * $scope.pagingPageSize;
           continueLoop = (start + $scope.pagingPageSize) < ($scope.selectedList.itemCount -1);
          var end = (continueLoop) ? (start + $scope.pagingPageSize) : ($scope.selectedList.itemCount -1);      
          $scope.visitedPages.push({page: i, items: list.items.slice(start,end)});
        }
      }
      else{
        $scope.selectedList.items = $scope.selectedList.items.concat(list.items);
        $scope.visitedPages.push({page: $scope.currentPage, items: list.items});
        $scope.visitedPages = $scope.visitedPages.sort(function(obj1, obj2){   
          var sorterval1 = obj1.page;      
          var sorterval2 = obj2.page;       
          return sorterval1 - sorterval2;         
        })
      }
      var firstItemOnCurrentpage = {};
      $scope.selectedList.items = [];
      $scope.visitedPages.forEach(function(page){
        $scope.selectedList.items = $scope.selectedList.items.concat(page.items);
        if($scope.currentPage === page.page){
          firstItemOnCurrentpage = page.items[0];
        }
      })

        $scope.selectedList.items.forEach(function(item, index){
          if(item.listitemid === firstItemOnCurrentpage.listitemid){
            $scope.startingPoint = index;
            $scope.endPoint = angular.copy(index + $scope.pagingPageSize);
            $scope.setCartItemsDisplayFlag();
          }
        })

      $scope.appendingList = true;
      $scope.appendedItems = list.items;
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items); 
      $scope.addItemWatches($scope.startingPoint, $scope.endPoint);
    }
    function startLoading() {
      $scope.loadingResults = true;
    }
    function stopLoading() {
      $scope.loadingResults = false;
       blockUI.stop();
    }

    function init() {
      $scope.lists = lists;
      CartService.getShipDates().then(function(shipdates){

        if(shipdates && shipdates.length > 0){
          $scope.shipDates = shipdates;
          $scope.useParlevel = $stateParams.useParlevel === 'true' ? true : false;
        
          if (selectedCart) {
            setSelectedCart(selectedCart);
            $scope.isChangeOrder = selectedCart.hasOwnProperty('ordernumber') ? true : false;
            if(selectedCart.requestedshipdate && moment(selectedCart.requestedshipdate.slice(0,10)) < moment($scope.shipDates[0].shipdate)){
               $scope.openErrorMessageModal('The ship date requested for this order has expired. Select Cancel to return to the home screen without making changes. Select Accept to update to the next available ship date.');
              selectedCart.requestedshipdate = $scope.shipDates[0].shipdate;
            }
          } else {
            // create new cart if no cart was selected
            $scope.generateNewCartForDisplay();
          }

          $scope.visitedPages.push({page: 1, items: selectedList.items});
           setSelectedList(selectedList);
          $scope.setCartItemsDisplayFlag();
          if($stateParams.cartId !== 'New' && $stateParams.searchTerm){
            $scope.filterItems($stateParams.searchTerm);
          }
          if($stateParams.createdFromPrint){
            $stateParams.createdFromPrint = false;
            $scope.createdFromPrint = false;
            $scope.openPrintOptionsModal($scope.selectedList, $scope.selectedCart);
          }
          blockUI.stop();
        }
        else{
          alert('An error has occurred retrieving available shipping dates. Please contact your DSR for more information.');
          $state.go('menu.home');
          return;
        }        
      })
    }

    if($stateParams.sortingParams && $stateParams.sortingParams.sort.length > 0){
      $scope.sort = $stateParams.sortingParams.sort;
    }
    else{
      $scope.sort = [{
      field: 'position',
      order: false
    }];
    }

    var listPagingModel = new ListPagingModel( 
      selectedList.listid,
      setSelectedList,
      appendListItems,
      startLoading,
      stopLoading,
      $scope.sort
    );

    /**********
    PAGING
    **********/

    $scope.refreshQuantities = function(){
      $scope.clearedWhilePristine = false;
        flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
        getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items)
    }
    $scope.filterItems = function(searchTerm) {  
      if($stateParams.searchTerm || $scope.addToOrderForm.$pristine){
        if($stateParams.searchTerm ){
          $scope.orderSearchTerm = $stateParams.searchTerm;
        }
        $scope.visitedPages = [];
        listPagingModel.filterListItems(searchTerm);
        $stateParams.searchTerm = '';
        clearItemWatches(watches);       
      }
      else{      
          $scope.validateAndSave().then(function(resp){
            if($scope.isRedirecting(resp)){
              //do nothing
            }
            else{
              var continueSearch = resp;       
              if(continueSearch){           
                $scope.visitedPages = [];
                $scope.addToOrderForm.$setPristine();
                listPagingModel.filterListItems(searchTerm);
                clearItemWatches(watches);         
              }
            }        
          })      
      }   
    };

    $scope.validateAndSave = function(){
      if($scope.addToOrderForm.$invalid){
          var r = confirm('Unsaved data will be lost. Do you wish to continue?');
          return r;   
      }
      else{  
          if($scope.selectedCart.id === 'New'){
             $scope.createFromSearch = true;
          }           
          return $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
      }      
    };

    $scope.clearFilter = function(){ 

          $scope.orderSearchTerm = '';
         $stateParams.searchTerm = '';
      if($scope.addToOrderForm.$pristine){

        $scope.filterItems( $scope.orderSearchTerm)
        $scope.clearedWhilePristine = true;
      
      }
      else{
       $scope.validateAndSave().then(function(resp){
        if($scope.isRedirecting(resp)){        
        return
        }
        else{
        var clearSearchTerm = resp;
       
      if(clearSearchTerm){
        $scope.filterItems($scope.orderSearchTerm);
      }
    }
      })
    }
     $scope.setCurrentPageAfterRedirect(1);
    angular.element(orderSearchForm.searchBar).focus();
    };
  

      Mousetrap.bind(['alt+x'], function(e) {
        $scope.clearFilter();
      });

      Mousetrap.bind(['alt+s'], function(e) {
        $scope.updateOrderClick($scope.selectedList, $scope.selectedCart).then(function(resp){
          $scope.isRedirecting(resp);
        })
      });

      Mousetrap.bind(['alt+z'], function(e) {      
       angular.element(orderSearchForm.searchBar).focus();
      });

      Mousetrap.bind(['alt+o'], function(e){
        angular.element(quickOrder).focus();{
          $scope.openQuickAddModal()
        }
      })

    $scope.confirmQuantity = function(type, item, value) {

      if((value === undefined || value === 0) && type === 'onhand'){
        item.onhand = '0';
        $scope.onItemOnHandAmountChanged(item);
      }
      if((!value || value === undefined) && type === 'quantity'){
        item.quantity = '0';
      }
          var pattern = /^([0-9])\1+$/; // repeating digits pattern
          if (value > 50 || (value > 0 && pattern.test(value))) {
            var isConfirmed = window.confirm('Do you want to continue with entered quatity of ' + value + '?');
            if (!isConfirmed) {
              // clear input
            if(type==='quantity'){
              item.quantity = null;
            }
            else{
              item.onhand=null;
            }
            }
          } 
        };
  
    $scope.openItemUsageSummaryModal = function(item, type) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/itemusagesummarymodal.html',
        controller: 'ItemUsageSummaryModalController',
        windowClass: 'color-background-modal',
        scope: $scope,
        resolve: {
          item: function() {
            return item;
          }
        }
      });
    };

    $scope.sortList = function(sortBy, sortOrder) {
      var r = ($scope.addToOrderForm.$pristine) ? true : confirm('Unsaved data will be lost. Do you wish to continue?');
      if(r){
        $scope.visitedPages = [];
        
        if (sortBy === $scope.sort[0].field) {
          sortOrder = (sortOrder === 'asc') ? 'desc' : 'asc';
        } else {
          sortOrder = 'asc';
        }
        $scope.sort = [{
          field: sortBy,
          order: sortOrder
        }];
        clearItemWatches(watches);
        listPagingModel.sortListItems($scope.sort);
      }
    };

    $scope.redirect = function(listId, cart) {
      if(!$scope.unsavedChangesConfirmation()){
        return;
      } 
      $scope.addToOrderForm.$setPristine();
      var cartId;    
      if ($scope.isChangeOrder) {
        cartId = cart.ordernumber;
      } else {
        cartId = cart.id;
      }
      var timeset = moment().format('YYYYMMDDHHmm');
      var orderList ={
          listId: listId,
          cartId: cartId,
          timeset: timeset
          }
      var allSets = [];
          allSets = LocalStorage.getLastOrderList();
        if(!allSets || (allSets[0] && !allSets[0].timeset)){
          allSets = [];
        }

        var matchFound = false;
        if(orderList.cartId !== 'New'){
          allSets.forEach(function(set){
            if(set.cartId === orderList.cartId){
              set.listId = orderList.listId;
              set.timeset =  moment().format('YYYYMMDDHHmm');
              matchFound = true;
            }
          });
          if(!matchFound){
            allSets.push(orderList);
          }
        }
       var searchTerm = '';
        if($scope.orderSearchTerm && $scope.createFromSearch){
         var searchTerm = $scope.orderSearchTerm;
        }

        LocalStorage.setLastOrderList(allSets);

      var sameListItems= [];
      if(listId === $scope.selectedList.listid){
        sameListItems = $scope.selectedList.items;
      }
      else{
        sameListItems = undefined;
      }
        var continueToCart = $scope.continueToCart;
        
      blockUI.start("Loading List...").then(function(){
        $state.go('menu.addtoorder.items', { 
          listId: listId,
          cartId: cartId,
          useParlevel: $scope.useParlevel,
          continueToCart: continueToCart,
          listItems: sameListItems,
          searchTerm: searchTerm,
          createdFromPrint: $scope.createdFromPrint,
          currentPage: $scope.retainedPage})
      });

    };

    $scope.unsavedChangesConfirmation = function(){
      if($scope.addToOrderForm.$dirty){
          var r = confirm('Unsaved data will be lost. Do you wish to continue?');
          return r;   
      }
      else{
        return true;
      }  
    };

    /**********
    CARTS
    **********/

    $scope.startRenamingCart = function(cartName) {
      $scope.tempCartName = cartName;
      $scope.isRenaming = true;
    };

    $scope.renameCart = function(cartId, name) {
      var duplicateName = false;
      CartService.cartHeaders.forEach(function(header){
        if(name === header.name){
          duplicateName = (header.id === cartId) ? false : true;
            $scope.isRenaming = duplicateName;
        }
      });

      if(duplicateName){
        $scope.tempCartName = '';
        angular.element(renameCartForm.cartName).focus();
        toaster.pop('error', 'Error Saving Cart -- Cannot have two carts with the same name. Please rename this cart');
      }
      else{
        $scope.addToOrderForm.$setDirty();
        if (cartId === 'New') {
        // don't need to call the backend function for new cart
        $scope.selectedCart.name = name;
        $scope.isRenaming = false;
        CartService.renameCart = false;
        $scope.updateOrderClick($scope.selectedList, $scope.selectedCart).then(function(resp){
          $scope.isRedirecting(resp);
        })
      } else {
        // call backend to update cart
        var cart = angular.copy($scope.selectedCart);
        cart.name = name;
        CartService.updateCart(cart).then(function(updatedCart) {
          $scope.selectedCart.name = updatedCart.name;
          $scope.isRenaming = false;
          CartService.renameCart = false;
        });
      }
    }      
    };

    $scope.generateNewCartForDisplay = function() {
      var cart = {};
      cart.items = [];
      cart.id = 'New';
      cart.requestedshipdate = $scope.shipDates[0].shipdate;
      $scope.selectedCart = cart;
      $scope.isChangeOrder = false;
      $scope.startRenamingCart($scope.selectedCart.name);
    };

    /**********
    FORM EVENTS
    **********/



    var processingUpdateCart = false;
    function updateCart(cart) {
      if (!processingUpdateCart) {
        processingUpdateCart = true;
        return CartService.updateCart(cart).then(function(updatedCart) {
          setSelectedCart(updatedCart);
          $scope.setCartItemsDisplayFlag();
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
          $scope.addToOrderForm.$setPristine();

          var newItemCount = updatedCart.items.length - $scope.origItemCount;
          $scope.origItemCount = updatedCart.items.length;

          if(newItemCount > 0){
            $scope.displayMessage('success', 'Successfully added ' + newItemCount + ' Items to Cart ' + updatedCart.name + '.');
          }else if(newItemCount < 0){
              $scope.displayMessage('success', 'Successfully removed ' + Math.abs(newItemCount) + ' Items from Cart ' + updatedCart.name + '.');
          }
          else{
            $scope.displayMessage('success', 'Successfully Saved Cart ' + updatedCart.name + '.');
           }
           processingUpdateCart = false
           return updatedCart; 
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function() {
          processingUpdateCart = false;
          if($scope.continueToCart){ 
          $state.go('menu.cart.items', {cartId: basketId});
          }
        });
      }
    }

    
    function createNewCart(items, shipDate, name) {
      $analytics.eventTrack('Create Order', {  category: 'Orders', label: 'From List' });
      if (!processingSaveCart) {
        var processingSaveCart = true;
        return CartService.createCart(items, shipDate, name).then(function(cart) {
            CartService.getCartHeaders().finally(function(cartHeaders) {
              $scope.loadingCarts = false;
              $scope.carts = CartService.cartHeaders;
            });
          $scope.addToOrderForm.$setPristine();
          $scope.retainedPage = $scope.currentPage;
          $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to New Cart.');
          return cart;
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function(){
          processingSaveCart = false;          
        });
      }
    };

    var processingSaveChangeOrder = false;
    function updateChangeOrder(order) {
      if (!processingSaveChangeOrder) {
        processingSaveChangeOrder = true;

        return OrderService.updateOrder(order).then(function(cart) {
          setSelectedCart(cart);
          $scope.setCartItemsDisplayFlag();
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
          refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
          $scope.addToOrderForm.$setPristine();

          if($scope.continueToCart){ 
          $state.go('menu.cart.items', {cartId: order.ordernumber});
          }
          var newItemCount = cart.items.length - $scope.origItemCount;
          $scope.origItemCount = cart.items.length;

          if(newItemCount > 0){
            $scope.displayMessage('success', 'Successfully added ' + newItemCount + ' Items to Order # ' + order.invoicenumber + '.');
          }else if(newItemCount < 0){
            $scope.displayMessage('success', 'Successfully removed ' + Math.abs(newItemCount) + ' Items from Order # ' + order.invoicenumber + '.');
          }
          else{
            $scope.displayMessage('success', 'Successfully Saved Order ' + order.invoicenumber + '.');
          }

          processingSaveChangeOrder = false;
          return cart;
        }, function() {
          $scope.displayMessage('error', 'Error adding items to change order.');
        }).finally(function() {
          processingSaveChangeOrder = false;
          if($scope.continueToCart){ 
            $state.go('menu.cart.items', {cartId: order.ordernumber});
          }
        });
      }
    }

    $scope.isRedirecting = function(resp){
      if(resp.message && resp.message === "Creating cart..."){
        $scope.redirect($scope.selectedList.listid, resp);
        return true;
      }
      else{
        return false;
      }
    }

    $scope.saveAndContinue = function(){
      $scope.continueToCart = true;
      $scope.updateOrderClick($scope.selectedList, $scope.selectedCart).then(function(resp){
        $scope.isRedirecting(resp);
      CartService.getCartHeaders().finally(function(cartHeaders) {
        $scope.loadingCarts = false;
        $scope.carts = CartService.cartHeaders;
        });
      })
    }

    $scope.saveAndRetainQuantity = function(){
    $stateParams.listItems = $scope.selectedList.items;
    if($scope.selectedCart.id === 'New'){
      $scope.createFromSearch = true;
    }
    $scope.updateOrderClick($scope.selectedList, $scope.selectedCart).then(function(resp){
      $scope.isRedirecting(resp);
    })
    $scope.addToOrderForm.$setPristine();
    }

    $scope.updateOrderClick = function(list, cart) {
      clearItemWatches(cartWatches);
      var cartItems = getCombinedCartAndListItems(cart.items, list.items);
      UtilityService.deleteFieldFromObjects(cartItems, ['listitemid']);

      var updatedCart = angular.copy(cart);
      updatedCart.items = cartItems;

      var invalidItemFound = false;

      updatedCart.items.forEach(function(cartitem){
        if (!cartitem.extPrice && !(cartitem.extPrice > 0)){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Cannot create cart. Item ' + cartitem.itemnumber +' is invalid.  Please contact DSR for more information.');
        }
      })

      if (invalidItemFound){
        var deferred = $q.defer();
        deferred.resolve(invalidItemFound);
        return deferred.promise;
      }

      if ((cartItems && cartItems.length > 0) || $scope.addToOrderForm.$dirty){
        if ($scope.isChangeOrder) {
         return updateChangeOrder(updatedCart);         
        } else {
          if (updatedCart && updatedCart.id && updatedCart.id !== 'New') {
            return updateCart(updatedCart);            
      
          } else {
            return createNewCart(cartItems, updatedCart.requestedshipdate, updatedCart.name);
          }
        }
      }
      else{
        return false;
      }
    };

    function refreshSubtotal(cartItems, listItems) {
      var items = getCombinedCartAndListItems(cartItems, listItems);
      $scope.selectedCart.subtotal = PricingService.getSubtotalForItems(items);
      return $scope.selectedCart.subtotal;
    }

    // update quantity from on hand amount and par level
    $scope.onItemOnHandAmountChanged = function(item) {
      var offset = item.onhand;
      if(item.onhand && item.onhand.toString() === 'true'){
        offset= 0;
      }
      if (!isNaN(item.onhand)) {
        if(item.onhand < 0){
          item.onhand = offset = 0;
        }
        var quantity = Math.ceil(item.parlevel - offset);
        if (quantity > -1) {
          item.quantity = quantity;
        } else {
          item.quantity = 0;
        }
      }
    };

    $scope.saveBeforePrint = function(){
      if($scope.addToOrderForm.$pristine && $scope.selectedCart.id !== 'New'){
        $scope.openPrintOptionsModal($scope.selectedList, $scope.selectedCart);
      }
      else{
        if($scope.selectedCart.id === 'New'){
        $scope.createdFromPrint = true;
        $scope.addToOrderForm.$setDirty();
      }         
      $scope.validateAndSave().then(function(resp){
        if($scope.isRedirecting(resp)){
          //do nothing
        }
        else{
          $scope.openPrintOptionsModal($scope.selectedList, $scope.selectedCart);
        }
      })
      }
    }

    $scope.openPrintOptionsModal = function(list, cart) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/printoptionsmodal.html',
        controller: 'PrintOptionsModalController',
        scope: $scope,
        resolve: {
          list: function() {
            return list;
          },
          cart: function() {
            return cart;
          },
          pagingModelOptions: function() {
            return { 
              sort: $scope.sort,
              terms: $scope.orderSearchTerm
            };
          }
        }
      });
    };

    $scope.openErrorMessageModal = function(message) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/errormessagemodal.html',
        controller: 'ErrorMessageModalController',
        scope: $scope,
        resolve: {
          message: function() {
            return message;
          }
          }
      });
    };

    init();

  }]);
