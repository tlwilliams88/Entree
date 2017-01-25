using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static double GetFixedPrice(int qty, double sellPrice, bool catchWeight, double shippedWeight)
        {
            double total = 0;
            if (catchWeight)
            {
                total = shippedWeight * sellPrice;
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
    }
}
