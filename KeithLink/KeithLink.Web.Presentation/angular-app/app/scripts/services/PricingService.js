'use strict';

/**
 * @ngdoc function
 * @name bekApp.service:Pricing
 * @description
 * # Pricing
 * Service of the bekApp
 */
angular.module('bekApp')
  .factory('PricingService', [function() {

  function calculateCatchWeightPrice(item) {
    // Case - (Avg Weight * Qty) * Price
    // Package – ((Avg Weight/Pack) * Qty) * Price

    var averageWeight = item.average_weight ? item.average_weight : 1; // use avg weight = 1 if weight = 0

    if (item.each) {
      // calculate avg weight per pack when buying by the pack
      averageWeight = averageWeight / parseInt(item.pack);
    }

    return averageWeight * calculateQuantityPrice(item);
  }

  function calculateQuantityPrice(item) {
    var unitPrice = 0;

    var quantity = item.quantity ? item.quantity : 0;

    if (item.price) { // check if change order item
      unitPrice = item.price;
    } else {
      // determine if using case price or package price
      unitPrice = Service.getUnitPriceForItem(item);
    }
    return parseFloat(unitPrice) * quantity;
  }

  var Service = {

    getUnitPriceForItem: function(item) {
      return Math.round(parseFloat(item.each ? item.packageprice : item.caseprice) * 100) / 100;
    },
    
    getPriceForItem: function(item) {
      var price = 0;
      if (item) {
        if(item.hasPackagePrice && !item.hasCasePrice){
          item.each = true;
        }
        if (item.catchweight) {
          price = calculateCatchWeightPrice(item);     
        } else {
          price = calculateQuantityPrice(item);
        }
      }
      return price;
    },

    getSubtotalForItems: function(items) {
      var subtotal = 0;
      angular.forEach(items, function(item, index) {
        subtotal += Service.getPriceForItem(item);
      });
      return subtotal;
    },

    getSubtotalForItemsWithPrice: function(itemsWithPrice, fieldName) {
      if (!fieldName) {
        fieldName = 'extPrice';
      }
      var subtotal = 0;
      angular.forEach(itemsWithPrice, function(item, index) {
        subtotal += item[fieldName];
      });
      return subtotal;
    },

    hasPrice: function(price) {
      if (typeof price === 'number') {
        return price > 0;
      } else {
        return price !== '$0.00' && price !== '0' && price !== '0.00';
      }
    },

    hasPackagePrice: function(item) {
      return Service.hasPrice(item.packageprice);
    },

    hasCasePrice: function(item) {
      return Service.hasPrice(item.caseprice);
    },

    canOrderItem: function(item) {
      return Service.hasCasePrice(item) || Service.hasPackagePrice(item) || item.price;
    },

    updateCaculatedFields: function(items) {
      if (items && items.length) {
        items.forEach(function(item) {
          item.canOrder = Service.canOrderItem(item);
          item.hasUnitPrice = Service.hasPrice(item.unitPrice);
          item.hasPackagePrice = Service.hasPackagePrice(item);
          item.hasCasePrice = Service.hasCasePrice(item);
          if(item.each){
            item.each = item.hasPackagePrice;
          }
        });
      }
    }

  };

  return Service;

}]);
