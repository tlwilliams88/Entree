using KeithLink.Svc.Core.Models.Orders;
using KeithLink.Svc.Core.Models.Profile;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Cart
{
	public interface IShoppingCartLogic
	{
		Guid CreateCart(UserProfile user, UserSelectedContext catalogInfo, ShoppingCart cart);
		Guid? AddItem(UserProfile user, Guid cartId, ShoppingCartItem newItem);
		
		void UpdateItem(UserProfile user, Guid cartId, ShoppingCartItem updatedItem);
		void UpdateCart(UserSelectedContext catalogInfo, UserProfile user, ShoppingCart cart, bool deleteOmmitedItems);

		void DeleteCart(UserProfile user, Guid cartId);
		void DeleteCarts(Guid userId, List<Guid> cartIds);
		void DeleteItem(UserProfile user, Guid cartId, Guid itemId);

		List<ShoppingCart> ReadAllCarts(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly);
		ShoppingCart ReadCart(UserProfile user, Guid cartId);

		NewOrderReturn SaveAsOrder(UserProfile user, Guid cartId);

	}
}
