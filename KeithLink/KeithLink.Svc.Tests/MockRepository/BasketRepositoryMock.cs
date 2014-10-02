using KeithLink.Svc.Core.Interface.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CS = KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Test.MockRepository
{

	public class BasketRepositoryMock: IBasketRepository
	{
		private List<CS.Basket> MockBaskets;

		public BasketRepositoryMock()
		{
			MockBaskets = new List<CS.Basket>() 
			{ 
				new CS.Basket() { Id = Guid.NewGuid().ToString("B"), DisplayName = "Mock A", BranchId = "fdf", Status = "CustomerList"
				},
				new CS.Basket() { Id = Guid.NewGuid().ToString("B"), DisplayName = "Mock B", BranchId = "fdf", Status = "ShoppingCart"
				}
			};
		}

		public void DeleteBasket(Guid userId, Guid cartId)
		{
			return;
		}

        public Guid CreateOrUpdateBasket(Guid userId, string branchId, CS.Basket basket, List<CS.LineItem> items, bool runPipelines = false)
		{
			return Guid.NewGuid();
		}

        public void DeleteItem(Guid userId, Guid basketId, Guid itemId, bool runPipelines = false)
		{
			return;
		}

        public void UpdateItem(Guid userId, Guid basketId, CS.LineItem updatedItem, bool runPipelines = false)
		{
			return;
		}

        public Guid? AddItem(Guid userId, Guid basketId, CS.LineItem newItem, CS.Basket basket, bool runPipelines = false)
		{
			return Guid.NewGuid();
		}

        public List<CS.Basket> ReadAllBaskets(Guid userId, bool runPipelines = false)
		{
			return MockBaskets;
		}

        public CS.Basket ReadBasket(Guid userId, Guid basketId, bool runPipelines = false)
		{
			return MockBaskets[0];
		}

        public CS.Basket ReadBasket(Guid userId, string basketName, bool runPipelines = false)
		{
			return MockBaskets[1];
		}


		public void DeleteBasket(Guid userId, string basketName)
		{
			throw new NotImplementedException();
		}
	}
}
