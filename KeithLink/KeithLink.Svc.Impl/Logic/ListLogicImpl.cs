using KeithLink.Svc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Impl.Logic
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

			if (item == null)
				list.Items.Add(updatedItem);
			else
			{
				item.Label = updatedItem.Label;
				item.ParLevel = updatedItem.ParLevel;
				item.Position = updatedItem.Position;
				item.ItemNumber = updatedItem.ItemNumber;
			}

			listRepository.UpdateList(list);
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
			list.Items.RemoveAll(i => i.ListItemId.Equals(itemId));

            listRepository.UpdateList(list);
        }

        public List<UserList> ReadAllLists(bool headerInfoOnly)
        {
			var lists = listRepository.ReadAllLists();

			if (headerInfoOnly)
				return lists.Select(l => new UserList() { ListId = l.ListId, Name = l.Name }).ToList();
			else
			{
				lists.ForEach(delegate(UserList list)
				{
					if (list.Items != null)
						list.Items.Sort();
				});
				return lists;
			}
        }

        public UserList ReadList(Guid listId)
        {
			var list = listRepository.ReadList(listId);

			if(list.Items != null)
				list.Items.Sort();
			
			return list;
        }

        public List<string> ReadListLabels(Guid listId)
        {
            var lists = listRepository.ReadList(listId);
            
            if (lists == null || lists.Items == null)
                return null;

            return lists.Items.Where(l => l.Label != null).Select(i => i.Label).ToList();
        }

        public List<string> ReadListLabels()
        {
            var lists = listRepository.ReadAllLists();
            return lists.Where(i => i.Items != null).SelectMany(l => l.Items.Where(b => b.Label != null).Select(i => i.Label)).ToList();
        }
    }
}
