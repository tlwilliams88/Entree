using Entree.Core.Models.Lists.History;
using Entree.Core.Models.SiteCatalog;

namespace Entree.Core.Interface.Lists {
    public interface IHistoryListHeaderRepository {
        HistoryListHeader GetHistoryListHeader(UserSelectedContext customerInfo);
    }
}
