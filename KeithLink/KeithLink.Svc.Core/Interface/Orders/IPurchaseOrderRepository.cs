using KeithLink.Svc.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
	public interface IPurchaseOrderRepository
	{
		PurchaseOrder ReadPurchaseOrder(Guid userId, string orderNumber);

        PurchaseOrder ReadPurchaseOrderByTrackingNumber(string confirmationNumber);

		List<PurchaseOrder> ReadPurchaseOrders(Guid userId, string customerId, bool header = false);
        
        string UpdatePurchaseOrder(PurchaseOrder order);
        
        string SubmitChangeOrder(Guid userId, Guid orderGroupId);

		List<PurchaseOrder> ReadPurchaseOrderHeadersInDateRange(Guid userId, string customerId, DateTime startDate, DateTime endDate);
	}
}
