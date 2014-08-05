using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core
{
    public interface IListRepository
    {
        Guid CreateList(UserList list);
        Guid? AddItem(Guid listId, ListItem newItem);
        void UpdateItem(Guid listId, ListItem updatedItem);
        void UpdateListName(Guid listId, string name);

        void DeleteList(Guid listId);
        void DeleteItem(Guid listId, Guid itemId);

        List<UserList> ReadAllLists();
        UserList ReadList(Guid listId);
        List<string> ReadListLabels(Guid listId);
        List<string> ReadListLabels();
        
        
    }
}
