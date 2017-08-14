using KeithLink.Svc.Core.Models.SiteCatalog;
using KeithLink.Svc.Core.Models.Lists.Contract;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IContractListHeadersRepository {
        ContractListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo);
    }
}
