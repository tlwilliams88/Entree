using Entree.Core.Models.Lists;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entree.Core.Interface.Lists
{
    public interface IContractChangesRepository
    {
        List<ContractChange> ReadNextSet();

        void Update(long Id, bool Sent);

        void Purge(int PurgeDays);
    }
}
