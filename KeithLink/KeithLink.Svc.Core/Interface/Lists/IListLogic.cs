using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IListLogic
    {
        Guid CreateList(Guid userId, string branchId, UserList list);
		Guid? AddItem(Guid userId, Guid listId, ListItem newItem);
		void UpdateItem(Guid userId, Guid listId, ListItem updatedItem);
		void UpdateList(Guid userId, UserList list);

		void DeleteList(Guid userId, Guid listId);
		void DeleteItem(Guid userId, Guid listId, Guid itemId);

		List<UserList> ReadAllLists(Guid userId, string branchId, bool headerInfoOnly);
		UserList ReadList(Guid userId, Guid listId);
		List<string> ReadListLabels(Guid userId, Guid listId);
		List<string> ReadListLabels(Guid userId, string branchId);

		void MarkFavoriteProducts(Guid userId, string branchId, ProductsReturn products);
    }
}
