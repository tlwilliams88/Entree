using KeithLink.Svc.Core.Models.Lists;
using KeithLink.Svc.Core.Models.Lists.Contract;
using KeithLink.Svc.Core.Models.SiteCatalog;

using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IContractListDetailsRepository
    {
        List<ContractListDetail> ReadContractListDetails(long parentHeaderId);
    }
}
