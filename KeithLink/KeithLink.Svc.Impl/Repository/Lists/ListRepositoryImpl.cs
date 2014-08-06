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

        private readonly Guid EXAMPLEUSERID = Guid.Parse("95436e7d-d09f-426b-a0c3-d4d702ee7422"); //TODO: Use real UserId once Auth/Profiles are completed

        public ListRepositoryImpl()
        {
            orderContext = OrderContext.Create(Configuration.CSSiteName);
        }

        public Guid CreateList(UserList list)
        {
            var newBasket = orderContext.GetBasket(EXAMPLEUSERID, list.Name);
            var orderForm = new OrderForm();

			if(list.Items != null)
				foreach (var item in list.Items)
					orderForm.LineItems.Add(new LineItem() { DisplayName = item.Label, ProductId = item.ItemNumber });
            
			
			newBasket.OrderForms.Add(orderForm);

            newBasket.Save();
            return newBasket.OrderGroupId;
        }
		       
        public void UpdateList(UserList list)
        {
			var basket = orderContext.GetBasket(EXAMPLEUSERID, list.ListId);

			if (basket == null) //Throw error?
				return;

			basket.Name = list.Name;
			
			for (int x = 0; x < basket.LineItemCount; x++)
			{
				if (list.Items != null && list.Items.Where(i => i.ListItemId.Equals(basket.OrderForms[0].LineItems[x].LineItemId)).Any())
					basket.OrderForms[0].LineItems.Remove(basket.OrderForms[0].LineItems[x]);
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
						//existingItem.Index = item.Position;
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

        public void DeleteList(Guid listId)
        {
			var basket = orderContext.GetBasket(EXAMPLEUSERID, listId);

			if (basket != null)
				basket.Delete();
        }

        public void DeleteItem(UserList list, Guid itemId)
        {
            throw new NotImplementedException();
        }

        public List<UserList> ReadAllLists()
        {
			var baskets = orderContext.GetBasketsForUser(EXAMPLEUSERID);

			return baskets.Cast<OrderGroup>().Select(b => new UserList() { ListId = b.OrderGroupId, Name = b.Name, Items = b.OrderForms[0].LineItems.Cast<LineItem>().Select(l => new ListItem() { ItemNumber = l.ProductId, Label = l.DisplayName, ListItemId = l.LineItemId, ParLevel = (int)l.Quantity, Position = l["LinePosition"] == null ? 0 : int.Parse(l["LinePosition"].ToString()) }).ToList() }).ToList();

        }

        public UserList ReadList(Guid listId)
        {
			var basket = orderContext.GetBasket(EXAMPLEUSERID, listId);

			if (basket == null)
				return null;

			return new UserList()
			{
				ListId = basket.OrderGroupId,
				Name = basket.Name,
				Items = basket.OrderForms[0].LineItems.Cast<LineItem>().Select(l => new ListItem() { ItemNumber = l.ProductId, Label = l.DisplayName, ListItemId = l.LineItemId, ParLevel = (int)l.Quantity, Position = l["LinePosition"] == null ? 0 : int.Parse(l["LinePosition"].ToString()) }).ToList()
			};
        }
	}

}


