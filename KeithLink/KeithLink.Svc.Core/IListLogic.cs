using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public interface IListLogic
    {
        Guid CreateList(UserList list);
        Guid? AddItem(Guid listId, ListItem newItem);
        void UpdateItem(Guid listId, ListItem updatedItem);
        void UpdateList(UserList list);

        void DeleteList(Guid listId);
        void DeleteItem(Guid listId, Guid itemId);

        List<UserList> ReadAllLists(bool headerInfoOnly);
        UserList ReadList(Guid listId);
        List<string> ReadListLabels(Guid listId);
        List<string> ReadListLabels();
    }
}
