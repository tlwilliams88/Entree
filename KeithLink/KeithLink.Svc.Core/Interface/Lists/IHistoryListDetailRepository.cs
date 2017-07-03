using System.Collections.Generic;

using KeithLink.Svc.Core.Models.Lists.History;

namespace KeithLink.Svc.Core.Interface.Lists {
    public interface IHistoryListDetailRepository {
        List<HistoryListDetail> GetAllHistoryDetails(long listId);
    }
}
