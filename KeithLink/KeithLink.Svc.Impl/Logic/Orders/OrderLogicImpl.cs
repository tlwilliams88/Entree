﻿using KeithLink.Svc.Core.Interface.Orders;
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
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Enumerations.Order;

namespace KeithLink.Svc.Impl.Logic.Orders
{
	public class OrderLogicImpl: IOrderLogic
	{
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly ICatalogLogic catalogLogic;
		private IListServiceRepository listServiceRepository;
		private readonly IOrderQueueLogic orderQueueLogic;
        private IPriceLogic priceLogic;
        private IEventLogRepository eventLogRepository;

		public OrderLogicImpl(IPurchaseOrderRepository purchaseOrderRepository, ICatalogLogic catalogLogic,
            IListServiceRepository listServiceRepository, IOrderQueueLogic orderQueueLogic, IPriceLogic priceLogic, IEventLogRepository eventLogRepository)
		{
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.catalogLogic = catalogLogic;
			this.listServiceRepository = listServiceRepository;
            this.orderQueueLogic = orderQueueLogic;
            this.priceLogic = priceLogic;
            this.eventLogRepository = eventLogRepository;
		}

		public List<Order> ReadOrders(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true)
		{
			var orders = purchaseOrderRepository.ReadPurchaseOrders(userProfile.UserId, catalogInfo.CustomerId);

			var returnOrders = orders.Select(p => ToOrder(p)).ToList();
			var notes = listServiceRepository.ReadNotes(userProfile, catalogInfo);
            
			returnOrders.ForEach(delegate(Order order)
			{
				LookupProductDetails(userProfile, catalogInfo, order, notes);
                if (omitDeletedItems)
                    order.Items = order.Items.Where(x => x.Status != "deleted").ToList();
			});

			return returnOrders;
		}

		public Core.Models.Orders.Order ReadOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber, bool omitDeletedItems = true)
		{
			var order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, orderNumber);
			var returnOrder = ToOrder(order);
			var notes = listServiceRepository.ReadNotes(userProfile, catalogInfo);
            
			LookupProductDetails(userProfile, catalogInfo, returnOrder, notes);

