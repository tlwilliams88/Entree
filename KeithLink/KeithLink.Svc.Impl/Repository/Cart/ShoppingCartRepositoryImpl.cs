using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.Cart;
using KeithLink.Svc.Core.Models.ShoppingCart;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Svc.Impl.Models.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Common.Core.Extensions;
using RT = KeithLink.Svc.Impl.RequestTemplates;

namespace KeithLink.Svc.Impl.Repository.Cart
{
	public class ShoppingCartRepositoryImpl: IShoppingCartRepository
	{
		private readonly string BasketStatus = "ShoppingCart";

		public Guid CreateOrUpdateCart(Guid userId, string branchId, ShoppingCart cart)
		{
			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;

			if (cart.CartId != Guid.Empty)
				updateOrder.SearchCriteria.Model.Id = cart.CartId.ToString("B");
			else
				updateOrder.SearchCriteria.Model.Name = cart.FormattedName(branchId);

			updateOrder.Model.Properties["BranchId"] = branchId;
			updateOrder.Model.Name = cart.FormattedName(branchId);
			updateOrder.Model.Properties["DisplayName"] = cart.Name;
			updateOrder.Model.Status = BasketStatus;
			updateOrder.Model.Properties.Add("Id");
			updateOrder.UpdateOptions.ReturnModel = new Basket();


			if (cart.Items != null)
				foreach (var item in cart.Items)
				{
					var newItem = new LineItem() { ProductId = item.ItemNumber, Quantity = item.Quantity };
					newItem.Properties["Notes"] = item.Notes;
					newItem.CatalogName = branchId;
					if (item.CartItemId == Guid.Empty)
					{
						var lineItemCreate = new CommerceCreateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
						lineItemCreate.Model = newItem;
						updateOrder.RelatedOperations.Add(lineItemCreate);
					}
					else
					{
						var lineItemUpdate = new CommerceUpdateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
						lineItemUpdate.SearchCriteria.Model.Id = item.CartItemId.ToString("B");
						lineItemUpdate.Model = newItem;
						updateOrder.RelatedOperations.Add(lineItemUpdate);
					}
				}

			// create the request
			var response = FoundationService.ExecuteRequest(updateOrder.ToRequest());

			if (response.OperationResponses.Count != 1)
				return Guid.Empty;

			return ((CommerceUpdateOperationResponse)response.OperationResponses[0]).CommerceEntities[0].Id.ToGuid();
		}

		public Guid? AddItem(Guid userId, Guid cartId, ShoppingCartItem newItem)
		{
			var basket = ReadCart(userId, cartId);
			var existingIds = basket.Items.Select(i => i.CartItemId).ToList();

			var test = RT.Orders.AddToCart(basket.FormattedName(basket.BranchId), userId.ToString("B"), "0", "true", basket.BranchId, newItem.ItemNumber,string.Empty, newItem.Quantity.ToString(), newItem.Notes);

			//CS returns all of the items, so this is how we have to determine the Id for the newly created item
			var newId = ((Basket)test[0]).LineItems.Where(b => !existingIds.Any(i => i.Equals(b.Id.ToGuid()))).FirstOrDefault();

			if (newId != null)
				return newId.Id.ToGuid();

			return null;
		}

		public void DeleteCart(Guid userId, Guid cartId)
		{
			var deleteBasket = new CommerceDelete<Basket>();
			deleteBasket.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			deleteBasket.SearchCriteria.Model.Properties["BasketType"] = 0;
			deleteBasket.SearchCriteria.Model.Id = cartId.ToString("B");

			FoundationService.ExecuteRequest(deleteBasket.ToRequest());
		}

		public ShoppingCart DeleteItem(Guid userId, Guid cartId, Guid itemId)
		{
			var cart = ReadCart(userId, cartId);

			var basket = RT.Orders.DeleteLineItem(cart.FormattedName(cart.BranchId), userId.ToString("B"), "0", "true", itemId.ToString("B"));
			return ToShoppingCart(((Basket)basket[0]));
		}

		public List<ShoppingCart> ReadAllCarts(Guid userId, string branchId)
		{
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);


			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

			var basketList = new List<ShoppingCart>();

			foreach (Basket basket in basketResponse.CommerceEntities.Cast<CommerceEntity>().Where(b => b.Properties["BranchId"].ToString().Equals(branchId) && b.Properties["Status"].ToString().Equals(BasketStatus)))
			{
				basketList.Add(ToShoppingCart(basket));
			}

			return basketList;
		}

		public ShoppingCart ReadCart(Guid userId, Guid cartId)
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
			return ToShoppingCart(((Basket)basketResponse.CommerceEntities[0]));
		}

		private ShoppingCart ToShoppingCart(Basket basket)
		{
			return new ShoppingCart()
			{
				CartId = basket.Id.ToGuid(),
				Name = basket.Properties["DisplayName"].ToString(),
				BranchId = basket.Properties["BranchId"].ToString(),
				Items = basket.LineItems.Select(l => new ShoppingCartItem()
				{
					ItemNumber = l.ProductId,
					CartItemId = l.Id.ToGuid(),
					Notes = l.Properties["Notes"] == null ? string.Empty : l.Properties["Notes"].ToString(),
					Quantity = l.Quantity.HasValue ? l.Quantity.Value : 0
				}).ToList()
			};

		}
		
		public void UpdateItem(Guid userId, Guid cartId, ShoppingCartItem updatedItem)
		{
			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;
			updateOrder.SearchCriteria.Model.Id = cartId.ToString("B");


			var newItem = new LineItem() { ProductId = updatedItem.ItemNumber, Quantity = updatedItem.Quantity };
			newItem.Properties["Notes"] = updatedItem.Notes;
			var lineItemUpdate = new CommerceUpdateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
			lineItemUpdate.SearchCriteria.Model.Id = updatedItem.CartItemId.ToString("B");
			lineItemUpdate.Model = newItem;
			updateOrder.RelatedOperations.Add(lineItemUpdate);

			FoundationService.ExecuteRequest(updateOrder.ToRequest());
			
		}
	}
}
