using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Impl.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using RT = KeithLink.Svc.Impl.RequestTemplates;
using KeithLink.Svc.Core.Models.Generated;

namespace KeithLink.Svc.Impl.Repository.Cart
{
	public class ShoppingCartRepositoryImpl: IShoppingCartRepository
	{
		public void DeleteCart(Guid userId, Guid cartId)
		{
			var deleteBasket = new CommerceDelete<Basket>();
			deleteBasket.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			deleteBasket.SearchCriteria.Model.Properties["BasketType"] = 0;
			deleteBasket.SearchCriteria.Model.Id = cartId.ToString("B");

			FoundationService.ExecuteRequest(deleteBasket.ToRequest());
		}
		
		public Basket ReadCart(Guid userId, Guid cartId)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;
			queryBaskets.SearchCriteria.Model.Id = cartId.ToString("B");

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
			return ((Basket)basketResponse.CommerceEntities[0]);
		}
		
		public List<Basket> ReadAllCarts(Guid userId, string branchId)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);


			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
			
			return basketResponse.CommerceEntities.Cast<CommerceEntity>().Select(i => (Basket)i).Where(b => b.BranchId.Equals(branchId)).ToList();
		}
		
		public Guid? AddItem(Guid userId, Guid cartId, LineItem newItem)
		{
			var basket = ReadCart(userId, cartId);
			//var existingIds = basket.LineItems.Select(i => i.Id).ToList();

			////var test = RT.Orders.AddToCart(basket.FormattedName(basket.BranchId), userId.ToString("B"), "0", "true", basket.BranchId, newItem.ProductId, string.Empty, newItem.Quantity.ToString(), newItem.Notes);

			////CS returns all of the items, so this is how we have to determine the Id for the newly created item
			//var newId = ((Basket)test[0]).LineItems.Where(b => !existingIds.Any(i => i.Equals(b.Id.ToGuid()))).FirstOrDefault();

			//if (newId != null)
			//	return newId.Id.ToGuid();

			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;
			updateOrder.SearchCriteria.Model.Id = cartId.ToString("B");
			updateOrder.UpdateOptions.ReturnModel = new Basket();

			var lineItemUpdate = new CommerceCreateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
			lineItemUpdate.Model = newItem;
			updateOrder.RelatedOperations.Add(lineItemUpdate);


			FoundationService.ExecuteRequest(updateOrder.ToRequest());

			var newBasket = ReadCart(userId, cartId);

			var newId =newBasket.LineItems.Where(b => !basket.LineItems.Any(i => i.Id.Equals(b.Id.ToGuid()))).FirstOrDefault();

			if (newId != null)
				return newId.Id.ToGuid();

			return null;
		}
		
		public void UpdateItem(Guid userId, Guid cartId, LineItem updatedItem)
		{
			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;
			updateOrder.SearchCriteria.Model.Id = cartId.ToString("B");


			var lineItemUpdate = new CommerceUpdateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
			lineItemUpdate.SearchCriteria.Model.Id = updatedItem.Id;
			lineItemUpdate.Model = updatedItem;
			updateOrder.RelatedOperations.Add(lineItemUpdate);

			FoundationService.ExecuteRequest(updateOrder.ToRequest());
		}
		
		public void DeleteItem(Guid userId, Guid cartId, Guid itemId)
		{

			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;
			updateOrder.SearchCriteria.Model.Id = cartId.ToString("B");
			

			var lineItemUpdate = new CommerceDeleteRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
			lineItemUpdate.SearchCriteria.Model.Id = itemId.ToString("B");
			updateOrder.RelatedOperations.Add(lineItemUpdate);

			FoundationService.ExecuteRequest(updateOrder.ToRequest());
		}
		
		public Guid CreateOrUpdateCart(Guid userId, string branchId, Basket basket, List<LineItem> items)
		{
			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;

			if (!string.IsNullOrEmpty(basket.Id))
				updateOrder.SearchCriteria.Model.Id = basket.Id;
			else
				updateOrder.SearchCriteria.Model.Name = basket.Name;
			updateOrder.Model = basket;
			updateOrder.UpdateOptions.ReturnModel = new Basket();


			if(items != null)
				foreach (var item in items)
				{
					if (string.IsNullOrEmpty(item.Id) || item.Id == Guid.Empty.ToString("B"))
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
	}
}
