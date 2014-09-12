using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Text;

namespace KeithLink.Svc.Core.Extensions {
    public static class OrderDetailExtension {
        public static string ToMainframeFormat(this OrderDetail value) {
            if (value.LineNumber == 0) { throw new ArgumentException("LineNumber has not been set", "LineNumber"); }
            if (value.ItemNumber.Length == 0) { throw new ArgumentException("ItemNumber has not been set", "ItemNumber"); }

            StringBuilder output = new StringBuilder("D");

            output.Append(value.ItemNumber);
            output.Append(value.OrderedQuantity.ToString().PadLeft(3, '0'));

            switch (value.UnitOfMeasure) {
                case UnitOfMeasure.Case:
                    output.Append(" ");
                    break;
                case UnitOfMeasure.Package:
                    output.Append("P");
                    break;
                default:
                    throw new ArgumentException("UnitOfMeasure has an unrecognized value", "UnitOfMeasure");
            }

            output.Append(value.Catchweight ? "Y" : " ");
            output.Append(value.LineNumber.ToString().PadLeft(5, '0'));

            switch (value.ItemChange) {
                case LineType.NoChange:
                    output.Append(" ");
                    break;
                case LineType.Change:
                    output.Append("C");
                    break;
                case LineType.Delete:
                    output.Append("D");
                    break;
                case LineType.Add:
                    output.Append("A");
                    break;
                default:
                    throw new ArgumentException("ItemChange has an unrecognized value", "ItemChange");
            }

            output.Append(value.SubOriginalItemNumber.PadRight(6));
            output.Append(value.ReplacedOriginalItemNumber.PadRight(6));
            output.Append(value.ItemStatus.PadRight(1));

            return output.ToString();
        }
    }
}
