using Entree.Core.Models.Lists.MandatoryItem;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists
{
    public interface IMandatoryItemsListHeadersRepository
    {
        MandatoryItemsListHeader GetListHeaderForCustomer(UserSelectedContext catalogInfo);
        long SaveMandatoryItemsHeader(MandatoryItemsListHeader model);
    }
}
