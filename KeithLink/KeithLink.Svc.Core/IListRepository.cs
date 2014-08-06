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
        void UpdateItem(ListItem updatedItem);
        void UpdateList(UserList list);

        void DeleteList(Guid listId);
        void DeleteItem(UserList list, Guid itemId);

        List<UserList> ReadAllLists();
        UserList ReadList(Guid listId);
               
        
    }
}
