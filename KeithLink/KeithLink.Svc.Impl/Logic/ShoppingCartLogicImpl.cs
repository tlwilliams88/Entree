
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic
{
	public class ShoppingCartLogicImpl: IShoppingCartLogic
	{
		private readonly IShoppingCartRepository shoppingCartRepository;
		private readonly ICatalogRepository catalogRepository;
		private readonly IPriceRepository priceRepository;
		

		public ShoppingCartLogicImpl(IShoppingCartRepository shoppingCartRepository, ICatalogRepository catalogRepository, IPriceRepository priceRepository)
		{
			this.shoppingCartRepository = shoppingCartRepository;
			this.catalogRepository = catalogRepository;
			this.priceRepository = priceRepository;
		}
		
		public Guid CreateCart(UserProfile user, string branchId, ShoppingCart cart)
		{
			return shoppingCartRepository.CreateOrUpdateCart(user.UserId, branchId, cart);
		}

		public Guid? AddItem(UserProfile user, Guid cartId, ShoppingCartItem newItem)
		{
			return shoppingCartRepository.AddItem(user.UserId, cartId, newItem);
		}

		public void UpdateItem(UserProfile user, Guid cartId, ShoppingCartItem updatedItem)
		{
			var cart = shoppingCartRepository.ReadCart(user.UserId, cartId);

			if (cart == null)
				return;

			var item = cart.Items.Where(i => i.CartItemId.Equals(updatedItem.CartItemId)).FirstOrDefault();

			if (item == null)
				cart.Items.Add(updatedItem);
			else
			{
				item.Notes = updatedItem.Notes;
				item.Quantity = updatedItem.Quantity;
				item.ItemNumber = updatedItem.ItemNumber;
			}

			shoppingCartRepository.CreateOrUpdateCart(user.UserId, cart.BranchId, cart);
		}

		public void UpdateCart(UserProfile user, ShoppingCart cart)
		{
			var updateCart = shoppingCartRepository.ReadCart(user.UserId, cart.CartId);


			if (updateCart == null) 
				return;

			updateCart.Name = cart.Name;

			var itemsToRemove = new List<Guid>();

			foreach (var item in updateCart.Items)
			{
				if (cart.Items != null && !cart.Items.Where(i => i.CartItemId.Equals(item.CartItemId)).Any())
					itemsToRemove.Add(item.CartItemId);
			}

			if (cart.Items != null)
			{
				foreach (var item in cart.Items)
				{
					if (item.CartItemId == null)
						updateCart.Items.Add(item);
					else
					{
						var existingItem = updateCart.Items.Where(i => i.CartItemId.Equals(item.CartItemId)).FirstOrDefault();
						if (existingItem == null)
							continue;
						existingItem.Quantity = item.Quantity;
						existingItem.Notes = item.Notes;
					}
				}
			}

			shoppingCartRepository.CreateOrUpdateCart(user.UserId, updateCart.BranchId, updateCart);

			foreach (var toDelete in itemsToRemove)
			{
				shoppingCartRepository.DeleteItem(user.UserId, updateCart.CartId, toDelete);
			}
		}

		public void DeleteCart(UserProfile user, Guid cartId)
		{
			shoppingCartRepository.DeleteCart(user.UserId, cartId);
		}

		public ShoppingCart DeleteItem(UserProfile user, Guid cartId, Guid itemId)
		{
			var cart = shoppingCartRepository.DeleteItem(user.UserId, cartId, itemId);
			if (cart.Items != null)
				cart.Items.Sort();
			LookupProductDetails(user, cart);
			return cart;
		}

		public List<ShoppingCart> ReadAllCarts(UserProfile user, string branchId, bool headerInfoOnly)
		{
			var lists = shoppingCartRepository.ReadAllCarts(user.UserId, branchId);

			if (headerInfoOnly)
				return lists.Select(l => new ShoppingCart() { CartId = l.CartId, Name = l.Name }).ToList();
			else
			{
				lists.ForEach(delegate(ShoppingCart list)
				{
					LookupProductDetails(user, list);
				});
				return lists;
			}
		}

		public ShoppingCart ReadCart(UserProfile user, Guid cartId)
		{
			var cart = shoppingCartRepository.ReadCart(user.UserId, cartId);
			if (cart == null)
				return null;
			if (cart.Items != null)
				cart.Items.Sort();
			LookupProductDetails(user, cart);
			return cart;
		}

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
		
	}
}
