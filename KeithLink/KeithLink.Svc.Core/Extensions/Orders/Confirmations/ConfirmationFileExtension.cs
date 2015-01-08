using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Models.Orders.Confirmations;
using KeithLink.Svc.Core.Models.Orders.History;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Orders.Confirmations {
    public static class ConfirmationFileExtension {
        public static OrderHistoryFile ToOrderHistoryFile(this ConfirmationFile confirmation) {
            OrderHistoryFile history = new OrderHistoryFile();

            foreach (ConfirmationDetail confDetail in confirmation.Detail) {
                OrderHistoryDetail detail = new OrderHistoryDetail() {
                    LineNumber = int.Parse(confDetail.RecordNumber),
                    ItemNumber = confDetail.ItemNumber,
                    OrderQuantity = confDetail.QuantityOrdered,
                    UnitOfMeasure = confDetail.BrokenCase.Equals("Y", StringComparison.InvariantCultureIgnoreCase) ? UnitOfMeasure.Package : UnitOfMeasure.Case,
                    ShippedQuantity = confDetail.QuantityShipped,
                    TotalShippedWeight = confDetail.ShipWeight,
                    SellPrice = (confDetail.BrokenCase.Equals("Y", StringComparison.InvariantCultureIgnoreCase) ? confDetail.SplitPriceGross : confDetail.SalesGross),
                    ItemStatus = confDetail.ReasonNotShipped
                    //CatchWeight,
                    //FutureItem,
                    //ItemDeleted,
                    //ReplacedOriginalItemNumber,
                    //SubbedOriginalItemNumber,
                };

                history.Details.Add(detail);
            }

            history.Header.BranchId = confirmation.Header.Branch;
            history.Header.ControlNumber = confirmation.Header.ConfirmationNumber;
            history.Header.CustomerNumber = confirmation.Header.CustomerNumber;
            history.Header.InvoiceNumber = confirmation.Header.InvoiceNumber;
            history.Header.DeliveryDate = confirmation.Header.ShipDate;
            //history.Header.ControlNumber = confirmation.Header.RemoteOrderNumber;
            history.Header.RouteNumber = confirmation.Header.RouteNumber;
            history.Header.StopNumber = confirmation.Header.StopNumber;
            history.Header.OrderStatus = confirmation.Header.ConfirmationStatus;
            history.Header.OrderStatus = string.Empty;
            history.Header.OrderSystem = OrderSource.Entree;
            //history.Header.PONumber
            //history.Header.FutureItems
            //history.Header.ErrorStatus

            return history;
        }
    }
}
