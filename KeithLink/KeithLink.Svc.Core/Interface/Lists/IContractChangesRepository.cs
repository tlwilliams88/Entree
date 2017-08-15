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

        void Update(string CustomerNumber, string BranchId, bool Sent);

        void Purge(int PurgeDays);
    }
}
