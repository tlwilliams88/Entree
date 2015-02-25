using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Enumerations;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Invoices;

namespace KeithLink.Svc.Core.Extensions.Orders.History {
    public static class OrderHistoryDetailExtension {
        #region attributes
        private const int DETAIL_LENGTH_ITEMNUM = 6;
        private const int DETAIL_LENGTH_LINNUM = 5;
        private const int DETAIL_LENGTH_ORDQTY = 3;
        private const int DETAIL_LENGTH_SHIPQTY = 5;
        private const int DETAIL_LENGTH_UOM = 1;
        private const int DETAIL_LENGTH_SELLPRICE = 10;
        private const int DETAIL_LENGTH_CWT = 1;
        private const int DETAIL_LENGTH_ITEMDEL = 1;
        private const int DETAIL_LENGTH_SUBORG = 6;
        private const int DETAIL_LENGTH_REPLORG = 6;
        private const int DETAIL_LENGTH_ITEMSTS = 1;
        private const int DETAIL_LENGTH_FUTURE = 1;
        private const int DETAIL_LENGTH_WEIGHT = 8;

        private const int DETAIL_STARTPOS_ITEMNUM = 1;
        private const int DETAIL_STARTPOS_LINNUM = 7;
        private const int DETAIL_STARTPOS_ORDQTY = 12;
        private const int DETAIL_STARTPOS_SHIPQTY = 15;
        private const int DETAIL_STARTPOS_UOM = 20;
        private const int DETAIL_STARTPOS_SELLPRICE = 21;
        private const int DETAIL_STARTPOS_CWT = 31;
        private const int DETAIL_STARTPOS_ITEMDEL = 32;
        private const int DETAIL_STARTPOS_SUBORG = 33;
        private const int DETAIL_STARTPOS_REPLORG = 39;
        private const int DETAIL_STARTPOS_ITEMSTS = 45;
        private const int DETAIL_STARTPOS_FUTURE = 46;
        private const int DETAIL_STARTPOS_WEIGHT = 47;
        #endregion

        #region methods
        public static void MergeWithEntityFrameworkModel(this OrderHistoryDetail value, ref EF.OrderHistoryDetail entity) {
            entity.ItemNumber = value.ItemNumber;
            entity.LineNumber = value.LineNumber;
            entity.OrderQuantity = value.OrderQuantity;
            entity.ShippedQuantity = value.ShippedQuantity;
            entity.UnitOfMeasure = value.UnitOfMeasure.ToShortString();
            entity.CatchWeight = value.CatchWeight;
            entity.ItemDeleted = value.ItemDeleted;
            entity.SubbedOriginalItemNumber = value.SubbedOriginalItemNumber;
            entity.ReplacedOriginalItemNumber = value.ReplacedOriginalItemNumber;
            entity.ItemStatus = value.ItemStatus;
            entity.TotalShippedWeight = decimal.Parse(value.TotalShippedWeight.ToString());
            entity.SellPrice = (decimal)value.SellPrice;
        }

        public static void Parse(this OrderHistoryDetail value, string record) {
            if (record.Length >= DETAIL_STARTPOS_ITEMNUM + DETAIL_LENGTH_ITEMNUM) { value.ItemNumber = record.Substring(DETAIL_STARTPOS_ITEMNUM, DETAIL_LENGTH_ITEMNUM); }

            if (record.Length >= DETAIL_STARTPOS_LINNUM + DETAIL_LENGTH_LINNUM) {
                int lineNumber;
                int.TryParse(record.Substring(DETAIL_STARTPOS_LINNUM, DETAIL_LENGTH_LINNUM), out lineNumber);
                value.LineNumber = lineNumber;
            }

            if (record.Length >= DETAIL_STARTPOS_ORDQTY + DETAIL_LENGTH_ORDQTY) {
                int ordQty;
                int.TryParse(record.Substring(DETAIL_STARTPOS_ORDQTY, DETAIL_LENGTH_ORDQTY), out ordQty);
                value.OrderQuantity = ordQty;
            }

            if (record.Length >= DETAIL_STARTPOS_SHIPQTY + DETAIL_LENGTH_SHIPQTY) {
                int shippedQty;
                int.TryParse(record.Substring(DETAIL_STARTPOS_SHIPQTY, DETAIL_LENGTH_SHIPQTY), out shippedQty);
                value.ShippedQuantity = shippedQty;
            }

            if (record.Length >= DETAIL_STARTPOS_UOM + DETAIL_LENGTH_UOM) { value.UnitOfMeasure = (record.Substring(DETAIL_STARTPOS_UOM, DETAIL_LENGTH_UOM) == "P" ? UnitOfMeasure.Package : UnitOfMeasure.Case); }

            if (record.Length >= DETAIL_STARTPOS_SELLPRICE + DETAIL_LENGTH_SELLPRICE) {
                double sellPrice;
                double.TryParse(record.Substring(DETAIL_STARTPOS_SELLPRICE, DETAIL_LENGTH_SELLPRICE), out sellPrice);
                value.SellPrice = sellPrice;
            }

            if (record.Length >= DETAIL_STARTPOS_CWT + DETAIL_LENGTH_CWT) { value.CatchWeight = (record.Substring(DETAIL_STARTPOS_CWT, DETAIL_LENGTH_CWT) == "Y"); }
            if (record.Length >= DETAIL_STARTPOS_ITEMDEL + DETAIL_LENGTH_ITEMDEL) { value.ItemDeleted = (record.Substring(DETAIL_STARTPOS_ITEMDEL, DETAIL_LENGTH_ITEMDEL) == "Y"); }
            if (record.Length >= DETAIL_STARTPOS_SUBORG + DETAIL_LENGTH_SUBORG) { value.SubbedOriginalItemNumber = record.Substring(DETAIL_STARTPOS_SUBORG, DETAIL_LENGTH_SUBORG).Trim(); }
            if (record.Length >= DETAIL_STARTPOS_REPLORG + DETAIL_LENGTH_REPLORG) { value.ReplacedOriginalItemNumber = record.Substring(DETAIL_STARTPOS_REPLORG, DETAIL_LENGTH_REPLORG).Trim(); }
            if (record.Length >= DETAIL_STARTPOS_ITEMSTS + DETAIL_LENGTH_ITEMSTS) { value.ItemStatus = record.Substring(DETAIL_STARTPOS_ITEMSTS, DETAIL_LENGTH_ITEMSTS); }
            if (record.Length >= DETAIL_STARTPOS_FUTURE + DETAIL_LENGTH_FUTURE) { value.FutureItem = (record.Substring(DETAIL_STARTPOS_FUTURE, DETAIL_LENGTH_FUTURE) == "Y"); }

            if (record.Length >= DETAIL_STARTPOS_WEIGHT + DETAIL_LENGTH_WEIGHT) {
                double weight;
                double.TryParse(record.Substring(DETAIL_STARTPOS_WEIGHT, DETAIL_LENGTH_WEIGHT), out weight);
                value.TotalShippedWeight = weight;
            }
        }

