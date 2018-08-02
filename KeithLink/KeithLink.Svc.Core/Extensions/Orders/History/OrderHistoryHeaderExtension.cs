using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Extensions.Enumerations;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Orders.History;
using EF = KeithLink.Svc.Core.Models.Orders.History.EF;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Concurrent;
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
        private const int HEADER_LENGTH_ORDDATE = 14;

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
        private const int HEADER_STARTPOS_ORDDATE = 63;
        #endregion

        #region methods

        private static void FillEtaInformation(EF.OrderHistoryHeader orderHistory, Order order)
        {
            order.ScheduledDeliveryTime = orderHistory.ScheduledDeliveryTime;
            order.EstimatedDeliveryTime = orderHistory.EstimatedDeliveryTime;
            order.ActualDeliveryTime = orderHistory.ActualDeliveryTime;
            order.DeliveryOutOfSequence = orderHistory.DeliveryOutOfSequence;
        }

        public static void Parse(this OrderHistoryHeader header, string record)
        {
            if (record.Length >= HEADER_STARTPOS_ORDSYS + HEADER_LENGTH_ORDSYS)
            {
                OrderSource tempOrderSource = new OrderSource();
                header.OrderSystem = tempOrderSource.Parse(record.Substring(HEADER_STARTPOS_ORDSYS, HEADER_LENGTH_ORDSYS)); 
            }

            if (record.Length >= HEADER_STARTPOS_BRANCH + HEADER_LENGTH_BRANCH)
            {
                header.BranchId = record.Substring(HEADER_STARTPOS_BRANCH, HEADER_LENGTH_BRANCH).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_CUSTNUM + HEADER_LENGTH_CUSTNUM)
            {
                header.CustomerNumber = record.Substring(HEADER_STARTPOS_CUSTNUM, HEADER_LENGTH_CUSTNUM).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_DELVDATE + HEADER_LENGTH_DELVDATE)
            {
                string deliveryDate = record.Substring(HEADER_STARTPOS_DELVDATE, HEADER_LENGTH_DELVDATE);
                int year = int.Parse(deliveryDate.Substring(0, 4));
                int month = int.Parse(deliveryDate.Substring(4, 2));
                int day = int.Parse(deliveryDate.Substring(6, 2));

                header.DeliveryDate = new DateTime(year, month, day).ToLongDateFormat();
            }

            if (record.Length >= HEADER_STARTPOS_PONUM + HEADER_LENGTH_PONUM)
            {
                header.PONumber = record.Substring(HEADER_STARTPOS_PONUM, HEADER_LENGTH_PONUM).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_CTRLNUM + HEADER_LENGTH_CTRLNUM)
            {
                header.ControlNumber = record.Substring(HEADER_STARTPOS_CTRLNUM, HEADER_LENGTH_CTRLNUM).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_INVNUM + HEADER_LENGTH_INVNUM)
            {
                header.InvoiceNumber = record.Substring(HEADER_STARTPOS_INVNUM, HEADER_LENGTH_INVNUM).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_ORDSTS + HEADER_LENGTH_ORDSTS)
            {
                header.OrderStatus = record.Substring(HEADER_STARTPOS_ORDSTS, HEADER_LENGTH_ORDSTS).Trim();
            }

            // don't set Future Item flag here
            // don't set Error Status flag here

            if (record.Length >= HEADER_STARTPOS_RTENUM + HEADER_LENGTH_RTENUM)
            {
                header.RouteNumber = record.Substring(HEADER_STARTPOS_RTENUM, HEADER_LENGTH_RTENUM).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_STPNUM + HEADER_LENGTH_STPNUM)
            {
                header.StopNumber = record.Substring(HEADER_STARTPOS_STPNUM, HEADER_LENGTH_STPNUM).Trim();
            }

            if (record.Length >= HEADER_STARTPOS_ORDDATE + HEADER_LENGTH_ORDDATE)
            {
                string rawOrderDateTime = record.Substring(HEADER_STARTPOS_ORDDATE, HEADER_LENGTH_ORDDATE);
                if (string.IsNullOrWhiteSpace(rawOrderDateTime) == false)
                {
                    //value.OrderDateTime = DateTime.ParseExact(orderDateTime, "yyyyMMddHHmmss", null);

                    int year = int.Parse(rawOrderDateTime.Substring(0, 4));
                    int month = int.Parse(rawOrderDateTime.Substring(4, 2));
                    int day = int.Parse(rawOrderDateTime.Substring(6, 2));
                    int hour = int.Parse(rawOrderDateTime.Substring(8, 2));
                    int minute = int.Parse(rawOrderDateTime.Substring(10, 2));
                    int second = int.Parse(rawOrderDateTime.Substring(12, 2));
                    
                    var orderDateTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Local);
                    header.OrderDateTime = orderDateTime.ToLongDateFormatWithTime();
                }
            }
        }

        public static void MergeWithEntity(this OrderHistoryHeader header, ref EF.OrderHistoryHeader entity)
        {
            entity.OrderSystem = header.OrderSystem.ToShortString();
            entity.BranchId = header.BranchId;
            entity.CustomerNumber = header.CustomerNumber;
            entity.InvoiceNumber = header.InvoiceNumber;
            entity.OrderDateTime = header.OrderDateTime;
            entity.DeliveryDate = header.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
            entity.PONumber = header.PONumber ?? entity.PONumber;
            //entity.ControlNumber = header.ControlNumber.Trim();
            // the original control number is actually set from the entity already
            // and because the order history header is actually a converted confirmation
            // it does not know the original control number so it is seeing it as null
            // and screwing up the original control number
            //entity.OriginalControlNumber = string.IsNullOrEmpty(header.OriginalControlNumber) ? header.ControlNumber.Trim() : header.OriginalControlNumber.Trim();
            entity.OrderStatus = header.OrderStatus;
            entity.FutureItems = header.FutureItems;
            entity.ErrorStatus = header.ErrorStatus;
            entity.RouteNumber = header.RouteNumber;
            entity.StopNumber = header.StopNumber;
            //entity.IsSpecialOrder = 

            if (string.IsNullOrEmpty(entity.ControlNumber))
            {
                entity.ControlNumber = header.ControlNumber.Trim();
                if (string.IsNullOrEmpty(entity.OriginalControlNumber))
                {
                    entity.OriginalControlNumber = header.ControlNumber.Trim();
                }
            }
            else
            {
                // update the history file with EF data
                int valueControlNumber;
                if (!int.TryParse(header.ControlNumber, out valueControlNumber))
                {
                    valueControlNumber = 0;
                }

                int entityControlNumber;
                if (!int.TryParse(entity.ControlNumber, out entityControlNumber))
                {
                    entityControlNumber = 0;
                }

                if (entityControlNumber > valueControlNumber)
                {
                    header.ControlNumber = entity.ControlNumber;
                }
                else
                {
                    entity.ControlNumber = header.ControlNumber;
                }
                //value.ControlNumber = entityControlNumber >= valueControlNumber? entity.ControlNumber : value.ControlNumber;
                header.OriginalControlNumber = entity.OriginalControlNumber?? entity.ControlNumber;
            }
        }

        public static EF.OrderHistoryHeader ToEntityFrameworkModel(this OrderHistoryHeader header)
        {
            EF.OrderHistoryHeader entity = new EF.OrderHistoryHeader();

            entity.OrderSystem = header.OrderSystem.ToShortString();
            entity.BranchId = header.BranchId;
            entity.CustomerNumber = header.CustomerNumber;
            entity.InvoiceNumber = header.InvoiceNumber;
            entity.OrderDateTime = header.OrderDateTime;
            entity.DeliveryDate = header.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
            entity.PONumber = header.PONumber;
            entity.ControlNumber = header.ControlNumber;
            entity.OriginalControlNumber = string.IsNullOrEmpty(header.OriginalControlNumber) ? header.ControlNumber.Trim() : header.OriginalControlNumber.Trim();
            entity.OrderStatus = header.OrderStatus;
            entity.FutureItems = header.FutureItems;
            entity.ErrorStatus = header.ErrorStatus;
            entity.RouteNumber = header.RouteNumber;
            entity.StopNumber = header.StopNumber;

            return entity;
        }

		public static Order ToOrder(this EF.OrderHistoryHeader entity)
		{
			Order order = new Order();

			//retVal.OrderNumber = value.InvoiceNumber;
            order.OrderNumber = entity.ControlNumber;

			switch (entity.OrderStatus.Trim())
			{
				case "":
					order.Status = "Ordered";
					break;
				case "I":
					order.Status = "Invoiced";
					break;
				case "P":
					order.Status = "Processing";
					break;
				case "D":
					order.Status = "Deleted";
					break;
				default:
					break;
			}

            order.CreatedDate = DateTime.SpecifyKind(entity.CreatedUtc.ToLocalTime(), DateTimeKind.Unspecified);

            if (string.IsNullOrWhiteSpace(entity.OrderDateTime) == false)
            {
                var orderDateTime = entity.OrderDateTime.ToDateTime();
                if (orderDateTime.HasValue)
                {
                    order.CreatedDate = orderDateTime.Value;
                }
            }

            order.DeliveryDate = entity.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
			order.InvoiceNumber = entity.InvoiceNumber.Trim();
			order.InvoiceStatus = "N/A";
			order.ItemCount = entity.OrderDetails == null ? 0 : entity.OrderDetails.Count;

            order.RequestedShipDate = entity.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
			order.IsChangeOrderAllowed = false;
			order.CommerceId = Guid.Empty;
            FillEtaInformation(entity, order);
			order.PONumber = entity.PONumber;
            order.IsSpecialOrder = entity.IsSpecialOrder;
		    order.OrderTotal = (double)entity.OrderSubtotal;
            
            order.OrderSystem = new OrderSource().Parse(entity.OrderSystem).ToString(); 

			if (entity.OrderDetails != null && entity.OrderDetails.Count > 0)
			{
				var lineItems = new BlockingCollection<OrderLine>();

                Parallel.ForEach(entity.OrderDetails, d => 
                    {
                        lineItems.Add(d.ToOrderLine(entity.OrderStatus));
                    });

				order.Items = lineItems.OrderBy(i => i.LineNumber).ToList();
			}

			return order;
		}

		public static Order ToOrderHeaderOnly(this EF.OrderHistoryHeader entity)
		{
			Order order = new Order();

			order.OrderNumber = entity.InvoiceNumber;

			switch (entity.OrderStatus.Trim())
			{
				case "":
					order.Status = "Ordered";
					break;
				case "I":
					order.Status = "Invoiced";
					break;
				case "P":
					order.Status = "Processing";
					break;
				default:
					order.Status = "Unknown";
					break;
			}

            order.CreatedDate = DateTime.SpecifyKind(entity.CreatedUtc.ToLocalTime(), DateTimeKind.Unspecified);
            order.DeliveryDate = entity.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
			order.InvoiceNumber = entity.InvoiceNumber.Trim();
			order.InvoiceStatus = "N/A";
			order.ItemCount = entity.OrderDetails == null ? 0 : entity.OrderDetails.Count;
            order.OrderTotal = (double)entity.OrderDetails.Sum(d => d.ShippedQuantity * d.SellPrice); 
            order.RequestedShipDate = entity.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
			order.IsChangeOrderAllowed = false;
			order.CommerceId = Guid.Empty;
            FillEtaInformation(entity, order);

			return order;
		}

        public static OrderHistoryHeader ToOrderHistoryHeader(this EF.OrderHistoryHeader entity)
        {
            OrderHistoryHeader header = new OrderHistoryHeader();

            header.OrderSystem = OrderSource.Entree; // TODO: entity.OrderSystem.ToShortString();
            header.BranchId = entity.BranchId;
            header.CustomerNumber = entity.CustomerNumber;
            header.InvoiceNumber = entity.InvoiceNumber;
            header.DeliveryDate = entity.DeliveryDate.ToDateTime().Value.ToLongDateFormat();
            header.PONumber = entity.PONumber;
            header.ControlNumber = entity.ControlNumber;
            header.OriginalControlNumber = string.IsNullOrEmpty(entity.OriginalControlNumber) ? entity.ControlNumber.Trim() : entity.OriginalControlNumber.Trim();
            header.OrderStatus = entity.OrderStatus;
            header.FutureItems = entity.FutureItems;
            header.ErrorStatus = entity.ErrorStatus;
            header.RouteNumber = entity.RouteNumber;
            header.StopNumber = entity.StopNumber;
			
            return header;
        }

        public static OrderHistoryHeader ToOrderHistoryHeader(this CS.PurchaseOrder value, UserSelectedContext customerInfo, string specialCatalogId = null)
        {
            OrderHistoryHeader header = new OrderHistoryHeader();

            header.OrderSystem = OrderSource.Entree;

            //if (specialCatalogId == null) // TODO: What to do about branch for unfi?
                header.BranchId = customerInfo.BranchId;
            //else
            //    header.BranchId = specialCatalogId;

            header.CustomerNumber = customerInfo.CustomerId;
            header.InvoiceNumber = value.Properties["MasterNumber"] == null ? "Processing" : value.Properties["MasterNumber"].ToString();
            header.DeliveryDate = value.Properties["RequestedShipDate"].ToString().ToDateTime().Value.ToLongDateFormat();
            header.PONumber = value.Properties["PONumber"] == null ? string.Empty : value.Properties["PONumber"].ToString();
            header.ControlNumber = value.Properties["OrderNumber"].ToString();
            header.OriginalControlNumber = value.Properties["OrderNumber"].ToString();

            // OrderStatus for Order History is either a blank space (normal), I (invoiced), D (deleted), or P (processing)
            //header.OrderStatus = System.Text.RegularExpressions.Regex.Replace(header.Status, "([a-z])([A-Z])", "$1 $2");
            header.OrderStatus = string.Empty;

            return header;
        }
        #endregion
    }
}
