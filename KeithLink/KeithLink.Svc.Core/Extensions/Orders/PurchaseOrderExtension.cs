﻿using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Models.Orders;
using CS = KeithLink.Svc.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Extensions.Enumerations;

namespace KeithLink.Svc.Core.Extensions.Orders {
    public static class PurchaseOrderExtension {
        public static Order ToOrder(this CS.PurchaseOrder value) {
            Order retVal =  new Order() {
                CreatedDate = DateTime.Parse(value.Properties["DateCreated"].ToString()),
                OrderNumber = value.Properties["OrderNumber"].ToString(),
                OrderTotal = Double.Parse(value.Properties["Total"].ToString()),
                InvoiceNumber = value.Properties["MasterNumber"] == null ? "Pending" : value.Properties["MasterNumber"].ToString(),
                IsChangeOrderAllowed = (value.Properties["MasterNumber"] != null && (value.Status.StartsWith("Confirmed"))), // if we have a master number (invoice #) and a confirmed status
                Status = System.Text.RegularExpressions.Regex.Replace(value.Status, "([a-z])([A-Z])", "$1 $2"),
                RequestedShipDate = value.Properties["RequestedShipDate"].ToString(),
                InvoiceStatus = "N/A",
				Items = value.Properties["LineItems"] == null ? null : ((CommerceServer.Foundation.CommerceRelationshipList)value.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem)l.Target)).ToList(),
                CommerceId = Guid.Parse(value.Id),
				PONumber = value.Properties["PONumber"] == null ? string.Empty : value.Properties["PONumber"].ToString(),
				OrderSystem = new OrderSource().Parse("B").ToString()
            };
            
            retVal.ItemCount = retVal.Items == null ? 0 : retVal.Items.Count;

            return retVal;
        }

		public static Order ToOrderHeader(this CS.PurchaseOrder value)
		{
			Order retVal = new Order()
			{
				CreatedDate = DateTime.Parse(value.Properties["DateCreated"].ToString()),
				OrderNumber = value.Properties["OrderNumber"].ToString(),
				OrderTotal = Double.Parse(value.Properties["Total"].ToString()),
				InvoiceNumber = value.Properties["MasterNumber"] == null ? "Pending" : value.Properties["MasterNumber"].ToString(),
				IsChangeOrderAllowed = (value.Properties["MasterNumber"] != null && (value.Status.StartsWith("Confirmed"))), // if we have a master number (invoice #) and a confirmed status
				Status = System.Text.RegularExpressions.Regex.Replace(value.Status, "([a-z])([A-Z])", "$1 $2"),
				RequestedShipDate = value.Properties["RequestedShipDate"].ToString(),
				InvoiceStatus = "N/A",
				CommerceId = Guid.Parse(value.Id)
			};

			retVal.ItemCount = retVal.Items == null ? 0 : retVal.Items.Count;

			return retVal;
		}

        private static OrderLine ToOrderLine(this CS.LineItem lineItem)
        {
            OrderLine ol = new OrderLine()
            {
                ItemNumber = lineItem.ProductId,
                Quantity = (short)lineItem.Quantity,
                Price = (double)lineItem.PlacedPrice,
                QuantityOrdered = lineItem.Properties["QuantityOrdered"] == null ? 0 : (int)lineItem.Properties["QuantityOrdered"],
                QantityShipped = lineItem.Properties["QuantityShipped"] == null ? 0 : (int)lineItem.Properties["QuantityShipped"],
                SubstitutedItemNumber = lineItem.Properties["SubstitutedItemNumber"] == null ? null : (string)lineItem.Properties["SubstitutedItemNumber"],
                MainFrameStatus = lineItem.Properties["MainFrameStatus"] == null ? null : (string)lineItem.Properties["MainFrameStatus"],
                Each = (bool)lineItem.Properties["Each"],
                ChangeOrderStatus = (lineItem.Status != null && (lineItem.Status.Equals("deleted", StringComparison.InvariantCultureIgnoreCase))) ? "deleted" : "",
            };
            int ln;
            int.TryParse(lineItem.LinePosition, out ln);
            if (ln > 0) ol.LineNumber = ln;
            return ol;
        }
    }
}
