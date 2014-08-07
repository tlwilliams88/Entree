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
        Guid CreateList(UserList list);
        void UpdateList(UserList list);

        void DeleteList(Guid listId);
        
        List<UserList> ReadAllLists();
        UserList ReadList(Guid listId);
               
        
    }
}
