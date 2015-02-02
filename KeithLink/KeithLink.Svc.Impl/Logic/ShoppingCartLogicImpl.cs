﻿using KeithLink.Common.Core.Extensions;
using KeithLink.Svc.Core.Enumerations.Order;
using KeithLink.Svc.Core.Enumerations.List;
using KeithLink.Svc.Core.Extensions;
using KeithLink.Svc.Core.Extensions.Orders;
using KeithLink.Svc.Core.Extensions.Orders.History;
using KeithLink.Svc.Core.Interface.Common;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Interface.Orders.History;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using CS = KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace KeithLink.Svc.Impl.Logic
{
	public class ShoppingCartLogicImpl: IShoppingCartLogic {
        #region attributes
        private readonly IBasketRepository basketRepository;
		private readonly ICatalogLogic catalogLogic;
		private readonly IPriceLogic priceLogic;
		private readonly IPurchaseOrderRepository purchaseOrderRepository;
		private readonly IGenericQueueRepository queueRepository;
		private readonly IBasketLogic basketLogic;
		private readonly IListServiceRepository listServiceRepository;
        private readonly IOrderQueueLogic orderQueueLogic;
		private readonly IOrderServiceRepository orderServiceRepository;
		#endregion

        #region ctor
        public ShoppingCartLogicImpl(IBasketRepository basketRepository, ICatalogLogic catalogLogic, IPriceLogic priceLogic,
									 IOrderQueueLogic orderQueueLogic, IPurchaseOrderRepository purchaseOrderRepository, IGenericQueueRepository queueRepository, 
                                     IListServiceRepository listServiceRepository, IBasketLogic basketLogic, IOrderServiceRepository orderServiceRepository) {
			this.basketRepository = basketRepository;
			this.catalogLogic = catalogLogic;
			this.priceLogic = priceLogic;
			this.purchaseOrderRepository = purchaseOrderRepository;
			this.queueRepository = queueRepository;
			this.listServiceRepository = listServiceRepository;
			this.basketLogic = basketLogic;
            this.orderQueueLogic = orderQueueLogic;
			this.orderServiceRepository = orderServiceRepository;
		}
        #endregion

        #region methods
		public Guid? AddItem(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, ShoppingCartItem newItem)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket == null)
				return null;

			//Does item already exist? If so, just update the quantity
			var existingItem = basket.LineItems.Where(l => l.ProductId.Equals(newItem.ItemNumber) && l.Each.Equals(newItem.Each));
			if (existingItem.Any())
			{
				existingItem.First().Quantity += newItem.Quantity;
				basketRepository.UpdateItem(basket.UserId.ToGuid(), cartId, existingItem.First());
				return existingItem.First().Id.ToGuid();
			}
						
			return basketRepository.AddItem(cartId, newItem.ToLineItem(basket.BranchId), basket);
		}

		private string CartName(string name, UserSelectedContext catalogInfo)
		{
			return string.Format("s{0}_{1}_{2}", catalogInfo.BranchId.ToLower(), catalogInfo.CustomerId, Regex.Replace(name, @"\s+", ""));
		}
        
        public Guid CreateCart(UserProfile user, UserSelectedContext catalogInfo, ShoppingCart cart)
		{
			var newBasket = new CS.Basket();
			newBasket.BranchId = catalogInfo.BranchId.ToLower();
			newBasket.DisplayName = cart.Name;
			newBasket.ListType = (int)BasketType.Cart;
			newBasket.Name = CartName(cart.Name, catalogInfo);
			newBasket.CustomerId = catalogInfo.CustomerId;
			newBasket.Shared = true;

			newBasket.RequestedShipDate = cart.RequestedShipDate;

			return basketRepository.CreateOrUpdateBasket(user.UserId, catalogInfo.BranchId.ToLower(), newBasket, cart.Items.Select(l => l.ToLineItem(catalogInfo.BranchId.ToLower())).ToList());
		}

		public void SetActive(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
		{
			//var cart = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
			
			//MarkCurrentActiveCartAsInactive(user, catalogInfo, catalogInfo.BranchId.ToLower());
			//cart.Active = true;
			//basketRepository.CreateOrUpdateBasket(cart.UserId.ToGuid(), cart.BranchId, cart, cart.LineItems);
		}

		
        public void DeleteCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket == null)
				return;

			basketRepository.DeleteBasket(basket.UserId.ToGuid(), cartId);
		}
        
        public void DeleteCarts(UserProfile user, UserSelectedContext catalogInfo, List<Guid> cartIds)
		{
			foreach (var cartId in cartIds)
			{
				var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
				if (basket != null)
					basketRepository.DeleteBasket(basket.UserId.ToGuid(), cartId);
			}
		}
		
        public void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, Guid cartId, Guid itemId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket == null)
				return;

			basketRepository.DeleteItem(basket.UserId.ToGuid(), cartId, itemId);
		}
        
		private void LookupProductDetails(UserProfile user, UserSelectedContext catalogInfo, ShoppingCart cart, List<KeithLink.Svc.Core.Models.Lists.ListItemModel> notes)
		{
			if (cart.Items == null)
				return;

			var products = catalogLogic.GetProductsByIds(cart.BranchId, cart.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var pricing = priceLogic.GetPrices(catalogInfo.BranchId, catalogInfo.CustomerId, DateTime.Now.AddDays(1), products.Products);

			var productHash = products.Products.ToDictionary(p => p.ItemNumber);
			var priceHash = pricing.Prices.ToDictionary(p => p.ItemNumber);
			var notesHash = new Dictionary<string, KeithLink.Svc.Core.Models.Lists.ListItemModel>();
			if (notes != null)
				notesHash = notes.ToDictionary(n => n.ItemNumber);
			

			Parallel.ForEach(cart.Items, item =>
			{
				var prod = productHash.ContainsKey(item.ItemNumber) ? productHash[item.ItemNumber] : null;
				var price = priceHash.ContainsKey(item.ItemNumber) ? priceHash[item.ItemNumber] : null;
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
                    item.AverageWeight = prod.AverageWeight;
				}
				if (price != null)
				{
					item.PackagePrice = price.PackagePrice.ToString();
					item.CasePrice = price.CasePrice.ToString();

				}

				item.Notes = notesHash.ContainsKey(item.ItemNumber) ? notesHash[item.ItemNumber].Notes : null;
			});			
			
		}
		
        private void MarkCurrentActiveCartAsInactive(UserProfile user, UserSelectedContext catalogInfo, string branchId)
		{
			//var currentlyActiveCart = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, BasketType.Cart).Where(b => b.BranchId.Equals(branchId) && b.Active.Equals(true)).FirstOrDefault();

			//if (currentlyActiveCart != null)
			//{
			//	currentlyActiveCart.Active = false;
			//	basketRepository.CreateOrUpdateBasket(currentlyActiveCart.UserId.ToGuid(), currentlyActiveCart.BranchId, currentlyActiveCart, currentlyActiveCart.LineItems);
			//}
		}

		public List<ShoppingCart> ReadAllCarts(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly)
		{
			if (String.IsNullOrEmpty(catalogInfo.CustomerId))
				return new List<ShoppingCart>();

			var lists = basketLogic.RetrieveAllSharedCustomerBaskets(user, catalogInfo, BasketType.Cart);

			var listForBranch = lists.Where(b => b.BranchId.Equals(catalogInfo.BranchId.ToLower()) &&
				!string.IsNullOrEmpty(b.CustomerId) &&
				b.CustomerId.Equals(catalogInfo.CustomerId));

			var userActiveCart = orderServiceRepository.GetUserActiveCart(catalogInfo, user.UserId);

			if (headerInfoOnly)
				return listForBranch.Select(l => new ShoppingCart() { CartId = l.Id.ToGuid(), Name = l.DisplayName, Active = userActiveCart != null && userActiveCart.CartId == l.Id.ToGuid() }).ToList();
			else
			{
				var returnCart = listForBranch.Select(b => ToShoppingCart(b, userActiveCart)).ToList();
				var notes = listServiceRepository.ReadNotes(user, catalogInfo);

				returnCart.ForEach(delegate(ShoppingCart list)
				{
					LookupProductDetails(user, catalogInfo, list, notes);
				});
				return returnCart;
			}
		}
		
		public ShoppingCart ReadCart(UserProfile user, UserSelectedContext catalogInfo, Guid cartId)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
			if (basket == null)
				return null;
			var userActiveCart = orderServiceRepository.GetUserActiveCart(catalogInfo, user.UserId);
			var cart = ToShoppingCart(basket, userActiveCart);
			var notes = listServiceRepository.ReadNotes(user, catalogInfo);

			LookupProductDetails(user, catalogInfo, cart, notes);
			return cart;
		}
        
        private void WriteOrderFileToQueue(UserProfile user, string controlNumber, CS.PurchaseOrder newPurchaseOrder)
        {
            
        }
		
        public NewOrderReturn SaveAsOrder(UserProfile user,  UserSelectedContext catalogInfo, Guid cartId)
		{
			//Check that RequestedShipDate
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);

			if (basket.RequestedShipDate == null)
				throw new ApplicationException("Requested Ship Date is required before submitting an order");
            if (basket.LineItems == null || basket.LineItems.Count == 0)
                throw new ApplicationException("Cannot submit order with 0 line items");

			//Save to Commerce Server
			com.benekeith.FoundationService.BEKFoundationServiceClient client = new com.benekeith.FoundationService.BEKFoundationServiceClient();
			var orderNumber = client.SaveCartAsOrder(basket.UserId.ToGuid(), cartId);
			var newPurchaseOrder = purchaseOrderRepository.ReadPurchaseOrder(basket.UserId.ToGuid(), orderNumber);

            orderServiceRepository.SaveOrderHistory(newPurchaseOrder.ToOrderHistoryFile(catalogInfo)); // save to order history
            orderQueueLogic.WriteFileToQueue(user.EmailAddress, orderNumber, newPurchaseOrder, OrderType.NormalOrder); // send to queue

			return new NewOrderReturn() { OrderNumber = orderNumber }; //Return actual order number
		}
        
		private ShoppingCart ToShoppingCart(CS.Basket basket, UserActiveCartModel activeCart)
		{
			return new ShoppingCart()
			{
				CartId = basket.Id.ToGuid(),
				Name = basket.DisplayName,
				BranchId = basket.BranchId,
				RequestedShipDate = basket.RequestedShipDate,
				Active = activeCart != null && activeCart.CartId == basket.Id.ToGuid(),
				PONumber = basket.PONumber,
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

        public void UpdateCart(UserSelectedContext catalogInfo, UserProfile user, ShoppingCart cart, bool deleteOmmitedItems)
		{
			var updateCart = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cart.CartId);
			
			if (updateCart == null)
				return;

			updateCart.DisplayName = cart.Name;
			updateCart.Name = CartName(cart.Name, catalogInfo);
						
			updateCart.RequestedShipDate = cart.RequestedShipDate;
			updateCart.PONumber = cart.PONumber;

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

			var duplicates = lineItems.Cast<CS.LineItem>().GroupBy(l => new { l.ProductId, l.Each }).Select(i => new { Ech = i.Select(p => p.Each).First(), Key = i.Key, Cnt = i.Count() }).Where(w => w.Cnt > 1).ToList();

			foreach (var duplicate in duplicates)
			{
				var keepGuid = lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals((bool)duplicate.Key.Each)).First();

				lineItems.Where(i => i.Id.Equals(keepGuid.Id)).First().Quantity = lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals(duplicate.Key.Each)).Sum(s => s.Quantity);

				itemsToRemove.AddRange(lineItems.Where(l => l.ProductId.Equals(duplicate.Key.ProductId) && l.Each.Equals(duplicate.Key.Each) && !l.Id.Equals(keepGuid.Id)).Select(p => p.Id.ToGuid()).ToList());

			}

			basketRepository.CreateOrUpdateBasket(updateCart.UserId.ToGuid(), updateCart.BranchId, updateCart, lineItems);

			if(deleteOmmitedItems)
				foreach (var toDelete in itemsToRemove)
				{
					basketRepository.DeleteItem(updateCart.UserId.ToGuid(), cart.CartId, toDelete);
				}

			
				

		}
        
        public void UpdateItem(UserProfile user,  UserSelectedContext catalogInfo, Guid cartId, ShoppingCartItem updatedItem)
		{
			var basket = basketLogic.RetrieveSharedCustomerBasket(user, catalogInfo, cartId);
			if (basket == null)
				return;

			basketRepository.UpdateItem(basket.UserId.ToGuid(), cartId, updatedItem.ToLineItem(basket.BranchId));
		}
        #endregion


		public QuickAddReturnModel CreateQuickAddCart(UserProfile user, UserSelectedContext catalogInfo, List<QuickAddItemModel> items)
		{
			//Create a shoppingcart model and pass to the existing createcart method
			var shoppingCart = new ShoppingCart();
			shoppingCart.Items = new List<ShoppingCartItem>();
			shoppingCart.BranchId = catalogInfo.BranchId;
			shoppingCart.Name = string.Format("Quick Add - {0} - {1} {2}", catalogInfo.CustomerId, DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());

			var itemErrorMessage = new StringBuilder();

			foreach (var item in items)
			{
				if (item == null || string.IsNullOrEmpty(item.ItemNumber))
					continue;

				//Verify they have access to item, if the item is invalid, log then return error
				var prod = catalogLogic.GetProductById(catalogInfo, item.ItemNumber, user);

				if (prod == null)
					itemErrorMessage.AppendFormat("Item {0} is not a valid item #", item.ItemNumber);
				else
				{
					shoppingCart.Items.Add(new ShoppingCartItem()
					{
						ItemNumber = item.ItemNumber,
						Each = item.Each,
						Quantity = item.Quantity
					}
					);
				}

			}
						
			if (itemErrorMessage.Length > 0)
				return new QuickAddReturnModel() { Success = false, ErrorMessage = itemErrorMessage.ToString() };

			return new QuickAddReturnModel() { Success = true, CartId = CreateCart(user, catalogInfo, shoppingCart) };

		}
	}
}
