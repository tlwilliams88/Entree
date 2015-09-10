'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$modal', '$stateParams', '$filter', '$timeout', 'blockUI', 'lists', 'selectedList', 'selectedCart', 'CartService', 'ListService', 'OrderService', 'UtilityService', 'PricingService', 'ListPagingModel', 'LocalStorage', '$analytics',
    function ($scope, $state, $modal, $stateParams, $filter, $timeout, blockUI, lists, selectedList, selectedCart, CartService, ListService, OrderService, UtilityService, PricingService, ListPagingModel, LocalStorage, $analytics) {
    
    
    // redirect to url with correct parameters
       var basketId;
    if ($stateParams.cartId !== 'New') {
      basketId = selectedCart.id || selectedCart.ordernumber;
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

    function onItemQuantityChanged(newVal, oldVal) {
      console.log('onItemQuantityChanged');
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var object = changedExpression.substr(0, changedExpression.indexOf('.'));
      var item = $scope[object].items[idx];
      if(newVal !== oldVal){
        item.initInputs = true;
        refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
        $scope.itemCount = getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items).length;
      }
      item.extPrice = PricingService.getPriceForItem(item);

    }
    var watches = [];
    $scope.addItemWatches = function(startingIndex, endingIndex) {
      endingIndex = ($scope.selectedList.itemCount < (startingIndex + endingIndex)) ? $scope.selectedList.itemCount : endingIndex;
      console.log('addItemWatches');
      for (var i = startingIndex; i < endingIndex; i++) {
        watches.push($scope.$watch('selectedList.items[' + i + '].quantity', onItemQuantityChanged));
        watches.push($scope.$watch('selectedList.items[' + i + '].each', onItemQuantityChanged));
      }
    }
    function clearItemWatches(watchers) {
      console.log('clearItemWatches');
      watchers.forEach(function(watch) {
        watch();
      });
      watchers = [];
    }

    var cartWatches = [];
    $scope.addCartWatches = function() {
      console.log('addCartWatches');
      for (var i = 0; i < $scope.selectedCart.items.length; i++) {
        cartWatches.push($scope.$watch('selectedCart.items[' + i + '].quantity', onItemQuantityChanged));
        cartWatches.push($scope.$watch('selectedCart.items[' + i + '].each', onItemQuantityChanged));
      }
    }

        // combine cart and list items and total their quantities
    function getCombinedCartAndListItems(cartItems, listItems) {
      console.log('getCombinedCartAndListItems');
      var items = angular.copy(cartItems.concat(listItems));
      // combine quantities if itemnumber is a duplicate
      var newCartItems = [];
      
      angular.forEach(items, function(item, index) {
        var duplicateItem = UtilityService.findObjectByField(newCartItems, 'itemnumber', item.itemnumber);
        item.quantity = parseInt(item.quantity, 10);
        if (duplicateItem) {
          if(item.quantity){
            duplicateItem.quantity = duplicateItem.quantity ? duplicateItem.quantity += item.quantity : item.quantity;
          }
        } else {
          // do not double-count items in both the list and cart
          var repeatItemsQuantity = 0;
          listItems.forEach(function(listitem){
            if(item.itemnumber === listitem.itemnumber && listitem.quantity && listitem.quantity > 0){
              repeatItemsQuantity = repeatItemsQuantity + listitem.quantity;
            }
          })
          if (item.isHidden === true && repeatItemsQuantity !== 0) { 
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
      console.log('flagDuplicateCartItems');
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
                $scope.selectedList.items[index].quantity = '';
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

  $scope.pagingPageSize = LocalStorage.getPageSize();
  $scope.pageChanged = function(page) {
    console.log('pageChanged');
      $scope.selectedList.allSelected = false;
      $scope.startingPoint = ((page.currentPage - 1)*parseInt($scope.pagingPageSize));
      $scope.endPoint = $scope.startingPoint + parseInt($scope.pagingPageSize);
      $scope.setRange();
  };

  $scope.setRange = function(){
    console.log('setRange');
    $scope.endPoint = $scope.endPoint;
    $scope.rangeStart = $scope.startingPoint + 1;
    $scope.rangeEnd = ($scope.endPoint > $scope.selectedList.itemCount) ? $scope.selectedList.itemCount : $scope.endPoint;
  }

   $scope.rowChanged = function(index, field){
    $scope.destroyedOnField = field;
    $scope.indexOfSDestroyedRow = index;
   }

    function setSelectedCart(cart) {
      console.log('setSelectedCart');
      $scope.selectedCart = cart;
      addCartWatches();
    }
    function setSelectedList(list) {
      $scope.selectedList = list;
      console.log('setSelectedList');
       $scope.startingPoint = 0;
      
      $scope.endPoint = parseInt($scope.pagingPageSize);
      $scope.currentPage = 1;
      $scope.setRange();
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);    

      if($stateParams.listItems){
       $stateParams.listItems.forEach(function(item){
         $scope.selectedList.items.forEach(function(selectedlistitem){
          if(item.listitemid === selectedlistitem.listitemid){
            selectedlistitem.quantity = item.quantity; 
          }
         })
        })       
       $stateParams.listItems = undefined;
      }
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items);
    }
    function appendListItems(list) {
      console.log('appendListItems');
      $stateParams.listItems = $scope.selectedList.items;
      var originalItemCount = $scope.selectedList.items.length;
      $scope.selectedList.items = $scope.selectedList.items.concat(list.items);
      $scope.appendingList = true;
      $scope.appendedItems = list.items;
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items); 
      //addItemWatches(originalItemCount);
    }
    function startLoading() {
      console.log('startLoading');
      $scope.loadingResults = true;
    }
    function stopLoading() {
      console.log('stopLoading');
      $scope.loadingResults = false;
    }

    function init() {
      console.log('init');
      $scope.lists = lists;
      $scope.shipDates = CartService.shipDates;
      $scope.useParlevel = $stateParams.useParlevel === 'true' ? true : false;
      
      if (selectedCart) {
        setSelectedCart(selectedCart);
        $scope.isChangeOrder = selectedCart.hasOwnProperty('ordernumber') ? true : false;
      } else {
        // create new cart if no cart was selected
        $scope.generateNewCartForDisplay();
      }
       setSelectedList(selectedList);

      if($stateParams.cartId !== 'New' && $stateParams.searchTerm){
        $scope.filterItems($stateParams.searchTerm);
      }
    }

    $scope.sort = {
      field: 'position',
      sortDescending: false
    };
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
      console.log('refreshQuantities');
      $scope.clearedWhilePristine = false;
        flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
        getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items)
    }
    $scope.filterItems = function(searchTerm) {  
      console.log('filterItems');
     
      if($stateParams.searchTerm || $scope.addToOrderForm.$pristine){
        if($stateParams.searchTerm ){
          $scope.orderSearchTerm = $stateParams.searchTerm;
        }
        listPagingModel.filterListItems(searchTerm);
        $stateParams.searchTerm = '';
        clearItemWatches(watches);       
      }
      else{      
          $scope.validateAndSave().then(function(resp){
           var continueSearch = resp;
     
        if(continueSearch){           
          $scope.addToOrderForm.$setPristine();    
          listPagingModel.filterListItems(searchTerm);
          clearItemWatches(watches);         
        }
        return resp;  
        })      
      }   
    };

    $scope.validateAndSave = function(){
      console.log('validateAndSave');
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
      console.log('clearFilter');

          $scope.orderSearchTerm = '';
         $stateParams.searchTerm = '';
      if($scope.addToOrderForm.$pristine){

        $scope.filterItems( $scope.orderSearchTerm)
        $scope.clearedWhilePristine = true;
      
      }
      else{
       $scope.validateAndSave().then(function(resp){
        var clearSearchTerm = resp;
       
      if(clearSearchTerm){
        $scope.filterItems($scope.orderSearchTerm);
      } 
      })
    }
    angular.element(orderSearchForm.searchBar).focus();
    };
  

      Mousetrap.bind(['alt+x', 'option+x'], function(e) { 
        $scope.clearFilter();        
      });

      Mousetrap.bind(['alt+s', 'option+s'], function(e) {
        $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
      });

      Mousetrap.bind(['alt+z', 'option+z'], function(e) {      
       angular.element(orderSearchForm.searchBar).focus();
      });

    $scope.confirmQuantity = function(type, item, value) {
      console.log('confirmQuantity');
      if(value === undefined && type === 'onhand'){
        item.onhand = 0;
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
      console.log('openItemUsageSummaryModal');
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
      console.log('sortList');
      if (sortBy === $scope.sort.field) {
        sortOrder = !sortOrder;
      } else {
        sortOrder = false;
      }
      $scope.sort = {
        field: sortBy,
        sortDescending: sortOrder
      };
      clearItemWatches(watches);
      listPagingModel.sortListItems($scope.sort);
    };

    $scope.redirect = function(listId, cart, useParlevel) {
      console.log('redirect');
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
        var continueToCart = $scope.continueToCart
      $state.go('menu.addtoorder.items', { listId: listId, cartId: cartId, useParlevel: useParlevel, continueToCart: continueToCart, listItems: sameListItems, searchTerm: searchTerm})
    };

    /**********
    CARTS
    **********/

    $scope.startRenamingCart = function(cartName) {
      console.log('startRenamingCart');
      $scope.tempCartName = cartName;
      $scope.isRenaming = true;
    };

    $scope.renameCart = function(cartId, name) {
      console.log('renameCart');

      if (cartId === 'New') {
        // don't need to call the backend function for new cart
        $scope.selectedCart.name = name;
        $scope.isRenaming = false;
        CartService.renameCart = false;
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
    };

    $scope.generateNewCartForDisplay = function() {
      console.log('generateNewCartForDisplay');
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
      console.log('updateCart');
      if (!processingUpdateCart) {
        processingUpdateCart = true;
        return CartService.updateCart(cart).then(function(updatedCart) {
          setSelectedCart(updatedCart);
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
      console.log('createNewCart');
      $analytics.eventTrack('Create Order', {  category: 'Orders', label: 'From List' });
      if (!processingSaveCart) {
        var processingSaveCart = true;
        return CartService.createCart(items, shipDate, name).then(function(cart) {
          $scope.addToOrderForm.$setPristine();          
          $scope.redirect($scope.selectedList.listid, cart);
          $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to New Cart.');          
          return cart;
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function(){
          processingSaveCart = false;
          $scope.createFromSearch = false;         
        });
      }
    }

    var processingSaveChangeOrder = false;
    function updateChangeOrder(order) {
      console.log('updateChangeOrder');
      if (!processingSaveChangeOrder) {
        processingSaveChangeOrder = true;

        return OrderService.updateOrder(order).then(function(cart) {
          setSelectedCart(cart);
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

    $scope.saveAndContinue = function(){
      console.log('saveAndContinue');
      $scope.continueToCart = true;
      $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
    }

    $scope.saveAndRetainQuantity = function(){
      console.log('saveAndRetainQuantity');
      $stateParams.listItems = $scope.selectedList.items;
      if($scope.selectedCart.id === 'New'){
           $scope.createFromSearch = true;
        }
       $scope.updateOrderClick($scope.selectedList, $scope.selectedCart)
          $scope.addToOrderForm.$setPristine();
    }

    $scope.updateOrderClick = function(list, cart) {
      console.log('updateOrderClick');
      clearItemWatches(cartWatches);
      var cartItems = getCombinedCartAndListItems(cart.items, list.items);
      UtilityService.deleteFieldFromObjects(cartItems, ['listitemid']);

      var updatedCart = angular.copy(cart);
      updatedCart.items = cartItems;

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
    };

    function refreshSubtotal(cartItems, listItems) {
      console.log('refreshSubtotal');
      var items = getCombinedCartAndListItems(cartItems, listItems);
      $scope.selectedCart.subtotal = PricingService.getSubtotalForItems(items);
      return $scope.selectedCart.subtotal;
    }

    // update quantity from on hand amount and par level
    $scope.onItemOnHandAmountChanged = function(item) {
      console.log('onItemOnHandAmountChanged');
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

    init();

  }]);
