using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Text;

namespace KeithLink.Svc.Core.Extensions {
    public static class OrderHeaderExtension {
        public static string ToMainframeFormat(this OrderHeader value) {
            if (value.Branch.Length != 3) { throw new ArgumentException("Branch is an invalid format", "Branch"); }
            if (value.CustomerNumber.Length != 6) { throw new ArgumentException("CustomerNumber is in an invalid format", "CustomerNumber"); }

            StringBuilder output = new StringBuilder("H");

            switch (value.OrderingSystem) {
                case OrderSource.KeithCom:
                    output.Append("B");
                    break;
                default:
                    throw new ArgumentException("Unkown OrderingSystem", "OrderingSystem");
            }

            output.Append(value.Branch);
            output.Append(value.CustomerNumber);
            output.Append(value.DeliveryDate.ToString("yyyyMMdd"));
            output.Append(value.PONumber.PadRight(20));
            output.Append(value.Specialinstructions.PadRight(80));
            output.Append(value.ControlNumber.ToString().PadLeft(7, '0'));

            switch (value.OrderType) {
                case OrderType.NormalOrder:
                    output.Append(" ");
                    break;
                case OrderType.ChangeOrder:
                    output.Append("C");
                    break;
                case OrderType.DeleteOrder:
                    output.Append("D");
                    break;
                default:
                    throw new ArgumentException("Unknown OrderType", "OrderType");
            }

            output.Append(value.InvoiceNumber.PadLeft(8, '0'));
            output.Append(value.OrderCreateDateTime.ToString("yyyyMMddHHmmss"));
            output.Append(value.OrderSendDateTime.ToString("yyyyMMddHHmmss"));
            output.Append(value.UserId.PadRight(50));
            output.Append(value.OrderFilled ? "Y" : " ");
            output.Append(value.FutureOrder ? "Y" : " ");

            return output.ToString();
        }
    }
}
