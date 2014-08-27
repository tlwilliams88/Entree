
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

namespace KeithLink.Svc.Impl.Logic
{
	public class ShoppingCartLogicImpl: IShoppingCartLogic
	{
		private readonly IShoppingCartRepository shoppingCartRepository;
		private readonly ICatalogRepository catalogRepository;
		private readonly IPriceRepository priceRepository;

		private readonly string BasketStatus = "ShoppingCart";

		public ShoppingCartLogicImpl(IShoppingCartRepository shoppingCartRepository, ICatalogRepository catalogRepository, IPriceRepository priceRepository)
		{
			this.shoppingCartRepository = shoppingCartRepository;
			this.catalogRepository = catalogRepository;
			this.priceRepository = priceRepository;
		}
		
		public Guid CreateCart(UserProfile user, string branchId, ShoppingCart cart)
		{
			var newBasket = new CS.Basket();
			newBasket.BranchId = branchId;
			newBasket.DisplayName = cart.Name;
			newBasket.Status = BasketStatus;
			newBasket.Name = cart.FormattedName(branchId);

			return shoppingCartRepository.CreateOrUpdateCart(user.UserId, branchId, newBasket, cart.Items.Select(l => new CS.LineItem() { CatalogName = branchId, Notes = l.Notes, ProductId = l.ItemNumber, Quantity = l.Quantity }).ToList());
		}

		public Guid? AddItem(UserProfile user, Guid cartId, ShoppingCartItem newItem)
		{
			var basket = shoppingCartRepository.ReadCart(user.UserId, cartId);
			if (basket == null)
				return null;

			var newLineItem = new CS.LineItem() { ProductId = newItem.ItemNumber, Notes = newItem.Notes, Quantity = newItem.Quantity, CatalogName = basket.BranchId };

			return shoppingCartRepository.AddItem(user.UserId, cartId, newLineItem);
		}

		public void UpdateItem(UserProfile user, Guid cartId, ShoppingCartItem updatedItem)
		{
			var updatedLineItem = new CS.LineItem() { Id = updatedItem.CartItemId.ToString("B"), ProductId = updatedItem.ItemNumber, Notes = updatedItem.Notes, Quantity = updatedItem.Quantity };

			shoppingCartRepository.UpdateItem(user.UserId, cartId, updatedLineItem);
		}

		public void UpdateCart(UserProfile user, ShoppingCart cart)
		{
			var updateCart = shoppingCartRepository.ReadCart(user.UserId, cart.CartId);
			
			if (updateCart == null)
				return;

			updateCart.DisplayName = cart.Name;
			updateCart.Name = cart.FormattedName(updateCart.BranchId);
			var itemsToRemove = new List<Guid>();
			var lineItems = new List<CS.LineItem>();

			if (cart.Items != null)
			{
				itemsToRemove = updateCart.LineItems.Where(b => !cart.Items.Any(c => c.CartItemId.ToString("B").Equals(b.Id))).Select(l => l.Id.ToGuid()).ToList();
				lineItems = cart.Items.Select(s => new CS.LineItem() { Id = s.CartItemId == null ? Guid.Empty.ToString("B") : s.CartItemId.ToString("B"), ProductId = s.ItemNumber, Notes = s.Notes, Quantity = s.Quantity, CatalogName = updateCart.BranchId }).ToList();
			}
			
			shoppingCartRepository.CreateOrUpdateCart(user.UserId, updateCart.BranchId, updateCart, lineItems);

			foreach (var toDelete in itemsToRemove)
			{
				shoppingCartRepository.DeleteItem(user.UserId, cart.CartId, toDelete);
			}
		}

		public void DeleteCart(UserProfile user, Guid cartId)
		{
			shoppingCartRepository.DeleteCart(user.UserId, cartId);
		}

		public void DeleteItem(UserProfile user, Guid cartId, Guid itemId)
		{
			shoppingCartRepository.DeleteItem(user.UserId, cartId, itemId);
		}

		public List<ShoppingCart> ReadAllCarts(UserProfile user, string branchId, bool headerInfoOnly)
		{
			var lists = shoppingCartRepository.ReadAllCarts(user.UserId, branchId);
			var listForBranch = lists.Where(b => b.BranchId.Equals(branchId) && b.Status.Equals(BasketStatus));
			if (headerInfoOnly)
				return listForBranch.Select(l => new ShoppingCart() { CartId = l.Id.ToGuid(), Name = l.Name }).ToList();
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
			var basket = shoppingCartRepository.ReadCart(user.UserId, cartId);
			if (basket == null)
				return null;

			var cart = ToShoppingCart(basket);

			LookupProductDetails(user, cart);
			return cart;
		}

		#region Helper Methods

		private void LookupProductDetails(UserProfile user, ShoppingCart cart)
		{
			if (cart.Items == null)
				return;

			var products = catalogRepository.GetProductsByIds(cart.BranchId, cart.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var pricing = priceRepository.GetPrices(user.BranchId, user.CustomerId, DateTime.Now.AddDays(1), products.Products); 

			cart.Items.ForEach(delegate(ShoppingCartItem item)
			{

				var prod = products.Products.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				var price = pricing.Prices.Where(p => p.ItemNumber.Equals(item.ItemNumber)).FirstOrDefault();
				if (prod != null)
				{
					item.Name = prod.Name;
					item.PackSize = string.Format("{0} / {1}", prod.Cases, prod.Size);
				}
				if (price != null)
				{
					item.PackagePrice = price.PackagePrice;
					item.CasePrice = price.CasePrice;
				}
			});

		}

		private ShoppingCart ToShoppingCart(CS.Basket basket)
		{
			return new ShoppingCart()
			{
				CartId = basket.Id.ToGuid(),
				Name = basket.DisplayName,
				BranchId = basket.BranchId,
				Items = basket.LineItems.Select(l => new ShoppingCartItem()
				{
					ItemNumber = l.ProductId,
					CartItemId = l.Id.ToGuid(),
					Notes = l.Notes,
					Quantity = l.Quantity.HasValue ? l.Quantity.Value : 0
				}).ToList()
			};

		}

		#endregion
	}
}
