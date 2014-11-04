using KeithLink.Svc.Core.Enumerations.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Enumerations {
    public static class OrderSourceExtension {

        #region methods
        public static void Parse(this OrderSource value, string input) {
            switch (input) {
                case "B":
                    value = OrderSource.Entree;
                    break;
                case "K":
                    value = OrderSource.DSR;
                    break;
                default:
                    value = OrderSource.Other;
                    break;
            }
        }

        public static string ToShortString(this OrderSource value) {
            switch (value) {
                case OrderSource.Entree:
                    return "B";
                case OrderSource.DSR:
                    return "K";
                case OrderSource.CustomerService:
                    return string.Empty;
                case OrderSource.Other:
                    return string.Empty;
                default:
                    return string.Empty;
            }
        }
        #endregion
    }
}
