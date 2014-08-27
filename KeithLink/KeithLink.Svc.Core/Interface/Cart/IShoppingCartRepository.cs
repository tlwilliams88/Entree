using KeithLink.Svc.Core.Models.Generated;
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
		void DeleteCart(Guid userId, Guid cartId);
		
		Guid CreateOrUpdateCart(Guid userId, string branchId, Basket basket, List<LineItem> items);

		void DeleteItem(Guid userId, Guid cartId, Guid itemId);
		
		void UpdateItem(Guid userId, Guid cartId, LineItem updatedItem);
		
		Guid? AddItem(Guid userId, Guid cartId, LineItem newItem);

		List<Basket> ReadAllCarts(Guid userId, string branchId);
		
		Basket ReadCart(Guid userId, Guid cartId);
	}
}
