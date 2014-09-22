using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.Orders;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class PurchaseOrderRepositoryImpl: IPurchaseOrderRepository
	{
		public PurchaseOrder ReadPurchaseOrder(Guid userId, string orderNumber)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 1;
			queryBaskets.SearchCriteria.Model.Properties["OrderNumber"] = orderNumber;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

			var here = ((PurchaseOrder)basketResponse.CommerceEntities[0]);

			return ((PurchaseOrder)basketResponse.CommerceEntities[0]);
		}
	}
}
