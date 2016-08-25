'use strict';

angular.module('bekApp')
.controller('CartQuickAddModalController', ['$rootScope', '$scope', '$modalInstance', '$filter', 'CartService', 'OrderService', 'cart', 'toaster', 'ProductService', '$q', 'PricingService',
  function ($rootScope, $scope, $modalInstance, $filter, CartService, OrderService, cart, toaster, ProductService, $q, PricingService) {

  var newItems = [];

  $scope.enableSubmit = false;
  $scope.quickAddItems = [];

  if(cart) {
    $scope.existingCart = true;
    $scope.isChangeOrder = cart.hasOwnProperty('ordernumber') ? true : false;
  };

  $scope.addRow = function() {
    $scope.quickAddItems.push({
      itemnumber: '',
      quantity: 0,
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

  $scope.createCart = function(items) {
    // filter items where quantity is greater than 0 and item number is valid
    var newCartItems = getRowsWithQuantity(items);

    $scope.validateItems(newCartItems).then(function(validatedItems){
      if($scope.enableSubmit){
        CartService.quickAdd(newCartItems).then(function(cartId) {
          $modalInstance.close(cartId);
          $scope.displayMessage('success', 'Successfully created new cart.');
        }, function(error) {
          $scope.displayMessage('error', error);
        });
      }
    });

  };

  $scope.updateCart = function(items){
    $scope.validateItems(items).then(function(validatedItems){
      if($scope.enableSubmit){
        angular.forEach(validatedItems, function(item){
          if(item.product && item.valid){
            var newItem = true;

            if(cart && cart.items){
              cart.items.forEach(function(origItem){
                //combine quantity into first instance of duplicate item in existing cart items
                if(origItem.itemnumber === item.product.itemnumber && origItem.each === item.product.each && newItem){
                  origItem.quantity = origItem.quantity + item.product.quantity;
                  item.product.quantity = 0;
                  newItem = false;
                }
              });       
            }
            if(newItem){
              item.product.quantity = item.item.quantity;
              item.product.each = item.item.each;
              newItems.push(item.product);
            }          
          }
        });
      }  
      if($scope.enableSubmit){
        $rootScope.$broadcast('QuickAddUpdate', cart.items, newItems);
        $modalInstance.dismiss();
      }   
    });
  };

  $scope.close = function () {
    $modalInstance.dismiss('cancel');
  };

  $scope.addRow();

}]);