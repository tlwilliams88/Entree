using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IContractListDetailsRepository
    {
        List<ContractListDetail> GetContractListDetails(long parentHeaderId);
    }
}
