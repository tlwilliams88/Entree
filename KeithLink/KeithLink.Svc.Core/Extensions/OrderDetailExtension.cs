using KeithLink.Svc.Core.Models.Orders;
using System;
using System.Text;

namespace KeithLink.Svc.Core.Extensions {
    public static class OrderDetailExtension {
        private static string GetLineType(LineType value) {
            switch (value) {
                case LineType.NoChange:
                    return " ";
                case LineType.Change:
                    return "C";
                case LineType.Delete:
                    return "D";
                case LineType.Add:
                    return "A";
                default:
                    throw new ArgumentException("ItemChange has an unrecognized value", "ItemChange");
            }
        }

        private static string GetUOM(UnitOfMeasure UOM) {
            switch (UOM) {
                case UnitOfMeasure.Case:
                    return " ";
                case UnitOfMeasure.Package:
                    return "P";
                default:
                    throw new ArgumentException("UnitOfMeasure has an unrecognized value", "UnitOfMeasure");
            }
        }

        public static string ToMainframeFormat(this OrderDetail value) {
            if (value.LineNumber == 0) { throw new ArgumentException("LineNumber has not been set", "LineNumber"); }
            if (value.ItemNumber.Length == 0) { throw new ArgumentException("ItemNumber has not been set", "ItemNumber"); }

            StringBuilder output = new StringBuilder("D");

            output.Append(value.ItemNumber);
            output.Append(value.OrderedQuantity.ToString().PadLeft(3, '0'));
            output.Append(GetUOM(value.UnitOfMeasure));
            output.Append(value.Catchweight ? "Y" : " ");
            output.Append(value.LineNumber.ToString().PadLeft(5, '0'));
            output.Append(GetLineType(value.ItemChange));
            output.Append(value.SubOriginalItemNumber.PadRight(6));
            output.Append(value.ReplacedOriginalItemNumber.PadRight(6));
            output.Append(value.ItemStatus.PadRight(1));

            return output.ToString();
        }
    }
}
