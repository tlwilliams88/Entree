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
        Guid CreateList(Guid userId, string branchId, UserList list);
		Guid? AddItem(Guid userId, Guid listId, ListItem newItem);
		UserList AddItems(UserProfile user, Guid listId, List<ListItem> newItems, bool allowDuplicates);

		void UpdateItem(Guid userId, Guid listId, ListItem updatedItem);
		void UpdateList(Guid userId, UserList list);

		void DeleteList(Guid userId, Guid listId);
		void DeleteItem(Guid userId, Guid listId, Guid itemId);
		void DeleteItems(Guid userId, Guid listId, List<Guid> itemIds);

		List<UserList> ReadAllLists(UserProfile user, string branchId, bool headerInfoOnly);
		UserList ReadList(UserProfile user, Guid listId);
		List<string> ReadListLabels(Guid userId, Guid listId);
		List<string> ReadListLabels(Guid userId, string branchId);

		void MarkFavoriteProductsAndNotes(Guid userId, string branchId, ProductsReturn products);

    }
}
