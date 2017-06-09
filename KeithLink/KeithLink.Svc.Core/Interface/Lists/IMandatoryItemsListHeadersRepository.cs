using KeithLink.Svc.Core.Models.Lists.MandatoryItem;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists
{
    public interface IMandatoryItemsListHeadersRepository
    {
        MandatoryItemsListHeader GetMandatoryItemsHeader(string userId, UserSelectedContext catalogInfo, bool headerOnly);
    }
}