            // handel special change order logic to hide deleted line items
            if (returnOrder.IsChangeOrderAllowed && omitDeletedItems) // change order eligible - remove lines marked as 'deleted'
                returnOrder.Items = returnOrder.Items.Where(x => x.Status != "deleted").ToList();
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
                IsChangeOrderAllowed = (purchaseOrder.Properties["MasterNumber"] != null && (purchaseOrder.Status.StartsWith("Confirmed"))), // if we have a master number (invoice #) and a confirmed status
                Status = System.Text.RegularExpressions.Regex.Replace(purchaseOrder.Status, "([a-z])([A-Z])", "$1 $2"),
                RequestedShipDate = purchaseOrder.Properties["RequestedShipDate"] == null ? DateTime.Now : (DateTime)purchaseOrder.Properties["RequestedShipDate"],
				Items = ((CommerceServer.Foundation.CommerceRelationshipList)purchaseOrder.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem)l.Target)).ToList(),
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
                QantityShipped = lineItem.Properties["QuantityShipped"] == null ? 0 : (int)lineItem.Properties["QuantityShipped"],
                Status = lineItem.Status,
                SubstitutedItemNumber = lineItem.Properties["SubstitutedItemNumber"] == null ? null : (string)lineItem.Properties["SubstitutedItemNumber"],
                MainFrameStatus = lineItem.Properties["MainFrameStatus"] == null ? null : (string)lineItem.Properties["MainFrameStatus"],
                Each = (bool)lineItem.Properties["Each"]
			};
		}

		private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, Order order, List<KeithLink.Svc.Core.Models.Lists.ListItemModel> notes)
		{
			if (order.Items == null)
				return;

			var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, order.Items.Select(l => l.ItemNumber).ToList(), user);
			var pricing = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products);

			Parallel.ForEach(order.Items, item => {
				var prod = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				var price = pricing.Prices.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
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
                    item.CatchWeight = prod.CatchWeight;
                    item.TempZone = prod.TempZone;
				}
				if (price != null)
				{
					item.PackagePrice = price.PackagePrice.ToString();
					item.CasePrice = price.CasePrice.ToString();
				}
				if (note.Any())
					item.Notes = notes.Where(n => n.ItemNumber.Equals(prod.ItemNumber)).Select(i => i.Notes).FirstOrDefault();
			});

		}


        
		public Core.Models.Orders.Order UpdateOrder(UserSelectedContext catalogInfo, UserProfile user, Order order, bool deleteOmmitedItems)
        {
            Order existingOrder = this.ReadOrder(user, catalogInfo, order.OrderNumber, false);
			var notes = listServiceRepository.ReadNotes(user, catalogInfo);
            
            LookupProductDetails(user, catalogInfo, order, notes);
            UpdateExistingOrderInfo(order, existingOrder);
            
            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            List<com.benekeith.FoundationService.PurchaseOrderLineItemUpdate> itemUpdates = new List<com.benekeith.FoundationService.PurchaseOrderLineItemUpdate>();

            foreach (OrderLine line in existingOrder.Items)
            {
                itemUpdates.Add(new com.benekeith.FoundationService.PurchaseOrderLineItemUpdate() { ItemNumber = line.ItemNumber, Quantity = line.Quantity, Status = line.Status, Catalog = catalogInfo.BranchId, Each = line.Each, CatchWeight = line.CatchWeight  });
            }
            var orderNumber = client.UpdatePurchaseOrder(user.UserId, existingOrder.CommerceId, order.RequestedShipDate, itemUpdates.ToArray());

            return this.ReadOrder(user, catalogInfo, order.OrderNumber);
        }

        private void UpdateExistingOrderInfo(Order order, Order existingOrder)
        {
            // work through adds, deletes, changes based on item number
            foreach (OrderLine newLine in order.Items)
            {
                OrderLine existingLine = existingOrder.Items.Where(x => x.ItemNumber == newLine.ItemNumber).FirstOrDefault();
                if (existingLine != null)
                { // compare and update if necessary
                    if (existingLine.Quantity != newLine.Quantity || existingLine.Each != newLine.Each)
                    {
                        existingLine.Quantity = newLine.Quantity;
                        existingLine.Each = newLine.Each;
                        existingLine.Status = "changed";
                    }
                }
                else
                { // new line
                    existingOrder.Items.Add(new OrderLine() { ItemNumber = newLine.ItemNumber, Quantity = newLine.Quantity, Status = "added" });
                }
            }
            // handle deletes
            foreach (OrderLine existingLine in existingOrder.Items)
            {
                OrderLine newLine = order.Items.Where(x => x.ItemNumber == existingLine.ItemNumber).FirstOrDefault();
                if (newLine == null)
                {
                    existingLine.Status = "deleted";
                    eventLogRepository.WriteInformationLog("Deleting line: " + existingLine.ItemNumber);
                }
            }
        }

        public NewOrderReturn SubmitChangeOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber)
        {
            CS.PurchaseOrder order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, orderNumber); // TODO: incorporate multi user query

            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            string newOrderNumber = client.SaveOrderAsChangeOrder(userProfile.UserId, Guid.Parse(order.Id));

            order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, newOrderNumber);

            orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, newOrderNumber, order, OrderType.ChangeOrder);

            client.CleanUpChangeOrder(userProfile.UserId, Guid.Parse(order.Id));

            return new NewOrderReturn() { OrderNumber = newOrderNumber };
        }

        public NewOrderReturn CancelOrder(UserProfile userProfile, UserSelectedContext catalogInfo, Guid commerceId)
        {
            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            string newOrderNumber = client.CancelPurchaseOrder(userProfile.UserId, commerceId);
            CS.PurchaseOrder order = purchaseOrderRepository.ReadPurchaseOrder(userProfile.UserId, newOrderNumber);
            orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, newOrderNumber, order, OrderType.DeleteOrder);
            return new NewOrderReturn() { OrderNumber = newOrderNumber };
        }

    }
}
