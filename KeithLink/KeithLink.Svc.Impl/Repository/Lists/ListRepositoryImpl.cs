using CommerceServer.Core.Runtime.Orders;
using KeithLink.Svc.Core.Interface.Lists;
using KeithLink.Svc.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository.Lists
{
    public class ListRepositoryImpl: IListRepository
    {
        private readonly OrderContext orderContext;

        
        public ListRepositoryImpl()
        {
            orderContext = OrderContext.Create(Configuration.CSSiteName);
        }

        public Guid CreateList(Guid userId, string branchId, UserList list)
        {
			var newBasket = orderContext.GetBasket(userId, list.FormattedName(branchId));
			
            var orderForm = new OrderForm();
			newBasket["DisplayName"] = list.Name;
			newBasket["BranchId"] = branchId;

			if(list.Items != null)
				foreach (var item in list.Items)
				{
					var newItem = new LineItem() { DisplayName = item.Label, ProductId = item.ItemNumber };
					newItem["LinePosition"] = item.Position;
					orderForm.LineItems.Add(newItem);
				}
			
			newBasket.OrderForms.Add(orderForm);

            newBasket.Save();
            return newBasket.OrderGroupId;
        }

		public void UpdateList(Guid userId, UserList list)
        {
			var basket = orderContext.GetBasket(userId, list.ListId);

			if (basket == null) //Throw error?
				return ;

			basket["DisplayName"] = list.Name;
			
			for (int x = 0; x < basket.LineItemCount; x++)
			{
				if (list.Items != null && !list.Items.Where(i => i.ListItemId.Equals(basket.OrderForms[0].LineItems[x].LineItemId)).Any())
					basket.OrderForms[0].LineItems.Remove(x);
			}

			if (list.Items != null)
			{
				foreach (var item in list.Items.OrderBy(s => s.Position))
				{
					var existingItem = basket.OrderForms[0].LineItems.Cast<LineItem>().Where(l => l.LineItemId.Equals(item.ListItemId)).FirstOrDefault();
					if (existingItem != null)
					{
						existingItem["LinePosition"] = item.Position;
						existingItem.ProductId = item.ItemNumber;
						existingItem.Quantity = item.ParLevel;
						existingItem.DisplayName = item.Label;
					}
					else
					{
						var newItem = new LineItem() { DisplayName = item.Label, ProductId = item.ItemNumber, Quantity = item.ParLevel };
						newItem["LinePosition"] = item.Position;
						basket.OrderForms[0].LineItems.Add(newItem);
					}
				}
			}
			
			basket.Save();
        }

		public void DeleteList(Guid userId, Guid listId)
        {
			var basket = orderContext.GetBasket(userId, listId);

			if (basket != null)
				basket.Delete();
        }

		public List<UserList> ReadAllLists(Guid userId, string branchId)
        {
			var baskets = orderContext.GetBasketsForUser(userId);

			return baskets.Cast<OrderGroup>().Where(i => i["BranchId"].ToString() == branchId).Select(b => new UserList() { 
				ListId = b.OrderGroupId, 
				Name = b["DisplayName"].ToString(), 
				BranchId = b["BranchId"].ToString(),
				Items = b.OrderForms[0].LineItems.Cast<LineItem>().Select(l => new ListItem() { 
					ItemNumber = l.ProductId, 
					Label = l.DisplayName,
					ListItemId = l.LineItemId,
					ParLevel = (int)l.Quantity, Position = l["LinePosition"] == null ? 0 : int.Parse(l["LinePosition"].ToString()) }).ToList() }).ToList();

        }

		public UserList ReadList(Guid userId, Guid listId)
        {
			var basket = orderContext.GetBasket(userId, listId);

			if (basket == null)
				return null;

			return ToUserList(basket);
        }

		public UserList DeleteItem(Guid userId, Guid listId, Guid itemId)
		{
			var basket = orderContext.GetBasket(userId, listId);

			basket.OrderForms[0].LineItems.Remove(basket.OrderForms[0].LineItems.IndexOf(itemId));
			basket.Save();

			return ToUserList(basket);
		}

		private UserList ToUserList(Basket basket)
		{
			return new UserList()
			{
				ListId = basket.OrderGroupId,
				Name = basket["DisplayName"].ToString(),
				BranchId = basket["BranchId"].ToString(),
				Items = basket.OrderForms[0].LineItems.Cast<LineItem>().Select(l => new ListItem()
					{
						ItemNumber = l.ProductId,
						Label = l.DisplayName,
						ListItemId = l.LineItemId,
						ParLevel = (int)l.Quantity,
						Position = l["LinePosition"] == null ? 0 : int.Parse(l["LinePosition"].ToString())
					}).ToList()
			};
			
		}


		public UserList ReadList(Guid userId, string listName)
		{
			
			var list = orderContext.GetBasket(userId, listName);
			if (list == null || list["BranchId"] == null)
				return null;
			return ToUserList(list);
		}


		public Guid? AddItem(Guid userId, Guid listId, ListItem newItem)
		{
			var basket = orderContext.GetBasket(userId, listId);

			var newCSItem = new LineItem() { DisplayName = newItem.Label, ProductId = newItem.ItemNumber, Quantity = newItem.ParLevel };
			newCSItem["LinePosition"] = newItem.Position;
			basket.OrderForms[0].LineItems.Add(newCSItem);

			basket.Save();

			return newCSItem.LineItemId;
		}
	}

}


