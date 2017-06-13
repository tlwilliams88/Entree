using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IMandatoryItemsListHeadersRepository
    {
        MandatoryItemsListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo);
        long SaveMandatoryItemsHeader(MandatoryItemsListHeader model);
    }
}
