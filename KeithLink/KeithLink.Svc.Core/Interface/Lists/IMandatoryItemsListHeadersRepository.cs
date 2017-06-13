using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IMandatoryItemsListHeadersRepository
    {
        MandatoryItemsListHeader GetMandatoryItemsHeader(UserSelectedContext catalogInfo);
        void SaveMandatoryItemsHeader(MandatoryItemsListHeader model);
    }
}
