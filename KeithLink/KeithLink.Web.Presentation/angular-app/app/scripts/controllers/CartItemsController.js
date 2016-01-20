'use strict';
 
/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', '$modal', '$q', 'ENV', 'Constants', 'CartService', 'OrderService', 'UtilityService', 'PricingService', 'changeOrders', 'originalBasket', 'criticalItemsLists',
    function($scope, $state, $stateParams, $filter, $modal, $q, ENV, Constants, CartService, OrderService, UtilityService, PricingService, changeOrders, originalBasket, criticalItemsLists) {
 
    // redirect to url with correct ID as a param
    var basketId = originalBasket.id || originalBasket.ordernumber;
    if ($stateParams.cartId !== basketId.toString()) {
      $state.go('menu.cart.items', {cartId: basketId}, {location:'replace', inherit:false, notify: false});
    }

    // update cartHeaders in MenuController
    $scope.$parent.$parent.cartHeaders = CartService.cartHeaders;
 
    var watches = [];
    function onQuantityChange(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var item = $scope.currentCart.items[idx];
      if (item) {
        item.extPrice = PricingService.getPriceForItem(item);
      }
 
      $scope.currentCart.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.currentCart.items);
    }
    function addItemWatches(startingIndex) {
      for (var i = startingIndex; i < $scope.currentCart.items.length; i++) {
        watches.push($scope.$watch('currentCart.items[' + i + '].quantity', onQuantityChange));
        watches.push($scope.$watch('currentCart.items[' + i + '].each', onQuantityChange));
      }
    }
    function clearItemWatches() {
      watches.forEach(function(watch) {
        watch();
      });
      watches = [];
    }
 
    $scope.sortBy = 'createddate'; // sort items in the order they were added to the cart
    $scope.sortOrder = false;
    CartService.updateNetworkStatus();
    $scope.isOffline = CartService.isOffline;
    $scope.carts = CartService.cartHeaders;
    $scope.shipDates = CartService.shipDates;
    $scope.changeOrders = OrderService.changeOrderHeaders;
    $scope.isChangeOrder = originalBasket.hasOwnProperty('ordernumber') ? true : false;
    if($scope.isChangeOrder){
      originalBasket.items =  OrderService.filterDeletedOrderItems(originalBasket);
    }
    $scope.currentCart = angular.copy(originalBasket);
    $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);
    $scope.isMobile = ENV.mobileApp;
    $scope.invalidSelectedDate = false;
    $scope.$watch(function () { 
      return CartService.isOffline;
    }, function (newVal, oldVal) {
      if (typeof newVal !== 'undefined') {
        $scope.isOffline = CartService.isOffline;
        $scope.resetSubmitDisableFlag(true);
      }
    });
 
    if (!$scope.isChangeOrder) {
      CartService.setActiveCart($scope.currentCart.id);
    }
 
    addItemWatches(0);    
   
    // set mandatory and reminder lists
    criticalItemsLists.forEach(function(list) {
      if (list.ismandatory) {
        $scope.mandatoryList = list;
      } else if (list.isreminder) {
        $scope.reminderList = list;
      }
    });
 
    // set default selected critical items list
    if ($scope.mandatoryList) {
      $scope.mandatoryList.active = true;
    } else if ($scope.reminderList) {
      $scope.reminderList.active = true;
    } else {
      $scope.mandatoryList = {};
      $scope.reminderList = {};
    }

    $scope.resetSubmitDisableFlag = function(checkForm){
      $scope.disableSubmitButtons = ((!$scope.currentCart.items || $scope.currentCart.items.length === 0) || $scope.isOffline || $scope.invalidSelectedDate);
    };
    $scope.resetSubmitDisableFlag(false);
 
    function selectNextCartId() {
      var redirectId;
      if ($scope.carts.length > 0) {
        redirectId = $scope.carts[0].id;
      } else if ($scope.changeOrders.length > 0) {
        redirectId = $scope.changeOrders[0].ordernumber;
      }
      return redirectId;
    }
 
    $scope.goToCart = function(cartId, isChangeOrder) {
      if (!cartId) {
        cartId = selectNextCartId();
      }
      $state.go('menu.cart.items', {cartId: cartId} );
    };
 
    $scope.cancelChanges = function() {
      $scope.currentCart = angular.copy(originalBasket);
       $scope.resetSubmitDisableFlag(true);
      $scope.cartForm.$setPristine();
    };
 
    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.validateShipDate = function(shipDate){
      var cutoffDate = moment(shipDate.cutoffdatetime);
      var now = moment();
      $scope.invalidSelectedDate = (now > cutoffDate) ? true : false;
      if($scope.invalidShipDate){
        CartService.getShipDates().then(function(result){
          $scope.shipDates = result;
        })
      }
      $scope.resetSubmitDisableFlag(true);
      return $scope.invalidSelectedDate;
    }
 
    $scope.selectShipDate = function(shipDate) {

      if($scope.validateShipDate(shipDate)){
          return;
        }

      $scope.currentCart.requestedshipdate = shipDate.shipdate;
      $scope.selectedShipDate = shipDate;
      
      if($scope.cartForm){
        $scope.cartForm.$setDirty();
       }
    };
 
      if($scope.currentCart && !$scope.currentCart.requestedshipdate){      
          $scope.selectShipDate($scope.shipDates[0]);   
      }else{
          var requestedDate = $scope.currentCart.requestedshipdate;
          var firstAvailableDate = $scope.shipDates[0].shipdate;

          if (requestedDate < firstAvailableDate) {
           $scope.selectShipDate($scope.shipDates[0]);  
          }
      }
 
    $scope.sortByPrice = function(item) {
       if (item.price) {
         return item.price;
       } else {
        return item.each ? item.packageprice : item.caseprice;
       }
    };

    function invalidItemCheck(items) {
      var invalidItemFound = false;
      items.forEach(function(item){
        if (!item.extPrice && !(item.extPrice > 0)){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Please enter a quantity for item ' + item.itemnumber +' before saving or submitting the cart.');
        }
      })
      return invalidItemFound;
   }

    var processingSaveCart = false;

    $scope.saveCart = function(cart) {

     var invalidItemFound =  invalidItemCheck(cart.items);
     if (!processingSaveCart && !invalidItemFound) {
        processingSaveCart = true;
        var updatedCart = angular.copy(cart);
        
        // delete items if quantity is 0 or price is 0
            updatedCart.items = $filter('filter')( updatedCart.items, function(item){ 
          return item.quantity > 0 && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item)); 
        });
          $scope.currentCart.items = updatedCart.items;
          $scope.resetSubmitDisableFlag(true);
          return CartService.updateCart(updatedCart).then(function(savedCart) {
          $scope.currentCart.isRenaming = false;
          $scope.sortBy = null;
          $scope.sortOrder = false;
 
          // clear and reapply all watches on item quantity and each fields
          clearItemWatches();
          $scope.currentCart = savedCart;
          addItemWatches(0);
 
          $scope.cartForm.$setPristine();
          $scope.displayMessage('success', 'Successfully saved cart ' + savedCart.name);
          return savedCart.id;
        }, function() {
          $scope.displayMessage('error', 'Error saving cart ' + updatedCart.name);
        }).finally(function() {
          processingSaveCart = false;
        });
      }
    };   
   
   var processingSubmitOrder = false;

    $scope.submitOrder = function(cart) {

     var invalidItemFound =  invalidItemCheck(cart.items);
     if (!processingSaveCart && !invalidItemFound) {
        processingSubmitOrder = true;        

        if($scope.validateShipDate($scope.selectedShipDate)){
          return;
        }

        $scope.saveCart(cart)
          .then(CartService.submitOrder)
          .then(function(data) {
            $state.go('menu.orderitems', { invoiceNumber: data.ordernumber });
            $scope.displayMessage('success', 'Successfully submitted order.');
          }, function(error) {
            $scope.displayMessage('error', 'Error submitting order.');
          }).finally(function() {
            processingSubmitOrder = false;
          });
      }
    };
 
    $scope.renameCart = function (cartId, cartName) {
      var cart = angular.copy($scope.currentCart);
      cart.name = cartName;
 
      CartService.updateCart(cart).then(function(data) {
        $scope.currentCart.isRenaming = false;
        $scope.currentCart.name = cartName;
        $scope.displayMessage('success', 'Successfully renamed cart to ' + cartName + '.');
      }, function() {
        $scope.displayMessage('error', 'Error renaming cart.');
      });
    };
 
    $scope.createNewCart = function() {
      CartService.renameCart = true;
      CartService.createCart().then(function(newCart) {
        $state.go('menu.cart.items', {cartId: newCart.id});
        $scope.displayMessage('success', 'Successfully created new cart.');
      }, function() {
        $scope.displayMessage('error', 'Error creating new cart.');
      });
    };
 
    $scope.deleteCart = function(cart) {
      CartService.deleteCart(cart.id).then(function() {
        $scope.goToCart();
        $scope.displayMessage('success', 'Successfully deleted cart.');
        $state.go('menu.order');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });
    };
 
    $scope.deleteItem = function(item) {
      var idx = $scope.currentCart.items.indexOf(item);
      $scope.currentCart.items.splice(idx, 1);
      $scope.resetSubmitDisableFlag(true);
      $scope.cartForm.$setDirty();
    };
 
    /************
    CHANGE ORDERS
    ************/
 
    var processingSaveChangeOrder = false;
    $scope.saveChangeOrder = function(order) {
      var invalidItemFound =  invalidItemCheck(order.items);
      if (!processingSaveChangeOrder && !invalidItemFound) {
        processingSaveChangeOrder = true;
 
        var changeOrder = angular.copy(order);
 
        changeOrder.items.forEach(function(item) {
          if (typeof item.quantity === 'string') {
            item.quantity = parseInt(item.quantity, 10);
          }
        });
 
        changeOrder.items = $filter('filter')( changeOrder.items, function(item){ 
          return item.quantity > 0 && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item) || (item.price && PricingService.hasPrice(item.price))); 
        });
 
        return OrderService.updateOrder(changeOrder).then(function(order) {
          $scope.currentCart = order;
          $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);
 
          // recalculate ext price
          $scope.currentCart.items.forEach(function(item) {
            item.extPrice = PricingService.getPriceForItem(item);
          });
 
          $scope.cartForm.$setPristine();
          $scope.displayMessage('success', 'Successfully updated change order.');
          return order.ordernumber;
        }, function(error) {
          $scope.displayMessage('error', 'Error updating change order ' + order.invoicenumber + '.');
        }).finally(function() {
          processingSaveChangeOrder = false;
        });
      }
    };
 
    var processingResubmitOrder = false;
    $scope.resubmitOrder = function(order) {
      var invalidItemFound =  invalidItemCheck(order.items);
      if (!processingSaveChangeOrder && !invalidItemFound) {
        processingResubmitOrder = true;

        if($scope.validateShipDate($scope.selectedShipDate)){
          return;
        }
        
        $scope.saveChangeOrder(order)
          .then(OrderService.resubmitOrder)
          .then(function(invoiceNumber) {
            $scope.displayMessage('success', 'Successfully submitted change order.');
            $state.go('menu.orderitems', { invoiceNumber: invoiceNumber });
          }, function(error) {
            $scope.displayMessage('error', 'Error re-submitting order.');
          }).finally(function() {
            processingResubmitOrder = false;
          });
      }
      
    };
 
    var processingCancelOrder = false;
    $scope.cancelOrder = function(changeOrder) {
      if (!processingCancelOrder) {
        processingCancelOrder = true;
        
        OrderService.cancelOrder(changeOrder.commerceid).then(function() {
          var changeOrderFound = UtilityService.findObjectByField($scope.changeOrders, 'commerceid', changeOrder.commerceid);
          var idx = $scope.changeOrders.indexOf(changeOrderFound);
          $scope.changeOrders.splice(idx, 1);
          $scope.goToCart();
          $scope.displayMessage('success', 'Successfully cancelled order ' + changeOrder.ordernumber + '.');
          $state.go('menu.order');
        }, function(error) {
          $scope.displayMessage('error', 'Error cancelling order ' + changeOrder.ordernumber + '.');
        }).finally(function() {
          processingCancelOrder = false;
        });
      }
    };
 
    // INFINITE SCROLL
    var itemsPerPage = Constants.infiniteScrollPageSize;
    $scope.itemsToDisplay = itemsPerPage;
    $scope.infiniteScrollLoadMore = function() {
      var items = $scope.currentCart.items.length;
 
      if ($scope.currentCart && $scope.itemsToDisplay < items) {
        $scope.itemsToDisplay += itemsPerPage;
      }
    };
 
    $scope.openOrderImportModal = function () {
 
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/orderimportmodal.html',
        controller: 'ImportModalController',
        resolve: {
          customListHeaders: ['ListService', function(ListService) {
            return ListService.getCustomListHeaders();
          }]
        }
      });
    };
 
    /*************************
    REMINDER / MANDATORY ITEMS
    *************************/
 
    $scope.changeAllSelectedItems = function(items, selectAll) {
      angular.forEach(items, function(item, index) {
        item.isSelected = selectAll;
      });
    };
 
    $scope.addSelectedToCart = function(items) {
      if (items && items.length > 0) {
        items = $filter('filter')(items, {isSelected: true});
       
        // set quantity
        items.forEach(function(item) {
          item.quantity = Math.ceil(item.parlevel - item.qtyInCart) || 1;
 
          if ($scope.isChangeOrder) {
            item.price = item.each ? item.packageprice : item.caseprice;
          }
 
        });
 
        // add watches to new items to update price
        var originalItemCount = $scope.currentCart.items.length;
        $scope.currentCart.items = $scope.currentCart.items.concat(items);
        addItemWatches(originalItemCount);
        $scope.resetSubmitDisableFlag(true);
        $scope.cartForm.$setDirty();
        if ($scope.reminderList) {
          $scope.reminderList.allSelected = false;
        }
        if ($scope.mandatoryList) {
          $scope.mandatoryList.allSelected = false;
        }
        $scope.changeAllSelectedItems(items, false);
      }    
    };
 
    // on page load
    // if ($stateParams.renameCart === 'true' && !$scope.isChangeOrder) {
    //   $scope.startEditCartName(originalBasket.name);
    // }
    if (CartService.renameCart === true) {
      console.log('rename cart');
      $scope.startEditCartName(originalBasket.name);
      CartService.renameCart = false;
    }
  }]);