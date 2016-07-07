'use strict';

angular.module('bekApp')
.controller('CartQuickAddModalController', ['$rootScope', '$scope', '$modalInstance', '$filter', 'CartService', 'OrderService', 'cart', 'toaster', 'ProductService', '$q', 'PricingService',
  function ($rootScope, $scope, $modalInstance, $filter, CartService, OrderService, cart, toaster, ProductService, $q, PricingService) {

  var newItems = [];

  $scope.enableSubmit = false;
  $scope.items = [];
  $scope.quickadditems = [];

  if(cart) {
    var origCart = cart;
    $scope.existingCart = true;
  };

  $scope.addRow = function() {
    if(origCart && origCart.items.length && $scope.items.length == 0){
       $scope.isChangeOrder = origCart.hasOwnProperty('ordernumber') ? true : false;
       origCart.items.forEach(function(item){
        $scope.items.push(item)
      });
    }
    $scope.quickadditems.push({
      itemnumber: '',
      quantity: 0,
      each: false
    });
  };

  $scope.removeRow = function(item) {
    var idx = $scope.quickadditems.indexOf(item);
    $scope.quickadditems.splice(idx, 1);
  };

  function getRowsWithQuantity(items) {
    return $filter('filter')( items, function(item) {
      return item.quantity > 0 && item.itemnumber && item.itemnumber.length === 6; 
    });
  };

  $scope.validateItems = function(items) {
    var invalidItemsExist = false;
    $scope.validationItems = getRowsWithQuantity(items);

    if ($scope.validationItems.length > 0) {
      CartService.validateQuickAdd($scope.validationItems).then(function(validatedItems) {
        items.forEach(function(item) {
          var validatedItem = [];
          validatedItems.forEach(function(valItem, index) {     
            if(item.itemnumber === valItem.itemnumber){
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
      });
    } else {
      $scope.enableSubmit = false;
    }
  };

  $scope.createCart = function(items) {
    // filter items where quantity is greater than 0 and item number is valid
    var newItems = getRowsWithQuantity(items);
    CartService.quickAdd(newItems).then(function(cartId) {
      $modalInstance.close(cartId);
      $scope.displayMessage('success', 'Successfully created new cart.');
    }, function(error) {
      $scope.displayMessage('error', error);
    });
  };

  $scope.updateCart = function(items){
    var promises = [];
    var catalogType;
    $scope.validateItems(items);
    angular.forEach(items, function(item){
      if(item.itemnumber !== ''){
        var deferred = $q.defer();
        if(!item.is_specialty_catalog){
          catalogType = "BEK"
        } else {
          catalogType = "UNFI"
        }
        ProductService.getProductDetails(item.itemnumber, catalogType).then(function(product){
          product.quantity = item.quantity;
          product.each = item.each;
          product.reason = item.reason;
          if(product.reason == 'Each not allowed for this item'){
            product.isvalid = false;
            return;
          } else {
            product.isvalid = true;
          }
          if(origCart && origCart.items){
            var i = 0;
            var origproduct = $filter('filter')(origCart.items, {itemnumber: product.itemnumber});
              if(origproduct.length && (origproduct[0].each === item.each)){
                if(origproduct[0].quantity){
                  origproduct[0].quantity = parseInt(origproduct[0].quantity)
                  item.quantity = parseInt(item.quantity)
                  product.quantity = origproduct[0].quantity + item.quantity;
                }
              } else if (i < 1 && (origproduct.length || !origproduct.length)) {
                i++;
                product.quantity = parseInt(product.quantity);
                product.extPrice = PricingService.getPriceForItem(product);
                newItems.push(product);
              } else {
                return false;
              }
              uniqueCartItems(origCart.items);
                $scope.distinctCartItems.forEach(function(origItem){
                if(origItem.itemnumber === product.itemnumber && origItem.each === product.each){
                  return origItem.quantity = parseInt(product.quantity);
                }
              })
            i = 0;
          } else {
            origCart.items.push(product);
          }
          deferred.resolve(product);
        })
        promises.push(deferred.promise);
      } else {
        return false;
      }
    })

    $q.all(promises).then(function(){
      $rootScope.$broadcast('QuickAddUpdate', origCart.items, newItems);
      $scope.quickadditems = [];
      newItems = [];
      $scope.quickAddForm.$setPristine();
      $scope.addRow();
    })
  };

  function uniqueCartItems(origCartItems){
    var isUnique = {};
    $scope.distinctCartItems = [];
    for( var i in origCartItems ){
      if( typeof(isUnique[origCartItems[i].itemnumber]) == "undefined"){
        $scope.distinctCartItems.push(origCartItems[i]);
      }
      isUnique[origCartItems[i].itemnumber] = 0;
    }
    return $scope.distinctCartItems;
  }

  $scope.updateOrder = function(items){
    $scope.validateItems(items);
    origCart.items = getRowsWithQuantity(items)
    OrderService.updateFromQuickAdd(origCart).then(function(){
      $scope.displayMessage('success', 'Successfully added new items to order.');
    }, function(error) {
      $scope.displayMessage('error', 'Error saving cart. Please try saving again.');
    });
  };

  $scope.close = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.addRow();

}]);