using KeithLink.Svc.Core.Enumerations.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Enumerations {
    public static class OrderSourceExtension {

        #region methods
        public static OrderSource Parse(this OrderSource value, string input) {
            switch (input) {
                case "B":
                    return OrderSource.Entree;
                case "C":
                    return OrderSource.CustomerService;
                case "K":
                    return OrderSource.DSR;
                case "T":
                    return OrderSource.KeithNet;
                default:
                    return OrderSource.Other;
            }
        }

        public static string ToShortString(this OrderSource value) {
            switch (value) {
                case OrderSource.Entree:
                    return "B";
                case OrderSource.DSR:
                    return "K";
                case OrderSource.CustomerService:
                    return "C";
                case OrderSource.KeithNet:
                    return "T";
                case OrderSource.Other:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
