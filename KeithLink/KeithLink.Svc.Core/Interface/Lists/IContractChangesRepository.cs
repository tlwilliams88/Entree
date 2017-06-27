using KeithLink.Svc.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IContractChangesRepository
    {
        List<ContractChange> ReadNextSet();

        void Update(long ParentListId, bool Sent);

        void Purge(int PurgeDays);
    }
}
