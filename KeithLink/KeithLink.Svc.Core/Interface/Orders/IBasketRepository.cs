using KeithLink.Svc.Core.Enumerations.List;
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
		void DeleteBasket(Guid userId, string basketName);

		Guid CreateOrUpdateBasket(Guid userId, string branchId, Basket basket, List<LineItem> items, bool runPipelines = false);

		void DeleteItem(Guid userId, Guid basketId, Guid itemId, bool runPipelines = false);

		void UpdateItem(Guid userId, Guid basketId, LineItem updatedItem, bool runPipelines = false);

		Guid? AddItem(Guid basketId, LineItem newItem, Basket basket, bool runPipelines = false);

		List<Basket> ReadAllBaskets(Guid userId, BasketType type, bool runPipelines = false);

		Basket ReadBasket(Guid userId, Guid basketId, bool runPipelines = false);
		Basket ReadBasket(Guid userId, string basketName, bool runPipelines = false);

		
	}
}
