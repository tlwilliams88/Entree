﻿using CommerceServer.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Interface
{
	[ServiceContract]
	public interface IBEKFoundationService : IOperationService
	{
		[OperationContract]
		string SaveCartAsOrder(Guid userId, Guid cartId);
            
		[OperationContract]
        string SaveOrderAsChangeOrder(Guid userId, Guid cartId);

        [OperationContract]
        string UpdatePurchaseOrder(Guid userId, Guid orderId, DateTime requestedShipDate, List<PurchaseOrderLineItemUpdate> itemUpdates);
	}

    public class PurchaseOrderLineItemUpdate
    {
        public string ItemNumber { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public string Catalog { get; set; }
    }
}