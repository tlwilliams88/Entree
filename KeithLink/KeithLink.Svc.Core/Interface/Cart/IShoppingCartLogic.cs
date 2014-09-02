using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Cart
{
	public interface IShoppingCartLogic
	{
		Guid CreateCart(UserProfile user, string branchId, ShoppingCart cart);
		Guid? AddItem(UserProfile user, Guid cartId, ShoppingCartItem newItem);
		void UpdateItem(UserProfile user, Guid cartId, ShoppingCartItem updatedItem);
		void UpdateCart(UserProfile user, ShoppingCart cart);

		void DeleteCart(UserProfile user, Guid cartId);
		void DeleteItem(UserProfile user, Guid cartId, Guid itemId);

		List<ShoppingCart> ReadAllCarts(UserProfile user, string branchId, bool headerInfoOnly);
		ShoppingCart ReadCart(UserProfile user, Guid cartId);

		string SaveAsOrder(UserProfile user, Guid cartId);

	}
}
