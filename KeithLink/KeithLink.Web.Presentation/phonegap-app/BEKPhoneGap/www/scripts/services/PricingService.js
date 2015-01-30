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

    var averageWeight = item.average_weight ? item.item.average_weight : 1; // use avg weight = 1 if weight = 0

    if (item.each) {
      // calculate avg weight per pack when buying by the pack
      averageWeight = averageWeight / parseInt(item.pack);
    }

    return averageWeight * calculateQuantityPrice(item);
  }

  function calculateQuantityPrice(item) {
    var unitPrice = 0;

    if (item.price) { // check if change order item
      unitPrice = item.price;
    } else {
      // determine if using case price or package price
      unitPrice = item.each ? item.packageprice : item.caseprice;
    }

    return unitPrice * item.quantity;
  }

  var Service = {
    
    getPriceForItem: function(item) {
      var price = 0;
      
      if (item.catchweight) {
        price = calculateCatchWeightPrice(item);     
      } else {
        price = calculateQuantityPrice(item);
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

    hasPackagePrice: function(item) {
      return (item.packageprice > 0 || (item.packageprice !== '$0.00' && item.packageprice !== '0'));
    },

    hasCasePrice: function(item) {
      return (item.caseprice > 0 || (item.caseprice !== '$0.00' && item.caseprice !== '0'));
    },

    canOrderItem: function(item) {
      return Service.hasCasePrice(item);
    }

  };

  return Service;

}]);