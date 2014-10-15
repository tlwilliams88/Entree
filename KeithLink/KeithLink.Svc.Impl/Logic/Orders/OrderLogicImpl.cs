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

namespace KeithLink.Svc.Impl.Logic.Orders
{
	public class OrderLogicImpl: IOrderLogic
	{
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly ICatalogRepository catalogRepository;
		private readonly IItemNoteLogic itemNoteLogic;


		public OrderLogicImpl(IPurchaseOrderRepository purchaseOrderRepository, ICatalogRepository catalogRepository,
			IItemNoteLogic itemNoteLogic)
		{
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.catalogRepository = catalogRepository;
			this.itemNoteLogic = itemNoteLogic;
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
				LineItems = ((CommerceServer.Foundation.CommerceRelationshipList)purchaseOrder.Properties["LineItems"]).Select(l => ToOrderLine((CS.LineItem)l.Target)).ToList()
			};
		}

		private OrderLine ToOrderLine(CS.LineItem lineItem)
		{
			return new OrderLine()
			{
				ItemNumber = lineItem.ProductId,
				Quantity = (short)lineItem.Quantity,
				Price = (double)lineItem.PlacedPrice,
			};
		}

		private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, Order order)
		{
			if (order.LineItems == null)
				return;

			var products = catalogRepository.GetProductsByIds(catalogInfo.BranchId, order.LineItems.Select(l => l.ItemNumber).ToList());
			var notes = itemNoteLogic.ReadNotes(user.UserId);

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
					item.Notes = note.First().Note;
			});
		}
	}
}
