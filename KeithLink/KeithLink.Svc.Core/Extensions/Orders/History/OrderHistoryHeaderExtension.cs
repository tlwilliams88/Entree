using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Enumerations;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Extensions.Orders.History {
    public static class OrderHistoryHeaderExtension {
        #region attributes
        private const int HEADER_LENGTH_ORDSYS = 1;
        private const int HEADER_LENGTH_BRANCH = 3;
        private const int HEADER_LENGTH_CUSTNUM = 6;
        private const int HEADER_LENGTH_DELVDATE = 8;
        private const int HEADER_LENGTH_PONUM = 20;
        private const int HEADER_LENGTH_CTRLNUM = 7;
        private const int HEADER_LENGTH_INVNUM = 8;
        private const int HEADER_LENGTH_ORDSTS = 1;
        private const int HEADER_LENGTH_FUTURE = 1;
        private const int HEADER_LENGTH_ERRSTS = 1;
        private const int HEADER_LENGTH_RTENUM = 3;
        private const int HEADER_LENGTH_STPNUM = 3;

        private const int HEADER_STARTPOS_ORDSYS = 1;
        private const int HEADER_STARTPOS_BRANCH = 2;
        private const int HEADER_STARTPOS_CUSTNUM = 5;
        private const int HEADER_STARTPOS_DELVDATE = 11;
        private const int HEADER_STARTPOS_PONUM = 19;
        private const int HEADER_STARTPOS_CTRLNUM = 39;
        private const int HEADER_STARTPOS_INVNUM = 46;
        private const int HEADER_STARTPOS_ORDSTS = 54;
        private const int HEADER_STARTPOS_FUTURE = 55;
        private const int HEADER_STARTPOS_ERRSTS = 56;
        private const int HEADER_STARTPOS_RTENUM = 57;
        private const int HEADER_STARTPOS_STPNUM = 60;
        #endregion

        #region methods

        private static void FillEtaInformation(EF.OrderHistoryHeader orderHistory, Order order)
        {
            order.ScheduledDeliveryTime = orderHistory.ScheduledDeliveryTime;
            order.EstimatedDeliveryTime = orderHistory.EstimatedDeliveryTime;
            order.ActualDeliveryTime = orderHistory.ActualDeliveryTime;
            order.DeliveryOutOfSequence = orderHistory.DeliveryOutOfSequence;
        }

        public static void Parse(this OrderHistoryHeader value, string record) {
            if (record.Length >= HEADER_STARTPOS_ORDSYS + HEADER_LENGTH_ORDSYS) {
                OrderSource tempOrderSource = new OrderSource();
                value.OrderSystem = tempOrderSource.Parse(record.Substring(HEADER_STARTPOS_ORDSYS, HEADER_LENGTH_ORDSYS)); 
            }
            if (record.Length >= HEADER_STARTPOS_BRANCH + HEADER_LENGTH_BRANCH) { value.BranchId = record.Substring(HEADER_STARTPOS_BRANCH, HEADER_LENGTH_BRANCH).Trim(); }
            if (record.Length >= HEADER_STARTPOS_CUSTNUM + HEADER_LENGTH_CUSTNUM) { value.CustomerNumber = record.Substring(HEADER_STARTPOS_CUSTNUM, HEADER_LENGTH_CUSTNUM).Trim(); }

            if (record.Length >= HEADER_STARTPOS_DELVDATE + HEADER_LENGTH_DELVDATE) {
                string deliveryDate = record.Substring(HEADER_STARTPOS_DELVDATE, HEADER_LENGTH_DELVDATE);
                value.DeliveryDate = new DateTime(int.Parse(deliveryDate.Substring(0, 4)),
                                                   int.Parse(deliveryDate.Substring(4, 2)),
                                                   int.Parse(deliveryDate.Substring(6, 2)));
            }

            if (record.Length >= HEADER_STARTPOS_PONUM + HEADER_LENGTH_PONUM) { value.PONumber = record.Substring(HEADER_STARTPOS_PONUM, HEADER_LENGTH_PONUM).Trim(); }
            if (record.Length >= HEADER_STARTPOS_CTRLNUM + HEADER_LENGTH_CTRLNUM) { value.ControlNumber = record.Substring(HEADER_STARTPOS_CTRLNUM, HEADER_LENGTH_CTRLNUM).Trim(); }
            if (record.Length >= HEADER_STARTPOS_INVNUM + HEADER_LENGTH_INVNUM) { value.InvoiceNumber = record.Substring(HEADER_STARTPOS_INVNUM, HEADER_LENGTH_INVNUM).Trim(); }
            if (record.Length >= HEADER_STARTPOS_ORDSTS + HEADER_LENGTH_ORDSTS) { value.OrderStatus = record.Substring(HEADER_STARTPOS_ORDSTS, HEADER_LENGTH_ORDSTS).Trim(); }

            // don't set Future Item flag here
            // don't set Error Status flag here

            if (record.Length >= HEADER_STARTPOS_RTENUM + HEADER_LENGTH_RTENUM) { value.RouteNumber = record.Substring(HEADER_STARTPOS_RTENUM, HEADER_LENGTH_RTENUM).Trim(); }
            if (record.Length >= HEADER_STARTPOS_STPNUM + HEADER_LENGTH_STPNUM) { value.StopNumber = record.Substring(HEADER_STARTPOS_STPNUM, HEADER_LENGTH_STPNUM).Trim(); }
        }

        public static void MergeWithEntity(this OrderHistoryHeader value, ref EF.OrderHistoryHeader entity) {
            entity.OrderSystem = value.OrderSystem.ToShortString();
            entity.BranchId = value.BranchId;
            entity.CustomerNumber = value.CustomerNumber;
            entity.InvoiceNumber = value.InvoiceNumber;
            entity.DeliveryDate = value.DeliveryDate;
            entity.PONumber = value.PONumber;
            entity.ControlNumber = value.ControlNumber.Trim();
            // the original control number is actually set from the entity already
            // and because the order history header is actually a converted confirmation
            // it does not know the original control number so it is seeing it as null
            // and screwing up the original control number
            //entity.OriginalControlNumber = string.IsNullOrEmpty(value.OriginalControlNumber) ? value.ControlNumber.Trim() : value.OriginalControlNumber.Trim();
            entity.OrderStatus = value.OrderStatus;
            entity.FutureItems = value.FutureItems;
            entity.ErrorStatus = value.ErrorStatus;
            entity.RouteNumber = value.RouteNumber;
            entity.StopNumber = value.StopNumber;
        }

        public static EF.OrderHistoryHeader ToEntityFrameworkModel(this OrderHistoryHeader value) {
            EF.OrderHistoryHeader retVal = new EF.OrderHistoryHeader();

            retVal.OrderSystem = value.OrderSystem.ToShortString();
            retVal.BranchId = value.BranchId;
            retVal.CustomerNumber = value.CustomerNumber;
            retVal.InvoiceNumber = value.InvoiceNumber;
            retVal.DeliveryDate = value.DeliveryDate;
            retVal.PONumber = value.PONumber;
            retVal.ControlNumber = value.ControlNumber;
            retVal.OriginalControlNumber = string.IsNullOrEmpty(value.OriginalControlNumber) ? value.ControlNumber.Trim() : value.OriginalControlNumber.Trim();
            retVal.OrderStatus = value.OrderStatus;
            retVal.FutureItems = value.FutureItems;
            retVal.ErrorStatus = value.ErrorStatus;
            retVal.RouteNumber = value.RouteNumber;
            retVal.StopNumber = value.StopNumber;

            return retVal;
        }

		public static Order ToOrder(this EF.OrderHistoryHeader value)
		{
			Order retVal = new Order();

			retVal.OrderNumber = value.InvoiceNumber;

			switch (value.OrderStatus.Trim())
			{
				case "":
					retVal.Status = "Ordered";
					break;
				case "I":
					retVal.Status = "Invoiced";
					break;
				case "P":
					retVal.Status = "Processing";
					break;
				case "D":
					retVal.Status = "Deleted";
					break;
				default:
					break;
			}

			retVal.DeliveryDate = value.DeliveryDate;
			retVal.InvoiceNumber = value.InvoiceNumber.Trim();
			retVal.InvoiceStatus = "N/A";
			retVal.ItemCount = value.OrderDetails == null ? 0 : value.OrderDetails.Count;
            retVal.CreatedDate = DateTime.SpecifyKind(value.CreatedUtc.ToLocalTime(), DateTimeKind.Unspecified);
			retVal.RequestedShipDate = (DateTime)value.DeliveryDate;
			retVal.IsChangeOrderAllowed = false;
			retVal.CommerceId = Guid.Empty;
            FillEtaInformation(value, retVal);
			retVal.PONumber = value.PONumber;

            retVal.OrderSystem = new OrderSource().Parse(value.OrderSystem).ToString(); 

			switch (value.OrderSystem.Trim())
			{

			}

			if (value.OrderDetails != null && value.OrderDetails.Count > 0)
			{
				System.Collections.Concurrent.BlockingCollection<OrderLine> lineItems = new System.Collections.Concurrent.BlockingCollection<OrderLine>();

                Parallel.ForEach(value.OrderDetails, d => {
                    lineItems.Add(d.ToOrderLine(value.OrderStatus));
                });

				retVal.Items = lineItems.OrderBy(i => i.LineNumber).ToList();
			}

			return retVal;
		}

		public static Order ToOrderHeaderOnly(this EF.OrderHistoryHeader value)
		{
			Order retVal = new Order();

			retVal.OrderNumber = value.InvoiceNumber;

			switch (value.OrderStatus.Trim())
			{
				case "":
					retVal.Status = "Ordered";
					break;
				case "I":
					retVal.Status = "Invoiced";
					break;
				case "P":
					retVal.Status = "Processing";
					break;
				default:
					retVal.Status = "Unknown";
					break;
			}

			retVal.DeliveryDate = value.DeliveryDate;
			retVal.InvoiceNumber = value.InvoiceNumber.Trim();
			retVal.InvoiceStatus = "N/A";
			retVal.ItemCount = value.OrderDetails == null ? 0 : value.OrderDetails.Count;
            retVal.OrderTotal = (double)value.OrderDetails.Sum(d => d.ShippedQuantity * d.SellPrice); 
			retVal.CreatedDate = DateTime.SpecifyKind(value.CreatedUtc.ToLocalTime(), DateTimeKind.Unspecified);
            retVal.RequestedShipDate = (DateTime)(value.DeliveryDate.HasValue ? value.DeliveryDate : DateTime.Now);
			retVal.IsChangeOrderAllowed = false;
			retVal.CommerceId = Guid.Empty;
            FillEtaInformation(value, retVal);

			return retVal;
		}

        public static OrderHistoryHeader ToOrderHistoryHeader(this EF.OrderHistoryHeader value)
        {
            OrderHistoryHeader retVal = new OrderHistoryHeader();

            retVal.OrderSystem = OrderSource.Entree; // TODO: value.OrderSystem.ToShortString();
            retVal.BranchId = value.BranchId;
            retVal.CustomerNumber = value.CustomerNumber;
            retVal.InvoiceNumber = value.InvoiceNumber;
            retVal.DeliveryDate = value.DeliveryDate;
            retVal.PONumber = value.PONumber;
            retVal.ControlNumber = value.ControlNumber;
            retVal.OriginalControlNumber = string.IsNullOrEmpty(value.OriginalControlNumber) ? value.ControlNumber.Trim() : value.OriginalControlNumber.Trim();
            retVal.OrderStatus = value.OrderStatus;
            retVal.FutureItems = value.FutureItems;
            retVal.ErrorStatus = value.ErrorStatus;
            retVal.RouteNumber = value.RouteNumber;
            retVal.StopNumber = value.StopNumber;
			
            return retVal;
        }

        public static OrderHistoryHeader ToOrderHistoryHeader(this CS.PurchaseOrder value, UserSelectedContext customerInfo) {
            OrderHistoryHeader retVal = new OrderHistoryHeader();

            retVal.OrderSystem = OrderSource.Entree;
            retVal.BranchId = customerInfo.BranchId;
            retVal.CustomerNumber = customerInfo.CustomerId;
            retVal.InvoiceNumber = value.Properties["MasterNumber"] == null ? "Processing" : value.Properties["MasterNumber"].ToString();
            retVal.DeliveryDate = value.Properties["RequestedShipDate"] == null ? DateTime.Now : (DateTime)value.Properties["RequestedShipDate"];
            retVal.PONumber = value.Properties["PONumber"] == null ? string.Empty : value.Properties["PONumber"].ToString();
            retVal.ControlNumber = value.Properties["OrderNumber"].ToString();
            retVal.OriginalControlNumber = value.Properties["OrderNumber"].ToString();

            // OrderStatus for Order History is either a blank space (normal), I (invoiced), D (deleted), or P (processing)
            //retVal.OrderStatus = System.Text.RegularExpressions.Regex.Replace(value.Status, "([a-z])([A-Z])", "$1 $2");
            retVal.OrderStatus = string.Empty;

            return retVal;
        }
        #endregion
    }
}
