using KeithLink.Svc.Core.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Cart
{
	public interface IShoppingCartRepository
	{
		Guid CreateOrUpdateCart(Guid userId, string branchId, ShoppingCart cart);

		Guid? AddItem(Guid userId, Guid cartId, ShoppingCartItem newItem);
		void UpdateItem(Guid userId, Guid cartId, ShoppingCartItem updatedItem);
		void DeleteCart(Guid userId, Guid cartId);
		ShoppingCart DeleteItem(Guid userId, Guid cartId, Guid itemId);

		List<ShoppingCart> ReadAllCarts(Guid userId, string branchId);
		ShoppingCart ReadCart(Guid userId, Guid cartId);

	}
}
