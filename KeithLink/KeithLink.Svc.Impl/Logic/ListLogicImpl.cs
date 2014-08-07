using KeithLink.Svc.Core.Interface.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;

namespace KeithLink.Svc.Impl.Logic
{
    public class ListLogicImpl: IListLogic
    {
        private readonly IListRepository listRepository;
		private readonly ICatalogRepository catalogRepository;
        //TODO: Everything should only work with list for the current user. Waiting for Auth/login to be completed.

        public ListLogicImpl(IListRepository listRepository, ICatalogRepository catalogRepository)
        {
            this.listRepository = listRepository;
			this.catalogRepository = catalogRepository;
        }

        public Guid CreateList(UserList list)
        {
			
            return listRepository.CreateList(list);
        }

        public Guid? AddItem(Guid listId, ListItem newItem)
        {
			if (string.IsNullOrEmpty(newItem.Branch))
				return null; //TODO: throw exception, branch is required

            var list = listRepository.ReadList(listId);

            if (list == null)
                return null;

            newItem.ListItemId = Guid.NewGuid();

			if (list.Items == null)
				list.Items = new List<ListItem>(); ;

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
					LookupProductDetails(list);
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
			LookupProductDetails(list);
			return list;
        }

        public List<string> ReadListLabels(Guid listId)
        {
            var lists = listRepository.ReadList(listId);
            
            if (lists == null || lists.Items == null)
                return null;

            return lists.Items.Where(l => l.Label != null).Select(i => i.Label).Distinct().ToList();
        }

        public List<string> ReadListLabels()
        {
            var lists = listRepository.ReadAllLists();
			return lists.Where(i => i.Items != null).SelectMany(l => l.Items.Where(b => b.Label != null).Select(i => i.Label)).Distinct().ToList();
        }

		private void LookupProductDetails(UserList list)
		{
			if (list.Items == null)
				return;

			list.Items.ForEach(delegate (ListItem listItem)
			{
				//Lookup the item
				var product = catalogRepository.GetProductById(listItem.Branch, listItem.ItemNumber);

				if (product != null)
				{
					listItem.Name = product.Name;
					listItem.PackSize = string.Format("{0} / {1}", product.Cases, product.Size);
				}
			});
			
		}

    }
}
