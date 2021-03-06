'use strict';

/**
 * @ngdoc function
 * @name bekApp.controller:CartController
 * @description
 * # CartController
 * Controller of the bekApp
 */
angular.module('bekApp')
  .controller('CartItemsController', ['$scope', '$state', '$stateParams', '$filter', '$modal', '$q', 'ENV', 'Constants', 'LocalStorage',
   'CartService', 'OrderService', 'UtilityService', 'PricingService', 'DateService', 'changeOrders', 'originalBasket', 'criticalItemsLists', 'AnalyticsService', 'ProductService', '$timeout', 'SessionRecordingService',
    function($scope, $state, $stateParams, $filter, $modal, $q, ENV, Constants, LocalStorage, CartService, OrderService, UtilityService,
     PricingService, DateService, changeOrders, originalBasket, criticalItemsLists, AnalyticsService, ProductService, $timeout, SessionRecordingService) {

    // redirect to url with correct ID as a param
    var basketId = originalBasket.id || originalBasket.ordernumber;
    if ($stateParams.cartId !== basketId.toString()) {
      $state.go('menu.cart.items', {cartId: basketId}, {location:'replace', inherit:false, notify: false});
    }

    var savedCartAfterOffline = false;
    $scope.$on('Online', function(){
      if(savedCartAfterOffline == false) {
          $scope.saveCart($scope.currentCart);
          savedCartAfterOffline = true;
      }
    })

    // update cartHeaders in MenuController
    $scope.$parent.$parent.cartHeaders = CartService.cartHeaders;

    $scope.showRecommendedItems = ENV.showRecommendedItems;
    $scope.isMobileDevice = UtilityService.isMobileDevice();

    var watches = [];
    function onQuantityChange(newVal, oldVal) {
      var changedExpression = this.exp; // jshint ignore:line
      var idx = changedExpression.substr(changedExpression.indexOf('[') + 1, changedExpression.indexOf(']') - changedExpression.indexOf('[') - 1);
      var item = $scope.currentCart.items[idx];
      if (item) {
        item.extPrice = PricingService.getPriceForItem(item);

        calculatePieces($scope.currentCart.items);
      }

      $scope.currentCart.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.currentCart.items);

      validateCart();
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

    $scope.cartSubmissionApproved = false;
    $scope.sortBy = 'createddate'; // sort items in the order they were added to the cart
    $scope.sortOrder = false;
    CartService.updateNetworkStatus();
    $scope.isOffline = CartService.isOffline;
    $scope.cartContainsSpecialItems = CartService.cartContainsSpecialItems;
    $scope.carts = CartService.cartHeaders;
    $scope.shipDates = CartService.shipDates;
    $scope.changeOrders = OrderService.changeOrderHeaders;
    $scope.isChangeOrder = originalBasket.hasOwnProperty('ordernumber') ? true : false;
    if($scope.isChangeOrder){
      originalBasket.items =  OrderService.filterDeletedOrderItems(originalBasket);
    }
    $scope.currentCart = angular.copy(originalBasket);
    updateRecommendedItems();
    $scope.currentCart.isRenaming = false;
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

    OrderService.getRecentlyOrderedUNFIItems().then(function(recentlyOrdered){
      if(recentlyOrdered){
        $scope.recentlyOrderedUnfiItems = recentlyOrdered.items;
      }
      else{
        $scope.recentlyOrderedUnfiItems = [];
      }
    });

    if (!$scope.isChangeOrder) {
      CartService.setActiveCart($scope.currentCart.id);
    }

    addItemWatches(0);

    // set default selected critical items list
    criticalItemsLists.forEach(function(list) {
      if (list.ismandatory) {
        $scope.mandatoryList = list;

        $scope.mandatoryList.items.forEach(function(item) {
          var idx = $scope.currentCart.items.indexOf(item);

          if(idx > -1) {
            item.qtyInCart = $scope.currentCart.items[idx].quantity;
          }
        })
      } else if (list.isreminder) {
        $scope.reminderList = list;

        $scope.reminderList.items.forEach(function(item) {
          var idx = $scope.currentCart.items.indexOf(item);

          if(idx > -1) {
            item.qtyInCart = $scope.currentCart.items[idx].quantity;
          }
        })
      }
    });

    // set mandatory and reminder lists
    // add property isMandatory for carts items that are on the mandatory list
    function setMandatoryAndReminder(cart){
      if ($scope.mandatoryList) {
        $scope.mandatoryList.active = true;
        if($scope.mandatoryList.items && $scope.mandatoryList.items.length > 0){
          cart.items.forEach(function(item){
            if($filter('filter')($scope.mandatoryList.items, {itemnumber: item.itemnumber}).length>0){
              item.isMandatory = true;
            }
          });
        }
      } else if ($scope.reminderList) {
        $scope.reminderList.active = true;
      } else {
        $scope.mandatoryList = {};
        $scope.reminderList = {};
      }
    }
    setMandatoryAndReminder($scope.currentCart);

    function updateRecommendedItems(){
      if($scope.showRecommendedItems == true) {
        $timeout(function() {
          var pagesize = ENV.isMobileApp == 'true' ? Constants.recommendedItemParameters.Mobile.pagesize : Constants.recommendedItemParameters.Desktop.Cart.pagesize,
          getimages = ENV.isMobileApp == 'true' ? Constants.recommendedItemParameters.Mobile.getimages : Constants.recommendedItemParameters.Desktop.getimages;
          ProductService.getRecommendedItems($scope.currentCart.items, pagesize, getimages).then(function(resp) {
            $scope.recommendedItems = resp;
          });
        }, 30);

      }
    }

    $scope.resetSubmitDisableFlag = function(checkForm){
      $scope.disableSubmitButtons = ((!$scope.currentCart.items || $scope.currentCart.items.length === 0) || $scope.isOffline || $scope.invalidSelectedDate || $scope.processingSubmitOrder );
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

    $scope.scrollToTop = function($var) {
      $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').css({'display': 'inline'});
      var duration = 300;
      event.preventDefault();
      jQuery('html, body').animate({scrollTop: 0}, duration);
      return false;
    };
  
    $(window).scroll(function() {
      if($(this).scrollTop() > 190){
        $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').fadeIn('fast');
        $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').css('visibility', 'visible');
      } else {
        $('.back-to-top, .back-to-top-desktop, .floating-save-mobile').fadeOut('fast');
      }
    });

    $scope.cancelChanges = function() {
      var originalCart = angular.copy(originalBasket);
      originalCart.items.forEach(function(item) {
        item.extPrice = PricingService.getPriceForItem(item);
      });
      originalCart.subtotal = PricingService.getSubtotalForItemsWithPrice(originalCart.items);
      setMandatoryAndReminder(originalCart);
      $scope.currentCart = originalCart;
      updateRecommendedItems();
      $scope.resetSubmitDisableFlag(true);
      $scope.cartForm.$setPristine();
    };

    $scope.startEditCartName = function(cartName) {
      $scope.editCart = {};
      $scope.editCart.name = angular.copy(cartName);
      $scope.currentCart.isRenaming = true;
    };

    $scope.validateShipDate = function(shipDate){
      var cutoffDate = DateService.momentObject(shipDate.cutoffdatetime,'').format();
      var now = DateService.momentObject().tz('America/Chicago').format();

      $scope.invalidSelectedDate = (now > cutoffDate) ? true : false;
      if($scope.invalidSelectedDate){
        CartService.getShipDates().then(function(result){
          $scope.shipDates = result;
        });
      }
      $scope.resetSubmitDisableFlag(true);
      return $scope.invalidSelectedDate;
    };

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

    $scope.sortByPrice = function(item) {
       if (item.price) {
         return item.price;
       } else {
        return item.each ? item.packageprice : item.caseprice;
       }
    };

    var invalidItemFound = false;
    var processingSaveCart = false;

    function invalidItemCheck(items) {
      var invalidItemFound = false;
      items.forEach(function(item){
        if (!item.extPrice && !(item.extPrice > 0) && !item.isMandatory && (item.status && item.status.toUpperCase() !== 'OUT OF STOCK') && (item.status && item.status.toUpperCase() !== 'DELETED')){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Please delete or enter a quantity for item ' + item.itemnumber +' before saving or submitting the cart.');
        } else if(item.isMandatory && item.status && item.quantity == 0 && $scope.isChangeOrder && (item.status && item.status.toUpperCase() !== 'OUT OF STOCK' && item.status.toUpperCase() !== 'DELETED')){
          invalidItemFound = true;
          $scope.displayMessage('error', 'Please enter a quantity for item ' + item.itemnumber +' before saving or submitting the cart.');
        }
      });
      return invalidItemFound;
   }

   $scope.addItemToCart = function(item) {
    item.quantity = item.newQuantity;

    delete item.newQuantity;

    $scope.currentCart.items.push(item);

    updateRecommendedItems();

    if($scope.isChangeOrder == true) {
      $scope.saveChangeOrder($scope.currentCart)
    } else {
      $scope.saveCart($scope.currentCart);
    }

   };

    processingSaveCart = false;

    $scope.saveCart = function(cart) {

     var invalidItemFound =  invalidItemCheck(cart.items);
     if (!processingSaveCart && !invalidItemFound) {
        processingSaveCart = true;
        var updatedCart = angular.copy(cart);

        // delete items if quantity is 0 or price is 0
          updatedCart.items = $filter('filter')( updatedCart.items, function(item){
            return (item.quantity > 0 || (item.quantity === 0 && item.status && item.status.toUpperCase() === 'OUT OF STOCK')) && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item) || (item.price && PricingService.hasPrice(item.price)));
          });
          $scope.currentCart.items = updatedCart.items;
          $scope.currentCart.items = $filter('filter')($scope.currentCart.items, {status: '!Deleted'});
          if(CartService.cartHeaders.length == 0) {
            CartService.getCartHeaders();
          }
          var currentCartHeaders = $filter('filter')(CartService.cartHeaders, {id: updatedCart.id});

          currentCartHeaders[0].items.forEach(function(cartItem) {
            updatedCart.items.forEach(function(item) {
              if(item.itemnumber == cartItem.itemnumber && item.each == cartItem.each) {
                item.cartitemid = cartItem.cartitemid;
              }
            })
          })

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

    $scope.submitOrder = function (cart)
    {
      var invalidItemFound =  invalidItemCheck(cart.items);
      var invalidSelectedDate = $scope.validateShipDate($scope.selectedShipDate)

      if (!processingSaveCart && !invalidItemFound && !invalidSelectedDate)
      {
        processingSubmitOrder = true;
        $scope.disableSubmitButtons = true;

        CartService.isSubmitted(cart.id)
          .then(function (hasBeenSubmitted)
          {
            if (!hasBeenSubmitted && (cart.approval.approved == true || $scope.cartSubmissionApproved == true))
            {
              processingSaveCart = false;
              $scope.saveCart(cart)
                .then(CartService.submitOrder)
                .then(function (data) { processSubmitSuccess(data, cart); }, processSubmitError);
            }
          })
          .finally(function ()
          {
            processingSubmitOrder = false;
            $scope.disableSubmitButtons = false;
          });
      }
    };

      function processSubmitSuccess(data, cart)
      {
        $scope.setRecentlyOrderedUNFIItems(cart);
        var orderNumber = -1;
        var index;
        if (data && data.ordersReturned)
        {
          for (index in data.ordersReturned)
          {
            if (data.ordersReturned[index].catalogType === Constants.catalogType.BEK)
            {
              orderNumber = data.ordersReturned[index].ordernumber;
            }
          }
        }

        var status = '';
        var message = '';

        if (data && data.ordersReturned)
        {
          if (orderNumber === -1)
          {
            //no BEK items bought
            if (data.ordersReturned && data.ordersReturned.length && data.ordersReturned.length !== data.numberOfOrders)
            {
              status = 'error';
              message = 'One or more catalog orders failed. Please contact your DSR representative for assistance';
            } else
            {
              status = 'success';
              message = 'Successfully submitted order.';
            }

            if (data.ordersReturned && data.ordersReturned[0] !== null)
            {
              orderNumber = data.ordersReturned[0].ordernumber;
            } else
            {
              orderNumber = null;
            }
          } else
          {
            //BEK oderNumber exists
            if (data.ordersReturned.length !== data.numberOfOrders)
            {
              status = 'error';
              message = 'We are unable to fulfill your special order items. Please contact your DSR representative for assistance';
            } else
            {
              status = 'success';
              message = 'Successfully submitted order.';
            }
          }
        }

        var customerName = $scope.selectedUserContext.customer.customerName;

        AnalyticsService.recordCheckout(cart,
          Constants.checkoutSteps.SubmitCart, // step
          ""); //option

        AnalyticsService.recordTransaction(orderNumber,
          cart,
          $scope.selectedUserContext.customer.customerNumber,
          $scope.selectedUserContext.customer.customerBranch);

        SessionRecordingService.tagOrder(orderNumber);

        $state.go('menu.orderitems', { invoiceNumber: orderNumber });
        $scope.displayMessage(status, message);
      }

      function processSubmitError(error)
      {
        $scope.displayMessage('error', 'Error submitting order.');
      }

    $scope.setRecentlyOrderedUNFIItems = function(cart){
      var itemsAdded = false;
      if(cart.items && cart.items.length > 0){
        var unfiItems = $filter('filter')(cart.items, {is_specialty_catalog: true});
        if(unfiItems.length > 0){
          unfiItems.forEach(function(unfiItem){
            if($filter('filter')($scope.recentlyOrderedUnfiItems, {itemnumber: unfiItem.itemnumber}).length === 0){
              $scope.recentlyOrderedUnfiItems.unshift(unfiItem);
              itemsAdded = true;
            }
          });
          if(itemsAdded){
            OrderService.UpdateRecentlyOrderedUNFIItems($scope.recentlyOrderedUnfiItems);
          }
        }
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
        $scope.displayMessage('success', 'Successfully deleted cart.');
        $state.go('menu.order');
      }, function() {
        $scope.displayMessage('error', 'Error deleting cart.');
      });
    };

    $scope.deleteItem = function(item) {
        AnalyticsService.recordRemoveItem(
          item,
          LocalStorage.getCustomerNumber(),
          LocalStorage.getBranchId());

        var idx = $scope.currentCart.items.indexOf(item);
        $scope.currentCart.items.splice(idx, 1);
        calculatePieces($scope.currentCart.items);
        updateRecommendedItems();
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
          return (item.quantity > 0 || (item.quantity === 0 && item.status && item.status.toUpperCase() === 'OUT OF STOCK')) && (PricingService.hasPackagePrice(item) || PricingService.hasCasePrice(item) || (item.price && PricingService.hasPrice(item.price)));
        });

        return OrderService.updateOrder(changeOrder).then(function(order) {
          $scope.currentCart = order;
          $scope.selectedShipDate = CartService.findCutoffDate($scope.currentCart);

          // recalculate ext price
          $scope.currentCart.items.forEach(function(item) {
            item.extPrice = PricingService.getPriceForItem(item);
          });
          $scope.cartForm.$setPristine();
          $scope.currentCart.subtotal = PricingService.getSubtotalForItemsWithPrice($scope.currentCart.items);
          setMandatoryAndReminder($scope.currentCart);
          $scope.displayMessage('success', 'Successfully updated change order.');
          return order.ordernumber;
        }, function(error) {
          $scope.displayMessage('error', 'Error updating change order ' + order.invoicenumber + '.');
        }).finally(function() {
          processingSaveChangeOrder = false;
        });
      }

      SessionRecordingService.tagChangeOrder(order.orderNumber);

    };

    var processingResubmitOrder = false;
    $scope.resubmitOrder = function(order) {
      var invalidItemFound =  invalidItemCheck(order.items);

      if (!processingSaveChangeOrder && !invalidItemFound) {
        processingResubmitOrder = true;

        if($scope.validateShipDate($scope.selectedShipDate)){
          return;
        }

        //OrderService.isSubmitted(order.ordernumber).then(function(hasBeenSubmitted){
          //if(!hasBeenSubmitted){
            $scope.saveChangeOrder(order)
            .then(OrderService.resubmitOrder)
            .then(function(invoiceNumber) {
              $scope.setRecentlyOrderedUNFIItems(order);
              $scope.displayMessage('success', 'Successfully submitted change order.');

              AnalyticsService.recordCheckout(order, 
                                              Constants.checkoutSteps.SubmitChangeOrder, // step
                                              ''); //option

              $state.go('menu.orderitems', { invoiceNumber: invoiceNumber });
            }, function(error) {
              $scope.displayMessage('error', 'Error re-submitting order.');
            }).finally(function() {
              processingResubmitOrder = false;
            });
          //}
        //});
      }
    };

    var processingCancelOrder = false;
    $scope.cancelOrder = function(changeOrder) {
      if (!processingCancelOrder) {
        processingCancelOrder = true;

        OrderService.cancelOrder(changeOrder.commerceid).then(function() {
          var changeOrderFound = UtilityService.findObjectByField($scope.changeOrders, 'commerceid', changeOrder.commerceid);          var idx = $scope.changeOrders.indexOf(changeOrderFound);
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

    $scope.openCartExportModal = function(cartid) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/exportmodal.html',
        controller: 'ExportModalController',
        resolve: {
          location: function() {
            return {category:'Cart', action:'Export Cart'};
          },
          headerText: function () {
            return 'Orders';
          },
          exportMethod: function() {
            return CartService.exportCart;
          },
          exportConfig: function() {
            return CartService.getCartExportConfig(cartid);
          },
          exportParams: function() {
            return cartid;
          },
          exportType: function() {
             return Constants.exportType.cartItemsExport;
          }
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

      $scope.saveCart($scope.currentCart);
      
      updateRecommendedItems();
    };

    function calculatePieces(items){
      //total piece count for cart info box

      $scope.itemCount = $filter('filter')($scope.currentCart.items, {quantity: '!0'})
      $scope.piecesCount = 0;
        items.forEach(function(item){
          if(item.quantity){
            $scope.piecesCount = $scope.piecesCount + parseInt(item.quantity);
          }
        });
    }

    function validateCart() {
      $scope.cartSubmissionApproved = $scope.currentCart.subtotal > 0 ? $scope.currentCart.approval.approvedamount <= $scope.currentCart.subtotal : false;
      $scope.currentCart.approval.remainingamount = $scope.currentCart.approval.approvedamount - $scope.currentCart.subtotal;

      if($scope.cartSubmissionApproved == false && $scope.currentCart.approval.message == null) {
        $scope.currentCart.approval.message = "The cart total does not meet or exceed the minimum approved amount.  Please contact your DSR for more information.";
      }
    }

    $scope.openErrorMessageModal = function(message) {
      var modalInstance = $modal.open({
        templateUrl: 'views/modals/errormessagemodal.html',
        controller: 'ErrorMessageModalController',
        scope: $scope,
        backdrop:'static',
        resolve: {
          message: function() {
            return message;
          }
          }
      });

      modalInstance.result.then(function(resp) {
        if(resp){
          $scope.selectShipDate($scope.shipDates[0]);
          if($scope.isChangeOrder){
            $scope.saveChangeOrder($scope.currentCart);
          }
          else{
            $scope.saveCart($scope.currentCart);
          }
        }
        else{
          $state.go('menu.order');
        }
      });
    };

      if($scope.currentCart && !$scope.currentCart.requestedshipdate){
          $scope.selectShipDate($scope.shipDates[0]);
      }else{
            if(DateService.momentObject($scope.currentCart.requestedshipdate.slice(0,10),'') < DateService.momentObject($scope.shipDates[0].shipdate,'')) {
            $scope.openErrorMessageModal('The ship date requested for this order has expired. Select Cancel to return to the home screen without making changes. Select Accept to update to the next available ship date.');
          }
      }

    // on page load
    // if ($stateParams.renameCart === 'true' && !$scope.isChangeOrder) {
    //   $scope.startEditCartName(originalBasket.name);
    // }
    if (CartService.renameCart === true) {
      // console.log('rename cart');
      $scope.startEditCartName(originalBasket.name);
      CartService.renameCart = false;
    }

    AnalyticsService.recordCheckout($scope.currentCart, 
                                    Constants.checkoutSteps.ViewCart, // step
                                    ""); //option

  }]);

