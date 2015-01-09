using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Orders.Confirmations;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Orders.History {
    public static class OrderHistoryFileExtension {
        public static ConfirmationFile ToConfirmationFile(this OrderHistoryFile historyFile) {
            ConfirmationFile confirmation = new ConfirmationFile();

            foreach (OrderHistoryDetail historyDetail in historyFile.Details) {
                ConfirmationDetail detail = new ConfirmationDetail() {
                    RecordNumber = historyDetail.LineNumber.ToString(),
                    ItemNumber = historyDetail.ItemNumber,
                    QuantityOrdered = historyDetail.OrderQuantity,
                    BrokenCase = (historyDetail.UnitOfMeasure == UnitOfMeasure.Package ? "Y" : "N"),
                    QuantityShipped = historyDetail.ShippedQuantity,
                    ShipWeight = historyDetail.TotalShippedWeight,
                    SalesGross = historyDetail.SellPrice * historyDetail.ShippedQuantity,
                    SalesNet = historyDetail.SellPrice * historyDetail.ShippedQuantity,
                    PriceNet = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? historyDetail.SellPrice : 0.0),
                    PriceGross = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? historyDetail.SellPrice : 0.0),
                    SplitPriceNet = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? 0.0 : historyDetail.SellPrice),
                    SplitPriceGross = (historyDetail.UnitOfMeasure == UnitOfMeasure.Case ? 0.0 : historyDetail.SellPrice),
                    ReasonNotShipped = historyDetail.ItemStatus
                    //CaseCube
                    //CaseWeight
                };

                detail.ConfirmationMessage = detail.DisplayStatus();
                confirmation.Detail.Add(detail);
            }

            confirmation.Header.Branch = historyFile.Header.BranchId;
            confirmation.Header.ConfirmationNumber = historyFile.Header.ControlNumber;
            confirmation.Header.CustomerNumber = historyFile.Header.CustomerNumber;
            confirmation.Header.InvoiceNumber = historyFile.Header.InvoiceNumber;
            confirmation.Header.ConfirmationDate = DateTime.Now;
            confirmation.Header.ShipDate = historyFile.Header.DeliveryDate;
            confirmation.Header.RemoteOrderNumber = historyFile.Header.ControlNumber;
            // a confirmation will never have this data, and it is coming back wrong now
            //confirmation.Header.RouteNumber = historyFile.Header.RouteNumber;
            //confirmation.Header.StopNumber = historyFile.Header.StopNumber;
            confirmation.Header.TotalQuantityOrdered = confirmation.Detail.Sum(d => d.QuantityOrdered);
            confirmation.Header.TotalQuantityShipped = confirmation.Detail.Sum(d => d.QuantityShipped);
            //confirmation.Header.TotalInvoice
            confirmation.Header.ConfirmationStatus = historyFile.Header.OrderStatus;
            confirmation.Header.ConfirmationMessage = confirmation.Header.GetDisplayStatus();
            //confirmation.Header.SpecialInstructions
            //confirmation.Header.SpecialInstructionsExtended
            //confirmation.Header.TotalCube
            //confirmation.Header.TotalWeight

            return confirmation;
        }

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
