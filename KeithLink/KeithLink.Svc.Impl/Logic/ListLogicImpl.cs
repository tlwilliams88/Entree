using KeithLink.Svc.Core.Interface.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Interface.SiteCatalog;
using KeithLink.Svc.Core.Models.SiteCatalog;
using CommerceServer.Core.Runtime.Orders;

namespace KeithLink.Svc.Impl.Logic
{
    public class ListLogicImpl: IListLogic
    {
		private const string FAVORITESLIST = "Favorites";
		
        private readonly IListRepository listRepository;
		private readonly ICatalogRepository catalogRepository;
        //TODO: Everything should only work with list for the current user. Waiting for Auth/login to be completed.

        public ListLogicImpl(IListRepository listRepository, ICatalogRepository catalogRepository)
        {
            this.listRepository = listRepository;
			this.catalogRepository = catalogRepository;
        }

		public Guid CreateList(Guid userId, string branchId, UserList list)
        {
			return listRepository.CreateList(userId, branchId, list);
        }

		public Guid? AddItem(Guid userId, Guid listId, ListItem newItem)
        {
			return listRepository.AddItem(userId, listId, newItem);
		}

		public void UpdateItem(Guid userId, Guid listId, ListItem updatedItem)
        {
			var list = listRepository.ReadList(userId, listId);

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

			listRepository.CreateList(userId, list.BranchId, list);
        }

		public void UpdateList(Guid userId, UserList list)
        {
			var updateList = listRepository.ReadList(userId, list.ListId);


			if (updateList == null) //Throw error?
				return;

			updateList.Name = list.Name;

			var itemsToRemove = new List<Guid>();

			foreach (var item in updateList.Items)
			{
				if (list.Items != null && !list.Items.Where(i => i.ListItemId.Equals(item.ListItemId)).Any())
					itemsToRemove.Add(item.ListItemId);
			}

			if (list.Items != null)
			{
				foreach (var item in list.Items)
				{
					if (item.ListItemId == null)
						updateList.Items.Add(item);
					else
					{
						var existingItem = updateList.Items.Where(i => i.ListItemId.Equals(item.ListItemId)).FirstOrDefault();
						if (existingItem == null)
							continue;
						existingItem.Label = item.Label;
						existingItem.ParLevel = item.ParLevel;
						existingItem.Position = item.Position;
					}
				}
			}

			listRepository.CreateList(userId, updateList.BranchId, updateList);

			foreach (var toDelete in itemsToRemove)
			{
				listRepository.DeleteItem(userId, updateList.ListId, toDelete);
			}

			
        }

		public void DeleteList(Guid userId, Guid listId)
        {
			listRepository.DeleteList(userId, listId);
        }

		public UserList DeleteItem(Guid userId, Guid listId, Guid itemId)
        {
			var list = listRepository.DeleteItem(userId, listId, itemId);
			if (list.Items != null)
				list.Items.Sort();
			LookupProductDetails(userId, list);
			return list;
        }

		public List<UserList> ReadAllLists(Guid userId, string branchId, bool headerInfoOnly)
        {
			var lists = listRepository.ReadAllLists(userId, branchId);

			if (!lists.Where(l => l.Name.Equals(FAVORITESLIST)).Any())
			{
				//favorites list doesn't exist yet, create an empty one
				listRepository.CreateList(userId, branchId, new UserList() { Name = FAVORITESLIST});
				lists = listRepository.ReadAllLists(userId, branchId);
			}

			if (headerInfoOnly)
				return lists.Select(l => new UserList() { ListId = l.ListId, Name = l.Name }).ToList();
			else
			{
				lists.ForEach(delegate(UserList list)
				{
					LookupProductDetails(userId, list);
					if (list.Items != null)
						list.Items.Sort();
				});
				return lists;
			}
        }

		public UserList ReadList(Guid userId, Guid listId)
        {
			var list = listRepository.ReadList(userId, listId);
			if (list == null)
				return null;
			if(list.Items != null)
				list.Items.Sort();
			LookupProductDetails(userId, list);
			return list;
        }

		public List<string> ReadListLabels(Guid userId, Guid listId)
        {
			var lists = listRepository.ReadList(userId, listId);
            
            if (lists == null || lists.Items == null)
                return null;

            return lists.Items.Where(l => l.Label != null).Select(i => i.Label).Distinct().ToList();
        }

		public List<string> ReadListLabels(Guid userId, string branchId)
        {
			var lists = listRepository.ReadAllLists(userId, branchId);
			return lists.Where(i =>  i.Items != null).SelectMany(l => l.Items.Where(b => b.Label != null).Select(i => i.Label)).Distinct().ToList();
        }

		private void LookupProductDetails(Guid userId, UserList list)
		{
			if (list.Items == null)
				return;

			var products = catalogRepository.GetProductsByIds(list.BranchId, list.Items.Select(i => i.ItemNumber).Distinct().ToList());
			var favorites = listRepository.ReadList(userId, string.Format("{0}_{1}", list.BranchId, FAVORITESLIST));

			list.Items.ForEach(delegate (ListItem listItem)
			{

				var prod = products.Products.Where(p => p.ItemNumber.Equals(listItem.ItemNumber)).FirstOrDefault();

				if (prod != null)
				{
					listItem.Name = prod.Name;
					listItem.PackSize = string.Format("{0} / {1}", prod.Cases, prod.Size);
				}
				if (favorites != null)
				{
					listItem.Favorite = favorites.Items.Where(i => i.ItemNumber.Equals(listItem.ItemNumber)).Any();
				}
			});
			
		}
		
		/// <summary>
		/// Checks if any of the products are in the user's Favorites list. 
		/// If so, their Favorite property is set to "true"
		/// </summary>
		/// <param name="branchId">The branch/catalog to use</param>
		/// <param name="products">List of products</param>
		public void MarkFavoriteProducts(Guid userId, string branchId, ProductsReturn products)
		{
			var list = listRepository.ReadList(userId, string.Format("{0}_{1}", branchId, FAVORITESLIST));

			if (list == null || list.Items == null)
				return;

			products.Products.ForEach(delegate(Product product)
			{
				product.Favorite = list.Items.Where(i => i.ItemNumber.Equals(product.ItemNumber)).Any();
			});
		}
	}
}
