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

        List<UserList> ReadAllLists();
        UserList ReadList(Guid listId);
        List<string> ReadListLabels(Guid listId);
        List<string> ReadListLabels();
        
    }
}
