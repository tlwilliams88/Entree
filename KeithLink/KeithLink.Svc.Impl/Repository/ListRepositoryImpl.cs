using CommerceServer.Core.Runtime.Orders;
using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Repository
{
    public class ListRepositoryImpl: IListRepository
    {
        private readonly OrderContext orderContext;

        private readonly Guid EXAMPLEUSERID = Guid.Parse("95436e7d-d09f-426b-a0c3-d4d702ee7422"); //TODO: Used real UserId once Auth/Profiles are completed

        public ListRepositoryImpl()
        {
            orderContext = OrderContext.Create(Configuration.CSSiteName);
        }

        public Guid CreateList(UserList list)
        {
            var newBasket = orderContext.GetBasket(EXAMPLEUSERID, list.Name);
            var orderForm = new OrderForm();
            foreach (var item in list.Items)
                orderForm.LineItems.Add(new LineItem() { DisplayName = item.Label, ProductId = item.ProductId });
            newBasket.OrderForms.Add(orderForm);
            newBasket.Save();
            return newBasket.OrderGroupId;
        }

        public void UpdateItem(ListItem updatedItem)
        {
            throw new NotImplementedException();
        }

        public void UpdateList(UserList list)
        {
            throw new NotImplementedException();
        }

        public void DeleteList(Guid listId)
        {
            throw new NotImplementedException();
        }

        public void DeleteItem(UserList list, Guid itemId)
        {
            throw new NotImplementedException();
        }

        public List<UserList> ReadAllLists()
        {
            throw new NotImplementedException();
        }

        public UserList ReadList(Guid listId)
        {
            throw new NotImplementedException();
        }
    }
}
