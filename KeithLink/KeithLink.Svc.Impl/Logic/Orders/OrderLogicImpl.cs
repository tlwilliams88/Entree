using KeithLink.Common.Core.Extensions;
using KeithLink.Common.Core.Logging;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Profile;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Impl.Logic.Orders
{
	public class OrderLogicImpl: IOrderLogic
	{
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly ICatalogLogic catalogLogic;
		private IListServiceRepository listServiceRepository;
        private IOrderServiceRepository orderServiceRepository;
		private readonly IOrderQueueLogic orderQueueLogic;
        private IPriceLogic priceLogic;
        private IEventLogRepository eventLogRepository;
        private IUserProfileLogic userProfileLogic;
		private ICustomerRepository customerRepository;

		public OrderLogicImpl(IPurchaseOrderRepository purchaseOrderRepository, ICatalogLogic catalogLogic, IOrderServiceRepository orderServiceRepository,
            IListServiceRepository listServiceRepository, IOrderQueueLogic orderQueueLogic, IPriceLogic priceLogic, IEventLogRepository eventLogRepository,
			IUserProfileLogic userProfileLogic, ICustomerRepository customerRepository)
		{
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.catalogLogic = catalogLogic;
			this.listServiceRepository = listServiceRepository;
            this.orderServiceRepository = orderServiceRepository;
            this.orderQueueLogic = orderQueueLogic;
            this.priceLogic = priceLogic;
            this.eventLogRepository = eventLogRepository;
            this.userProfileLogic = userProfileLogic;
			this.customerRepository = customerRepository;
		}

		public List<Order> ReadOrders(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true, bool header = false)
		{
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            var orders = purchaseOrderRepository.ReadPurchaseOrders(customer.CustomerId, catalogInfo.CustomerId, false);

			var returnOrders = orders.Select(p => ToOrder(p, header)).ToList();
			var notes = listServiceRepository.ReadNotes(userProfile, catalogInfo);
            
			returnOrders.ForEach(delegate(Order order)
			{
				LookupProductDetails(userProfile, catalogInfo, order, notes);
                if (omitDeletedItems)
                    order.Items = order.Items.Where(x => x.MainFrameStatus != "deleted").ToList();
			});

			return returnOrders;
		}

        public List<Order> ReadOrderHistories(UserProfile userProfile, UserSelectedContext catalogInfo, bool omitDeletedItems = true)
        {
            var orders = orderServiceRepository.GetCustomerOrders(userProfile.UserId, catalogInfo);

            //var returnOrders = orders.Select(p => ToOrder(p)).ToList();
            var notes = listServiceRepository.ReadNotes(userProfile, catalogInfo);

            //return returnOrders;
            return orders;
        }

		public Core.Models.Orders.Order ReadOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber, bool omitDeletedItems = true)
		{
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

			var order = purchaseOrderRepository.ReadPurchaseOrder(customer.CustomerId, orderNumber);
			var returnOrder = ToOrder(order, false);
			var notes = listServiceRepository.ReadNotes(userProfile, catalogInfo);
            

			LookupProductDetails(userProfile, catalogInfo, returnOrder, notes);

            // handel special change order logic to hide deleted line items
            if (returnOrder.IsChangeOrderAllowed && omitDeletedItems) // change order eligible - remove lines marked as 'deleted'
                returnOrder.Items = returnOrder.Items.Where(x => x.ChangeOrderStatus != "deleted").ToList();
			return returnOrder;
		}

		private Order ToOrder(CS.PurchaseOrder purchaseOrder, bool headerOnly)
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
				Items = purchaseOrder.Properties["LineItems"] == null || headerOnly ? new List<OrderLine>() : ((CommerceServer.Foundation.CommerceRelationshipList)purchaseOrder.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem)l.Target)).ToList(),
				ItemCount = purchaseOrder.Properties["LineItems"] == null ? 0 : ((CommerceServer.Foundation.CommerceRelationshipList)purchaseOrder.Properties["LineItems"]).Count,
                CommerceId = Guid.Parse(purchaseOrder.Id),
			};
		}

        private Order ToOrder(Core.Models.Orders.History.OrderHistoryHeader orderHistoryHeader)
        {
            return new Order()
            {
                CreatedDate = orderHistoryHeader.DeliveryDate.HasValue ? orderHistoryHeader.DeliveryDate.Value : DateTime.Now,
                OrderNumber = orderHistoryHeader.ControlNumber,
                OrderTotal = orderHistoryHeader.Items.Sum(l => l.SellPrice),
                InvoiceNumber = orderHistoryHeader.InvoiceNumber,
                IsChangeOrderAllowed = false,
                Status = orderHistoryHeader.OrderStatus,
                RequestedShipDate = DateTime.UtcNow,
                Items = orderHistoryHeader.Items.Select(l => ToOrderLine(l)).ToList(),
                CommerceId = Guid.Empty, // could be orders from any system
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
                ChangeOrderStatus = lineItem.Status,
                SubstitutedItemNumber = lineItem.Properties["SubstitutedItemNumber"] == null ? null : (string)lineItem.Properties["SubstitutedItemNumber"],
                MainFrameStatus = lineItem.Properties["MainFrameStatus"] == null ? null : (string)lineItem.Properties["MainFrameStatus"],
                Each = (bool)lineItem.Properties["Each"]
			};
		}

        private OrderLine ToOrderLine(Core.Models.Orders.History.OrderHistoryDetail lineItem)
        {
            return new OrderLine()
            {
                ItemNumber = lineItem.ItemNumber,
                Quantity = (short)lineItem.ShippedQuantity,
                Price = (double)lineItem.SellPrice,
                QuantityOrdered = lineItem.OrderQuantity,
                QantityShipped = lineItem.ShippedQuantity,
                //Status = lineItem.ItemStatus,
                SubstitutedItemNumber = !String.IsNullOrEmpty(lineItem.ReplacedOriginalItemNumber.Trim()) ? lineItem.ReplacedOriginalItemNumber :
                    !String.IsNullOrEmpty(lineItem.SubbedOriginalItemNumber.Trim()) ? lineItem.SubbedOriginalItemNumber : string.Empty,
                MainFrameStatus = lineItem.ItemStatus,
                Each = lineItem.UnitOfMeasure == UnitOfMeasure.Package ? true : false
            };
        }

		private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, Order order, List<KeithLink.Svc.Core.Models.Lists.ListItemModel> notes)
		{
			if (order.Items == null)
				return;

			var products = catalogLogic.GetProductsByIds(catalogInfo.BranchId, order.Items.Select(l => l.ItemNumber).ToList());
			var pricing = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products);

			Parallel.ForEach(order.Items, item => {
				var prod = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				var price = pricing.Prices.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				var note = notes.Where(n => n.ItemNumber.Equals(item.ItemNumber));
				if (prod != null)
				{
					item.Name = prod.Name;
					item.Description = prod.Description;
                    item.Pack = prod.Pack;
                    item.Size = prod.Size;
					item.StorageTemp = prod.Nutritional.StorageTemp;
					item.Brand = prod.Brand;
					item.BrandExtendedDescription = prod.BrandExtendedDescription;
					item.ReplacedItem = prod.ReplacedItem;
					item.ReplacementItem = prod.ReplacementItem;
					item.NonStock = prod.NonStock;
					item.ChildNutrition = prod.ChildNutrition;
                    item.CatchWeight = prod.CatchWeight;
                    item.TempZone = prod.TempZone;
					item.ItemClass = prod.ItemClass;
					item.CategoryId = prod.CategoryId;
					item.CategoryName = prod.CategoryName;
					item.UPC = prod.UPC;
					item.VendorItemNumber = prod.VendorItemNumber;
					item.Cases = prod.Cases;
					item.Kosher = prod.Kosher;
					item.ManufacturerName = prod.ManufacturerName;
					item.ManufacturerNumber = prod.ManufacturerNumber;
					item.AverageWeight = prod.AverageWeight;
					item.Nutritional = new Nutritional()
					{
						CountryOfOrigin = prod.Nutritional.CountryOfOrigin,
						GrossWeight = prod.Nutritional.GrossWeight,
						HandlingInstructions = prod.Nutritional.HandlingInstructions,
						Height = prod.Nutritional.Height,
						Length = prod.Nutritional.Length,
						Ingredients = prod.Nutritional.Ingredients,
						Width = prod.Nutritional.Width
					};
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
            
            /*if (order.Items == null || order.ItemCount == 0)
            {
                throw new ApplicationException("Cannot submit an order with zero line items");
            }
             * */
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            Order existingOrder = this.ReadOrder(user, catalogInfo, order.OrderNumber, false);
			var notes = listServiceRepository.ReadNotes(user, catalogInfo);
            
            LookupProductDetails(user, catalogInfo, order, notes);
            UpdateExistingOrderInfo(order, existingOrder);
            
            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            List<com.benekeith.FoundationService.PurchaseOrderLineItemUpdate> itemUpdates = new List<com.benekeith.FoundationService.PurchaseOrderLineItemUpdate>();

            foreach (OrderLine line in existingOrder.Items)
            {
                //itemUpdates.Add(new com.benekeith.FoundationService.PurchaseOrderLineItemUpdate() { ItemNumber = line.ItemNumber, Quantity = line.Quantity, Status = line.Status, Catalog = catalogInfo.BranchId, Each = line.Each, CatchWeight = line.CatchWeight });
                itemUpdates.Add(new com.benekeith.FoundationService.PurchaseOrderLineItemUpdate() { ItemNumber = line.ItemNumber, Quantity = line.Quantity, Status = line.ChangeOrderStatus, Catalog = catalogInfo.BranchId, Each = line.Each, CatchWeight = line.CatchWeight });
            }
            var orderNumber = client.UpdatePurchaseOrder(customer.CustomerId, existingOrder.CommerceId, order.RequestedShipDate, itemUpdates.ToArray());

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
						if (!string.IsNullOrEmpty(existingLine.MainFrameStatus))//If this hasn't been sent to the mainframe yet, then it's still an add, not a change
						{
							existingLine.ChangeOrderStatus = "changed";
						}
                    }
                }
                else
                { // new line
					existingOrder.Items.Add(new OrderLine() { ItemNumber = newLine.ItemNumber, Quantity = newLine.Quantity, Each = newLine.Each,  ChangeOrderStatus = "added" });
                }
            }
            // handle deletes
            foreach (OrderLine existingLine in existingOrder.Items)
            {
                OrderLine newLine = order.Items.Where(x => x.ItemNumber == existingLine.ItemNumber).FirstOrDefault();
                if (newLine == null)
                {
					existingLine.ChangeOrderStatus = "deleted";
                    eventLogRepository.WriteInformationLog("Deleting line: " + existingLine.ItemNumber);
                }
            }
        }

        public NewOrderReturn SubmitChangeOrder(UserProfile userProfile, UserSelectedContext catalogInfo, string orderNumber)
        {
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            CS.PurchaseOrder order = purchaseOrderRepository.ReadPurchaseOrder(customer.CustomerId, orderNumber); // TODO: incorporate multi user query

            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
			string newOrderNumber = client.SaveOrderAsChangeOrder(customer.CustomerId, Guid.Parse(order.Id));

			order = purchaseOrderRepository.ReadPurchaseOrder(customer.CustomerId, newOrderNumber);

            orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, newOrderNumber, order, OrderType.ChangeOrder);

			client.CleanUpChangeOrder(customer.CustomerId, Guid.Parse(order.Id));

            return new NewOrderReturn() { OrderNumber = newOrderNumber };
        }

        public bool ResendUnconfirmedOrder(UserProfile userProfile, int controlNumber, UserSelectedContext catalogInfo)
        {
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            string controlNumberMainFrameFormat = controlNumber.ToString("0000000.##");
            Guid userId = orderServiceRepository.GetUserIdForControlNumber(controlNumber);
            CS.PurchaseOrder order = purchaseOrderRepository.ReadPurchaseOrder(customer.CustomerId, controlNumberMainFrameFormat);
            string originalOrderNumber = order.Properties["OriginalOrderNumber"].ToString();
            OrderType type = originalOrderNumber == controlNumberMainFrameFormat ? OrderType.NormalOrder : OrderType.ChangeOrder;
            orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, controlNumberMainFrameFormat, order, type); // TODO, logic to compare original order number and control number
            return true;
        }

        public NewOrderReturn CancelOrder(UserProfile userProfile, UserSelectedContext catalogInfo, Guid commerceId)
        {
			var customer = customerRepository.GetCustomerByCustomerNumber(catalogInfo.CustomerId, catalogInfo.BranchId);

            com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
            string newOrderNumber = client.CancelPurchaseOrder(customer.CustomerId, commerceId);
			CS.PurchaseOrder order = purchaseOrderRepository.ReadPurchaseOrder(customer.CustomerId, newOrderNumber);
            orderQueueLogic.WriteFileToQueue(userProfile.EmailAddress, newOrderNumber, order, OrderType.DeleteOrder);
            return new NewOrderReturn() { OrderNumber = newOrderNumber };
        }

        public Order UpdateOrderForEta(UserProfile user, Order order)
        {
            if (Configuration.EnableEtaForUsers.Equals("none", StringComparison.InvariantCultureIgnoreCase)
                || (Configuration.EnableEtaForUsers.Equals("internal_only", StringComparison.InvariantCultureIgnoreCase)
                    && !userProfileLogic.IsInternalAddress(user.EmailAddress))
                )
            {
                ClearEtaInformation(order);
            }
            return order;
        }
        
        public List<Order> UpdateOrdersForSecurity(UserProfile user, List<Order> orders)
        {
            Parallel.ForEach(orders, o => { this.UpdateOrderForEta(user, o); });
            return orders;
        }

        private static void ClearEtaInformation(Order order)
        {
            order.EstimatedDeliveryTime = null;
        }
    }
}
