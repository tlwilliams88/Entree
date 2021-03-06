﻿// KeithLink
using KeithLink.Common.Core.Extensions;

using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Models.Orders;

// Core
using System;
using System.Text;

namespace KeithLink.Svc.Core.Extensions.Orders {
    public static class OrderHeaderExtension {
        public static string GetOrderSource(OrderSource value, string entreeCollectorType) {
            switch (value) {
                case OrderSource.Entree:
                    return entreeCollectorType;
                default:
                    throw new ArgumentException("Unkown OrderingSystem", "OrderingSystem");
            }
        }

        public static string GetOrderType(OrderType value) {
            switch (value) {
                case OrderType.NormalOrder:
                    return " ";
                case OrderType.ChangeOrder:
                    return "C";
                case OrderType.DeleteOrder:
                    return "D";
                default:
                    throw new ArgumentException("Unknown OrderType", "OrderType");
            }
        }

        public static string ToMainframeFormat(this OrderHeader value, string entreeCollectorType) {
            if (value.Branch.Length != 3) { throw new ArgumentException("Branch is an invalid format", "Branch"); }
            if (value.CustomerNumber.Length != 6) { throw new ArgumentException("CustomerNumber is in an invalid format", "CustomerNumber"); }

            StringBuilder output = new StringBuilder("H");

            output.Append(GetOrderSource(value.OrderingSystem, entreeCollectorType));
            output.Append(value.Branch);
            output.Append(value.CustomerNumber);
            output.Append(value.DeliveryDate.ToYearFirstDate());
            output.Append(value.PONumber.PadRight(20));
            output.Append(value.Specialinstructions.PadRight(80));
            output.Append(value.ControlNumber.ToString().PadLeft(7, '0'));
            output.Append(GetOrderType(value.OrderType));
            output.Append(value.InvoiceNumber.PadLeft(8, '0'));
            output.Append(value.OrderCreateDateTime.ToString("yyyyMMddHHmmss"));
            output.Append(value.OrderSendDateTime.ToYearFirstDateWithTime());
            output.Append(value.UserId.PadRight(50));
            output.Append(value.OrderFilled ? "Y" : " ");
            output.Append(value.FutureOrder ? "Y" : " ");

            return output.ToString();
        }
    }
}
