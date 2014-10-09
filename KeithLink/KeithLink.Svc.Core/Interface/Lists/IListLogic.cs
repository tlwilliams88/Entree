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
        Guid CreateList(Guid userId, UserSelectedContext catalogInfo, UserList list);
		Guid? AddItem(Guid userId, Guid listId, ListItem newItem);
		UserList AddItems(UserProfile user, UserSelectedContext catalogInfo, Guid listId, List<ListItem> newItems, bool allowDuplicates);

		void UpdateItem(Guid userId, Guid listId, ListItem updatedItem, UserSelectedContext catalogInfo);
		void UpdateList(Guid userId, UserList list, UserSelectedContext catalogInfo);

		void DeleteList(Guid userId, Guid listId);
		void DeleteLists(Guid userId, List<Guid> listIds);
		void DeleteItem(Guid userId, Guid listId, Guid itemId);
		void DeleteItems(Guid userId, Guid listId, List<Guid> itemIds);

		List<UserList> ReadAllLists(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly);
		UserList ReadList(UserProfile user, Guid listId, UserSelectedContext catalogInfo);
		List<string> ReadListLabels(Guid userId, Guid listId);
		List<string> ReadListLabels(Guid userId, UserSelectedContext catalogInfo);

		void MarkFavoriteProductsAndNotes(Guid userId, string branchId, ProductsReturn products, UserSelectedContext catalogInfo);

    }
}
