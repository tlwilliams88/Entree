'use strict';

angular.module('bekApp')
.controller('CreateOrderModalController', ['$scope', '$modalInstance', '$q', '$filter', '$analytics', 'CartService', 'ListService', 'LocalStorage', 'UtilityService', 'SessionService','CurrentCustomer', 'ShipDates', 'CartHeaders', 'Lists', 'CustomListHeaders', 'IsMobile', 'IsOffline', 'SelectedList', 'ApplicationSettingsService',
  function ($scope, $modalInstance, $q, $filter, $analytics, CartService, ListService, LocalStorage, UtilityService, SessionService, CurrentCustomer, ShipDates, CartHeaders, Lists, CustomListHeaders, IsMobile, IsOffline, SelectedList, ApplicationSettingsService) {

  /*******************
    DEFAULT DATA
  *******************/
  $scope.currentCustomer = CurrentCustomer;
  $scope.shipDates = ShipDates;
  $scope.lists = Lists;
  $scope.cartHeaders = CartHeaders;
  $scope.customListHeaders = CustomListHeaders;
  $scope.listIsOpen = true;
  $scope.quickAddIsOpen = false; 
  $scope.importIsOpen = false;
  $scope.isOffline = IsOffline;
  
  $scope.selectedCart = {
    name: UtilityService.generateName(SessionService.userProfile.firstname, $scope.cartHeaders),
    ponumber: '',
    requestedshipdate: $scope.shipDates[0].shipdate
  };

  if(SelectedList && SelectedList.value){
    $scope.selectedList = $filter('filter')($scope.lists, {listid: SelectedList.value})[0];
    $scope.defaultList = $scope.selectedList.listid;
  } else {
    $scope.selectedList = $filter('filter')($scope.lists, {name: 'History'})[0];
  }

  $scope.isMobile = IsMobile;

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

  $scope.toggleOpenTab = function(tab) {
    if(tab == 'List'){
      $scope.listIsOpen = true; 
      $scope.quickAddIsOpen = false; 
      $scope.importIsOpen = false;
    } else if(tab == 'Quick Add'){
      $scope.quickAddIsOpen = true; 
      $scope.listIsOpen = false; 
      $scope.importIsOpen = false;
    } else {
      $scope.importIsOpen = true; 
      $scope.listIsOpen = false; 
      $scope.quickAddIsOpen = false;
    }
  };

  /*******************
    CREATE FROM LIST
  ********************/

  $scope.createCart = function(cart, fromFunction) {
    if($scope.defaultList && $('.defaultCheckbox')[0] && $('.defaultCheckbox')[0].checked && $scope.listIsOpen){
      ApplicationSettingsService.setDefaultOrderList($scope.selectedList.listid);
    }

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

  $scope.setSelectedList = function(list) {
    $scope.selectedList = list;
  };

  $scope.setDefaultList = function() {
    $scope.defaultList = $scope.selectedList.listid;
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
  }

  $scope.validateItems = function(items) {
    $scope.isValidating = true;
    $scope.duplicateItemsExist = false;
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
                if(validatedItem.product){
                  item.name = validatedItem.product.name;
                  item.brand = validatedItem.product.brand_extended_description;
                  item.pack = validatedItem.product.pack;
                  item.size =  validatedItem.product.size;
                  item.packageprice = validatedItem.product.packageprice;
                  item.caseprice = validatedItem.product.caseprice;
                }
              }
              if(validatedItem && validatedItem.product){
                $scope.duplicateItems = $filter('filter')(items, {itemnumber:validatedItem.product.itemnumber, each:validatedItem.product.each});
                if($scope.duplicateItems.length > 1){
                  $scope.duplicateItemsExist = true;
                  validatedItem.reason = 2;
                  validatedItem.valid = false;
                }
              }

            });

            if (validatedItem.valid === false) {
              invalidItemsExist = true;
            } 

            if (validatedItem.reason === 0) {
              item.reason = 'Invalid item number';
            } else if (validatedItem.reason === 1) {
              item.reason = 'Each not allowed for this item';
            } else {
              item.valid = false;
              item.reason = 'Duplicate';
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
        if(item.product){
          item.product.quantity = item.item.quantity;
          item.product.each = item.item.each;
          $scope.selectedCart.items.push(item.product);
        }
      });
    });
  };

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