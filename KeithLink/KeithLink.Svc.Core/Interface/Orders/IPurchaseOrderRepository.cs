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

        PurchaseOrder ReadPurchaseOrderByInvoice(string branchid, string invoiceNumber);

		List<PurchaseOrder> ReadPurchaseOrders(Guid userId, string customerId);
        
        string UpdatePurchaseOrder(PurchaseOrder order);
        
        string SubmitChangeOrder(Guid userId, Guid orderGroupId);
	}
}
