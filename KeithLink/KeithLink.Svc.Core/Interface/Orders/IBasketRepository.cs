using KeithLink.Svc.Core.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Orders
{
	public interface IBasketRepository
	{
		void DeleteBasket(Guid userId, Guid cartId);

		Guid CreateOrUpdateBasket(Guid userId, string branchId, Basket basket, List<LineItem> items);

		void DeleteItem(Guid userId, Guid basketId, Guid itemId);

		void UpdateItem(Guid userId, Guid basketId, LineItem updatedItem);

		Guid? AddItem(Guid userId, Guid basketId, LineItem newItem);

		List<Basket> ReadAllBaskets(Guid userId);

		Basket ReadBasket(Guid userId, Guid basketId);
		Basket ReadBasket(Guid userId, string basketName);
		
	}
}
