using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Profile;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IListLogic
    {
        Guid CreateList(Guid userId, CatalogInfo catalogInfo, UserList list);
		Guid? AddItem(Guid userId, Guid listId, ListItem newItem);
		UserList AddItems(UserProfile user, CatalogInfo catalogInfo, Guid listId, List<ListItem> newItems, bool allowDuplicates);

		void UpdateItem(Guid userId, Guid listId, ListItem updatedItem, CatalogInfo catalogInfo);
		void UpdateList(Guid userId, UserList list, CatalogInfo catalogInfo);

		void DeleteList(Guid userId, Guid listId);
		void DeleteLists(Guid userId, List<Guid> listIds);
		void DeleteItem(Guid userId, Guid listId, Guid itemId);
		void DeleteItems(Guid userId, Guid listId, List<Guid> itemIds);

		List<UserList> ReadAllLists(UserProfile user, CatalogInfo catalogInfo, bool headerInfoOnly);
		UserList ReadList(UserProfile user, Guid listId, CatalogInfo catalogInfo);
		List<string> ReadListLabels(Guid userId, Guid listId);
		List<string> ReadListLabels(Guid userId, CatalogInfo catalogInfo);

		void MarkFavoriteProductsAndNotes(Guid userId, string branchId, ProductsReturn products, CatalogInfo catalogInfo);

    }
}
