using KeithLink.Svc.Core.Enumerations.Order;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Orders.History {
    public static class OrderHistoryFileExtension {
        private static OrderHistoryDetail ToOrderHistoryDetail(CS.LineItem lineItem, string branchId, string invoiceNumber) {
            OrderHistoryDetail detail = new OrderHistoryDetail();

            detail.LineNumber = Convert.ToInt16(lineItem.Properties["LinePosition"]);
            detail.ItemNumber = lineItem.ProductId;
            detail.OrderQuantity = (short)lineItem.Quantity;
            detail.ShippedQuantity = lineItem.Properties["QuantityShipped"] == null ? 0 : (int)lineItem.Properties["QuantityShipped"];
            detail.UnitOfMeasure = (bool)lineItem.Properties["Each"] ? UnitOfMeasure.Package : UnitOfMeasure.Case;
            detail.CatchWeight = (bool)lineItem.Properties["CatchWeight"];
            //detail.ItemDeleted =
            detail.SellPrice = (double)lineItem.PlacedPrice;
            detail.SubbedOriginalItemNumber = lineItem.Properties["SubstitutedItemNumber"] == null ? null : (string)lineItem.Properties["SubstitutedItemNumber"];
            //detail.ReplacedOriginalItemNumber =
            detail.ItemStatus = lineItem.Properties["MainFrameStatus"] == null ? null : (string)lineItem.Properties["MainFrameStatus"];

            return detail;
        }

        public static OrderHistoryFile ToOrderHistoryFile(this CS.PurchaseOrder value, UserSelectedContext customerInfo) {
            OrderHistoryFile retVal = new OrderHistoryFile();

            retVal.Header = value.ToOrderHistoryHeader(customerInfo);
            if (value.Properties["LineItems"] != null) {
                retVal.Details = ((CommerceServer.Foundation.CommerceRelationshipList)value.Properties["LineItems"])
                                   .Select(l => ToOrderHistoryDetail((CS.LineItem)l.Target, customerInfo.BranchId, retVal.Header.InvoiceNumber)).ToList();
            }
            

            return retVal;
        }
    }
}
