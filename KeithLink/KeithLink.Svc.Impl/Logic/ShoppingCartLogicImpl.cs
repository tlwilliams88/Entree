
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic
{
	public class ShoppingCartLogicImpl: IShoppingCartLogic
	{
		private readonly IBasketRepository basketRepository;
		private readonly ICatalogRepository catalogRepository;
		private readonly IPriceLogic priceLogic;
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly IQueueRepository queueRepository;
		private readonly IItemNoteLogic itemNoteLogic;

		private readonly string BasketStatus = "ShoppingCart";

		public ShoppingCartLogicImpl(IBasketRepository basketRepository, ICatalogRepository catalogRepository, IPriceLogic priceLogic, 
			IPurchaseOrderRepository purchaseOrderRepository, IQueueRepository queueRepository, IItemNoteLogic itemNoteLogic)
		{
			this.basketRepository = basketRepository;
			this.catalogRepository = catalogRepository;
			this.priceLogic = priceLogic;
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.queueRepository = queueRepository;
			this.itemNoteLogic = itemNoteLogic;
		}

		public Guid CreateCart(UserProfile user, CatalogInfo catalogInfo, ShoppingCart cart)
		{
			var newBasket = new CS.Basket();
			newBasket.BranchId = catalogInfo.BranchId.ToLower();
			newBasket.DisplayName = cart.Name;
			newBasket.Status = BasketStatus;
			newBasket.Name = cart.FormattedName(catalogInfo.BranchId.ToLower());

			if(cart.Active)
				MarkCurrentActiveCartAsInactive(user, catalogInfo.BranchId.ToLower());

			newBasket.Active = cart.Active;
			newBasket.RequestedShipDate = cart.RequestedShipDate;

			return basketRepository.CreateOrUpdateBasket(user.UserId, catalogInfo.BranchId.ToLower(), newBasket, cart.Items.Select(l => l.ToLineItem(catalogInfo.BranchId.ToLower())).ToList());
		}

		public Guid? AddItem(UserProfile user, Guid cartId, ShoppingCartItem newItem)
		{
			var basket = basketRepository.ReadBasket(user.UserId, cartId);

			if (basket == null)
				return null;

			//Does item already exist? If so, just update the quantity
			var existingItem = basket.LineItems.Where(l => l.ProductId.Equals(newItem.ItemNumber));
			if (existingItem.Any())
			{
				existingItem.First().Quantity += newItem.Quantity;
				basketRepository.UpdateItem(user.UserId, cartId, existingItem.First());
				return existingItem.First().Id.ToGuid();
			}
						
			return basketRepository.AddItem(user.UserId, cartId, newItem.ToLineItem(basket.BranchId), basket);
		}

		public void UpdateItem(UserProfile user, Guid cartId, ShoppingCartItem updatedItem)
		{
			var basket = basketRepository.ReadBasket(user.UserId, cartId);
			if (basket == null)
				return;

			basketRepository.UpdateItem(user.UserId, cartId, updatedItem.ToLineItem(basket.BranchId));
		}

		public void UpdateCart(UserProfile user, ShoppingCart cart, bool deleteOmmitedItems)
		{
			var updateCart = basketRepository.ReadBasket(user.UserId, cart.CartId);
			
			if (updateCart == null)
				return;

			updateCart.DisplayName = cart.Name;
			updateCart.Name = cart.FormattedName(updateCart.BranchId);

			if (cart.Active && (updateCart.Active.HasValue && !updateCart.Active.Value))
			{
				MarkCurrentActiveCartAsInactive(user, updateCart.BranchId);
			}

			updateCart.Active = cart.Active;
			updateCart.RequestedShipDate = cart.RequestedShipDate;

			var itemsToRemove = new List<Guid>();
			var lineItems = new List<CS.LineItem>();

			if (cart.Items != null)
			{
				itemsToRemove = updateCart.LineItems.Where(b => !cart.Items.Any(c => c.CartItemId.ToCommerceServerFormat().Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();
				foreach (var item in cart.Items)
				{
					var existingItem = updateCart.LineItems.Where(l => l.ProductId.Equals(item.ItemNumber));
					if (existingItem.Any() && item.CartItemId == Guid.Empty)
					{
						existingItem.First().Quantity += item.Quantity;
						lineItems.Add(existingItem.First());
					}	
					else
						lineItems.Add(item.ToLineItem(updateCart.BranchId));
				}
				
			}
			
			basketRepository.CreateOrUpdateBasket(user.UserId, updateCart.BranchId, updateCart, lineItems);

			if(deleteOmmitedItems)
				foreach (var toDelete in itemsToRemove)
				{
					basketRepository.DeleteItem(user.UserId, cart.CartId, toDelete);
				}
		}

		public void DeleteCart(UserProfile user, Guid cartId)
		{
			basketRepository.DeleteBasket(user.UserId, cartId);
		}

		public void DeleteItem(UserProfile user, Guid cartId, Guid itemId)
		{
			basketRepository.DeleteItem(user.UserId, cartId, itemId);
		}

		public List<ShoppingCart> ReadAllCarts(UserProfile user, CatalogInfo catalogInfo, bool headerInfoOnly)
		{
			var lists = basketRepository.ReadAllBaskets(user.UserId);
			var listForBranch = lists.Where(b => b.BranchId.Equals(catalogInfo.BranchId.ToLower()) && b.Status.Equals(BasketStatus));
			if (headerInfoOnly)
				return listForBranch.Select(l => new ShoppingCart() { CartId = l.Id.ToGuid(), Name = l.DisplayName }).ToList();
			else
			{
				var returnCart = listForBranch.Select(b => ToShoppingCart(b)).ToList();
				returnCart.ForEach(delegate(ShoppingCart list)
				{
					LookupProductDetails(user, list);
				});
				return returnCart;
			}
		}

		public ShoppingCart ReadCart(UserProfile user, Guid cartId)
		{
			var basket = basketRepository.ReadBasket(user.UserId, cartId);
			if (basket == null)
				return null;

			var cart = ToShoppingCart(basket);

			LookupProductDetails(user, cart);
			return cart;
		}

		#region Helper Methods

		private void MarkCurrentActiveCartAsInactive(UserProfile user, string branchId)
		{
			var currentlyActiveCart = basketRepository.ReadAllBaskets(user.UserId).Where(b => b.BranchId.Equals(branchId) && b.Active.Equals(true)).FirstOrDefault();

			if (currentlyActiveCart != null)
			{
				currentlyActiveCart.Active = false;
				basketRepository.CreateOrUpdateBasket(user.UserId, currentlyActiveCart.BranchId, currentlyActiveCart, currentlyActiveCart.LineItems);
			}
		}
		
		private void LookupProductDetails(UserProfile user, ShoppingCart cart)
		{
			if (cart.Items == null)
				return;

			var products = catalogRepository.GetProductsByIds(cart.BranchId, cart.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var pricing = priceLogic.GetPrices(user.BranchId, user.CustomerNumber, DateTime.Now.AddDays(1), products.Products);
			var notes = itemNoteLogic.ReadNotes(user.UserId);

			cart.Items.ForEach(delegate(ShoppingCartItem item)
			{

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
					item.CNDoc = prod.CNDoc;
				}
				if (price != null)
				{
					item.PackagePrice = price.PackagePrice.ToString();
					item.CasePrice = price.CasePrice.ToString();
					
				}
				if (note.Any())
					item.Notes = note.First().Note;
			});

		}

		private ShoppingCart ToShoppingCart(CS.Basket basket)
		{
			return new ShoppingCart()
			{
				CartId = basket.Id.ToGuid(),
				Name = basket.DisplayName,
				BranchId = basket.BranchId,
				RequestedShipDate = basket.RequestedShipDate,
				Active = basket.Active.HasValue ? basket.Active.Value : false,
				Items = basket.LineItems.Select(l => new ShoppingCartItem()
				{
					ItemNumber = l.ProductId,
					CartItemId = l.Id.ToGuid(),
					Notes = l.Notes,
					Quantity = l.Quantity.HasValue ? l.Quantity.Value : 0,
					Each = l.Each.HasValue ? l.Each.Value : false
				}).ToList()
			};

		}

		#endregion


		public string SaveAsOrder(UserProfile user, Guid cartId)
		{
			//Check that RequestedShipDate
			var basket = basketRepository.ReadBasket(user.UserId, cartId);

			if (basket.RequestedShipDate == null)
				throw new ApplicationException("Requested Ship Date is required before submitting an order");

			//Save to Commerce Server
			com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
			var orderNumber = client.SaveCartAsOrder(user.UserId, cartId);

			var newPurchaseOrder = purchaseOrderRepository.ReadPurchaseOrder(user.UserId, orderNumber);

			var newOrderFile = new OrderFile()
			{
				Header = new OrderHeader()
				{
					OrderingSystem = OrderSource.KeithCom,
					Branch = newPurchaseOrder.Properties["BranchId"].ToString(),
					CustomerNumber = user.CustomerNumber,
					DeliveryDate = newPurchaseOrder.Properties["RequestedShipDate"].ToString().ToDateTime().Value,
					PONumber = string.Empty,
                    Specialinstructions = string.Empty,
					ControlNumber = int.Parse(orderNumber),
					OrderType = OrderType.NormalOrder,
                    InvoiceNumber = string.Empty,
					OrderCreateDateTime = newPurchaseOrder.Properties["DateCreated"].ToString().ToDateTime().Value,
					OrderSendDateTime = DateTime.Now,
					UserId = user.EmailAddress,
					OrderFilled = false,
                    FutureOrder = false
				},
				Details = new List<OrderDetail>()
			};

			foreach (var lineItem in ((CommerceServer.Foundation.CommerceRelationshipList)newPurchaseOrder.Properties["LineItems"]))
			{
				var item = (CS.LineItem)lineItem.Target;

				newOrderFile.Details.Add(new OrderDetail()
				{
					ItemNumber = item.ProductId,
					OrderedQuantity = (short)item.Quantity,
                    UnitOfMeasure = ((bool)item.Each ? Core.Models.Orders.UnitOfMeasure.Package : Core.Models.Orders.UnitOfMeasure.Case),
					SellPrice = (double)item.PlacedPrice,
                    Catchweight = (bool)item.CatchWeight,
					LineNumber = (short)(newOrderFile.Details.Count + 1),
					ItemChange = LineType.Add,
                    SubOriginalItemNumber = string.Empty,
                    ReplacedOriginalItemNumber = string.Empty,
                    ItemStatus = string.Empty
				});
							
			}	

			
			System.IO.StringWriter sw = new System.IO.StringWriter();
			System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(newOrderFile.GetType());

			xs.Serialize(sw, newOrderFile);
			
			queueRepository.PublishToQueue(sw.ToString());
						
			return orderNumber; //Return actual order number
		}

		
	}
}
