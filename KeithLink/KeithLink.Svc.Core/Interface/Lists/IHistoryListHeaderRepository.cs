using KeithLink.Svc.Core.Models.Lists.History;
using KeithLink.Svc.Core.Models.SiteCatalog;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IHistoryListHeaderRepository {
        HistoryListHeader GetHistoryListHeader(UserSelectedContext customerInfo);
    }
}
