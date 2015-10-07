using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Helpers {
    public static class PricingHelper {
        public static double GetPrice(int qty, double casePrice, double splitPrice, 
                                      bool each, bool catchWeight, double avgWeight, 
                                      int pack){
        if(pack == 0) {pack = 1;}

        if (catchWeight) {
                if (each) {
                    return GetCatchweightPriceForPackage(qty, pack, avgWeight, splitPrice);
                } else {
                    return GetCatchweightPriceForCase(qty, avgWeight, casePrice);
                }
            } else {
                return qty * (each ? splitPrice : casePrice);
            }
        }

        public static double GetCatchweightPriceForCase(int qty, double weight, double price) {
            return (weight * qty) * price;
        }

        public static double GetCatchweightPriceForPackage(int qty, int pack, double weight, double price) {
            return ((weight / pack) * qty) * price;
        }
    }
}
