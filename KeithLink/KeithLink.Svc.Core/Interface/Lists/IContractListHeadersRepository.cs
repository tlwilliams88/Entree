using System;
using KeithLink.Svc.Core.Models.SiteCatalog;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IContractListHeadersRepository
    {
        ContractListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo);
    }
}
