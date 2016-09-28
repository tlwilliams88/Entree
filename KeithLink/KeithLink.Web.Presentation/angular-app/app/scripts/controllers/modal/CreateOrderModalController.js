'use strict';

angular.module('bekApp')
.controller('CreateOrderModalController', ['$scope', '$modalInstance', '$q', '$filter', '$analytics', 'CartService', 'ListService', 'LocalStorage', 'UtilityService', 'SessionService',
  function ($scope, $modalInstance, $q, $filter, $analytics, CartService, ListService, LocalStorage, UtilityService, SessionService) {

  //FOR LIST
  $scope.currentCustomer = LocalStorage.getCurrentCustomer();

  //FOR QUICK ADD
  $scope.enableSubmit = false;
  $scope.quickAddItems = [{
    itemnumber: '',
    quantity: '',
    each: false
  }];

  //FOR IMPORT
  $scope.upload = [];
  $scope.files = [];
  $scope.invalidType = false;    

  /*******************
    DEFAULT DATA
  *******************/

  ListService.getListHeaders().then(function(resp){
    $scope.lists = resp;
  })

  CartService.getShipDates().then(function(resp){
    $scope.shipDates = resp;
    $scope.selectedCart.requestedshipdate = $scope.shipDates[0].shipdate;
  })

  CartService.getCartHeaders().then(function(cartHeaders){
    $scope.cartHeaders = cartHeaders;
  });

  ListService.getCustomListHeaders().then(function(listHeaders){
    $scope.customerListHeaders = listHeaders;
  })

  $scope.selectedCart = {
    name: UtilityService.generateName(SessionService.userProfile.firstname, $scope.cartHeaders),
    requestedshipdate: '',
    ponumber: ''
  };
  $scope.selectedList = {
    name: 'History',
    listid: ''
  };

  /*******************
    CREATE FROM LIST
  ********************/

  $scope.createCart = function(cart, fromFunction) {

    CartService.createCart(cart.items, cart.requestedshipdate, cart.name, cart.ponumber).then(function(cart) {
      if(fromFunction == 'QuickAdd'){
        cart.type = 'QuickAdd';
      }
      cart.listid = $scope.selectedList.listid;
      $modalInstance.close(cart);
      $scope.displayMessage('success', 'Successfully created new cart.');
    }, function(error) {
      $scope.displayMessage('error', error);
    });

  };

  /***********************
    CREATE FROM QUICK ADD
  ************************/

  var newItems = [];

  $scope.addRow = function() {

    $scope.quickAddItems.push({
      itemnumber: '',
      quantity: '',
      each: false
    });

    $scope.enableSubmit = false;
  };

  $scope.removeRow = function(item) {
    var idx = $scope.quickAddItems.indexOf(item);
    $scope.quickAddItems.splice(idx, 1);
  };

  function getRowsWithQuantity(items) {
    return $filter('filter')( items, function(item) {
      return item.quantity > 0 && item.itemnumber && item.itemnumber.length > 0; 
    });
  };

  $scope.validateItems = function(items) {
    $scope.isValidating = true;
    var invalidItemsExist = false;
    var deferred =  $q.defer();
    var validationItems = getRowsWithQuantity(items);

    if (validationItems.length > 0) {
      return CartService.validateQuickAdd(validationItems).then(function(validatedItems) {
        $scope.isValidating = false;
        if(validatedItems){
          //assign validity and reasons
          items.forEach(function(item) {
            var validatedItem = [];
            validatedItems.forEach(function(valItem, index) {
              if(item.itemnumber === valItem.item.itemnumber && item.each === valItem.item.each){
                validatedItem = validatedItems[index];
                item.valid = validatedItem.valid;
              }
            });          

            if (validatedItem.valid === false) {
              invalidItemsExist = true;
            }

            if (validatedItem.reason === 0) {
              item.reason = 'Invalid item number';
            } else if (validatedItem.reason === 1) {
              item.reason = 'Each not allowed for this item';
            }
          });
          $scope.enableSubmit = !invalidItemsExist;        
          return validatedItems;
        }
        else{
          return [];
        }
      });
    } else {    
      $scope.enableSubmit = false;
      deferred.resolve([]);
      $scope.isValidating = false;
      return deferred.promise;
    }
  };

  $scope.validateItemsAndUpdateCart = function(items){
    // filter items where quantity is greater than 0 and item number is valid
    var newCartItems = getRowsWithQuantity(items);
    $scope.validateItems(newCartItems).then(function(cartItems){
      $scope.selectedCart.items = [];
      cartItems.forEach(function(item){
        item.product.quantity = item.item.quantity;
        item.product.each = item.item.each;
        $scope.selectedCart.items.push(item.product);
      })
    });
  }

  $scope.createCartFromQuickAdd = function(items) {

    if($scope.enableSubmit){
      $scope.createCart($scope.selectedCart, 'QuickAdd');
    }

  };

  /*********************
    CREATE FROM IMPORT
  *********************/  

  $scope.onFileSelect = function($files) {
     if(!$files.length){
      return;
     }
    $scope.files = [];
    var filetype = $files[0].name.slice($files[0].name.length -5,$files[0].name.length);
    filetype = filetype.slice(filetype.indexOf('.'), filetype.length);
   $scope.invalidType = (filetype !== '.xlsx' && filetype !== '.xls' && filetype !== '.csv' && filetype !== '.txt');             

    for (var i = 0; i < $files.length; i++) {     
      $scope.files.push($files[i]);      
    }
  };

  $scope.startOrderUpload = function(options) {
    var file = $scope.files[0];
    $analytics.eventTrack('Import Order', {category: 'Orders'});
    CartService.importCart(file, options, $scope.selectedCart).then(function(data) {
      data.type = 'Import';
      $modalInstance.close(data);
    });
  };

  $scope.close = function () {
    $modalInstance.dismiss('cancel');
  };

}]);