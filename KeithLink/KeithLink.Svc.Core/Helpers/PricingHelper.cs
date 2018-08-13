﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.ShoppingCart;

namespace KeithLink.Svc.Core.Helpers {
    public static class PricingHelper {
        /// <summary>
        /// This gets pricing for what we assume is an active item
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="casePrice"></param>
        /// <param name="splitPrice"></param>
        /// <param name="each"></param>
        /// <param name="catchWeight"></param>
        /// <param name="avgWeight"></param>
        /// <param name="pack"></param>
        /// <returns></returns>
        public static double GetPrice(int qty, double casePrice, double splitPrice,
                                      bool each, bool catchWeight, double avgWeight,
                                      int pack)
        {
            double total = 0;
            if (pack == 0) { pack = 1; }

            if (catchWeight)
            {
                if (each)
                {
                    total = GetCatchweightPriceForPackage(qty, pack, avgWeight, splitPrice);
                }
                else
                {
                    total = GetCatchweightPriceForCase(qty, avgWeight, casePrice);
                }
            }
            else
            {
                total = qty * (each ? splitPrice : casePrice);
            }
            return total;
        }

        /// <summary>
        /// This gets pricing for what we assume is a past or fixed item
        /// </summary>
        /// <param name="qty"></param>
        /// <param name="sellPrice"></param>
        /// <param name="catchWeight"></param>
        /// <param name="shippedWeight"></param>
        /// <returns></returns>
        public static double GetFixedPrice(int qty, double sellPrice, bool catchWeight, double shippedWeight, double averageWeight)
        {
            double total = 0;
            if (catchWeight)
            {
                if(shippedWeight > 0)
                {
                    total = shippedWeight * sellPrice;
                }
                else
                {
                    total = qty * averageWeight * sellPrice;
                }
            }
            else
            {
                total = qty * sellPrice;
            }
            return total;
        }

        public static double GetCatchweightPriceForCase(int qty, double weight, double price) {
            return (weight * qty) * price;
        }

        public static double GetCatchweightPriceForPackage(int qty, int pack, double weight, double price) {
            return ((weight / pack) * qty) * price;
        }

        public static int DeterminePackAndSize(string itemPack)
        {
            int pack;

            if (!int.TryParse(itemPack, out pack)) { pack = 1; }

            return pack;
        }
        public static double CalculateCartSubtotal(List<ShoppingCartItem> items)
        {
            double calcSubtotal = 0;

            foreach (var item in items)
            {
                int pack = DeterminePackAndSize(item.Pack);

                calcSubtotal += GetPrice((int)item.Quantity, item.CasePriceNumeric, item.PackagePriceNumeric, item.Each, item.CatchWeight, item.AverageWeight, pack);
            }

            return calcSubtotal;
        }
    }
}
