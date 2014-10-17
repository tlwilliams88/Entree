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
		Guid? AddItem(UserProfile user, UserSelectedContext catalogInfo, Guid listId, ListItem newItem);
		UserList AddItems(UserProfile user, UserSelectedContext catalogInfo, Guid listId, List<ListItem> newItems, bool allowDuplicates);

		void UpdateItem(UserProfile user, Guid listId, ListItem updatedItem, UserSelectedContext catalogInfo);
		void UpdateList(UserProfile user, UserList list, UserSelectedContext catalogInfo);

		void DeleteList(UserProfile user, UserSelectedContext catalogInfo, Guid listId);
		void DeleteLists(UserProfile user, UserSelectedContext catalogInfo, List<Guid> listIds);
		void DeleteItem(UserProfile user, UserSelectedContext catalogInfo, Guid listId, Guid itemId);
		void DeleteItems(UserProfile user, UserSelectedContext catalogInfo, Guid listId, List<Guid> itemIds);

		List<UserList> ReadAllLists(UserProfile user, UserSelectedContext catalogInfo, bool headerInfoOnly);
		UserList ReadList(UserProfile user, Guid listId, UserSelectedContext catalogInfo);
		List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo, Guid listId);
		List<string> ReadListLabels(UserProfile user, UserSelectedContext catalogInfo);

		void MarkFavoriteProductsAndNotes(UserProfile user, string branchId, ProductsReturn products, UserSelectedContext catalogInfo);

    }
}