        public static EF.OrderHistoryDetail ToEntityFrameworkModel(this OrderHistoryDetail value) {
            EF.OrderHistoryDetail retVal = new EF.OrderHistoryDetail();

            retVal.ItemNumber = value.ItemNumber;
            retVal.LineNumber = value.LineNumber;
            retVal.OrderQuantity = value.OrderQuantity;
            retVal.ShippedQuantity = value.ShippedQuantity;
            retVal.UnitOfMeasure = value.UnitOfMeasure.ToShortString();
            retVal.CatchWeight = value.CatchWeight;
            retVal.ItemDeleted = value.ItemDeleted;
            retVal.SubbedOriginalItemNumber = value.SubbedOriginalItemNumber;
            retVal.ReplacedOriginalItemNumber = value.ReplacedOriginalItemNumber;
            retVal.ItemStatus = value.ItemStatus;
            retVal.TotalShippedWeight = decimal.Parse(value.TotalShippedWeight.ToString());
            retVal.SellPrice = (decimal)value.SellPrice;

            return retVal;
        }

        public static OrderHistoryDetail ToOrderHistoryDetail(this EF.OrderHistoryDetail value)
        {
            OrderHistoryDetail retVal = new OrderHistoryDetail();

            retVal.ItemNumber = value.ItemNumber;
            retVal.LineNumber = value.LineNumber;
            retVal.OrderQuantity = value.OrderQuantity;
            retVal.ShippedQuantity = value.ShippedQuantity;
            retVal.UnitOfMeasure = value.UnitOfMeasure.Equals("C", StringComparison.InvariantCultureIgnoreCase) ? UnitOfMeasure.Case : UnitOfMeasure.Package;
            retVal.CatchWeight = value.CatchWeight;
            retVal.ItemDeleted = value.ItemDeleted;
            retVal.SubbedOriginalItemNumber = value.SubbedOriginalItemNumber;
            retVal.ReplacedOriginalItemNumber = value.ReplacedOriginalItemNumber;
            retVal.ItemStatus = value.ItemStatus;
            retVal.TotalShippedWeight = (double)value.TotalShippedWeight;
            retVal.SellPrice = (double)value.SellPrice;

            return retVal;
        }

        public static OrderLine ToOrderLine(this EF.OrderHistoryDetail value) {
            OrderLine lineItem = new OrderLine();

            lineItem.LineNumber = value.LineNumber;
            lineItem.ItemNumber = value.ItemNumber;
            lineItem.Quantity = (short)value.ShippedQuantity;
			lineItem.Price = (double)value.SellPrice;
            lineItem.QuantityOrdered = value.OrderQuantity;
            lineItem.QantityShipped = value.ShippedQuantity;
            if (!string.IsNullOrWhiteSpace(value.ReplacedOriginalItemNumber)) {
                lineItem.SubstitutedItemNumber = value.ReplacedOriginalItemNumber.Trim();
            } else if (!string.IsNullOrWhiteSpace(value.SubbedOriginalItemNumber)) {
                lineItem.SubstitutedItemNumber = value.SubbedOriginalItemNumber.Trim();
            }
            lineItem.MainFrameStatus = value.ItemStatus;
            lineItem.Each = value.UnitOfMeasure.Equals(UnitOfMeasure.Package.ToShortString()) ? true : false;

            return lineItem;
        }

		public static InvoiceItemModel ToInvoiceItem(this EF.OrderHistoryDetail value)
		{
			return new InvoiceItemModel()
			{
				ItemNumber = value.ItemNumber,
				ItemPrice = value.SellPrice,
				QuantityShipped = value.ShippedQuantity,
				QuantityOrdered = value.OrderQuantity,
				LineNumber = value.LineNumber.ToString(),
				ExtSalesNet = value.ShippedQuantity * value.SellPrice
			};
		}
        #endregion
    }
}
