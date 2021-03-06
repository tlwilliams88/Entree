'use strict';

angular.module('bekApp')
  .controller('AddToOrderController', ['$scope', '$state', '$stateParams', '$filter', '$timeout', 'lists', 'selectedList', 'selectedCart', 'CartService', 'ListService', 'OrderService', 'UtilityService', 'PricingService', 'ListPagingModel', '$analytics',
    function ($scope, $state, $stateParams, $filter, $timeout, lists, selectedList, selectedCart, CartService, ListService, OrderService, UtilityService, PricingService, ListPagingModel, $analytics) {
    
    // redirect to url with correct parameters
    var basketId;
    if ($stateParams.cartId !== 'New') {
      basketId = selectedCart.id || selectedCart.ordernumber;
    } else {
      basketId = 'New';
    }
    if ($stateParams.cartId !== basketId.toString() || $stateParams.listId !== selectedList.listid.toString()) {
      $state.go('menu.addtoorder.items', {cartId: basketId, listId: selectedList.listid}, {location:'replace', inherit:false, notify: false});
    }

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

    function flagDuplicateCartItems(cartItems, listItems) {
      // flag cart items that are in the list
      // hide those cart items from ui

      angular.forEach(cartItems, function(cartItem) {
        var duplicateItem = UtilityService.findObjectByField(listItems, 'itemnumber', cartItem.itemnumber);
        if (duplicateItem) {
          duplicateItem.quantity = cartItem.quantity; // set list item quantity 
          cartItem.isHidden = true;
        } else {
          cartItem.isHidden = false;
        }
      });
    }

    function setSelectedCart(cart) {
      $scope.selectedCart = cart;
      addCartWatches();
    }
    function setSelectedList(list) {
      $scope.selectedList = list;
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
      addItemWatches(0);
    }
    function appendListItems(list) {
      var originalItemCount = $scope.selectedList.items.length;
      $scope.selectedList.items = $scope.selectedList.items.concat(list.items);
      flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
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
    $scope.filterItems = function(searchTerm) {  
      $scope.addToOrderForm.$setPristine();
      listPagingModel.filterListItems(searchTerm);
      clearItemWatches(watches);
    };

    $scope.clearFilter = function(){   
      $scope.orderSearchTerm = '';
      $scope.filterItems( $scope.orderSearchTerm );     
    };
  
  //Used for Par Level colums in landscape view for mobile
   $(window).resize(function(){ 
    $scope.$apply(function(){ 
      $scope.checkOrientation(); 
    }); 
  });

  $scope.checkOrientation = function(){    
      $scope.isMobile = UtilityService.isMobileDevice(); 
       if($scope.isMobile && window.innerHeight < window.innerWidth){ 
       $timeout(function(){ 
         $scope.landscapeOrient = true;
        }, 0)
     }
      else{
          $timeout(function(){
          $scope.landscapeOrient = false;
         }, 0)  
       }
     };  
      
     $scope.checkOrientation();
     $(window).on("orientationchange",function(){
     $scope.checkOrientation();
     });

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

      $state.go('menu.addtoorder.items', { listId: listId, cartId: cartId, useParlevel: useParlevel });
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
      } else {
        // call backend to update cart
        var cart = angular.copy($scope.selectedCart);
        cart.name = name;
        CartService.updateCart(cart).then(function(updatedCart) {
          $scope.selectedCart.name = updatedCart.name;
          $scope.isRenaming = false;
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

    // combine cart and list items and total their quantities
    function getCombinedCartAndListItems(cartItems, listItems) {
      
      var items = angular.copy(cartItems.concat(listItems));

      // combine quantities if itemnumber is a duplicate
      var newCartItems = [];
      
      angular.forEach(items, function(item, index) {
        var duplicateItem = UtilityService.findObjectByField(newCartItems, 'itemnumber', item.itemnumber);
        item.quantity = parseInt(item.quantity, 10);
        if (duplicateItem) {
          duplicateItem.quantity += item.quantity;
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

    var processingUpdateCart = false; 
    function updateCart(cart) {
      if (!processingUpdateCart) {
        processingUpdateCart = true;
        CartService.updateCart(cart).then(function(updatedCart) {
          
          setSelectedCart(updatedCart);
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);

          $scope.addToOrderForm.$setPristine();
          $scope.displayMessage('success', 'Successfully added ' + updatedCart.items.length + ' Items to Cart ' + updatedCart.name + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function() {
          processingUpdateCart = false;
        });
      }
    }

    var processingSaveCart = false;
    function createNewCart(items, shipDate, name) {
      $analytics.eventTrack('Create Order', {  category: 'Orders', label: 'From List' });
      if (!processingSaveCart) {
        processingSaveCart = true;
        CartService.createCart(items, shipDate, name).then(function(cart) {
          $scope.addToOrderForm.$setPristine();
          $scope.redirect($scope.selectedList.listid, cart);
          $scope.displayMessage('success', 'Successfully added ' + items.length + ' Items to New Cart.');
        }, function() {
          $scope.displayMessage('error', 'Error adding items to cart.');
        }).finally(function(){
          processingSaveCart = false;
        });
      }
    }

    var processingSaveChangeOrder = false;
    function updateChangeOrder(order) {
      if (!processingSaveChangeOrder) {
        processingSaveChangeOrder = true;

        OrderService.updateOrder(order).then(function(cart) {
          setSelectedCart(cart);
          flagDuplicateCartItems($scope.selectedCart.items, $scope.selectedList.items);
          refreshSubtotal($scope.selectedCart.items, $scope.selectedList.items);

          $scope.addToOrderForm.$setPristine();
          $scope.displayMessage('success', 'Successfully added ' + order.items.length + ' Items to Order # ' + order.invoicenumber + '.');
        }, function() {
          $scope.displayMessage('error', 'Error adding items to change order.');
        }).finally(function() {
          processingSaveChangeOrder = false;
        });
      }
    }

    $scope.updateOrderClick = function(list, cart) {
      clearItemWatches(cartWatches);
      var cartItems = getCombinedCartAndListItems(cart.items, list.items);
      UtilityService.deleteFieldFromObjects(cartItems, ['listitemid']);

      var updatedCart = angular.copy(cart);
      updatedCart.items = cartItems;

      if (cartItems && cartItems.length > 0) {
        if ($scope.isChangeOrder) {
          updateChangeOrder(updatedCart);
        } else {
          if (updatedCart && updatedCart.id && updatedCart.id !== 'New') {
            updateCart(updatedCart);
          } else {
            createNewCart(cartItems, updatedCart.requestedshipdate, updatedCart.name);
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
