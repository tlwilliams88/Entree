using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Helpers {
    public static class PricingHelper {
        public static double GetCatchweightPriceForCase(int qty, double weight, double price) {
            return (weight * qty) * price;
        }

        public static double GetCatchweightPriceForPackage(int qty, int pack, double weight, double price) {
            return ((weight / pack) * qty) * price;
        }
    }
}
