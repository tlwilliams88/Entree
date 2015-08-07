'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$stateParams', '$filter', '$timeout', 'blockUI', 'lists', 'selectedList', 'selectedCart', 'CartService', 'ListService', 'OrderService', 'UtilityService', 'PricingService', 'ListPagingModel', 'LocalStorage', '$analytics',
    function ($scope, $state, $stateParams, $filter, $timeout, blockUI, lists, selectedList, selectedCart, CartService, ListService, OrderService, UtilityService, PricingService, ListPagingModel, LocalStorage, $analytics) {
    
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

    function onItemQuantityChanged(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var object = changedExpression.substr(0, changedExpression.indexOf('.'));
      var item = $scope[object].items[idx];
      item.extPrice = PricingService.getPriceForItem(item);
      refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
      $scope.itemCount = getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items).length;
    }
    var watches = [];
    function addItemWatches(startingIndex) {
      for (var i = startingIndex; i < $scope.selectedList.items.length; i++) {
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
    function addCartWatches() {
      for (var i = 0; i < $scope.selectedCart.items.length; i++) {
        cartWatches.push($scope.$watch('selectedCart.items[' + i + '].quantity', onItemQuantityChanged));
        cartWatches.push($scope.$watch('selectedCart.items[' + i + '].each', onItemQuantityChanged));
      }
    }

    $scope.$watch(function () { 
      return ListService.loadPage;
    }, function () {
     if($stateParams.searchTerm){
      $scope.filterItems($stateParams.searchTerm);
     }
    });

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
          //$stateParams.listItems will indicate whether or not the ATO page is being displayed after saving or after returning to the page from a state change.
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
            $scope.selectedList.items.forEach(function(listItem, index){
            if(listItem.itemnumber === lastDupeInDisplayedList.itemnumber && listItem.listitemid !== lastDupeInDisplayedList.listitemid){
              $scope.selectedList.items[index].quantity = '';
            }
              if(listItem.listitemid === lastDupeInDisplayedList.listitemid){
                $scope.selectedList.items[index].quantity = cartItem.quantity;
              }

            })
          }
        }
        } else {
          cartItem.isHidden = false;
        }
      });    

        $scope.appendedItems = [];           
        refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);
    }

    function setSelectedCart(cart) {
      $scope.selectedCart = cart;
      addCartWatches();
    }
    function setSelectedList(list) {
      $scope.selectedList = list;
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
      addItemWatches(0);
    }
    function appendListItems(list) {
      var originalItemCount = $scope.selectedList.items.length;
      $scope.selectedList.items = $scope.selectedList.items.concat(list.items);
      $scope.appendingList = true;
      $scope.appendedItems = list.items;
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
      getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items); 
      addItemWatches(originalItemCount);
    }
    function startLoading() {
      $scope.loadingResults = true;
    }
    function stopLoading() {
      $scope.loadingResults = false;
    }

    function init() {
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
    $scope.checkForSearchTerm = function(){
      if($stateParams.searchTerm){     
          $scope.filterItems($stateParams.searchTerm);
      }
    }
    $scope.filterItems = function(searchTerm) {  
     
      if($stateParams.searchTerm || $scope.addToOrderForm.$pristine){
        if($stateParams.searchTerm ){
          $scope.orderSearchTerm = $stateParams.searchTerm;
        }
        listPagingModel.filterListItems(searchTerm);
        $stateParams.searchTerm = '';
        clearItemWatches(watches);
        flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
        getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items)
      }
      else{      
          $scope.validateAndSave().then(function(resp){
           var continueSearch = resp;
     
        if(continueSearch){           
          $scope.addToOrderForm.$setPristine();    
          listPagingModel.filterListItems(searchTerm);
          clearItemWatches(watches);
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
          getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items)
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
      if($scope.addToOrderForm.$pristine){
         $scope.orderSearchTerm = '';
         $stateParams.searchTerm = '';
        $scope.filterItems( $scope.orderSearchTerm);
    }
    else{
       $scope.validateAndSave().then(function(resp){
        var clearSearchTerm = resp;
       
      if(clearSearchTerm){
        flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
        getCombinedCartAndListItems($scope.selectedCart.items, $scope.selectedList.items)      
        $scope.orderSearchTerm = '';
        $stateParams.searchTerm = '';
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
          var pattern = /^([0-9])\1+$/; // repeating digits pattern
          if (value > 50 || pattern.test(value)) {
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

	// $scope.loadEntireList = function() {
 //        blockUI.start();
 //        listPagingModel.loadAllData($scope.selectedList.items, $scope.selectedList.itemCount, $scope.loadingResults);     
 //        blockUI.stop();       
 //  };

    $scope.sortList = function(sortBy, sortOrder) {
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
    $scope.infiniteScrollLoadMore = function() {
      listPagingModel.loadMoreData($scope.selectedList.items, $scope.selectedList.itemCount, $scope.loadingResults, []);
    };
    $scope.redirect = function(listId, cart, useParlevel) {
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
      $scope.tempCartName = cartName;
      $scope.isRenaming = true;
    };

    $scope.renameCart = function(cartId, name) {

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
      $scope.continueToCart = true;
      $scope.updateOrderClick($scope.selectedList, $scope.selectedCart);
    }

    $scope.updateOrderClick = function(list, cart) {
      clearItemWatches(cartWatches);
      var cartItems = getCombinedCartAndListItems(cart.items, list.items);
      UtilityService.deleteFieldFromObjects(cartItems, ['listitemid']);

      var updatedCart = angular.copy(cart);
      updatedCart.items = cartItems;

      if (cartItems && cartItems.length > 0) {
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
      var items = getCombinedCartAndListItems(cartItems, listItems);
      $scope.selectedCart.subtotal = PricingService.getSubtotalForItems(items);
      return $scope.selectedCart.subtotal;
    }

    // update quantity from on hand amount and par level
    $scope.onItemOnHandAmountChanged = function(item) {
      if (!isNaN(item.onhand)) {
        if(item.onhand < 0){
          item.onhand = 0;
        }
        var quantity = Math.ceil(item.parlevel - item.onhand);
        if (quantity > -1) {
          item.quantity = quantity;
        } else {
          item.quantity = 0;
        }
      }
    };

    init();

  }]);
