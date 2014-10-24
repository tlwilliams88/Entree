using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Enumerations.Order;

namespace KeithLink.Svc.Impl.Logic.Orders
{
	public class OrderLogicImpl: IOrderLogic
	{
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly ICatalogRepository catalogRepository;
		private IListServiceRepository listServiceRepository;
		private readonly IQueueRepository queueRepository;

		public OrderLogicImpl(IPurchaseOrderRepository purchaseOrderRepository, ICatalogRepository catalogRepository,
			IListServiceRepository listServiceRepository, IQueueRepository queueRepository)
		{
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.catalogRepository = catalogRepository;
			this.listServiceRepository = listServiceRepository;
            this.queueRepository = queueRepository;
		}

		public List<Order> ReadOrders(UserProfile userProfile, UserSelectedContext catalogInfo)
		{
			var orders = purchaseOrderRepository.ReadPurchaseOrders(userProfile.UserId, catalogInfo.CustomerId);

			var returnOrders = orders.Select(p => ToOrder(p)).ToList();
			returnOrders.ForEach(delegate(Order order)
			{
				LookupProductDetails(userProfile, catalogInfo, order);
			});

			return returnOrders;
		}

		public Core.Models.Orders.Order ReadOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber)
		{
			var order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, orderNumber);
			var returnOrder = ToOrder(order);
			LookupProductDetails(userProfile, catalogInfo, returnOrder);
			return returnOrder;
		}

		private Order ToOrder(CS.PurchaseOrder purchaseOrder)
		{
			return new Order()
			{
				CreatedDate = purchaseOrder.Properties["DateCreated"].ToString().ToDateTime().Value,
				OrderNumber = purchaseOrder.Properties["OrderNumber"].ToString(),
				OrderTotal = purchaseOrder.Properties["Total"].ToString().ToDouble().Value,
                InvoiceNumber = purchaseOrder.Properties["MasterNumber"] == null ? string.Empty : purchaseOrder.Properties["MasterNumber"].ToString(),
                IsCangeOrderAllowed = (purchaseOrder.Properties["MasterNumber"] != null && (purchaseOrder.Status == "NewOrder" || purchaseOrder.Status == "Submitted")),
                Status = purchaseOrder.Status,
                RequestedShipDate = DateTime.Now, // TODO: wire up actual requested ship date
				LineItems = ((CommerceServer.Foundation.CommerceRelationshipList)purchaseOrder.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem)l.Target)).ToList(),
                CommerceId = Guid.Parse(purchaseOrder.Id)
			};
		}

		private OrderLine ToOrderLine(CS.LineItem lineItem)
		{
			return new OrderLine()
			{
				ItemNumber = lineItem.ProductId,
				Quantity = (short)lineItem.Quantity,
				Price = (double)lineItem.PlacedPrice,
                QuantityOrdered = lineItem.Properties["QuantityOrdered"] == null ? 0 : (int)lineItem.Properties["QuantityOrdered"],
                QantityShipped = lineItem.Properties["QuantityShipped"] == null ? 0 : (int)lineItem.Properties["QuantityShipped"]
			};
		}

		private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, Order order)
		{
			if (order.LineItems == null)
				return;

			var products = catalogRepository.GetProductsByIds(catalogInfo.BranchId, order.LineItems.Select(l => l.ItemNumber).ToList());
			var notes = listServiceRepository.ReadNotes(user, catalogInfo);

			order.LineItems.ForEach(delegate(OrderLine item)
			{

				var prod = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				var note = notes.Where(n => n.ItemNumber.Equals(item.ItemNumber));
				if (prod != null)
				{
					item.Name = prod.Name;
					item.PackSize = string.Format("{0} / {1}", prod.Pack, prod.Size);
					item.StorageTemp = prod.Nutritional.StorageTemp;
					item.Brand = prod.Brand;
					item.ReplacedItem = prod.ReplacedItem;
					item.ReplacementItem = prod.ReplacementItem;
					item.NonStock = prod.NonStock;
					item.ChildNutrition = prod.ChildNutrition;
				}
				
				if (note.Any())
					item.Notes = notes.Where(n => n.ItemNumber.Equals(prod.ItemNumber)).Select(i => i.Notes).FirstOrDefault();
			});
		}


        
		public Core.Models.Orders.Order UpdateOrder(UserSelectedContext catalogInfo, UserProfile user, Order order, bool deleteOmmitedItems)
        {
            Order existingOrder = this.ReadOrder(user, catalogInfo, order.OrderNumber);

            UpdateExistingOrderInfo(order, existingOrder);
            
            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            List<com.benekeith.FoundationService.PurchaseOrderLineItemUpdate> itemUpdates = new List<com.benekeith.FoundationService.PurchaseOrderLineItemUpdate>();

            foreach (OrderLine line in existingOrder.LineItems)
            {
                itemUpdates.Add(new com.benekeith.FoundationService.PurchaseOrderLineItemUpdate() { ItemNumber = line.ItemNumber, Quantity = line.Quantity, Status = line.Status, Catalog = catalogInfo.BranchId });
            }
            var orderNumber = client.UpdatePurchaseOrder(user.UserId, existingOrder.CommerceId, order.RequestedShipDate, itemUpdates.ToArray());

            return this.ReadOrder(user, catalogInfo, order.OrderNumber);
        }

        private static void UpdateExistingOrderInfo(Order order, Order existingOrder)
        {
            // work through adds, deletes, changes based on item number
            foreach (OrderLine newLine in order.LineItems)
            {
                OrderLine existingLine = existingOrder.LineItems.Where(x => x.ItemNumber == newLine.ItemNumber).FirstOrDefault();
                if (existingLine != null)
                { // compare and update if necessary
                    if (existingLine.Quantity != newLine.Quantity)
                    {
                        existingLine.Quantity = newLine.Quantity;
                        existingLine.Status = "changed";
                    }
                }
                else
                { // new line
                    existingOrder.LineItems.Add(new OrderLine() { ItemNumber = newLine.ItemNumber, Quantity = newLine.Quantity, Status = "added" });
                }
            }
            // handle deletes
            foreach (OrderLine existingLine in order.LineItems)
            {
                OrderLine newLine = order.LineItems.Where(x => x.ItemNumber == existingLine.ItemNumber).FirstOrDefault();
                if (newLine == null)
                {
                    existingLine.Status = "deleted";
                }
            }
        }

        public NewOrderReturn SubmitChangeOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber)
        {
            CS.PurchaseOrder order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, orderNumber); // TODO: incorporate multi user query

            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            string newOrderNumber = client.SaveOrderAsChangeOrder(userProfile.UserId, Guid.Parse(order.Id));

            order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, orderNumber);

            WriteOrderFileToQueue(userProfile, newOrderNumber, order);

            client.CleanUpChangeOrder(userProfile.UserId, Guid.Parse(order.Id));

            return new NewOrderReturn() { OrderNumber = newOrderNumber };
        }

        private void WriteOrderFileToQueue(UserProfile user, string controlNumber, CS.PurchaseOrder newPurchaseOrder)
        {
            var newOrderFile = new OrderFile()
            {
                Header = new OrderHeader()
                {
                    OrderingSystem = OrderSource.Entree,
                    Branch = newPurchaseOrder.Properties["BranchId"].ToString().ToUpper(),
                    CustomerNumber = newPurchaseOrder.Properties["CustomerId"].ToString(),
                    DeliveryDate = newPurchaseOrder.Properties["RequestedShipDate"].ToString().ToDateTime().Value,
                    PONumber = string.Empty,
                    Specialinstructions = string.Empty,
                    ControlNumber = int.Parse(controlNumber),
                    OrderType = OrderType.NormalOrder,
                    InvoiceNumber = string.Empty,
                    OrderCreateDateTime = newPurchaseOrder.Properties["DateCreated"].ToString().ToDateTime().Value,
                    OrderSendDateTime = DateTime.Now,
                    UserId = user.EmailAddress.ToUpper(),
                    OrderFilled = false,
                    FutureOrder = false
                },
                Details = new List<OrderDetail>()
            };

            foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)newPurchaseOrder.Properties["LineItems"]))
            {
                var item = (CS.LineItem)lineItem.Target;

                if (item.Status == null || String.IsNullOrEmpty(item.Status))
                    continue;

                newOrderFile.Details.Add(new OrderDetail()
                {
                    ItemNumber = item.ProductId,
                    OrderedQuantity = (short)item.Quantity,
                    UnitOfMeasure = ((bool)item.Each ? UnitOfMeasure.Package : UnitOfMeasure.Case),
                    SellPrice = (double)item.PlacedPrice,
                    Catchweight = (bool)item.CatchWeight,
                    //Catchweight = false,
                    LineNumber = Convert.ToInt16(lineItem.Target.Properties["LinePosition"]),
                    ItemChange = LineType.Add,
                    SubOriginalItemNumber = string.Empty,
                    ReplacedOriginalItemNumber = string.Empty,
                    ItemStatus = item.Status == "added" ? "A" : item.Status == "changed" ? "C" : "D"
                });
            }


            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(newOrderFile.GetType());

            xs.Serialize(sw, newOrderFile);

            queueRepository.PublishToQueue(sw.ToString());
        }
    }
}
