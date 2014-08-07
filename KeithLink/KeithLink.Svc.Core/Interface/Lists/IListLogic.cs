using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IListLogic
    {
        Guid CreateList(string branchId, UserList list);
        Guid? AddItem(Guid listId, ListItem newItem);
        void UpdateItem(Guid listId, ListItem updatedItem);
        void UpdateList(UserList list);

        void DeleteList(Guid listId);
        UserList DeleteItem(Guid listId, Guid itemId);

        List<UserList> ReadAllLists(string branchId, bool headerInfoOnly);
        UserList ReadList(Guid listId);
        List<string> ReadListLabels(Guid listId);
        List<string> ReadListLabels(string branchId);
    }
}
