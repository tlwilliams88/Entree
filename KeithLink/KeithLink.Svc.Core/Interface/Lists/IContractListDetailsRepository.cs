using KeithLink.Svc.Core.Models.Lists.Contract;
using System.Collections.Generic;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IContractListDetailsRepository
    {
        List<ContractListDetail> ReadContractListDetails(long parentHeaderId);
    }
}
