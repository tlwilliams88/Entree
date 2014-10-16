using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using RT = KeithLink.Svc.Impl.RequestTemplates;
using KeithLink.Svc.Core.Models.Generated;
using KeithLink.Svc.Core.Interface.Orders;


namespace KeithLink.Svc.Impl.Repository.Orders
{
	public class BasketRepositoryImpl : IBasketRepository
	{
		public void DeleteBasket(Guid userId, Guid cartId)
		{
			var deleteBasket = new CommerceDelete<Basket>();
			deleteBasket.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();
			deleteBasket.SearchCriteria.Model.Properties["BasketType"] = 0;
			deleteBasket.SearchCriteria.Model.Id = cartId.ToCommerceServerFormat();

			FoundationService.ExecuteRequest(deleteBasket.ToRequest());
		}

		public void DeleteBasket(Guid userId, string basketName)
		{
			var deleteBasket = new CommerceDelete<Basket>();
			deleteBasket.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();
			deleteBasket.SearchCriteria.Model.Properties["BasketType"] = 0;
			deleteBasket.SearchCriteria.Model.Properties["Name"] = basketName;

			FoundationService.ExecuteRequest(deleteBasket.ToRequest());
		}

		public Basket ReadBasket(Guid userId, Guid cartId, bool runPipelines = false)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;
			queryBaskets.SearchCriteria.Model.Id = cartId.ToCommerceServerFormat();
            queryBaskets.QueryOptions.RefreshBasket = runPipelines;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
			if (basketResponse.CommerceEntities.Count == 0)
				return null;

			return ((Basket)basketResponse.CommerceEntities[0]);
		}

        public List<Basket> ReadAllBaskets(Guid userId, string basketStatus, bool runPipelines = false)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;
			queryBaskets.SearchCriteria.Model.Properties["Status"] = basketStatus;

            queryBaskets.QueryOptions.RefreshBasket = runPipelines;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);


			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

			return basketResponse.CommerceEntities.Cast<CommerceEntity>().Select(i => (Basket)i).ToList();
		}

        public Guid? AddItem(Guid cartId, LineItem newItem, Basket basket, bool runPipelines = false)
		{
            var updateOrder = new CommerceUpdate<Basket,
                        CommerceModelSearch<Basket>,
                        CommerceBasketUpdateOptionsBuilder>();
			updateOrder.SearchCriteria.Model.UserId = basket.UserId;
            updateOrder.SearchCriteria.Model.BasketType = 0;
            updateOrder.SearchCriteria.Model.Id = cartId.ToCommerceServerFormat();
            updateOrder.UpdateOptions.RefreshBasket = runPipelines; // disable running of pipelines to optimize save time
            updateOrder.UpdateOptions.ToOptions().ReturnModel = (new Basket()).ToCommerceEntity();

            var lineItemUpdate = new CommerceCreateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
            lineItemUpdate.Model = newItem;
            updateOrder.RelatedOperations.Add(lineItemUpdate);
            updateOrder.UpdateOptions.ToOptions().ReturnModelQueries.Add(
                new CommerceQueryRelatedItem() { RelationshipName = Basket.RelationshipName.LineItems, Model = (new LineItem()).ToCommerceEntity() });

            var response = FoundationService.ExecuteRequest(updateOrder.ToRequest());

            CommerceServer.Foundation.CommerceRelationshipList lineItemsFromResponse =
                (response.OperationResponses[0] as CommerceUpdateOperationResponse)
                .CommerceEntities[0].Properties[Basket.RelationshipName.LineItems] as CommerceServer.Foundation.CommerceRelationshipList;

            var newId = lineItemsFromResponse.Where(b => !basket.LineItems.Any(i => i.Id.Equals(b.Target.Id))).FirstOrDefault();

            if (newId != null)
                return newId.Target.Id.ToGuid();

            return null;
		}

        public void UpdateItem(Guid userId, Guid cartId, LineItem updatedItem, bool runPipelines = false)
		{
            var updateOrder = new CommerceUpdate<Basket,
                            CommerceModelSearch<Basket>,
                            CommerceBasketUpdateOptionsBuilder>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;
			updateOrder.SearchCriteria.Model.Id = cartId.ToCommerceServerFormat();
            updateOrder.UpdateOptions.RefreshBasket = runPipelines;

			var lineItemUpdate = new CommerceUpdateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
			lineItemUpdate.SearchCriteria.Model.Id = updatedItem.Id;
			lineItemUpdate.Model = updatedItem;
			updateOrder.RelatedOperations.Add(lineItemUpdate);

			FoundationService.ExecuteRequest(updateOrder.ToRequest());
		}

        public void DeleteItem(Guid userId, Guid cartId, Guid itemId, bool runPipelines = false)
		{

			var updateOrder = new CommerceUpdate<Basket,
                            CommerceModelSearch<Basket>,
                            CommerceBasketUpdateOptionsBuilder>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;
			updateOrder.SearchCriteria.Model.Id = cartId.ToCommerceServerFormat();
            updateOrder.UpdateOptions.RefreshBasket = runPipelines;

			var lineItemUpdate = new CommerceDeleteRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
			lineItemUpdate.SearchCriteria.Model.Id = itemId.ToCommerceServerFormat();
			updateOrder.RelatedOperations.Add(lineItemUpdate);

			FoundationService.ExecuteRequest(updateOrder.ToRequest());
		}

        public Guid CreateOrUpdateBasket(Guid userId, string branchId, Basket basket, List<LineItem> items, bool runPipelines = false)
		{
            var updateOrder = new CommerceUpdate<Basket,
                            CommerceModelSearch<Basket>,
                            CommerceBasketUpdateOptionsBuilder>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;

			if (!string.IsNullOrEmpty(basket.Id))
				updateOrder.SearchCriteria.Model.Id = basket.Id;
			else
				updateOrder.SearchCriteria.Model.Name = basket.Name;
			updateOrder.Model = basket;
			updateOrder.UpdateOptions.ToOptions().ReturnModel = (new Basket()).ToCommerceEntity();
            updateOrder.UpdateOptions.RefreshBasket = runPipelines;


			if (items != null)
				foreach (var item in items)
				{
					if (string.IsNullOrEmpty(item.Id) || item.Id == Guid.Empty.ToCommerceServerFormat())
					{
						var lineItemCreate = new CommerceCreateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
						lineItemCreate.Model = item;
						updateOrder.RelatedOperations.Add(lineItemCreate);
					}
					else
					{
						var lineItemUpdate = new CommerceUpdateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
						lineItemUpdate.SearchCriteria.Model.Id = item.Id;
						lineItemUpdate.Model = item;
						updateOrder.RelatedOperations.Add(lineItemUpdate);
					}
				}

			// create the request
			var response = FoundationService.ExecuteRequest(updateOrder.ToRequest());

			if (response.OperationResponses.Count != 1)
				return Guid.Empty;

			return ((CommerceUpdateOperationResponse)response.OperationResponses[0]).CommerceEntities[0].Id.ToGuid();
		}


        public Basket ReadBasket(Guid userId, string basketName, bool runPipelines = false)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>, CommerceBasketQueryOptionsBuilder>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToCommerceServerFormat();
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;
			queryBaskets.SearchCriteria.Model.Properties["Name"] = basketName;
            queryBaskets.QueryOptions.RefreshBasket = runPipelines;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
			return ((Basket)basketResponse.CommerceEntities[0]);
		}		
	}
}

