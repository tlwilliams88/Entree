using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IListRepository
    {
		Guid CreateOrUpdateList(Guid userId, string branchId, UserList list);

		Guid? AddItem(Guid userId, Guid listId, ListItem newItem);

		void DeleteList(Guid userId, Guid listId);
		UserList DeleteItem(Guid userId, Guid listId, Guid itemId);

		List<UserList> ReadAllLists(Guid userId, string branchId);
		UserList ReadList(Guid userId, Guid listId);
		UserList ReadList(Guid userId, string listName);
        
    }
}
