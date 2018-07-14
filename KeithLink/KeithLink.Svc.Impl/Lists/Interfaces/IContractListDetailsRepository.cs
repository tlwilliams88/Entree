using System.Collections.Generic;

using Entree.Core.Models.Lists.Contract;

namespace Entree.Core.Interface.Lists {
    public interface IContractListDetailsRepository
    {
        List<ContractListDetail> GetContractListDetails(long parentHeaderId);
    }
}
