using CommerceServer.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Web;

namespace KeithLink.Svc.FoundationSvc.Interface
{
	[ServiceContract]
	public interface IBEKFoundationService : IOperationService
	{
        [OperationContract]
        string CancelPurchaseOrder(Guid userId, Guid orderId);

        [OperationContract]
        void CleanUpChangeOrder(Guid userId, Guid cartId);

        [OperationContract]
        System.Xml.XmlElement GetUnconfirmatedOrders();

        [OperationContract]
        string SaveOrderAsChangeOrder(Guid userId, Guid cartId);
            
		[OperationContract]
		string SaveCartAsOrder(Guid userId, Guid cartId);

        [OperationContract]
        string UpdatePurchaseOrder(Guid userId, Guid orderId, DateTime requestedShipDate, List<PurchaseOrderLineItemUpdate> itemUpdates);

        [OperationContract]
        void UpdatePurchaseOrderStatus(Guid userId, Guid orderId, string status);
	}

    public class PurchaseOrderLineItemUpdate
    {
        public string ItemNumber { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public string Catalog { get; set; }
        public bool Each { get; set; }
        public bool CatchWeight { get; set; }
    }
}