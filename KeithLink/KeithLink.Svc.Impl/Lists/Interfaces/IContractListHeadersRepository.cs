using Entree.Core.Models.SiteCatalog;
using Entree.Core.Models.Lists.Contract;

namespace Entree.Core.Interface.Lists
{
    public interface IContractListHeadersRepository {
        ContractListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo);
    }
}
