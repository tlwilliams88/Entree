using CommerceServer.Foundation;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Impl.Models.Generated;
using KeithLink.Common.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Impl.Helpers;
using KeithLink.Common.Core;
using RT = KeithLink.Svc.Impl.RequestTemplates;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ListRepositoryImpl: IListRepository
    {
		public Guid CreateOrUpdateList(Guid userId, string branchId, UserList list)
        {
			var updateOrder = new CommerceUpdate<Basket>();
			updateOrder.SearchCriteria.Model.UserId = userId.ToString();
			updateOrder.SearchCriteria.Model.BasketType = 0;

			if (list.ListId != Guid.Empty)
				updateOrder.SearchCriteria.Model.Id = list.ListId.ToString("B");
			else
				updateOrder.SearchCriteria.Model.Name = list.FormattedName(branchId);
			
			updateOrder.Model.Properties["BranchId"] = branchId;
			updateOrder.Model.Name = list.FormattedName(branchId);
			updateOrder.Model.Properties["DisplayName"] = list.Name;
			updateOrder.Model.Status = "InProcess";
			updateOrder.Model.Properties.Add("Id");
			updateOrder.UpdateOptions.ReturnModel = new Basket();

			
			if (list.Items != null)
				foreach (var item in list.Items)
				{
					var newItem = new LineItem() { ProductId = item.ItemNumber };
					newItem.Properties["LinePosition"] = item.Position;
					newItem.Properties["Label"] = item.Label;
					newItem.Properties["ParLevel"] = item.ParLevel;
					newItem.CatalogName = branchId;
					if (item.ListItemId == Guid.Empty)
					{
						var lineItemCreate = new CommerceCreateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
						lineItemCreate.Model = newItem;
						updateOrder.RelatedOperations.Add(lineItemCreate);
					}
					else
					{
						var lineItemUpdate = new CommerceUpdateRelatedItem<LineItem>(Basket.RelationshipName.LineItems);
						lineItemUpdate.Model = newItem;
						updateOrder.RelatedOperations.Add(lineItemUpdate);
					}
				}
			
			// create the request
			var response = FoundationService.ExecuteRequest(updateOrder.ToRequest());

			if(response.OperationResponses.Count !=1 )
				return Guid.Empty;

			return ((CommerceUpdateOperationResponse)response.OperationResponses[0]).CommerceEntities[0].Id.ToGuid();
        }

		public void DeleteList(Guid userId, Guid listId)
        {
			var deleteBasket = new CommerceDelete<Basket>();
			deleteBasket.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			deleteBasket.SearchCriteria.Model.Properties["BasketType"] = 0;
			deleteBasket.SearchCriteria.Model.Id = listId.ToString("B");

			var response = FoundationService.ExecuteRequest(deleteBasket.ToRequest());
        }

		public List<UserList> ReadAllLists(Guid userId, string branchId)
        {
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);


			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;

			var basketList = new List<UserList>();

			foreach (Basket basket in basketResponse.CommerceEntities.Cast<CommerceEntity>().Where(b => b.Properties["BranchId"].ToString().Equals(branchId)))
			{
				basketList.Add(new UserList() {
					ListId = basket.Id.ToGuid(),
					Name = basket.Properties["DisplayName"].ToString(),
					BranchId = basket.Properties["BranchId"].ToString(),
					Items = basket.LineItems.Select(l => new ListItem() {
						ItemNumber = l.ProductId,
						Label = l.Properties["Label"] == null ? string.Empty : l.Properties["Label"].ToString(),
						ListItemId = l.Id.ToGuid(),
						ParLevel = l.Properties["ParLevel"] == null ? 0 : (decimal)l.Properties["ParLevel"],
						Position = l.Properties["LinePosition"] == null ? 0 : int.Parse(l.Properties["LinePosition"].ToString())
					}).ToList()
				});
			}
						
			return basketList;

        }

		public UserList ReadList(Guid userId, Guid listId)
        {
			var queryBaskets = new CommerceQuery<CommerceEntity, CommerceModelSearch<CommerceEntity>>("Basket");
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;
			queryBaskets.SearchCriteria.Model.Id = listId.ToString("B");

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
			return ToUserList(((Basket)basketResponse.CommerceEntities[0]));
        }

		public UserList DeleteItem(Guid userId, Guid listId, Guid itemId)
		{
			var list = ReadList(userId, listId);

			var basket = RT.Orders.DeleteItemFromList(list.FormattedName(list.BranchId), userId.ToString("B"), "0", "true", itemId.ToString("B"));
			return ToUserList(((Basket)basket[0]));
			
		}

		private UserList ToUserList(Basket basket)
		{
			return new UserList()
			{
				ListId = basket.Id.ToGuid(),
				Name = basket.Properties["DisplayName"].ToString(),
				BranchId = basket.Properties["BranchId"].ToString(),
				Items = basket.LineItems.Select(l => new ListItem()
				{
					ItemNumber = l.ProductId,
					Label = l.Properties["Label"] == null ? string.Empty : l.Properties["Label"].ToString(),
					ListItemId = l.Id.ToGuid(),
					ParLevel = l.Properties["ParLevel"] == null ? 0M : (decimal)l.Properties["ParLevel"],
					Position = l.Properties["LinePosition"] == null ? 0 : int.Parse(l.Properties["LinePosition"].ToString())
				}).ToList()
			};
			
		}
		
		public UserList ReadList(Guid userId, string listName)
		{

			var queryBaskets = new CommerceQuery<Basket>();
			queryBaskets.SearchCriteria.Model.Properties["UserId"] = userId.ToString("B");
			queryBaskets.SearchCriteria.Model.Properties["BasketType"] = 0;
			queryBaskets.SearchCriteria.Model.Name = listName;

			var queryLineItems = new CommerceQueryRelatedItem<CommerceEntity>("LineItems", "LineItem");
			queryBaskets.RelatedOperations.Add(queryLineItems);

			var response = FoundationService.ExecuteRequest(queryBaskets.ToRequest());

			if (response.OperationResponses.Count == 0)
				return null;

			CommerceQueryOperationResponse basketResponse = response.OperationResponses[0] as CommerceQueryOperationResponse;
			return ToUserList(((Basket)basketResponse.CommerceEntities[0]));
		}
		
		public Guid? AddItem(Guid userId, Guid listId, ListItem newItem)
		{
			var basket = ReadList(userId, listId);
			var existingIds = basket.Items.Select(i => i.ListItemId).ToList();

			var test = RT.Orders.AddToList(basket.FormattedName(basket.BranchId), userId.ToString("B"), "0", "true", basket.BranchId, newItem.ItemNumber, newItem.ParLevel.ToString(), newItem.Label, newItem.Position.ToString());

			//CS returns all of the items, so this is how we have to determine the Id for the newly created item
			foreach (var item in ((Basket) test[0]).LineItems)
			{
				if (!existingIds.Contains(item.Id.ToGuid()))
					return item.Id.ToGuid();
			}

			return null;
		}
	}

}


