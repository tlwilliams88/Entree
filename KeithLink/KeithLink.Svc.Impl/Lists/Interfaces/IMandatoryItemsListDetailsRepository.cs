using System.Collections.Generic;
using Entree.Core.Models.Lists.MandatoryItem;

namespace Entree.Core.Interface.Lists
{
    public interface IMandatoryItemsListDetailsRepository
    {
        List<MandatoryItemsListDetail> GetAllByHeader(long parentHeaderId);

        long Save(MandatoryItemsListDetail model);

        void Delete(long id);
    }
}
