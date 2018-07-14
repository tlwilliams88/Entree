﻿using Entree.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Orders
{
	public interface IPurchaseOrderRepository
	{
		PurchaseOrder ReadPurchaseOrder(Guid customerId, string orderNumber);

        PurchaseOrder ReadPurchaseOrderByTrackingNumber(string confirmationNumber);

        List<PurchaseOrder> ReadPurchaseOrderHeadersByCustomerId(Guid customerId);

        List<PurchaseOrder> GetPurchaseOrdersByStatus(string queryStatus);

        string UpdatePurchaseOrder(PurchaseOrder order);
        
        string SubmitChangeOrder(Guid userId, Guid orderGroupId);

		List<PurchaseOrder> ReadPurchaseOrderHeadersInDateRange(Guid customerId, string customerNumber, DateTime startDate, DateTime endDate);
	}
}
