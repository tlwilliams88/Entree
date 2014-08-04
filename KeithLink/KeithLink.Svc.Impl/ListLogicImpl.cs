using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl
{
    public class ListLogicImpl: IListLogic
    {
        private readonly IListRepository listRepository;

        //TODO: Everything should only work with list for the current user. Waiting for Auth/login to be completed.

        public ListLogicImpl(IListRepository listRepository)
        {
            this.listRepository = listRepository;
        }

        public Guid CreateList(UserList list)
        {
            list.ListId = Guid.NewGuid();
            return listRepository.CreateList(list);
        }

        public Guid? AddItem(Guid listId, ListItem newItem)
        {
            var list = listRepository.ReadList(listId);

            if (list == null)
                return null;

            newItem.ListItemId = Guid.NewGuid();
            list.Items.Add(newItem);
            listRepository.UpdateList(list);

            return newItem.ListItemId;
        }

        public void UpdateItem(Guid listId, ListItem updatedItem)
        {
            var list = listRepository.ReadList(listId);

            if (list == null)
                return;

            var item = list.Items.Where(i => i.ListItemId.Equals(updatedItem.ListItemId)).FirstOrDefault();

            item.Label = updatedItem.Label;
            item.ParLevel = updatedItem.ParLevel;
            item.Position = updatedItem.Position;
            item.ProductId = updatedItem.ProductId;

            listRepository.UpdateItem(item);
        }

        public void UpdateList(UserList list)
        {
            listRepository.UpdateList(list);
        }

        public void DeleteList(Guid listId)
        {
            listRepository.DeleteList(listId);
        }

        public void DeleteItem(Guid listId, Guid itemId)
        {
            var list = listRepository.ReadList(listId);

            if (list == null)
                return;

            listRepository.DeleteItem(list, itemId);
        }

        public List<UserList> ReadAllLists()
        {
            return listRepository.ReadAllLists();
        }

        public UserList ReadList(Guid listId)
        {
            return listRepository.ReadList(listId);
        }

        public List<string> ReadListLabels(Guid listId)
        {
            return ReadListLabels(listId);
        }

        public List<string> ReadListLabels()
        {
            return ReadListLabels();
        }
    }
}
